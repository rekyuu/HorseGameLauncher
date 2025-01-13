{
    inputs = {
        nixpkgs.url = "github:nixos/nixpkgs/nixos-24.11";
    };

    outputs = { self, nixpkgs, ... }:
        let
            pkgs = nixpkgs.legacyPackages.x86_64-linux;
        in {
            devShells.x86_64-linux.default = with pkgs; mkShell rec {
                nativeBuildInputs =  [
                    dotnetCorePackages.sdk_9_0
                ];

                buildInputs = [
                    gtk3
                    gtk4
                    libadwaita
                ];

                LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath (buildInputs ++ nativeBuildInputs);
            };
        };
}
