@echo off
pushd Cad\Client
dotnet publish -c Release
popd

pushd Cad\Server
dotnet publish -c Release
popd

pushd Commands\Server
dotnet publish -c Release
popd

pushd Commands\Client
dotnet publish -c Release
popd

rmdir /s /q dist
mkdir dist

copy /y fxmanifest.lua dist
copy /y sync-config.json dist
copy /y Hud\index.html dist\Hud\index.html
xcopy /y /e Hud\dist dist\Hud\dist\
xcopy /y /e Commands\Client\bin\Release\net452\publish dist\Commands\
xcopy /y /e Commands\Server\bin\Release\netstandard2.0\publish dist\Commands\
xcopy /y /e Cad\Client\bin\Release\net452\publish dist\Cad\
xcopy /y /e Cad\Server\bin\Release\netstandard2.0\publish dist\Cad\