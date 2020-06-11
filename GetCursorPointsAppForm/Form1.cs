using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace GetCursorPointsAppForm
{
    public partial class Form1 : Form
    {

        private static int LeftDown = 0x0002;
        private static int LeftUp = 0x0004;

        public Form1()
        {
            InitializeComponent();

            LoadConfiguration();

            Process[] processes = Process.GetProcesses();

            //hWnd = FindWindow(null, "FiveLive");
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("user32.dll")]
        //public static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);



        private const int VK_LCONTROL = 0xA2; //Left Control key code
        private const int V = 0x56; //V key code

        private DateTime relaunchTime;

        //IntPtr hWnd = IntPtr.Zero;

        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {

            TimeSpan diff = DateTime.Now - relaunchTime;

            if (diff.Minutes >= 0 && diff.Minutes <= 5)
            {
                timer1.Enabled = false;
                Relaunch();
            }

            //hWnd = FindWindow(null, "FiveLive");
            ////hWnd = FindWindow(null, "FiveLive");
            //RECT rc;
            //GetWindowRect(hWnd, out rc);
            //Rectangle rect = new Rectangle(rc.left, rc.top, rc.right, rc.bottom);


            //Text = $"{Cursor.Position.X},{Cursor.Position.Y}";

            //Point p = Cursor.Position;
            //if (rect.Contains(p))
            //{
            //label1.Text = (Cursor.Position.X).ToString();
            //label2.Text = (Cursor.Position.Y).ToString();
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text.Equals("Start"))
            {
                button1.Text = "Stop";
                SaveConfiguration();
                HideToSystemTray();
                relaunchTime = dateTimePicker1.Value;
                timer1.Enabled = true;
            }
            else
            {
                button1.Text = "Start";
                timer1.Enabled = false;
            }
            


            //SetForegroundWindow(hWnd);
            //mouse_event(0x0001, 50, 0, 0, 0);
            //mouse_event(LeftDown | LeftUp, 50, 0, 0, 0);
            //mouse_event(LeftDown | LeftUp, 50, 0, 0, 0);

            //System.Threading.Thread.Sleep(10000);

            //mouse_event(0x0001, 0, 500, 100, 0);
            //mouse_event(LeftDown | LeftUp, 0, 500, 100, 0);

        }

        private void Relaunch()
        {
            button1.Text = "Start";
            CloseGame();
            System.Threading.Thread.Sleep(10000);
            CloseLauncher();
            System.Threading.Thread.Sleep(10000);
            StartLauncher();
        }

        private void StartLauncher()
        {
            Process.Start(PathTextBox.Text);
            System.Threading.Thread.Sleep(1000 * Convert.ToInt32(waitLauncherBox.Text));
            StartGame();
        }


        private void StartGame()
        {
            MooveMouse(750, 380);
            LeftButtonClick();
            System.Threading.Thread.Sleep(1000 * Convert.ToInt32(waitGameBox.Text));
            InputLoginData();
        }

        private void InputLoginData()
        {
            MooveMouse(766, 354);
            LeftButtonClick();
            Clipboard.Clear();
            Clipboard.SetText(LoginTextBox.Text);
            System.Threading.Thread.Sleep(5000);
            Paste();
            SendKeys.SendWait("{TAB}");
            Clipboard.Clear();
            Clipboard.SetText(PasswordTextBox.Text);
            System.Threading.Thread.Sleep(5000);
            Paste();
            SendKeys.SendWait("{TAB}");
            System.Threading.Thread.Sleep(5000);
            SendKeys.SendWait("{ENTER}");
            MooveMouse(346, 536);
            System.Threading.Thread.Sleep(5000);
            LeftButtonClick();
            System.Threading.Thread.Sleep(5000);
            LeftButtonClick();
        }

        private void LeftButtonClick()
        {
            mouse_event(LeftDown | LeftUp, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            //mouse_event(LeftDown | LeftUp, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        private void MooveMouse(int x, int y)
        {
            //mouse_event(0x0001, x, y, 0, 0);

            Cursor.Position = new Point(x, y);
        }

        private void Paste()
        {
            keybd_event(VK_LCONTROL, 0, 0, 0);
            keybd_event(V, 0, 0, 0);
            keybd_event(V, 0, 2, 0);
            keybd_event(VK_LCONTROL, 0, 2, 0);
        }

        private void CloseLauncher()
        {
            foreach (Process process in Process.GetProcessesByName("FiveLive Launcher"))
            {
                process.Kill();
            }
        }

        private void CloseGame()
        {
            foreach (Process process in Process.GetProcessesByName("GTA5"))
            {
                process.Kill();
            }
        }

        private void LoadConfiguration()
        {
            if (File.Exists("./configs.txt"))
            {
                using (StreamReader sr = new StreamReader("./configs.txt"))
                {
                    Configs configs = JsonConvert.DeserializeObject<Configs>(sr.ReadToEnd());

                    PathTextBox.Text = configs.path;
                    waitLauncherBox.Text = configs.waitLauncher.ToString();
                    waitGameBox.Text = configs.waitGame.ToString();
                    LoginTextBox.Text = configs.login;
                    PasswordTextBox.Text = configs.password;
                    dateTimePicker1.Value = configs.relaunchTime;
                }
            }
        }

        private void SaveConfiguration()
        {
            Configs configs = new Configs(PathTextBox.Text, Convert.ToInt32(waitLauncherBox.Text), Convert.ToInt32(waitGameBox.Text),
                LoginTextBox.Text, PasswordTextBox.Text, dateTimePicker1.Value);


            using (StreamWriter sw = new StreamWriter("./configs.txt"))
            {
                sw.WriteLine(JsonConvert.SerializeObject(configs));
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideToSystemTray();
            }
        }

        private void HideToSystemTray()
        {

            Hide();
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(1000);

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}
