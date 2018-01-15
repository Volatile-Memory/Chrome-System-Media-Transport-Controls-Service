function Debug-ProcessVS([int] $processId)
{
    $vsProcess = Get-Process devenv | Select-Object -First 1
    if (!$vsProcess) {throw "Visual Studio is not running"}
    $vsMajorVersion = $vsProcess.FileVersion -replace '^(\d+).*', '$1'
    $dte = [System.Runtime.InteropServices.Marshal]::GetActiveObject("VisualStudio.DTE.$vsMajorVersion.0")
    $debugee = $dte.Debugger.LocalProcesses | ? {$_.ProcessID -eq $processId}
    if (!$debugee) {throw "Process with ID $processId does not exist."}
    $debugee.Attach()
}

sleep -Seconds 3
Debug-ProcessVS((Get-Process -Name smtc)[0].Id)