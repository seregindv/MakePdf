if(Test-Path MakePdf.exe.config) {
	[xml]$config = Get-Content MakePdf.exe.config
	$outNode = $config.SelectSingleNode('/configuration/appSettings/add[@key="Directory"]')
	if(!$outNode) {
		return
	}
	$outPath = $outNode.GetAttribute('value')
}
[xml]$newConfig = Get-Content MakePdf.exe.confi_
$newOutNode = $newConfig.SelectSingleNode('/configuration/appSettings/add[@key="Directory"]')
if($newOutNode -and $outPath) {
	$valueAttr = $newOutNode.GetAttributeNode('value')
	$valueAttr.Value = $outPath
}
$newConfig.Save('MakePdf.exe.config')
Remove-Item MakePdf.exe.confi_