#!/bin/bash

echo "Initializing Git submodules..."
git submodule init
git submodule update

echo "Installing CoreRT dependencies..."
sudo apt-get install clang zlib1g-dev libkrb5-dev

echo "Downloading Raylib dependencies..."

echo "Downloading Raylib (Linux x64) native dependencies..."
wget -P NetRaylib/Raylib-cs/runtimes/linux-x64/native/ https://github.com/raysan5/raylib/releases/download/3.7.0/raylib-3.7.0_linux_amd64.tar.gz
tar -C NetRaylib/Raylib-cs/runtimes/linux-x64/native/ --transform 's/.*\///g' -xf NetRaylib/Raylib-cs/runtimes/linux-x64/native/raylib-3.7.0_linux_amd64.tar.gz raylib-3.7.0_linux_amd64/lib/libraylib.so.3.7.0
mv NetRaylib/Raylib-cs/runtimes/linux-x64/native/libraylib.so.3.7.0 NetRaylib/Raylib-cs/runtimes/linux-x64/native/raylib.so
rm NetRaylib/Raylib-cs/runtimes/linux-x64/native/raylib-3.7.0_linux_amd64.tar.gz

echo "Downloading Raylib (MacOS x64) native dependencies..."
wget -P NetRaylib/Raylib-cs/runtimes/osx-x64/native/ https://github.com/raysan5/raylib/releases/download/3.7.0/raylib-3.7.0_macos.tar.gz
tar -C NetRaylib/Raylib-cs/runtimes/osx-x64/native/ --transform 's/.*\///g' -xf NetRaylib/Raylib-cs/runtimes/osx-x64/native/raylib-3.7.0_macos.tar.gz raylib-3.7.0_macos/lib/libraylib.3.7.0.dylib
mv NetRaylib/Raylib-cs/runtimes/osx-x64/native/libraylib.3.7.0.dylib NetRaylib/Raylib-cs/runtimes/osx-x64/native/raylib.dylib
rm NetRaylib/Raylib-cs/runtimes/osx-x64/native/raylib-3.7.0_macos.tar.gz

echo "Downloading Raylib (Windows x64) native dependencies..."
wget -P NetRaylib/Raylib-cs/runtimes/win-x64/native/ https://github.com/raysan5/raylib/releases/download/3.7.0/raylib-3.7.0_win64_msvc16.zip
unzip -p NetRaylib/Raylib-cs/runtimes/win-x64/native/raylib-3.7.0_win64_msvc16.zip raylib-3.7.0_win64_msvc16/lib/raylib.dll > NetRaylib/Raylib-cs/runtimes/win-x64/native/raylib.dll
rm NetRaylib/Raylib-cs/runtimes/win-x64/native/raylib-3.7.0_win64_msvc16.zip

echo "Downloading Raylib (Windows x86) native dependencies..."
wget -P NetRaylib/Raylib-cs/runtimes/win-x86/native/ https://github.com/raysan5/raylib/releases/download/3.7.0/raylib-3.7.0_win32_msvc16.zip
unzip -p NetRaylib/Raylib-cs/runtimes/win-x86/native/raylib-3.7.0_win32_msvc16.zip raylib-3.7.0_win32_msvc16/lib/raylib.dll > NetRaylib/Raylib-cs/runtimes/win-x86/native/raylib.dll
rm NetRaylib/Raylib-cs/runtimes/win-x86/native/raylib-3.7.0_win32_msvc16.zip

echo "Install finished !"

read -s -n 1 -p "Press any key to close ..."
echo ""
