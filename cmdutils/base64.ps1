$MyScript = @'
$Title = "Hey buddy"
$Message = "I am a notification running in an encoded command"
$Type = "info"
Write-Host $Message
'@

$MyEncodedScript = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($MyScript))

Write-Host $MyEncodedScript 

#powershell.exe -EncodedCommand $MyEncodedScript
