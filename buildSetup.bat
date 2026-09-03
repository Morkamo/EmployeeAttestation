@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "POWERSHELL_EXE=%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe"
set "BUILD_SCRIPT=%SCRIPT_DIR%build-installer.ps1"

if not exist "%BUILD_SCRIPT%" (
    echo build-installer.ps1 not found:
    echo %BUILD_SCRIPT%
    exit /b 1
)

pushd "%SCRIPT_DIR%"
"%POWERSHELL_EXE%" -NoProfile -ExecutionPolicy Bypass -File "%BUILD_SCRIPT%"
set "EXIT_CODE=%ERRORLEVEL%"
popd

if not "%EXIT_CODE%"=="0" (
    echo.
    echo Build failed with exit code %EXIT_CODE%.
    exit /b %EXIT_CODE%
)

echo.
echo Build completed successfully.
exit /b 0
