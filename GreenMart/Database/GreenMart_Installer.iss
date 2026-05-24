#define MyAppName "GreenMart"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "GreenMart Software Solutions"
#define MyAppURL "https://www.greenmart.com/"
#define MyAppExeName "GreenMart.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".gmart"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D1E2F3A4-B5C6-4D7E-8F9A-0B1C2D3E4F5A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
ChangesAssociations=yes
DisableProgramGroupPage=yes
; Thông tin chi tiết để tránh bị nhận nhầm là malware
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} Installer
VersionInfoCopyright=Copyright (C) 2024 {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
; Tên file cài đặt đầu ra
OutputBaseFilename=GreenMart_Setup_v{#MyAppVersion}
; Icon cho file cài đặt
SetupIconFile=d:\DemoWPF\GreenMartApp\GreenMart\checkout.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
; Giảm mức độ nén và tắt SolidCompression để AV dễ dàng quét nội dung bên trong
Compression=lzma
SolidCompression=no
WizardStyle=modern
; Yêu cầu quyền Admin để cài đặt vào Program Files
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
; Tự động ghi đè và xử lý khi nâng cấp
DirExistsWarning=no
; Kiểm tra nếu app đang chạy để đóng lại trước khi ghi đè (Tránh lỗi File in use)
AppMutex=GreenMart_Mutex_12345
CloseApplications=yes
RestartApplications=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Source là đường dẫn đến thư mục build Release của bạn
Source: "d:\DemoWPF\GreenMartApp\GreenMart\GreenMart\bin\Release\net8.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "d:\DemoWPF\GreenMartApp\GreenMart\GreenMart\bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Chú ý: Đảm bảo bạn đã copy file icon vào thư mục đích nếu muốn dùng làm icon shortcut

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
; Đăng ký thông tin để Windows nhận diện ứng dụng hợp lệ
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

[Code]
/////////////////////////////////////////////////////////////////////
// Hàm kiểm tra và gỡ cài đặt phiên bản cũ
/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  // Registry path uses single { for the AppId
  sUnInstPath := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{D1E2F3A4-B5C6-4D7E-8F9A-0B1C2D3E4F5A}_is1';
  sUnInstallString := '';
  
  // Kiểm tra trong HKLM (64-bit hoặc 32-bit) và HKCU
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    if not RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString) then
      // Nếu không tìm thấy ở bản 64-bit, thử bản 32-bit (Wow6432Node)
      if not RegQueryStringValue(HKLM32, sUnInstPath, 'UninstallString', sUnInstallString) then
        RegQueryStringValue(HKCU32, sUnInstPath, 'UninstallString', sUnInstallString);
        
  Result := sUnInstallString;
end;

function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;

function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
  Result := 0;
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    // Chạy uninstaller cũ ở chế độ siêu im lặng (/VERYSILENT)
    if Exec(sUnInstallString, '/SILENT /VERYSILENT /SUPPRESSMSGBOXES /NORESTART', '', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 1
    else
      Result := -1;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  if IsUpgrade() then
  begin
    // Tự động gỡ cài đặt phiên bản cũ một cách âm thầm
    UnInstallOldVersion();
  end;
end;


