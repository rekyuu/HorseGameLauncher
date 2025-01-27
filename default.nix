{
    lib,
    buildDotnetModule,
    dotnetCorePackages,
}:
let
in buildDotnetModule rec {
    pname = "HorseGameLauncher";
    version = "0.0.0";

    nativeBuildInputs = [ ];
    buildInputs = [ ];

    src = ./.;
    nugetDeps = ./deps.json;

    projectFile = "src/${pname}.sln";
    dotnet-sdk = dotnetCorePackages.sdk_9_0;
    dotnet-runtime = dotnetCorePackages.runtime_9_0;

    executables = [ pname ];

    runtimeDeps = [ ];
}