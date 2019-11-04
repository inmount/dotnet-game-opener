using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dpz3;

namespace opener {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

            // 定位配置文件
            string path = System.Environment.CurrentDirectory;
            if (!path.EndsWith("\\")) path += "\\";
            string configFilePath = path + "config.ini";

            // 判断配置文件是否存在
            if (!System.IO.File.Exists(configFilePath)) {
                MessageBox.Show("未发现配置文件");
                Application.Exit();
            }

            using (dpz3.File.ConfFile cfg = new dpz3.File.ConfFile(configFilePath)) {
                this.textBox1.Text = cfg.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e) {

            // 定位配置文件
            string path = System.Environment.CurrentDirectory;
            if (!path.EndsWith("\\")) path += "\\";
            string configFilePath = path + "config.ini";

            // 判断配置文件是否存在
            if (!System.IO.File.Exists(configFilePath)) {
                MessageBox.Show("未发现配置文件");
                Application.Exit();
            }

            // 在线程中处理启动
            var task = new Task(() => {

                using (dpz3.File.ConfFile cfg = new dpz3.File.ConfFile(configFilePath)) {

                    // 实例化一个进程启动器
                    var proStartInfo = new System.Diagnostics.ProcessStartInfo();
                    var pro = new System.Diagnostics.Process();

                    // 设置执行文件和命令行参数
                    var cfgDefault = cfg["Default"];
                    proStartInfo.UseShellExecute = false;
                    proStartInfo.WorkingDirectory = cfgDefault["WorkPath"];

                    string filePath = cfgDefault["FilePath"];

                    // 判断是否进行游戏程序复制
                    if (this.checkBox1.Checked) {
                        string newPath = filePath + "." + Guid.NewGuid().ToString() + ".exe";
                        System.IO.File.Copy(filePath, newPath);
                        filePath = newPath;
                    }
                    proStartInfo.FileName = filePath;
                    proStartInfo.Arguments = cfgDefault["Args"];

                    // 设置环境变量
                    var cfgEnvir = cfg["Envir"];
                    foreach (var key in cfgEnvir.Keys) {
                        proStartInfo.Environment.Add(key, cfgEnvir[key]);
                    }

                    // 启动游戏客户端
                    pro.StartInfo = proStartInfo;
                    pro.Start();
                    pro.WaitForExit();

                    // 如存在副本，则清理副本
                    if (this.checkBox1.Checked) {
                        System.IO.File.Delete(filePath);
                    }

                }

            });
            task.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {

        }
    }
}
