Set-Location -Path .. -PassThru | Out-Null
$packagePath = "bin\Release"
$rarPath = "WinRar\WinRar.exe"

$probeCmd = [System.IO.Path]::GetFileName("WinRar.exe")
if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
{
    $rarCmd = $probeCmd
}
if(!$rarCmd) {
	$probeCmd = [System.IO.Path]::Combine($Env:ProgramW6432, $rarPath)
	if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
	{
	    $rarCmd = $probeCmd
	}
}
if(!$rarCmd) {
	$probeCmd = [System.IO.Path]::Combine($Env:ProgramFiles, $rarPath)
	if(Get-Command $probeCmd -ErrorAction SilentlyContinue)
	{
	    $rarCmd = $probeCmd
	}
}

$archiveName = "MakePdfSetup.exe"
$zipName = "MakePdfSetup.zip"
$archivePath = [System.IO.Path]::Combine($packagePath, $archiveName)
$zipPath = [System.IO.Path]::Combine($packagePath, $zipName)
$filesPath = [System.IO.Path]::Combine($packagePath, "*.*")

if(Test-Path $archivePath) {
    Remove-Item $archivePath
}
if(Test-Path $zipPath) {
    Remove-Item $zipPath
}

$rarArguments = @("a", "-zDeploy\package.ini", "-sfx", "-x*.pdb", "-x*vshost*", "-x*.xml", "-ep1", $archivePath, $filesPath)
& $rarCmd $rarArguments | Write-Output

$rarArguments = @("a", "-ep1", "-sfx", $archivePath, [System.IO.Path]::Combine($packagePath, "Templates"))
& $rarCmd $rarArguments | Write-Output

$rarArguments = @("rn", $archivePath, "MakePdf.exe.config", "MakePdf.exe.confi_")
& $rarCmd $rarArguments | Write-Output

$rarArguments = @("a", "-sfx", "-ep1", $archivePath, "Deploy\PostExtract.ps1")
& $rarCmd $rarArguments | Write-Output

$rarArguments = @("a", "-ep1", "-afzip", "-df", $zipPath, $archivePath)
& $rarCmd $rarArguments | Write-Output

$user = Read-Host -Prompt "FTP user (enter for default)"
if(!$user) {
	$user = "oseregindv"
}
$password = Read-Host -Prompt "FTP password" -AsSecureString

Write-Host "Uploading..."
# create the FtpWebRequest and configure it
[Environment]::CurrentDirectory = Get-Location
$content = [System.IO.File]::ReadAllBytes($zipPath)
$ftpPath = "ftp://seregindv.narod.ru/apps/" + $zipName
$ftp = [System.Net.FtpWebRequest]::Create($ftpPath)
$ftp = [System.Net.FtpWebRequest]$ftp
$ftp.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
$ftp.Credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $user, $password
$ftp.UseBinary = $true
$ftp.UsePassive = $true
# read in the file to upload as a byte array
$ftp.ContentLength = $content.Length
# get the request stream, and write the bytes into it
$rs = $ftp.GetRequestStream()
$rs.Write($content, 0, $content.Length)
# be sure to clean up after ourselves
$rs.Close()
$rs.Dispose()
$response = $ftp.GetResponse()
$response = [System.Net.FtpWebResponse]$response
Write-Host $response.StatusDescription