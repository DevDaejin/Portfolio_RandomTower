@echo off
setlocal enabledelayedexpansion

:: ==========================
:: 사용자 정의 영역
:: ==========================
set "PROTOC=protoc.exe"
set "PROTO_SRC=Protos"
set "CSHARP_OUT=C:\Users\Daejin\Desktop\Portfolio_RandomTower\RandomTower_Unity\Assets\Network\Proto"
set "PYTHON_OUT=C:\Users\Daejin\Desktop\Portfolio_RandomTower\LocalHostServer\python project\proto"

:: ==========================
:: 실행 시작
:: ==========================
echo [Protobuf] Generating C# and Python code from proto files...

if not exist "%CSHARP_OUT%" mkdir "%CSHARP_OUT%"
if not exist "%PYTHON_OUT%" mkdir "%PYTHON_OUT%"

:: .proto 파일 존재 확인
dir /b "%PROTO_SRC%\*.proto" >nul 2>&1
if errorlevel 1 (
    echo [Error] No .proto files found in %PROTO_SRC%.
    goto :PAUSE_AND_EXIT
)

:: === C# 코드 생성 (Unity용) ===
echo Generating C# code...
for %%f in (%PROTO_SRC%\*.proto) do (
    echo Compiling %%f to C#...
    "%PROTOC%" --proto_path="%PROTO_SRC%" --csharp_out="%CSHARP_OUT%" "%%f"
    if errorlevel 1 (
        echo [Error] C# protobuf generation failed on %%f
        goto :PAUSE_AND_EXIT
    )
)

:: === Python 코드 생성 ===
echo Generating Python code...
for %%f in (%PROTO_SRC%\*.proto) do (
    echo Compiling %%f to Python...
    "%PROTOC%" --proto_path="%PROTO_SRC%" --python_out="%PYTHON_OUT%" "%%f"
    if errorlevel 1 (
        echo [Error] Python protobuf generation failed on %%f
        goto :PAUSE_AND_EXIT
    )
)

echo.
echo [Success] All .proto files compiled successfully.
echo [C# OUT]     %CSHARP_OUT%
echo [Python OUT] %PYTHON_OUT%
goto :PAUSE_AND_EXIT

:PAUSE_AND_EXIT
echo.
echo [End of Script]
pause
endlocal
