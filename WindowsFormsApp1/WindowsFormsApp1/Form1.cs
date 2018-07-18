using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text==null|| textBox1.Text=="") {
                MessageBox.Show("没有输入端口号");
                return;
            }

            //进程id 由用户输入 必须是数字
            int pid = int.Parse(textBox1.Text);
            if (pid<=0) {
                MessageBox.Show("端口号必须大于0！");
            }
            
            Process pro = new Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.CreateNoWindow = true;
            pro.Start();
            pro.StandardInput.WriteLine("netstat -ano");
            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;
            Regex reg = new Regex("\\s+", RegexOptions.Compiled);

            string line = null;
            string dkStr = "";
            while ((line = pro.StandardOutput.ReadLine()) != null)
            {
                string[] lineArr = line.Split(':');
                line = line.Trim();
                if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))//选取tcp协议
                {
                    line = reg.Replace(line, ",");
                    string[] arr = line.Split(',');
                    string pidStr = "";
                    if (arr.Length>0) {
                        pidStr = arr[1].Split(':')[1];
                    }


                    if (pidStr== pid.ToString()) {
                        dkStr = arr[4];
                        pro.Start();
                        pro.StandardInput.WriteLine("taskkill /pid "+ dkStr + " -t -f");
                        pro.StandardInput.WriteLine("exit");
                    }
                }
            }
            pro.WaitForExit();//等待程序执行完退出进程
            pro.Close();
            if (dkStr.Length>0)
            {
                MessageBox.Show("已杀死端口:" + pid);
            }
            else {
                MessageBox.Show("找不到端口");
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
    }
}
