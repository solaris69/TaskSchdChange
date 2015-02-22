@echo off
IF EXIST TaskScheChange ( rd /s /q TaskScheChange )
mkdir TaskScheChange

copy ..\TaskSchdChange\bin\Release\Microsoft.Win32.TaskScheduler.dll TaskScheChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe TaskScheChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe.config TaskScheChange

IF EXIST tsk.wxs ( del /F tsk.wxs )
IF EXIST taskschdchange.wxs ( del /F taskschdchange.wxs )
copy taskschdchange.wxs.template taskschdchange.wxs
copy erms.ico TaskScheChange

REM fnr.exe" --cl --dir "D:\_0Compass\ReleaseDashboard\_GitHub\TaskSchdChange\Wix" --fileMask "tsk.wxs" --excludeFileMask "*.dll, *.exe" --find "SourceDIR" --replace "SourceDir\TaskScheChange"
REM SET wixbin="D:\WiX Toolset v3.9\bin"
SET wixbin="T:\_GitCoding\wix39-binaries"
SET workingdir="%~dp0TaskScheChange"
SET wixobj="%~dp0TaskScheChange\*.wixobj"
SET heatfile="%~dp0tsk.wxs"
SET mainfile="%~dp0taskschdchange.wxs"
SET msifile="%~dp0TaskScheChange_%1.msi"
SET fnrdir="%~dp0fnr.exe"
SET wixfile="%~dp0"
pushd %wixbin%
ECHO Gen Group folder
heat.exe dir %workingdir% -o %heatfile% -cg BinaryGroup -sfrag -gg -g1
%fnrdir% --cl --dir %wixfile:~0,-2%" --fileMask "taskschdchange.wxs" --find "@MAJORVERSION@" --replace "%1"
REM echo %fnrdir% --cl --dir %wixfile:~0,-2%" --fileMask "tsk.wxs" --find "SourceDir" --replace "SourceDir\TaskScheChange"
candle.exe %heatfile% %mainfile% -o %workingdir:~0,-1%\\"
REM light.exe -out %msifile% %wixobj%
light.exe -b %workingdir% -out %msifile% %wixobj%
popd