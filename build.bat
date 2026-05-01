@echo off

dotnet publish "TimeSync\TimeSync.csproj" -r win-x86 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained true