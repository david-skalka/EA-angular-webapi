[Setup]
AppName=EADotnetWebapi
AppVersion=0.1.0
WizardStyle=modern
DefaultDirName={autopf}\EADotnetWebapi
DefaultGroupName=EADotnetWebapi
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "..\EADotnetWebapiCli\bin\Debug\net6.0\*"; DestDir: "{app}\EADotnetWebapiCli"; Flags: recursesubdirs
Source: "..\EADotnetWebapiAddIn\bin\Debug\*"; DestDir: "{app}\EADotnetWebapiAddIn"; Flags: recursesubdirs



[Registry]
Root: HKCU; Subkey: "Software\Sparx Systems\EAAddins64"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Sparx Systems\EAAddins64\EADotnetWebapiAddIn"; Flags: uninsdeletekey; ValueType: string; ValueData: "EADotnetWebapiAddIn.EADotnetWebapiAddInClass"


[Run]
; Registrování sestavení pomocí regasm.exe
Filename: "{dotnet40}\regasm.exe"; Parameters: """{app}\EADotnetWebapiAddIn\EADotnetWebapiAddIn.dll"" /codebase"; StatusMsg: "Registering EADotnetWebapiAddIn.dll..."; Flags: runhidden

[UninstallRun]
; Odregistrování sestavení při odinstalaci
Filename: "{dotnet40}\regasm.exe"; Parameters: """{app}\EADotnetWebapiAddIn\EADotnetWebapiAddIn.dll"" /unregister"; StatusMsg: "Unregistering EADotnetWebapiAddIn.dll..."; Flags: runhidden