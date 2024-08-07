## PowerShell: Script To Telnet To Remote Hosts And Run Commands Against Them With Output To A File ##
## Overview: Useful for Telnet connections to Cisco Switches and other devices. Can add additional command strings
## Usage Examples: Add your environment specific details into the parameters below, or include when calling the script:
## ./tel.ps1 "127.0.0.1" "23" "run whomai"

param(
    [string] $remoteHost = "127.0.0.1", 
    [int] $port = 8888,
    [string] $command1 = "who", #You can add additional commands below here with additonal strings if you want
    [int] $commandDelay = 1000
   )
 
[string] $output = ""

## Read output from a remote host
function GetOutput
{
  ## Create a buffer to receive the response
  $buffer = new-object System.Byte[] 1024
  $encoding = new-object System.Text.AsciiEncoding
 
  $outputBuffer = ""
  $foundMore = $false
 
  ## Read all the data available from the stream, writing it to the
  ## output buffer when done.
  do
  {
    ## Allow data to buffer for a bit
    start-sleep -m 1000
 
    ## Read what data is available
    $foundmore = $false
    $stream.ReadTimeout = 1000
 
    do
    {
      try
      {
        $read = $stream.Read($buffer, 0, 1024)
 
        if($read -gt 0)
        {
          $foundmore = $true
          $outputBuffer += ($encoding.GetString($buffer, 0, $read))
        }
      } catch { $foundMore = $false; $read = 0 }
    } while($read -gt 0)
  } while($foundmore)
 
  $outputBuffer
}
 
function Main
{
  ## Open the socket, and connect to the computer on the specified port

    write-host "telnet $remoteHost $port -> $command1"
 
    trap { Write-Error "Could not connect to remote computer: $_"; exit }
    $socket = new-object System.Net.Sockets.TcpClient($remoteHost, $port)
 
    $stream = $socket.GetStream()
 
    $writer = new-object System.IO.StreamWriter $stream

    ## Receive the output that has buffered so far
    $SCRIPT:output += GetOutput

    Start-Sleep -m $commandDelay
    $writer.WriteLine($command1) #run command
    $writer.Flush()
    Start-Sleep -m $commandDelay
    $SCRIPT:output += GetOutput
    $writer.WriteLine("bye") #exit session
    $writer.Flush()
                
    ## Close the streams
    $writer.Close()
    $stream.Close()
    $output | Out-File ("$remoteHost-$port.txt") # output logs
    write-host $output
}
. Main