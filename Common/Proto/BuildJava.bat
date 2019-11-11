@echo off
set path==;%cd%\bin

protoc --version
rem 查找文件
for /f "delims=" %%i in ('dir /b "model\*.proto"') do echo %%i
for /f "delims=" %%i in ('dir /b/a "model\*.proto"') do protoc --java_out=D:\\Programs\MySpace\TYServer\trunk\ModuleProto\src\main\java\ model\%%i
pause