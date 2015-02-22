@echo off
IF NOT EXIST TaskScheChange ( mkdir TaskScheChange ) ELSE ( 
rd /s /q TaskScheChange
mkdir TaskScheChange
)
copy ..\TaskSchdChange\bin\Release\Microsoft.Win32.TaskScheduler.dll TaskScheChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe TaskScheChange
copy ..\TaskSchdChange\bin\Release\TaskSchdChange.exe.config TaskScheChange

REM fnr.exe" --cl --dir "D:\_0Compass\ReleaseDashboard\_GitHub\TaskSchdChange\Wix" --fileMask "tsk.wxs" --excludeFileMask "*.dll, *.exe" --find "SourceDIR" --replace "SourceDir\TaskScheChange"
SET wixbin="D:\WiX Toolset v3.9\bin"
SET workingdir="%~dp0TaskScheChange"
SET wixobj="%~dp0TaskScheChange\*.wixobj"
SET heatfile="%~dp0tsk.wxs"
SET mainfile="%~dp0TaskScheChange.wxs"
SET msifile="%~dp0TaskScheChange_%1.msi"
SET fnrdir="%~dp0fnr.exe"
SET wixfile="%~dp0"
pushd %wixbin%
ECHO Gen Group folder
heat.exe dir %workingdir% -o %heatfile% -cg BinaryGroup -sfrag -gg -g1
echo %fnrdir% --cl --dir %wixfile% --fileMask "tsk.wxs" --find "SourceDir" --replace "SourceDir\TaskScheChange"
candle.exe %heatfile% %mainfile% -o %workingdir%
light.exe -out %msifile% %wixobj%
popd