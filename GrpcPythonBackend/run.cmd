@echo off
setlocal
for %%I in ("%~dp0.") do set "PROJECT_DIR=%%~fI"
set "VENV_DIR=%PROJECT_DIR%\.venv"
set "PYTHON=%VENV_DIR%\Scripts\python.exe"

if not exist "%PYTHON%" (
    python -m venv "%VENV_DIR%"
    if errorlevel 1 exit /b 1
)

"%PYTHON%" -m pip install --disable-pip-version-check -r "%PROJECT_DIR%\requirements.txt"
if errorlevel 1 exit /b 1

set "PROTO_DIR=%PROJECT_DIR%\..\GrpcBackend\Protos"
"%PYTHON%" -m grpc_tools.protoc -I "%PROTO_DIR%" --python_out="%PROJECT_DIR%" --grpc_python_out="%PROJECT_DIR%" "%PROTO_DIR%\greeting.proto"
if errorlevel 1 exit /b 1

set "CERT_DIR=%TEMP%\ExamplesWpfGrpcPython"
set "CERT_PATH=%CERT_DIR%\localhost.pem"
set "KEY_PATH=%CERT_DIR%\localhost.key"
if not exist "%CERT_DIR%" mkdir "%CERT_DIR%"
if exist "%CERT_PATH%" if exist "%KEY_PATH%" goto CertificateReady
dotnet dev-certs https --export-path "%CERT_PATH%" --format PEM --no-password
if errorlevel 1 exit /b 1
:CertificateReady
if not exist "%KEY_PATH%" (
    echo The local HTTPS development private key was not exported.
    exit /b 1
)

set "GRPC_TLS_CERTIFICATE=%CERT_PATH%"
set "GRPC_TLS_PRIVATE_KEY=%KEY_PATH%"
pushd "%PROJECT_DIR%"
"%PYTHON%" server.py
set "SERVER_EXIT_CODE=%ERRORLEVEL%"
popd

del "%CERT_PATH%" "%KEY_PATH%" 2>nul
rmdir "%CERT_DIR%" 2>nul
exit /b %SERVER_EXIT_CODE%
