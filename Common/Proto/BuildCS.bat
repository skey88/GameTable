@echo on
set path=%cd%\bin
set CS_TARGET_PATH=..\Client\Assets\
protoc --version
rem �����ļ�

del %CS_TARGET_PATH%\*.cs /f /s /q

for /f "delims=" %%i in ('dir /b "model\*.proto"') do echo %%i
rem תcpp  for /f "delims=" %%i in ('dir /b/a "*.proto"') do protoc -I=. --cpp_out=. %%i
for /f "delims=" %%i in ('dir /b/a "model\*.proto"') do protogen -i:model\%%i -o:%CS_TARGET_PATH%\%%~ni.cs

pause