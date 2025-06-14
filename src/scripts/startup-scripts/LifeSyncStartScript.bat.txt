@echo off
setlocal EnableDelayedExpansion

REM ==========================================
REM START LIFESYNC API AND CLIENT - Enhanced Version
REM ==========================================

echo Starting LifeSync API and Client...

REM Store the directory of this script in a variable
set "SCRIPT_DIR=%~dp0"

REM ------------------------------------------
REM 1) Start the API
REM ------------------------------------------
echo Navigating to API folder...
pushd "%SCRIPT_DIR%\..\LifeSync.API" || (
    echo ERROR: API folder not found.
    exit /b 1
)
echo Starting the API...
start "LifeSync API" dotnet run
popd

REM ------------------------------------------
REM 2) Start the Client
REM ------------------------------------------
echo Navigating to Client folder...
pushd "%SCRIPT_DIR%\..\LifeSync.Client" || (
    echo ERROR: Client folder not found.
    exit /b 1
)
echo Starting the Client...
start "LifeSync Client" npm start
popd

REM ------------------------------------------
REM 3) Wait for the React frontend to start
REM ------------------------------------------
set "FRONTEND_PORT=4200"
set "FRONTEND_URL=http://localhost:%FRONTEND_PORT%"
set "MAX_RETRIES=12"
set "RETRY_COUNT=0"

echo Waiting for React frontend to start on port %FRONTEND_PORT%...
:wait_for_frontend
timeout /t 5 /nobreak >nul

REM Check if the port is listening (React default port)
netstat -ano | findstr ":%FRONTEND_PORT%" >nul
if %errorlevel% neq 0 (
    set /a RETRY_COUNT+=1
    if !RETRY_COUNT! geq %MAX_RETRIES% (
        echo ERROR: Frontend did not start within the expected time.
        exit /b 1
    )
    echo Frontend not yet available, retrying (!RETRY_COUNT!/%MAX_RETRIES%)...
    goto wait_for_frontend
)

REM ------------------------------------------
REM 4) Open the frontend in the default browser
REM ------------------------------------------
echo Frontend is now running. Opening browser at %FRONTEND_URL%...
start "" "%FRONTEND_URL%"

echo LifeSync API and Client started successfully.
exit /b 0
