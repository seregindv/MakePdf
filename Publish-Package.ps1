$packagePath = "bin\Release"
$user = Read-Host -Prompt "FTP user (enter for default)"
$password = Read-Host -Prompt "FTP password" -AsSecureString

$path = "WinRar\rar.exe"
$probeCmd = [System.IO.Path]::GetFileName($path)
if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
{
    $cmd = $probeCmd
}
$probeCmd = [System.IO.Path]::Combine($Env:ProgramW6432 ,$path)
if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
{
    $cmd = $probeCmd
}
$probeCmd = [System.IO.Path]::Combine($Env:ProgramFiles ,$path)
if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
{
    $cmd = $probeCmd
}

Start-Process $cmd "a -sfx -x*.pdb -x*vshost* -x*.xml MakePdf-sfx *.*" -Wait -WorkingDirectory $packagePath
Start-Process $cmd "a -sfx MakePdf-sfx Templates" -Wait -WorkingDirectory $packagePath
Start-Process $cmd "rn -sfx MakePdf-sfx.exe MakePdf.exe.config MakePdf.exe.confi_" -Wait -WorkingDirectory $packagePath
Start-Process $cmd "a -sfx $packagePath\MakePdf-sfx.exe PostExtract.ps1" -Wait

#extract path from config, put into confi_, [delete config], rename confi_