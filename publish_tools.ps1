
# this is a temporary build script, should be replaced with a propery CLI build tool later.
$outDir = "$PSScriptRoot/tools/"


# this will break the packager, so we can't use this at the moment. (Json deserialization must be replaced with JsonContext)
# &dotnet publish src/Titan.Tools.Packager -c Release -o "$outdir/Titan.Tools.Packager" -r win-x64 --self-contained true -p:PublishTrimmed=True -p:PublishAot=True -p:DebugType=None -p:DebugSymbols=false
&dotnet publish src/Titan.Tools.Packager -c Release -o "$outdir/Titan.Tools.Packager" -r win-x64 --self-contained true -p:PublishTrimmed=False  -p:PublishSingleFile=True -p:PublishAot=False -p:DebugType=None -p:DebugSymbols=false
if( $LASTEXITCODE -ne 0 ){
    Write-Host "Titan.Tools.Packager failed with exit code $LASTEXITCODE"
    exit -1
}

&dotnet publish src/Titan.Tools.ManifestBuilder -c Release -o "$outdir/Titan.Tools.ManifestBuilder" -r win-x64 --self-contained true -p:PublishSingleFile=True -p:DebugType=None -p:DebugSymbols=false
if( $LASTEXITCODE -ne 0 ){
    Write-Host "Titan.Tools.ManifestBuilder failed with exit code $LASTEXITCODE"
    exit -1
}

Write-Host "Build is complete."