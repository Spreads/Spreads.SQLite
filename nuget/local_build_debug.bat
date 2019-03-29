@echo off

dotnet restore ..\src\Spreads.SQLite
dotnet pack ..\src\Spreads.SQLite -c Debug -o C:\transient\LocalNuget -p:AutoSuffix=True

pause