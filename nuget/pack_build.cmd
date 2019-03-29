del ..\artifacts\*.nupkg

dotnet restore ..\src\Spreads.SQLite
dotnet pack ..\src\Spreads.SQLite -c Release -o ..\artifacts -p:AutoSuffix=True

pause