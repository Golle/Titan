# This script will setup the engine and build the tools used.


# replace this string when there's a new version out.
$dxcCompiler = "https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.7.2212/dxc_2022_12_16.zip"
$destinationFolder = "$PSScriptRoot/libs/dxc"
$destinationFile = "$destinationFolder/dxc.zip"

Write-Host "Download DirectX Compiler"
Remove-Item -Path $destinationFolder -Force -Recurse -Confirm: $false
mkdir $destinationFolder
Invoke-WebRequest -Uri $dxcCompiler -OutFile $destinationFile
Expand-Archive -Path $destinationFile -DestinationPath $destinationFolder
Remove-Item -Path $destinationFile -Force -Confirm:$false

Write-Host "building the tools"
# add error checks maybe?
& "$PSScriptRoot/publish_tools.ps1"
