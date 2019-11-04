using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client007 {
    public partial class Form1 : Form {

        private string[] args;

        public Form1() {
            InitializeComponent();
        }

        // 设置参数
        public void setArgs(string[] parems) {
            args = parems;
        }

        private void button1_Click(object sender, EventArgs e) {

            // 定位文件
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string path = System.IO.Path.GetDirectoryName(exePath);
            if (!path.EndsWith("\\")) path += "\\";
            string configFilePath = path + "config.ini";

            dpz3.File.UTF8File.WriteAllText(configFilePath, this.textBox1.Text);

            MessageBox.Show("参数配置保存成功，请将config.ini文件拷贝到opener目录使用");

        }

        private void Form1_Load(object sender, EventArgs e) {

            using (dpz3.File.ConfFile cfg = new dpz3.File.ConfFile()) {

                // 读取命令行参数
                StringBuilder sbArgs = new StringBuilder();
                for (int i = 0; i < args.Length; i++) {
                    if (sbArgs.Length > 0) sbArgs.Append(" ");
                    sbArgs.Append("\"");
                    sbArgs.Append(args[i]);
                    sbArgs.Append("\"");
                }

                // 设置执行文件和命令行参数
                var cfgDefault = cfg["Default"];
                cfgDefault["FilePath"] = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                cfgDefault["WorkPath"] = System.IO.Directory.GetCurrentDirectory();
                cfgDefault["Args"] = sbArgs.ToString();

                // 设置环境变量
                var cfgEnvir = cfg["Envir"];
                var envirs = Environment.GetEnvironmentVariables();
                foreach (var key in envirs.Keys) {
                    cfgEnvir[key.ToString()] = envirs[key].ToString();
                }

                this.textBox1.Text = cfg.ToString();

            }
        }
    }
}
