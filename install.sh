dotnet publish FexCompiler/FexCompiler.csproj -c Release -o ".build"
makepkg -s -f
sudo pacman -U ./FexCompiler-*.pkg.tar.zst