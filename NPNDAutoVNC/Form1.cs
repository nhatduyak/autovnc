namespace NPNDAutoVNC
{
    using Gma.UserActivityMonitor;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Odbc;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using WindowScrape.Types;

    public partial class Form1 : Form
    {
        private int _countCloseAd = 0x3b;
        private int _currentHour = 0;
        private bool _isGetPoint = false;
        private bool _isRestartAppOK = true;
        private bool _isstart = false;
        private bool _isstop = false;
        private Queue<int> _listQty = new Queue<int>();
        private List<int> _listQty24 = new List<int>();
        private Queue<string> _ShowAdCurr = new Queue<string>();
        private int _soluong;
        
        private const int BN_CLICKED = 0xf5;
        
        private static readonly uint LSFW_LOCK = 1;
        private static readonly uint LSFW_UNLOCK = 2;
        public const int MOUSEEVENTF_LEFTDOWN = 2;
        public const int MOUSEEVENTF_LEFTUP = 4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_RIGHTDOWN = 8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        private OdbcDataAdapter obj_oledb_da;
        private const int WM_CLOSE = 0x10;

        public Form1()
        {
            this.InitializeComponent();
            this.numericSoluongreset.Value = CTLConfig._SoluongReset;
            this._soluong = (int) this.numericSoluongreset.Value;
            this.txt1to24.Text = CTLConfig._Time1to24;
            this.loadMain();
        }

        private void btdoubleLeftclick_Click(object sender, EventArgs e)
        {
            this.sendMouseDoubleClick(new System.Drawing.Point((int) this.txtmouseX.Value, (int) this.txtmouseY.Value));
        }

        private void btdoublerightClcik_Click(object sender, EventArgs e)
        {
            this.sendMouseRightDoubleClick(new System.Drawing.Point((int) this.txtmouseX.Value, (int) this.txtmouseY.Value));
        }

        private void btGetPoint_Click(object sender, EventArgs e)
        {
            if (!this._isGetPoint)
            {
                HookManager.MouseMove += new MouseEventHandler(this.MouseMoved);
                this.labpoint.Visible = true;
                this._isGetPoint = true;
            }
            else
            {
                HookManager.MouseMove -= new MouseEventHandler(this.MouseMoved);
                this.labpoint.Visible = false;
                this._isGetPoint = false;
            }
        }

        private void btleftclick_Click(object sender, EventArgs e)
        {
            this.sendMouseLeftclick(new System.Drawing.Point((int) this.txtmouseX.Value, (int) this.txtmouseY.Value));
        }

        private void btRightClick_Click(object sender, EventArgs e)
        {
            this.sendMouseRightclick(new System.Drawing.Point((int) this.txtmouseX.Value, (int) this.txtmouseY.Value));
        }

        private void btSave_Click_1(object sender, EventArgs e)
        {
            ((DataTable) this.gridlist.DataSource).WriteXml(Path.Combine(Application.StartupPath, "list.xml"));
            CTLConfig.Setvalue(this.txt1to24.Text);
        }

        private void btsetmouse_Click(object sender, EventArgs e)
        {
            Thread.Sleep(0x2710);
            for (int i = 0; i < 5; i++)
            {
                SetCursorPos((int) this.txtmouseX.Value, (int) this.txtmouseY.Value);
                Thread.Sleep(0x5dc);
                this.sendMouseDoubleClick(new System.Drawing.Point((int) this.txtmouseX.Value, (int) this.txtmouseY.Value));
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            int qTy=0 ;
            try
            {
                this._isstop = false;
                this.timer5p.Enabled = false;
                this.timer5p.Stop();
                this.timer5p.Tick -= new EventHandler(this.timer5p_Tick);
                this.timer10p.Enabled = false;
                this.timer10p.Stop();
                this.timer10p.Tick -= new EventHandler(this.timer10p_Tick);
                if (this.checkBoxIsStartApp.Checked)
                {
                    this.StartAndrestartApp();
                }
                if (this.checkBoxStartAndClose.Checked)
                {
                    this.lookuprestartapp();
                }
                else
                {
                    int num = 60 - DateTime.Now.Minute;
                    qTy = (((num * 10) / 6) * this._listQty24[DateTime.Now.Hour]) / 100;
                    CtlError.WriteError("start click ", qTy.ToString());
                    if (qTy > 30)
                    {
                        this.Gen5p(qTy);
                        CtlError.WriteError("Gen5 ", this._listQty.Count.ToString());
                        int count = this._listQty.Count;
                        this.timer5p.Enabled = true;
                        this.timer5p.Tick += new EventHandler(this.timer5p_Tick);
                        this.timer5p_Tick(null, null);
                    }
                    else if (qTy > 0)
                    {
                        this.Gen10p(qTy);
                        CtlError.WriteError("Gen10 ", this._listQty.Count.ToString());
                        this.timer10p.Enabled = true;
                        this.timer10p.Tick += new EventHandler(this.timer10p_Tick);
                        this.timer10p_Tick(null, null);
                    }
                }
            }
            catch(Exception ex)
            {
                CtlError.WriteError("Loi btStart_Click qty=" + qTy.ToString(), ex.Message);
            }
            this.timerGiay.Enabled = true;
            this.timerGiay.Tick += new EventHandler(this.timerGiay_Tick);
            this.timerGiay.Start();
            this.btStop.Enabled = true;
            this.btStart.Enabled = false;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            this._isstop = true;
            this.timerGiay.Enabled = false;
            this.timerGiay.Stop();
            this.timerGiay.Tick -= new EventHandler(this.timerGiay_Tick);
            this.btStop.Enabled = false;
            this.btStart.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] processesByName = Process.GetProcessesByName(CTLConfig._ProcessName);
            SetActiveWindow(processesByName[0].MainWindowHandle);
            SetForegroundWindow(processesByName[0].MainWindowHandle);
            Thread.Sleep(0x3e8);
            string[] strArray = this.textBox1.Text.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string str in strArray)
            {
                SendKeys.Send(str);
                SendKeys.Send("\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dataSource = (DataTable) this.gridlist.DataSource;
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Application.StartupPath, "profile"));
            if (!info.Exists)
            {
                info.Create();
            }
            foreach (DataRow row in dataSource.Rows)
            {
                if (row["IP"].ToString() != string.Empty)
                {
                    using (StreamWriter writer = new StreamWriter(string.Format(Path.Combine(info.FullName, "{0}.vnc"), row["IP"].ToString().Trim().Replace(".", ""))))
                    {
                        string str = string.Format("\r\n[Connection]\r\nHost={0}\r\nUserName=Administrator\r\nPassword=a7f8fc867315b7ff\r\nEncryption=PreferOff\r\nSelectDesktop=\r\nProxyServer=\r\nProxyType=\r\nSingleSignOn=1\r\n[Options]\r\nUseLocalCursor=1\r\nFullScreen=0\r\nRelativePtr=0\r\nFullColour=0\r\nColourLevel=0\r\nPreferredEncoding=ZRLE\r\nAutoSelect=0\r\nShared=1\r\nSendPtrEvents=1\r\nSendKeyEvents=1\r\nSendCutText=1\r\nAcceptCutText=1\r\nShareFiles=1\r\nPointerEventInterval=200\r\nScaling=AspectFit\r\nMenuKey=F8\r\nEnableToolbar=1\r\nUseDesktopResize=1\r\nFullScreenChangeResolution=0\r\nUseAllMonitors=0\r\nDisableWinKeys=1\r\nEmulate3=0\r\nAutoReconnect=1\r\nVerifyId=2\r\nSuppressIME=1\r\nMonitor=\r\n", row["IP"].ToString().Trim());
                        writer.Write(str);
                        writer.Dispose();
                        writer.Close();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.StartAndrestartApp();
        }

        private void checkBoxStartAndClose_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxStartAndClose.Checked)
            {
                this.numTick.Enabled = true;
            }
            else
            {
                this.numTick.Enabled = false;
            }
        }

        public void CLick_Ad_Auto(string filename)
        {
            FileInfo[] files = new DirectoryInfo(Path.Combine(Application.StartupPath, "profile")).GetFiles();
            Process.Start(filename);
            Thread.Sleep(0xfa0);
            this.Startad(new System.Drawing.Point(CTLConfig._PointFirtApp.X, 60));
            this.Startad(new System.Drawing.Point(CTLConfig._PointFirtApp.X, CTLConfig._PointAppCl.Y));
            this.Startad(new System.Drawing.Point(720, 0x177));
            Thread.Sleep(0x3e8);
            Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
            if (process != null)
            {
                process.Kill();
            }
        }

        public void CloseAd(string ip)
        {
            Process.Start(CTLConfig._PathVNC);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Sleep));
            this.setValueControlByHandle(Process.GetProcessesByName(CTLConfig._ProcessName)[0], Convert.ToInt32(CTLConfig._ControlIDIP), ip);
            Process[] processesByName = Process.GetProcessesByName(CTLConfig._ProcessName);
            IntPtr hWnd = FindWindowEx(processesByName[0].MainWindowHandle, IntPtr.Zero, "Button", "Connect");
            SendMessage(hWnd, 0xf5, IntPtr.Zero, IntPtr.Zero);
            Thread.Sleep(0x7d0);
            this.setValueControlByHandle(Process.GetProcessesByName(CTLConfig._ProcessName)[0], Convert.ToInt32(CTLConfig._ControlIDPass), "1");
            IntPtr ptr2 = FindWindowEx(processesByName[0].MainWindowHandle, IntPtr.Zero, "Button", "OK");
            SendMessage(hWnd, 0xf5, IntPtr.Zero, IntPtr.Zero);
            this.CloseAndRestart(CTLConfig._pointRightClick, CTLConfig._PointStartApp, CTLConfig._PointAppCl);
        }

        public void CloseAndRestart(System.Drawing.Point RightClick, System.Drawing.Point appstart, System.Drawing.Point appclose)
        {
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time4rightClick));
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(500);
            this.sendMouseRightclick(RightClick);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time5closeapp));
            SetCursorPos(appclose.X, appclose.Y);
            Thread.Sleep(0x3e8);
            this.sendMouseLeftclick(appclose);
            SetCursorPos(appstart.X, appstart.Y);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time6StartApp));
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time6StartApp));
            this.sendMouseLeftclick(appstart);
        }

        

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                CTLConfig.SetSLApp(this.txtslapp.Text);
            }
            else if (e.KeyCode == Keys.F2)
            {
                CTLConfig.SetWidthApp(this.txtwidth.Text);
            }
            else if (e.KeyCode == Keys.F3)
            {
                CTLConfig.SetHightApp(this.txthight.Text);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public void Gen10p(int QTy)
        {
            Random random = new Random();
            for (int i = 5; i > 0; i--)
            {
                int num2 = QTy / i;
                int item = 1;
                if (num2 > 2)
                {
                    item = random.Next(1, QTy + 2);
                }
                else if (QTy > 0)
                {
                    item = random.Next(1, QTy);
                }
                this._listQty.Enqueue(item);
                CtlError.WriteError("Gen 10 sl click ", item.ToString());
                QTy -= item;
            }
        }

        public void Gen5p(int QTy)
        {
            try
            {
                Random random = new Random();
                for (int i = 10; i > 0; i--)
                {
                    int num2 = QTy / i;
                    int item = 1;
                    if (num2 > 2)
                    {
                        item = random.Next(1, (QTy / 2) + 2);
                    }
                    else
                    {
                        item = random.Next(1, QTy);
                    }
                    this._listQty.Enqueue(item);
                    CtlError.WriteError("Gen 5 sl click ", item.ToString());
                    QTy -= item;
                }
            }
            catch
            {
            }
        }

        public void GennerateQty()
        {
            int qTy = this._listQty24[DateTime.Now.Hour];
            if (qTy > 30)
            {
                this.Gen5p(qTy);
                this.timer5p.Enabled = true;
                this.timer5p.Tick += new EventHandler(this.timer5p_Tick);
                this.timer5p.Start();
                this.timer5p_Tick(null, null);
            }
            else
            {
                this.Gen10p(qTy);
                this.timer10p.Enabled = true;
                this.timer10p.Start();
                this.timer10p.Tick += new EventHandler(this.timer10p_Tick);
                this.timer10p_Tick(null, null);
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out MousePoint lpMousePoin);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);
        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();
        public System.Drawing.Point GetPointRandonApp(int slapp)
        {
            try
            {
                System.Drawing.Point point;
                int num = new Random().Next(1, slapp + 1);
                if (num == 1)
                {
                    point = CTLConfig._PointFirtApp;
                }
                else
                {
                    int num2 = (num - 1) % 4;
                    int num3 = (num - 1) / 4;
                    point = new System.Drawing.Point((num2 * CTLConfig._WidthApp) + CTLConfig._PointFirtApp.X, (num3 * CTLConfig._hightApp) + CTLConfig._PointFirtApp.Y);
                }
                return point;
            }
            catch
            {
                return new System.Drawing.Point(CTLConfig._PointFirtApp.X, CTLConfig._PointFirtApp.Y);
            }
        }

        private IntPtr GetValueControlByHandle(Process p, int ID)
        {
            try
            {
                int num2;
                IntPtr mainWindowHandle = p.MainWindowHandle;
                List<HwndObject> windows = HwndObject.GetWindows();
                int num = -1;
                for (num2 = 0; num2 < windows.Count; num2++)
                {
                    if (windows[num2].Hwnd == mainWindowHandle)
                    {
                        num = num2;
                        break;
                    }
                }
                List<HwndObject> children = windows[num].GetChildren();
                IntPtr zero = IntPtr.Zero;
                int num3 = -1;
                for (num2 = 0; num2 < children.Count; num2++)
                {
                    if (GetDlgCtrlID(children[num2].Hwnd) == ID)
                    {
                        num3 = num2;
                        zero = children[num2].Hwnd;
                        break;
                    }
                }
                return zero;
            }
            catch (Exception exception)
            {
                CtlError.WriteError("ErroLog", "setValueControlByHandle Loi khi set handle tren form", exception.Message);
                return IntPtr.Zero;
            }
        }

        private IntPtr GetValueControlByHandle(Process p, int ID, string title)
        {
            try
            {
                Thread.Sleep(0x3e8);
                IntPtr mainWindowHandle = p.MainWindowHandle;
                List<HwndObject> children = HwndObject.GetWindowByTitle(title).GetChildren();
                IntPtr zero = IntPtr.Zero;
                int num2 = -1;
                for (int i = 0; i < children.Count; i++)
                {
                    if (GetDlgCtrlID(children[i].Hwnd) == ID)
                    {
                        num2 = i;
                        zero = children[i].Hwnd;
                        break;
                    }
                }
                return zero;
            }
            catch (Exception exception)
            {
                CtlError.WriteError("ErroLog", "get ValueControlByHandle by title Loi khi set handle tren form", exception.Message);
                return IntPtr.Zero;
            }
        }

        public DataTable InitFuntion()
        {
            DataTable table = new DataTable("funtion");
            table.Columns.Add("Des");
            table.Columns.Add("X");
            table.Columns.Add("Y");
            table.Columns.Add("CtrID");
            table.Columns.Add("MemID");
            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["Des"] = "InputIP";
                row["X"] = "10";
                row["Y"] = "10";
                row["CtrID"] = "1";
                row["MemID"] = "1";
                table.Rows.Add(row);
            }
            return table;
        }

        

        public DataTable InitList()
        {
            DataTable table = new DataTable("List");
            table.Columns.Add("IP");
            table.Columns.Add("Action");
            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["IP"] = "192.168.1.20";
                row["Action"] = "0";
                table.Rows.Add(row);
            }
            return table;
        }

        public DataTable LoadCSVList(string filetable)
        {
            DataTable dataTable = new DataTable();
            try
            {
                OdbcConnection selectConnection = new OdbcConnection(("Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + Application.StartupPath.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False").Trim());
                selectConnection.Open();
                string selectCommandText = "select * from [" + filetable + "]";
                this.obj_oledb_da = new OdbcDataAdapter(selectCommandText, selectConnection);
                this.obj_oledb_da.Fill(dataTable);
                selectConnection.Close();
            }
            catch (Exception exception)
            {
                CtlError.WriteError("Loi load CSV file ", exception.Message);
                return dataTable;
            }
            return dataTable;
        }

        public void LoadIP()
        {
            DataTable table = new DataTable();
            table = CTLImportFileCSV.ReadFromCsv(Path.Combine(Application.StartupPath, "List.csv"), Encoding.ASCII, ',');
            this.gridlist.DataSource = table;
        }

        public void loadMain()
        {
            DataSet set = new DataSet();
            set.ReadXml(Path.Combine(Application.StartupPath, "list.xml"));
            this.gridlist.DataSource = set.Tables[0];
            string[] strArray = CTLConfig._Time1to24.Split(new char[] { ',' });
            foreach (string str in strArray)
            {
                this._listQty24.Add(Convert.ToInt32(str));
            }
            this.txtwidth.Text = CTLConfig._WidthApp.ToString();
            this.txthight.Text = CTLConfig._hightApp.ToString();
            this.txtslapp.Text = CTLConfig._SLApp.ToString();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool LockSetForegroundWindow(uint uLockCode);
        private void lookuprestartapp()
        {
            while (this.checkBoxStartAndClose.Checked)
            {
                this.StartAndrestartApp();
                if (this.checkReset.Checked)
                {
                    if (this._soluong > 0)
                    {
                        this._soluong--;
                    }
                    else
                    {
                        this.resetIDFA();
                        this._soluong = (int) this.numericSoluongreset.Value;
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public void MouseMoved(object sender, MouseEventArgs e)
        {
            this.labpoint.Text = string.Format("x={0}  y={1}", e.X, e.Y);
        }

        public void resetIDFA()
        {
            DataTable dataSource = (DataTable) this.gridlist.DataSource;
            foreach (DataRow row in dataSource.Rows)
            {
                if (row["IP"].ToString() != string.Empty)
                {
                    if (!this.Checkprofile.Checked)
                    {
                        this.SetVNCByIP(row["IP"].ToString());
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Process.Start(Path.Combine(Path.Combine(Application.StartupPath, "profile"), row["IP"].ToString().Trim().Replace(".", "") + ".vnc"));
                        Thread.Sleep(0xfa0);
                    }
                    System.Drawing.Point pointRandonApp = this.GetPointRandonApp(Convert.ToInt32("0" + this.txtslapp.Text));
                    this.ResetIDFA_CloseAndRestartLinkAcLINK(CTLConfig._pointRightClick, pointRandonApp, CTLConfig._PointAppCl);
                    Thread.Sleep(0x3e8);
                    Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                    if (process != null)
                    {
                        process.Kill();
                        process.Dispose();
                    }
                }
            }
            this._isRestartAppOK = true;
        }

        public void ResetIDFA_CloseAndRestart(System.Drawing.Point RightClick, System.Drawing.Point appstart, System.Drawing.Point appclose)
        {
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time4rightClick));
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(500);
            this.sendMouseRightclick(RightClick);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time5closeapp));
            SetCursorPos(CTLConfig._IDFA.X, CTLConfig._IDFA.Y);
            Thread.Sleep(0x9c4);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._IDFA.X, CTLConfig._IDFA.Y));
            Thread.Sleep(0x1388);
            SetCursorPos(CTLConfig._IDFASett.X, CTLConfig._IDFASett.Y);
            Thread.Sleep(0xfa0);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._IDFA.X, CTLConfig._IDFA.Y));
            Thread.Sleep(0xfa0);
            SetCursorPos(CTLConfig._IDFAIden.X, CTLConfig._IDFAIden.Y);
            Thread.Sleep(0x7d0);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._IDFAIden.X, CTLConfig._IDFAIden.Y));
            Thread.Sleep(0x9c4);
            SetCursorPos(CTLConfig._IDFAGen.X, CTLConfig._IDFAGen.Y);
            Thread.Sleep(0xdac);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._IDFAGen.X, CTLConfig._IDFAGen.Y));
            Thread.Sleep(200);
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(400);
            this.sendMouseRightclick(RightClick);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time5closeapp));
            SetCursorPos(appclose.X, appclose.Y);
            Thread.Sleep(500);
            this.sendMouseLeftclick(appclose);
            SetCursorPos(appstart.X, appstart.Y);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time6StartApp));
            this.sendMouseLeftclick(appstart);
        }

        public void ResetIDFA_CloseAndRestartLinkAcLINK(System.Drawing.Point RightClick, System.Drawing.Point appstart, System.Drawing.Point appclose)
        {
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time4rightClick));
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(500);
            this.sendMouseRightclick(RightClick);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time5closeapp));
            SetCursorPos(CTLConfig._Link.X, CTLConfig._Link.Y);
            Thread.Sleep(0x9c4);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._Link.X, CTLConfig._Link.Y));
            Thread.Sleep(500);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._Link.X, CTLConfig._Link.Y));
            Thread.Sleep(200);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._Link.X, CTLConfig._Link.Y));
            Thread.Sleep(0x1770);
            SetCursorPos(CTLConfig._ReLink.X, CTLConfig._ReLink.Y);
            Thread.Sleep(0x7d0);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._ReLink.X, CTLConfig._ReLink.Y));
            Thread.Sleep(0x7d0);
            SetCursorPos(CTLConfig._SetLink.X, CTLConfig._SetLink.Y);
            Thread.Sleep(0x7d0);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._SetLink.X, CTLConfig._SetLink.Y));
            Thread.Sleep(0x7d0);
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(400);
            this.sendMouseRightclick(RightClick);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time5closeapp));
            SetCursorPos(appclose.X, appclose.Y);
            Thread.Sleep(500);
            this.sendMouseLeftclick(appclose);
            SetCursorPos(appstart.X, appstart.Y);
            Thread.Sleep(Convert.ToInt32(CTLConfig._Time6StartApp));
            this.sendMouseLeftclick(appstart);
        }

        public void RightClick(System.Drawing.Point RightClick)
        {
            SetCursorPos(RightClick.X, RightClick.Y);
            Thread.Sleep(500);
            this.sendMouseRightclick(RightClick);
        }

        public void RightLeftClick(System.Drawing.Point RightClick)
        {
            SetCursorPos(CTLConfig._PointAppCl.X, CTLConfig._PointAppCl.Y);
            Thread.Sleep(500);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._PointAppCl.X, CTLConfig._PointAppCl.Y));
        }

        public bool Save_FileListIP()
        {
            try
            {
                string str = "IP,Action";
                DataTable dataSource = (DataTable) this.gridlist.DataSource;
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.StartupPath, "list.csv")))
                {
                    writer.WriteLine(str);
                    for (int i = 0; i < dataSource.Rows.Count; i++)
                    {
                        str = "";
                        if (dataSource.Rows[i]["IP"].ToString().Trim() != string.Empty)
                        {
                            str = dataSource.Rows[i]["IP"].ToString().Trim() + "," + dataSource.Rows[i]["Action"].ToString().Trim();
                            writer.WriteLine(str);
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                CtlError.WriteError("export csv ip", exception.Message);
                return false;
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private void sendMouseDoubleClick(System.Drawing.Point p)
        {
            mouse_event(6, p.X, p.Y, 0, 0);
            Thread.Sleep(150);
            mouse_event(6, p.X, p.Y, 0, 0);
        }

        private void sendMouseLeftclick(System.Drawing.Point p)
        {
            mouse_event(6, p.X, p.Y, 0, 0);
        }

        private void sendMouseRightclick(System.Drawing.Point p)
        {
            mouse_event(0x18, p.X, p.Y, 0, 0);
        }

        private void sendMouseRightDoubleClick(System.Drawing.Point p)
        {
            mouse_event(0x18, p.X, p.Y, 0, 0);
            Thread.Sleep(150);
            mouse_event(0x18, p.X, p.Y, 0, 0);
        }

        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private void setValueControlByHandle(Process p, int ID, string valueIP)
        {
            try
            {
                int num2;
                Thread.Sleep(0x3e8);
                IntPtr mainWindowHandle = p.MainWindowHandle;
                List<HwndObject> windows = HwndObject.GetWindows();
                int num = -1;
                for (num2 = 0; num2 < windows.Count; num2++)
                {
                    if (windows[num2].Hwnd == mainWindowHandle)
                    {
                        num = num2;
                        break;
                    }
                }
                List<HwndObject> children = windows[num].GetChildren();
                int num3 = -1;
                for (num2 = 0; num2 < children.Count; num2++)
                {
                    if (GetDlgCtrlID(children[num2].Hwnd) == ID)
                    {
                        num3 = num2;
                        break;
                    }
                }
                children[num3].Text = valueIP;
            }
            catch (Exception exception)
            {
                CtlError.WriteError("ErroLog", "setValueControlByHandle Loi khi set handle tren form", exception.Message);
            }
        }

        private void setValueControlByHandleByTitle(Process p, int ID, string valueIP, string title)
        {
            try
            {
                Thread.Sleep(0x3e8);
                IntPtr mainWindowHandle = p.MainWindowHandle;
                List<HwndObject> children = HwndObject.GetWindowByTitle(title).GetChildren();
                int num2 = -1;
                for (int i = 0; i < children.Count; i++)
                {
                    if (GetDlgCtrlID(children[i].Hwnd) == ID)
                    {
                        num2 = i;
                        break;
                    }
                }
                children[num2].Text = valueIP;
            }
            catch (Exception exception)
            {
                CtlError.WriteError("ErroLog", "setValueControlByHandle Loi khi set handle tren form", exception.Message);
            }
        }

        public void SetVNCByIP(string ip)
        {
            try
            {
                Process.Start(CTLConfig._PathVNC);
                Thread.Sleep(Convert.ToInt32(0x7d0));
                Process[] processesByName = Process.GetProcessesByName(CTLConfig._ProcessName);
                this.setValueControlByHandle(processesByName[0], Convert.ToInt32(CTLConfig._ControlIDIP), ip);
                SendMessage(FindWindowEx(processesByName[0].MainWindowHandle, IntPtr.Zero, "Button", "Connect"), 0xf5, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(Convert.ToInt32(CTLConfig._Time1Connectvnc));
                Thread.Sleep(Convert.ToInt32(CTLConfig._Time2inputPass));
                string title = "VNC Authentication: " + ip + " [No Encryption]";
                this.setValueControlByHandleByTitle(processesByName[0], Convert.ToInt32(CTLConfig._ControlIDPass), "1", title);
                Thread.Sleep(Convert.ToInt32(CTLConfig._Time3Connect));
                SendMessage(this.GetValueControlByHandle(processesByName[0], 1, title), 0xf5, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
            }
        }

        public void Startad(System.Drawing.Point Pad)
        {
            Thread.Sleep(0x7d0);
            SetCursorPos(Pad.X, Pad.Y);
            Thread.Sleep(500);
            this.sendMouseLeftclick(Pad);
            Thread.Sleep(100);
            this.sendMouseLeftclick(Pad);
            this.sendMouseLeftclick(Pad);
            Thread.Sleep(300);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._PointAD.X, CTLConfig._PointAD.Y));
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._PointAD.X, CTLConfig._PointAD.Y));
            Thread.Sleep(100);
            this.sendMouseLeftclick(new System.Drawing.Point(CTLConfig._PointAD.X, CTLConfig._PointAD.Y));
        }

        public void StartAndrestartApp()
        {
            try
            {
                this.Count_ClickAuto++;
                bool flag = false;
                DataTable dataSource = (DataTable) this.gridlist.DataSource;
                foreach (DataRow row in dataSource.Rows)
                {
                    if (row["IP"].ToString() != string.Empty)
                    {
                        if (!this.Checkprofile.Checked)
                        {
                            this.SetVNCByIP(row["IP"].ToString());
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Process.Start(Path.Combine(Path.Combine(Application.StartupPath, "profile"), row["IP"].ToString().Trim().Replace(".", "") + ".vnc"));
                            Thread.Sleep(0xfa0);
                        }
                        this.RightClick(CTLConfig._pointRightClick);
                        Thread.Sleep(0x3e8);
                        Process[] processesByName = Process.GetProcessesByName(CTLConfig._ProcessName);
                        foreach (Process process in processesByName)
                        {
                            if (process != null)
                            {
                                process.Kill();
                                process.Dispose();
                            }
                        }
                        this.RightLeftClick(CTLConfig._PointAppCl);
                    }
                }
                if (flag)
                {
                    this.Count_ClickAuto = 0;
                }
                this._isRestartAppOK = true;
            }
            catch (Exception)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.StartAndrestartApp();
        }

        private void timer10p_Tick(object sender, EventArgs e)
        {
            if ((this._listQty != null) && (this._listQty.Count > 1))
            {
                int num = this._listQty.Dequeue();
                DataTable dataSource = (DataTable) this.gridlist.DataSource;
                List<int> list = new List<int>();
                Random random = new Random();
                for (int i = 0; i < num; i++)
                {
                    int count = dataSource.Rows.Count;
                    int item = random.Next(0, count);
                    while (list.Contains(item))
                    {
                        item = random.Next(0, count);
                    }
                    string ex = dataSource.Rows[item]["IP"].ToString();
                    Process.Start(Path.Combine(Path.Combine(Application.StartupPath, "profile"), ex.Trim().Replace(".", "") + ".vnc"));
                    Thread.Sleep(0xfa0);
                    this.Startad(CTLConfig._PointAD);
                    CtlError.WriteError("Click may ", ex);
                    Thread.Sleep(0x3e8);
                    Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                    if (process != null)
                    {
                        process.Kill();
                    }
                }
                this._countCloseAd = 0x3b;
                this.timerCount.Interval = 0x3e8;
                this.timerCount.Enabled = true;
                this.timerCount.Start();
                this.timerCount.Tick += new EventHandler(this.timerCount_Tick);
            }
        }

        private void timer5p_Tick(object sender, EventArgs e)
        {
            if ((this._listQty != null) && (this._listQty.Count > 1))
            {
                int num = this._listQty.Dequeue();
                DataTable dataSource = (DataTable) this.gridlist.DataSource;
                List<int> list = new List<int>();
                Random random = new Random();
                for (int i = 0; i < num; i++)
                {
                    int count = dataSource.Rows.Count;
                    int item = random.Next(0, count);
                    while (list.Contains(item))
                    {
                        item = random.Next();
                    }
                    string ex = dataSource.Rows[item]["IP"].ToString();
                    Process.Start(Path.Combine(Path.Combine(Application.StartupPath, "profile"), ex.Trim().Replace(".", "") + ".vnc"));
                    Thread.Sleep(0xfa0);
                    this.Startad(CTLConfig._PointAD);
                    CtlError.WriteError("Click may ", ex);
                    Thread.Sleep(0x3e8);
                    Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                    if (process != null)
                    {
                        process.Kill();
                    }
                }
                this._countCloseAd = 0x3b;
                this.timerCount.Interval = 0x3e8;
                this.timerCount.Enabled = true;
                this.timerCount.Start();
                this.timerCount.Tick += new EventHandler(this.timerCount_Tick);
            }
        }

        private void timerCount_Tick(object sender, EventArgs e)
        {
            if (this._countCloseAd == 0)
            {
                int count = this._ShowAdCurr.Count;
                for (int i = 0; i < count; i++)
                {
                    string ip = this._ShowAdCurr.Dequeue();
                    this.SetVNCByIP(ip);
                    Thread.Sleep(0x9c4);
                    System.Drawing.Point pointRandonApp = this.GetPointRandonApp(Convert.ToInt32("0" + this.txtslapp.Text));
                    this.CloseAndRestart(CTLConfig._pointRightClick, pointRandonApp, CTLConfig._PointAppCl);
                    CtlError.WriteError("Close may ", ip);
                    Thread.Sleep(0x3e8);
                    Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                    if (process != null)
                    {
                        process.Kill();
                    }
                }
                this.timerCount.Enabled = false;
                this.timerCount.Stop();
            }
            this._countCloseAd--;
        }

        private void timerGiay_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now.Minute == 0) && (this._currentHour != DateTime.Now.Hour))
            {
                CtlError.WriteError("timer giay Click ", "con lai " + this._listQty.Count.ToString());
                this.timer5p.Enabled = false;
                this.timer5p.Stop();
                this.timer10p.Enabled = false;
                this.timer10p.Stop();
                this.timerphut.Enabled = false;
                this.timerphut.Stop();
                this._isstart = true;
                this._listQty.Clear();
                this._currentHour = DateTime.Now.Hour;
                this.btStart_Click(null, null);
                this._currentHour = DateTime.Now.Hour;
            }
        }

        private void timerphut_Tick(object sender, EventArgs e)
        {
            while (this._ShowAdCurr.Count > 0)
            {
                this.CloseAd(this._ShowAdCurr.Dequeue());
                Thread.Sleep(0x3e8);
                Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                if (process != null)
                {
                    process.Kill();
                }
            }
            this.timerGiay.Enabled = false;
        }
    }
}

