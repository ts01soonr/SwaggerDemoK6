$args | foreach-object {
Get-Process -name $_ -PipelineVariable pv | 
Measure-Object Workingset -sum -average | 
Select-object @{Name="Name";Expression = {$pv.name}},
Count,
@{Name="SumMB";Expression = {[math]::round($_.Sum/1MB,2)}},
@{Name="AvgMB";Expression = {[math]::round($_.Average/1MB,2)}}
}