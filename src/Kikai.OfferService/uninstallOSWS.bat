@ECHO off
SET ServiceName=OfferService
SET PS=powershell -nologo -command
SET SC=sc query %ServiceName%
set installUtil=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe
set executable=%~dp0\Kikai.OfferService.exe

:: Confirm it is running as admin
net session >nul 2>&1
if %errorLevel% == 0 (
  echo Administrative permissions confirmed.
) else (
  echo You need run this scrip as an Administrator.
  pause
  EXIT /B 0
)

:: check if service is running, if not check to see if it is installed.
set installed=0
set running=0

%SC% | find "does not exist" >nul
if %ERRORLEVEL% EQU 1 (set installed=1)

%PS% "get-service %ServiceName%" | find "Running" >nul
if %ERRORLEVEL% EQU 0 (
  set installed=1
  set running=1
)

if %running% EQU 1 (
  echo :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::.
  echo ::::: The service is running, stop the service ::::::::::::::::::::::::.
  echo :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::.
  net stop %ServiceName%
  pause
)

if %installed% EQU 1 (
  echo :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::.
  echo ::::: The service is installed, uninstall the service :::::::::::::::::.
  echo :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::.
  %installUtil% /u /ShowCallStack /LogToConsole=true /LogFile= %executable%
  pause
)

:: it is safer to do this, as sometimes it is not cleaned up properly
sc delete %ServiceName% >nul

if %installed% EQU 0 (
  echo The service is not installed.
  pause
)

EXIT /B 0
