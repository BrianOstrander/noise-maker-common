@echo off

set file=%1DllFolder.txt

if exist %file% (
  set /p dest= <%file%
) else (
  set dest=..\..\..\..\DLLs
)

if not exist %3%dest% (
  mkdir %3%dest%
)

copy /y %2*.* %3%dest%