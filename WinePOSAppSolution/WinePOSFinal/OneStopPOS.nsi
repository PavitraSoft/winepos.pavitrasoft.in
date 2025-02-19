Outfile "OneStopPOSTest.exe"
InstallDir "$PROGRAMFILES\OneStopPOSTest"
Page Directory
Page InstFiles

Section "Install"
    SetOutPath "$INSTDIR"
    File /r "bin\Release\net472\*.*"
	
    CreateShortcut "$DESKTOP\OneStopPOS.lnk" "$INSTDIR\OneStopPOS.exe"
SectionEnd