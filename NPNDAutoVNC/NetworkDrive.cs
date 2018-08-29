namespace NPNDAutoVNC
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class NetworkDrive
    {
        private const int CONNECT_CMD_SAVECRED = 0x1000;
        private const int CONNECT_COMMANDLINE = 0x800;
        private const int CONNECT_INTERACTIVE = 8;
        private const int CONNECT_PROMPT = 0x10;
        private const int CONNECT_REDIRECT = 0x80;
        private const int CONNECT_UPDATE_PROFILE = 1;
        private bool lf_Force = false;
        private bool lf_Persistent = false;
        private bool lf_SaveCredentials = false;
        private string ls_Drive = "s:";
        private bool ls_PromptForCredentials = false;
        private string ls_ShareName = @"\\10.10.1.23\mm770flr";
        private const int RESOURCETYPE_DISK = 1;

        public void MapDrive()
        {
            this.zMapDrive(null, null);
        }

        public void MapDrive(string Password)
        {
            this.zMapDrive(null, Password);
        }

        public void MapDrive(string Username, string Password)
        {
            this.zMapDrive(Username, Password);
        }

        public void RestoreDrives()
        {
            this.zRestoreDrive();
        }

        public void ShowConnectDialog(Form ParentForm)
        {
            this.zDisplayDialog(ParentForm, 1);
        }

        public void ShowDisconnectDialog(Form ParentForm)
        {
            this.zDisplayDialog(ParentForm, 2);
        }

        public void UnMapDrive()
        {
            this.zUnMapDrive(this.lf_Force);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2A(ref structNetResource pstNetRes, string psPassword, string psUsername, int piFlags);
        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);
        [DllImport("mpr.dll")]
        private static extern int WNetConnectionDialog(int phWnd, int piType);
        [DllImport("mpr.dll")]
        private static extern int WNetDisconnectDialog(int phWnd, int piType);
        [DllImport("mpr.dll")]
        private static extern int WNetRestoreConnectionW(int phWnd, string psLocalDrive);
        private void zDisplayDialog(Form poParentForm, int piDialog)
        {
            int error = -1;
            int phWnd = 0;
            if (poParentForm != null)
            {
                phWnd = poParentForm.Handle.ToInt32();
            }
            if (piDialog == 1)
            {
                error = WNetConnectionDialog(phWnd, 1);
            }
            else if (piDialog == 2)
            {
                error = WNetDisconnectDialog(phWnd, 1);
            }
            if (error > 0)
            {
                throw new Win32Exception(error);
            }
            poParentForm.BringToFront();
        }

        private void zMapDrive(string psUsername, string psPassword)
        {
            structNetResource pstNetRes = new structNetResource {
                iScope = 2,
                iType = 1,
                iDisplayType = 3,
                iUsage = 1,
                sRemoteName = @"\\10.10.1.23\mm770flr",
                sLocalName = "Y:"
            };
            int piFlags = 0;
            if (this.lf_SaveCredentials)
            {
                piFlags += 0x1000;
            }
            if (this.lf_Persistent)
            {
                piFlags++;
            }
            if (this.ls_PromptForCredentials)
            {
                piFlags += 0x18;
            }
            if (psUsername == "")
            {
                psUsername = null;
            }
            if (psPassword == "")
            {
                psPassword = null;
            }
            if (this.lf_Force)
            {
                try
                {
                    this.zUnMapDrive(true);
                }
                catch
                {
                }
            }
            int error = WNetAddConnection2A(ref pstNetRes, psPassword, psUsername, piFlags);
            if (error > 0)
            {
                throw new Win32Exception(error);
            }
        }

        private void zRestoreDrive()
        {
            int error = WNetRestoreConnectionW(0, null);
            if (error > 0)
            {
                throw new Win32Exception(error);
            }
        }

        private void zUnMapDrive(bool pfForce)
        {
            int piFlags = 0;
            if (this.lf_Persistent)
            {
                piFlags++;
            }
            int error = WNetCancelConnection2A(this.ls_Drive, piFlags, Convert.ToInt32(pfForce));
            if (error != 0)
            {
                error = WNetCancelConnection2A(this.ls_ShareName, piFlags, Convert.ToInt32(pfForce));
            }
            if (error > 0)
            {
                throw new Win32Exception(error);
            }
        }

        public bool Force
        {
            get
            {
                return this.lf_Force;
            }
            set
            {
                this.lf_Force = value;
            }
        }

        public string LocalDrive
        {
            get
            {
                return this.ls_Drive;
            }
            set
            {
                if (value.Length >= 1)
                {
                    this.ls_Drive = value.Substring(0, 1) + ":";
                }
                else
                {
                    this.ls_Drive = "";
                }
            }
        }

        public bool Persistent
        {
            get
            {
                return this.lf_Persistent;
            }
            set
            {
                this.lf_Persistent = value;
            }
        }

        public bool PromptForCredentials
        {
            get
            {
                return this.ls_PromptForCredentials;
            }
            set
            {
                this.ls_PromptForCredentials = value;
            }
        }

        public bool SaveCredentials
        {
            get
            {
                return this.lf_SaveCredentials;
            }
            set
            {
                this.lf_SaveCredentials = value;
            }
        }

        public string ShareName
        {
            get
            {
                return this.ls_ShareName;
            }
            set
            {
                this.ls_ShareName = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct structNetResource
        {
            public int iScope;
            public int iType;
            public int iDisplayType;
            public int iUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComment;
            public string sProvider;
        }
    }
}

