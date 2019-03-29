@echo off

dotnet restore ..\src\Spreads.SQLite
dotnet pack ..\src\Spreads.SQLite -c Release -o C:\transient\LocalNuget -p:AutoSuffix=True

pause
