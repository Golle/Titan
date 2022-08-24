
# this is a temporary build script, should be replaced with a propery CLI build tool later.
$outDir = "$PSScriptRoot/bin/"


&dotnet publish Titan.Tools.Packager -c Release -o "$outdir/Titan.Tools.Packager" -r win-x64 --self-contained true -p:PublishTrimmed=True -p:PublishAot=True -p:DebugType=None -p:DebugSymbols=false
if( $LASTEXITCODE -ne 0 ){
    Write-Host "Titan.Tools.Packager failed with exit code $LASTEXITCODE"
    exit -1
}

&dotnet publish Titan.Tools.ManifestBuilder -c Release -o "$outdir/Titan.Tools.ManifestBuilder" -r win-x64 --self-contained true -p:PublishSingleFile=True -p:DebugType=None -p:DebugSymbols=false
if( $LASTEXITCODE -ne 0 ){
    Write-Host "Titan.Tools.ManifestBuilder failed with exit code $LASTEXITCODE"
    exit -1
}

Write-Host "Build is complete."