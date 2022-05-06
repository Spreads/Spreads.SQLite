|   Linux   |  Windows  |    Mac    |
|:---------:|:---------:|:---------:|
| [![Build Status](https://dev.azure.com/DataSpreads/Spreads.SQLite/_apis/build/status/Spreads.Spreads.SQLite?branchName=Spreads&jobName=Linux)](https://dev.azure.com/DataSpreads/Spreads.SQLite/_build/latest?definitionId=9&branchName=Spreads) | [![Build Status](https://dev.azure.com/DataSpreads/Spreads.SQLite/_apis/build/status/Spreads.Spreads.SQLite?branchName=Spreads&jobName=Windows)](https://dev.azure.com/DataSpreads/Spreads.SQLite/_build/latest?definitionId=9&branchName=Spreads) | [![Build Status](https://dev.azure.com/DataSpreads/Spreads.SQLite/_apis/build/status/Spreads.Spreads.SQLite?branchName=Spreads&jobName=Mac)](https://dev.azure.com/DataSpreads/Spreads.SQLite/_build/latest?definitionId=9&branchName=Spreads) |


Spreads.SQLite
=====================

Extends [Microsoft.Data.Sqlite](https://github.com/aspnet/Microsoft.Data.Sqlite) with `FastQuery` class, which is 
a simple and fast wrapper over native functions that does not require ADO.NET ceremony and does not allocate anything.

NuGet binaries include [begin-concurrent](https://www.sqlite.org/src/doc/begin-concurrent/doc/begin_concurrent.md) and `wal2` features.

To build the amalgamation on Windows:

* Download zip from https://www.sqlite.org/src/timeline?r=begin-concurrent-wal2 or another branch. Git cloned repo does not work for some reason (line endings or something, zip is just simpler).
* In WSL run `./configure` then `make sqlite3.c`.
* Copy `sqlite3.c` and `sqlite3.h` to lib folder and use Makefile from MinGW or WSL to build shared libs (need to adjust some variables for each platform, not automated yet). Amalgamation produced on WSL works fine for Windows builds.
* Run `lib/compress.bat` and place compressed artifacts according to `Spreads.SQLite.csproj` imports.
* Clear `bin` and `obj` folders. Imported artifacts are often cached and not updated without this step.
* Rebuild .NET projects, run Tests.
* Run a publishing script from `nuget` folder. Local pack places a NuGet package in `C:/transient/LocalNuget` folder.
