@echo off
cls
dir bin /s /AD /b > clean.tmp
dir obj /s /AD /b >> clean.tmp
for /F  "tokens=*" %%A in (clean.tmp) do echo rmdir /S /Q "%%A" 
echo This command will remove ALL BIN and OBJ folders in this tree.
rem echo To run the commands as listed ...
rem pause
for /F  "tokens=*" %%A in (clean.tmp) do rmdir /S /Q "%%A" 
del clean.tmp
rem pause
timeout /t 5