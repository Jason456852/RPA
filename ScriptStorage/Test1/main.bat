@echo off
echo Arguments received: %1 %2 %3
timeout /nobreak /t 3 >nul
echo %~1 %~2 %~3
echo %~1 %~2 %~3 > "abc.txt"
exit /b 0