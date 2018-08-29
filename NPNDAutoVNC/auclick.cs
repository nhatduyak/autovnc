namespace NPNDAutoVNC
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class auclick : Form
    {
        private Button btstart;
        private IContainer components = null;
        private Label label1;
        private Label label2;
        public const int MOUSEEVENTF_LEFTDOWN = 2;
        public const int MOUSEEVENTF_LEFTUP = 4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_RIGHTDOWN = 8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private System.Windows.Forms.Timer timer1;

        public auclick()
        {
            this.InitializeComponent();
        }

        private void btstart_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = ((int) this.numericUpDown1.Value) * 0xea60;
            this.timer1.Enabled = true;
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            this.timer1_Tick(null, null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out MousePoint lpMousePoin);
        private void InitializeComponent()
        {
            this.components = new Container();
            this.numericUpDown1 = new NumericUpDown();
            this.label1 = new Label();
            this.btstart = new Button();
            this.numericUpDown2 = new NumericUpDown();
            this.label2 = new Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.numericUpDown1.BeginInit();
            this.numericUpDown2.BeginInit();
            base.SuspendLayout();
            this.numericUpDown1.Location = new Point(0x2c, 12);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size(0x38, 20);
            this.numericUpDown1.TabIndex = 0;
            int[] bits = new int[4];
            bits[0] = 2;
            this.numericUpDown1.Value = new decimal(bits);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(3, 14);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x23, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Phut :";
            this.btstart.Location = new Point(0x44, 40);
            this.btstart.Name = "btstart";
            this.btstart.Size = new Size(0x95, 0x1b);
            this.btstart.TabIndex = 2;
            this.btstart.Text = "Start";
            this.btstart.UseVisualStyleBackColor = true;
            this.btstart.Click += new EventHandler(this.btstart_Click);
            this.numericUpDown2.Location = new Point(0xb5, 12);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new Size(0x38, 20);
            this.numericUpDown2.TabIndex = 0;
            bits = new int[4];
            bits[0] = 1;
            this.numericUpDown2.Value = new decimal(bits);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x7b, 14);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x2b, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "So lan :";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
           // base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x107, 0x4d);
            base.Controls.Add(this.btstart);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.numericUpDown2);
            base.Controls.Add(this.numericUpDown1);
            base.Name = "auclick";
            this.Text = "auclick";
            this.numericUpDown1.EndInit();
            this.numericUpDown2.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private void sendMouseDoubleClick(Point p)
        {
            mouse_event(6, p.X, p.Y, 0, 0);
            Thread.Sleep(150);
            mouse_event(6, p.X, p.Y, 0, 0);
        }

        private void sendMouseLeftclick(Point p)
        {
            mouse_event(6, p.X, p.Y, 0, 0);
        }

        private void sendMouseRightclick(Point p)
        {
            mouse_event(0x18, p.X, p.Y, 0, 0);
        }

        private void sendMouseRightDoubleClick(Point p)
        {
            mouse_event(0x18, p.X, p.Y, 0, 0);
            Thread.Sleep(150);
            mouse_event(0x18, p.X, p.Y, 0, 0);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        public void Startad(Point Pad)
        {
            Thread.Sleep(0x7d0);
            SetCursorPos(Pad.X, Pad.Y);
            Thread.Sleep(500);
            this.sendMouseLeftclick(Pad);
            Thread.Sleep(300);
            this.sendMouseLeftclick(new Point(CTLConfig._PointAD.X, CTLConfig._PointAD.Y));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FileInfo[] files = new DirectoryInfo(Path.Combine(Application.StartupPath, "profile")).GetFiles();
            for (int i = 0; i < this.numericUpDown2.Value; i++)
            {
                int index = new Random().Next(files.Length);
                Process.Start(files[index].FullName);
                Thread.Sleep(0xfa0);
                this.Startad(new Point(CTLConfig._PointFirtApp.X, 60));
                this.Startad(new Point(CTLConfig._PointFirtApp.X, CTLConfig._PointAppCl.Y));
                this.Startad(new Point(0x27a, 0x177));
                Thread.Sleep(0x3e8);
                Process process = Process.GetProcessesByName(CTLConfig._ProcessName)[0];
                if (process != null)
                {
                    process.Kill();
                }
            }
        }
    }
}

