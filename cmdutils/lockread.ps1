#
# Simulates how outlook passes a file handle to searchrotocol host and performs locking. This was
# generating the exception "NotifyUnlockFile - lock count already 0" in the rollbackservice.
#

# This script block is used to start a child process. It reads the duplicated handle id from the parent process
# via stdin.
$childProcessScript = {  
   # the named event object which syncs actions between the 2 proceses
   $syncEvt = [system.threading.eventwaithandle]::new($false, 'manual', 'rbtestsync')
   # the other processe sends us the handle id
   write-host "$pid : Waiting for handle information"
   $dupHandleId = Read-Host
   write-host "$pid : Got handle $dupHandleId"
   # we create a SafeFileHandle from the handle id and then later a FileStream from that
   write-host "$pid : Creating safe handle"
   $dupHandle = [Microsoft.Win32.SafeHandles.SafeFileHandle]::New([int]$dupHandleId, $true)
   write-host "$pid : Creating FileStream from handle"
   $fs = [system.io.filestream]::new($dupHandle, [system.io.fileaccess]::write)
   # Perform a lock on the handle
   $fs.Lock(0,1)
   # Tell the parent process it can close the file, which will generate an UnlockFileAll
   $syncEvt.Set()
   # Wait for the parent to close the file so we know UnlockFileAll happened
   write-host "$pid : Waiting for parent close file"
   if (! $syncEvt.WaitOne(20000)) { write-host Timed out waiting for parent; exit; }
   write-host "$pid : Got go, writing file"
   $b = 9,9,9,9
   $fs.write($b, 2, 1)
   $fs.flush()
   $fs.unlock(0,1)
   write-host "$pid : Closing stream"
   $fs2.close()
   $fs2.dispose()
   $fs.close()
   $fs.dispose()
}

$nativeDef = @'
[DllImport("kernel32.dll")]
public static extern bool DuplicateHandle(IntPtr srcProcessHandle, IntPtr srcHandle, IntPtr targetProcessHandle, out IntPtr targetHandle, uint desiredAccess, bool inheritHandle, uint options);
'@
$native = add-type -memberdefinition $nativeDef -name 'native' -namespace 'winutil' -passthru
# the named event object which syncs actions between the 2 proceses
$syncEvt = [system.threading.eventwaithandle]::new($false, 'manual', 'rbtestsync')

# open file
$fs = [system.io.file]::open("c:\\test\\test.txt", "open", "write", "readwrite")

# start the child process
$pinfo = [system.diagnostics.processstartinfo]::new()
$pinfo.filename = "powershell.exe"
$pinfo.redirectstandarderror = $true
$pinfo.redirectstandardinput = $true
$pinfo.useshellexecute = $false
$pinfo.arguments = "-noprofile -command & {$childProcessScript}"
$p = [system.diagnostics.process]::new()
$p.StartInfo = $pinfo
$ret = $p.Start()

# Perform lock/unlock. Need to do this to prompt duplicatehandle/close sequence to generate the UnlockFileAll when the handle is closed.
$fs.lock(2,1)
$fs.unlock(2,1)

# duplicate the handle into the child process
$currentProcessHandle = [system.diagnostics.process]::getcurrentprocess().safehandle.dangerousgethandle()
$dupHandle = 0
$fsHandle = $fs.safefilehandle.dangerousgethandle()
$processHandle = $p.safehandle.dangerousgethandle()
$ret = $native::DuplicateHandle($currentProcessHandle, $fsHandle, $processHandle, [ref] $dupHandle, 0, $false, 2)
write-host $pid : DuplicateHandle returned: $ret
write-host $pid : Duplicate handle id: $dupHandle

# write the duplicate handle id to the child process
$p.StandardInput.writeline($dupHandle)

# wait for the child process to lock
write-host "$pid : Waiting for child to lock"
if (! $syncEvt.WaitOne(20000)) { write-host timed out waiting for child; exit; }
write-host $pid : Closing original handle
# close the original file handle. this will generate an UnlockFileAll operation
$fs.close()
$fs.dispose()
write-host $pid : Setting sync evet
# Tell the child to write
$syncEvt.Set()
write-host $pid : Waiting for child exit
$p.WaitForExit()
write-host $pid : Done



