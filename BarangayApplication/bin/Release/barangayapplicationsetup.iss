; -- Inno Setup Script for Barangay Application with SQL Server Connection Prompt (Windows Authentication only) --
; -- Now prompts the user if they want to run the SQL setup script --

[Setup]
AppName=Barangay Application
AppVersion=1.0
DefaultDirName={pf}\BarangayApplication
DefaultGroupName=Barangay Application
OutputBaseFilename=BarangayApplicationSetup
Compression=lzma
SolidCompression=yes

[Files]
Source: "BarangayApplication.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "SQL2022-SSEI-Expr.exe"; DestDir: "{tmp}"; Flags: ignoreversion
Source: "setup.sql"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Barangay Application"; Filename: "{app}\BarangayApplication.exe"

[Run]
Filename: "{tmp}\SQL2022-SSEI-Expr.exe"; Description: "Install SQL Server Express 2022 (required if not already installed)"; Flags: nowait postinstall runascurrentuser unchecked

[Code]
var
  SQLPage: TWizardPage;
  EdtServer, EdtPort: TEdit;
  LblServer, LblPort: TLabel;

procedure InitializeWizard;
begin
  SQLPage := CreateCustomPage(wpSelectDir, 'SQL Server Connection', 'Enter SQL Server connection details');

  LblServer := TLabel.Create(SQLPage);
  LblServer.Parent := SQLPage.Surface;
  LblServer.Left := 8;
  LblServer.Top := 16;
  LblServer.Caption := 'SQL Server address:';

  EdtServer := TEdit.Create(SQLPage);
  EdtServer.Parent := SQLPage.Surface;
  EdtServer.Left := 8;
  EdtServer.Top := LblServer.Top + LblServer.Height + 4;
  EdtServer.Width := 200;
  EdtServer.Text := 'localhost';

  LblPort := TLabel.Create(SQLPage);
  LblPort.Parent := SQLPage.Surface;
  LblPort.Left := 8;
  LblPort.Top := EdtServer.Top + EdtServer.Height + 12;
  LblPort.Caption := 'SQL Server port:';

  EdtPort := TEdit.Create(SQLPage);
  EdtPort.Parent := SQLPage.Surface;
  EdtPort.Left := 8;
  EdtPort.Top := LblPort.Top + LblPort.Height + 4;
  EdtPort.Width := 200;
  EdtPort.Text := '1433';
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  CmdLine, Server, Port, AuthStr: string;
  ResultCode: Integer;
  RunSQL: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    RunSQL := MsgBox('Do you want to run the SQL setup script now?'#13#10'(Choose "No" if you have already set up the database before.)', mbConfirmation, MB_YESNO or MB_DEFBUTTON1);
    if RunSQL = IDYES then
    begin
      Server := EdtServer.Text;
      Port := EdtPort.Text;
      AuthStr := '-E'; // Windows Authentication

      CmdLine := '-S ' + Server + ',' + Port + ' ' + AuthStr +
        ' -i "' + ExpandConstant('{app}\setup.sql') + '"';

      if not Exec('sqlcmd.exe', CmdLine, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
        MsgBox('Failed to run SQL script. Please ensure SQL Server is running and sqlcmd is installed.', mbError, MB_OK)
      else
        MsgBox('Database setup script executed successfully.', mbInformation, MB_OK);
    end
    else
    begin
      MsgBox('SQL setup script was skipped. If you need to run it later, you can do so manually.', mbInformation, MB_OK);
    end;
  end;
end;