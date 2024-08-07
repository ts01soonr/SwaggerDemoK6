$args | foreach-object {
# https://www.pdq.com/blog/powershell-get-cpu-usage-for-a-process-using-get-counter/
# $ProcessName = "java"
# Option A: This is if you just have the name of the process; partial name OK
# Option B: This is for if you just have the PID; it will get the name for you
#$ProcessPID = "6860"
#$ProcessName = (Get-Process -Id $ProcessPID).Name

$CpuCores = (Get-WMIObject Win32_ComputerSystem).NumberOfLogicalProcessors
$Samples = (Get-Counter "\Process($_*)\% Processor Time").CounterSamples
$Samples | Select `
InstanceName,
@{Name="CPU %";Expression={ [Decimal]::Round(($_.CookedValue / $CpuCores), 4)}}

}

Write-Host $P