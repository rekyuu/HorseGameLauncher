#! /usr/bin/env bash

nix-build -E 'with import <nixpkgs> {}; callPackage ./default.nix {}' -A fetch-deps
./result
rm result