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



