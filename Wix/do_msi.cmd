@echo off
IF EXIST TaskSchdChange ( rd /s /q TaskSchdChange )
mkdir TaskSchdChange

copy ..\TaskSchdChange\bin\Release\Microsoft.Win32.TaskScheduler.dll TaskSchdChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe TaskSchdChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe.config TaskSchdChange
copy ..\TaskSchdChange\bin\Release\Newtonsoft.Json.dll TaskSchdChange
copy ..\TaskSchdChange\bin\Release\Newtonsoft.Json.Schema.dll TaskSchdChange
copy ..\TaskSchdChange\bin\Release\Newtonsoft.Json.xml TaskSchdChange
copy ..\TaskSchdChange\bin\Release\Newtonsoft.Json.Schema.xml TaskSchdChange

IF EXIST tsk.wxs ( del /F tsk.wxs )
IF EXIST taskschdchange.wxs ( del /F taskschdchange.wxs )
copy taskschdchange.wxs.template taskschdchange.wxs
copy erms.ico TaskSchdChange

REM SET wixbin="T:\_GitCoding\wix39-binaries"
SET wixbin="D:\WiX Toolset v3.9\bin"
SET workingdir="%~dp0TaskSchdChange"
SET wixobj="%~dp0TaskSchdChange\*.wixobj"
SET heatfile="%~dp0tsk.wxs"
SET mainfile="%~dp0taskschdchange.wxs"
SET msifile="%~dp0TaskSchdChange_%1.msi"
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