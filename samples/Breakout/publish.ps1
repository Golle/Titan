&./build-assets.ps1
&dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:DebugType=none -p:PublishTrimmed=true --self-contained true -o release/

Copy-Item -Path assets -Destination release/assets -recurse -Force
