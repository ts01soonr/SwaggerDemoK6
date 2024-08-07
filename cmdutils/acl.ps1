if(3 -le $args.Length ){
    $Path= $args[0]
    $Type= $args[1]
    $OP= $args[2]
 
    if($Type.Equals("access")) {
        $ACL = Get-ACL -Path $Path
        $AccessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("Everyone","Read","Allow")
        if($OP.Equals("add")) { 
            $ACL.SetAccessRule($AccessRule)
            $ACL | Set-Acl -Path $Path
        }
        elseif($OP.Equals("x")) { 
            $ACL.RemoveAccessRule($AccessRule) 
            $ACL | Set-Acl -Path $Path
        }
        (Get-ACL -Path $Path).Access | Format-Table IdentityReference,FileSystemRights,AccessControlType,IsInherited,InheritanceFlags -AutoSize
        exit 
    }
    if($Type.Equals("audit")) {
        #target a folder
        $AuditUser = "Everyone"
        $AuditRules = "Delete,DeleteSubdirectoriesAndFiles,ChangePermissions,Takeownership"
        $InheritType = "ContainerInherit,ObjectInherit"
        $AuditType = "Success"
        $AccessRule = New-Object System.Security.AccessControl.FileSystemAuditRule($AuditUser,$AuditRules,$InheritType,"None",$AuditType)
        $ACL = Get-Acl $Path
        if($OP.Equals("add") ) { 
            $ACL.SetAuditRule($AccessRule)
            $ACL | Set-Acl $Path
        }
        elseif($OP.Equals("x") ) { 
            $ACL.RemoveAuditRuleSpecific($AccessRule)
            $ACL | Set-Acl $Path
        }
        #Write-Host "Processing >",$Path
        #$ACL | Set-Acl $Path
        Get-Acl -Audit $Path | format-list
        exit 
    }
    if($Type.Equals("owner")) {
        $ACL = Get-Acl -Path $Path
        $User = New-Object System.Security.Principal.Ntaccount($env:UserName)
        if($OP.Equals("add") ) {
            $ACL.SetOwner($User)
            $ACL | Set-Acl -Path $Path
        }
        (Get-ACL -Path $Path).Owner
        exit
    }
    if($Type.Equals("group")) {

        $ACL = Get-Acl -Path $Path
        #write-host Current primary group for $Path : $acl.Group

        # Update the group to "Backup Operators"
        #write-host Updating group...
        $groupSid = [System.Security.Principal.SecurityIdentifier]::new([System.Security.Principal.WellKnownSidType]::BuiltinBackupOperatorsSid,$null)
                
        if($OP.Equals("add") ) {
            $acl.SetGroup($groupSid)
            Set-Acl $Path $acl
            $ACL | Set-Acl -Path $Path
        }
        # write the acl
        # dump the group again
        (Get-ACL -Path $Path).Group
        exit
    }
}