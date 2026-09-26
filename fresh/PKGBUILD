pkgname=FexCompiler
pkgver=1.0.2
pkgrel=1
pkgdesc=".fex to .pdf compiler"
arch=('x86_64')
url="https://github.com/famoser/FexCompiler"
license=('MIT')
depends=('dotnet-runtime' 'texlive-bin')
source=()
options=(!debug)

package() {
    install -dm755 "$pkgdir/usr/lib/$pkgname"
    install -dm755 "$pkgdir/usr/bin"

    install -m644 ../.build/* "$pkgdir/usr/lib/$pkgname/"

    cat > "$pkgdir/usr/bin/$pkgname" <<EOF
#!/bin/sh
exec dotnet /usr/lib/$pkgname/FexCompiler.dll "\$@"
EOF

    chmod 755 "$pkgdir/usr/bin/$pkgname"
}