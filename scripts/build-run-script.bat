@echo off
:: Build the project
echo Building the project...
dotnet build ../sem5pi-24-25-dg38.sln

:: Check if the build was successful
if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b %ERRORLEVEL%
)

:: Run the project from the specific folder
echo [Running...]
dotnet run --project ../MDBackoffice/MDBackoffice.csproj