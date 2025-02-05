@echo off
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/Single_file
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/Multiple_files
color 3
echo Process Executed
pause
exit