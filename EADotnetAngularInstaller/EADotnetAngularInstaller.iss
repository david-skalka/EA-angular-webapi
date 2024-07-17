[Setup]
AppName=EADotnetAngular
AppVersion=0.1.0
WizardStyle=modern
DefaultDirName={autopf}\EADotnetAngular
DefaultGroupName=EADotnetAngular
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "..\EADotnetAngularCli\bin\Debug\net6.0\*"; DestDir: "{app}\EADotnetAngularCli"; Flags: recursesubdirs
Source: "..\EADotnetAngularAddIn\bin\Debug\*"; DestDir: "{app}\EADotnetAngularAddIn"; Flags: recursesubdirs
Source: "Resouces\config.json"; DestDir: "{userappdata}\EADotnetAngularAddIn";


[Registry]
Root: HKCU; Subkey: "Software\Sparx Systems\EAAddins64"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Sparx Systems\EAAddins64\EADotnetAngularAddIn"; Flags: uninsdeletekey; ValueType: string; ValueData: "EADotnetAngularAddIn.EADotnetAngularAddInClass"
Root: HKCU; Subkey: "Software\Sparx Systems\EAAddins64\EADotnetAngularAddIn"; Flags: uninsdeletekey; ValueName: CliInstallLocation; ValueType: string; ValueData: "{app}\EADotnetAngularCli\EADotnetAngularCli.exe"


[Run]
; Registrování sestavení pomocí regasm.exe
Filename: "{dotnet40}\regasm.exe"; Parameters: """{app}\EADotnetAngularAddIn\EADotnetAngularAddIn.dll"" /codebase"; StatusMsg: "Registering EADotnetAngularAddIn.dll..."; Flags: runhidden

[UninstallRun]
; Odregistrování sestavení při odinstalaci
Filename: "{dotnet40}\regasm.exe"; Parameters: """{app}\EADotnetAngularAddIn\EADotnetAngularAddIn.dll"" /unregister"; StatusMsg: "Unregistering EADotnetAngularAddIn.dll..."; Flags: runhidden