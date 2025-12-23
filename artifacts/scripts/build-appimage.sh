dotnet tool install --global DotnetPackaging.Tool

dotnetpackager appimage appdir --directory ../../Mixi/bin/Release/net10.0/ \
  --output-dir ../../artifacts/Mixi.AppDir

dotnetpackager appimage from-appdir --directory ../../artifacts/Mixi.AppDir \
  --output ../../artifacts/Mixi.appimage