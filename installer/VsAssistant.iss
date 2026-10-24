; Inno Setup script for VsAssistant.
; Built by .github/workflows/release.yml, or locally:
;   dotnet publish VsAssistant\VsAssistant.csproj -c Release -r win-x64 --self-contained -o publish
;   ISCC.exe /DAppVersion=1.0.0 installer\VsAssistant.iss

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\publish"
#endif

#define AppName "ВС Ассистент"
#define AppExeName "VsAssistant.exe"

[Setup]
; Never change AppId: it links upgrades and the uninstaller to earlier installs
AppId={{1E31EB92-209C-488A-9D99-13EA0D5B2518}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher=Oleh Ivanitskyy
AppPublisherURL=https://github.com/1OL1/VsAssistant
VersionInfoVersion={#AppVersion}

; Per-user install into %LocalAppData%\Programs, no admin rights needed
PrivilegesRequired=lowest
DefaultDirName={autopf}\VsAssistant
DisableProgramGroupPage=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Same mutex the app uses for its single instance check, so setup asks to exit the running app first
AppMutex=Local\VsAssistant.SingleInstance

SetupIconFile=..\VsAssistant\Resources\vs.ico
UninstallDisplayIcon={app}\{#AppExeName}
WizardStyle=modern
Compression=lzma2/max
SolidCompression=yes
OutputDir=..\artifacts
OutputBaseFilename=VsAssistant-Setup-{#AppVersion}

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[CustomMessages]
AutoStart=Запускать при входе в Windows

[Tasks]
Name: "autostart"; Description: "{cm:AutoStart}"
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[InstallDelete]
; Clean upgrade: files dropped in a newer version must not linger. User settings and logs live elsewhere.
Type: filesandordirs; Name: "{app}\*"

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
Name: "{userstartup}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: autostart

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
