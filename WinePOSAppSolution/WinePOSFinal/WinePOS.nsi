Outfile "OneStopPOSInstaller.exe"
InstallDir "$PROGRAMFILES\OnestopPOS"

# Define Install Types
InstType "Server"
InstType "Client"

# Pages
Page custom InstallTypePage
Page custom ServiceNamePage
Page Directory
Page InstFiles

# Variables
Var INSTALL_SSMS
Var SERVICE_NAME

Function InstallTypePage
    # Create a simple dialog to let the user choose Server or Client
    MessageBox MB_YESNO "Do you want to install as a SERVER? (Selecting 'No' will install as a CLIENT)" IDYES +2
    StrCpy $INSTALL_SSMS "1"
    Return
    StrCpy $INSTALL_SSMS "0"
FunctionEnd

Function ServiceNamePage
    # Show service name input only if "Server" is selected
    ${If} $INSTALL_SSMS == "1"
        nsDialogs::Create 1018
        Pop $0
        ${If} $0 == error
            Abort
        ${EndIf}

        # Create label
        ${NSD_CreateLabel} 10u 10u 100% 12u "Enter Service Name:"

        # Create input box
        ${NSD_CreateText} 10u 25u 90% 12u ""
        Pop $SERVICE_NAME

        nsDialogs::Show
    ${EndIf}
FunctionEnd

Section "Install"
    SetOutPath "$INSTDIR"
    File /r "bin\Release\net472\*.*"
    CreateShortcut "$DESKTOP\OnestopPOS.lnk" "$INSTDIR\OnestopPOS.exe"

    # Install SSMS only if Server option is selected
    ${If} $INSTALL_SSMS == "1"
        SetOutPath "$INSTDIR"
        File "ssms\SSMS-Setup.exe"
        ExecWait '"$INSTDIR\SSMS-Setup.exe" /quiet /norestart'
    ${EndIf}

    # Install Windows Service only if Server option is selected
    ${If} $INSTALL_SSMS == "1"
        ${If} $SERVICE_NAME != ""
            File "service\WinePOSReportService.exe"
            ExecWait '"$SYSDIR\sc.exe" create "$SERVICE_NAME" binPath= "$INSTDIR\WinePOSReportService.exe" start= auto'
            ExecWait '"$SYSDIR\sc.exe" start "$SERVICE_NAME"'
        ${EndIf}
    ${EndIf}
SectionEnd