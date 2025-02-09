Outfile "WinePOSSetup.exe"
InstallDir "$PROGRAMFILES\WinePOS"
Page Directory
Page InstFiles

Section "Install"
    SetOutPath "$INSTDIR"
    File /r "bin\Release\net472\*.*"
    CreateShortcut "$DESKTOP\WinePOSApp.lnk" "$INSTDIR\WinePOSFinal.exe"
SectionEnd