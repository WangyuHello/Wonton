@ECHO OFF
SETLOCAL
"powershell.exe" -File build.ps1 -useMagic %*
pause