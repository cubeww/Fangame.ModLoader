dotnet build
dotnet publish Fangame.ModLoader.Gui -r win-x64
if (Test-Path -LiteralPath ./publish) {
    Remove-Item -LiteralPath ./publish -Recurse
}
New-Item -ItemType Directory -Force -Path ./publish
Copy-Item -Path "./Fangame.ModLoader.Gui/bin/Release/net9.0-windows/win-x64/publish" -Destination "./publish" -Recurse -Force
Rename-Item -Path "./publish/publish" -NewName "Fangame.ModLoader" 
Copy-Item -Path "./WorkingDirectory/Mods" -Destination "./publish/Fangame.ModLoader" -Recurse -Force
Compress-Archive -Path "./publish/Fangame.ModLoader" -DestinationPath "./publish/Fangame.ModLoader.zip" -Force