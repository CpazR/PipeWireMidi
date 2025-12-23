dotnet tool install --global DotnetPackaging.Tool

dotnetpackager flatpak layout \
 --directory ../../Mixi/bin/Release/net10.0/ \
 --output-dir ../../artifacts/

dotnetpackager flatpak bundle \
 --directory ../../Mixi/bin/Release/net10.0/ \
 --output ../../artifacts/Mixi.flatpak \
 --system \
 --application-name "Mixi" \
 --summary "Mixi - Midi Volume Manager"
