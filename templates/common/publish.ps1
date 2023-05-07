$out = "$PSScriptRoot/release"

&dotnet publish $PSScriptRoot/src/Space -c Release -o $out -r win-x64 --self-contained true -p:PublishTrimmed=True -p:PublishAot=True -p:DebugType=None -p:DebugSymbols=false -p:ConsoleWindow=true -p:ConsoleLogging=true -p:Trace=false

if($LASTEXITCODE -ne 0){
    Write-Host "Failed to publish the project with code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Copy-Item $PSScriptRoot/assets/bin/manifest.titanpak $out -Force
Copy-Item $PSScriptRoot/../Titan/assets/bin/builtin.titanpak $out -Force