@echo off
rem ==============================================================
rem
rem Thin wrapper that invokes build.ps1, which uses the shared
rem Contensive5 build module at C:\Git\Contensive5\scripts\contensive-build.psm1
rem
rem Usage:
rem   build.cmd              - interactive build (pauses on error)
rem   build.cmd /nopause     - automated build (no pause on error)
rem
rem ==============================================================

set PAUSE_ON_ERROR=1
if "%1"=="/nopause" set PAUSE_ON_ERROR=0

rem prefer PowerShell 7 (pwsh), fall back to Windows PowerShell
where pwsh >nul 2>&1
if %ERRORLEVEL%==0 (
    pwsh -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1"
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1"
)

if errorlevel 1 (
   echo.
   echo Build failed.
   if %PAUSE_ON_ERROR%==1 pause
   exit /b %errorlevel%
)
