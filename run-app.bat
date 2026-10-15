@echo off
cd /d "%~dp0lesson2"
set ASPNETCORE_URLS=http://localhost:5037
set ASPNETCORE_ENVIRONMENT=Development
"C:\Program Files\dotnet\dotnet.exe" bin\Debug\net10.0\lesson2.dll > app.log 2>&1
