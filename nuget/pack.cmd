dotnet restore ..\src\Spreads.SQLite
dotnet pack ..\src\Spreads.SQLite -c RELEASE -o ..\artifacts

dotnet restore ..\src\Spreads.SQLite.EF
dotnet pack ..\src\Spreads.SQLite.EF -c RELEASE -o ..\artifacts

pause