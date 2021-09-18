using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.WinForms;
using System.Windows.Forms;
using jhrs.com.SiteMange.Extensions;
using Microsoft.Web.Administration;
using System.IO;

namespace jhrs.com.SiteMange
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser browser;
        public MainForm()
        {
            InitializeComponent();
            this.richTextBox1.LinkClicked += RichTextBox1_LinkClicked;
        }

        private TabPage blog;
        private void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (blog != null)
            {
                tabControl1.SelectedTab = blog;
            }
            else
            {
                blog = new TabPage("江湖人士");
                tabControl1.TabPages.Add(blog);
                var browser = new ChromiumWebBrowser(e.LinkText);
                browser.Dock = DockStyle.Fill;
                tabControl1.SelectedTab = blog;
                tabControl1.SelectedTab.Controls.Add(browser);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox2.BindEnum<ManagedPipelineMode>();
            comboBox1.BindEnum<ProcessModelIdentityType>();

            browser = new ChromiumWebBrowser("https://jhrs.com/zt/wa");
            browser.Dock = DockStyle.Fill;
            tabPage1.Controls.Add(browser);

            richTextBox2.Text = richTextBox3.Text = richTextBox4.Text = richTextBox1.Text = @"1、当前程序使用 vs2019， .net 5 开发
2、第2个Tab选项卡界面创建的IIS程序池是供第3个选项卡添加网站使用的，实际项目中，你自己指定一个IIS上已经存在的应用程序池也是可以的，不过最建的建议是为每个网站创建新的应用程序池，包括网站里面嵌套的应用程序。
3、创建应用程序池时，只要是通过手工打开IIS添加应用程序池设置的参数，都可以通过代码完成。示例程序只是演示一点点功能而已。
4、添加网站时，可以绑定多个IP，域名，端口；在IIS上咋玩的，代码也是可以玩出这种效果地。示例程序只提供了绑定一个域名IP而已，实际中有多个的话，循环一下就完事了。
5、虚拟目录和应用程序可以任意嵌套，它们的创建也是可以通过代码来完成。
6、完整的示例代码参见博客文章地址：https://jhrs.com";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入网站名称");
                return;
            }
            if (textBox6.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请指定网站路径");
                return;
            }
            if (comboBox3.SelectedItem == null)
            {
                MessageBox.Show("请选择应用程序池，如无可选项，请先创建！");
                return;
            }
            try
            {
                ServerManager serverManager = new ServerManager();
                Site site = serverManager.Sites.Add(textBox1.Text.Trim(), textBox6.Text, int.Parse(textBox13.Text));
                site.ServerAutoStart = true;
                site.Applications[0].ApplicationPoolName = comboBox3.SelectedText;

                site.Bindings.Clear();
                site.Bindings.Add($"{textBox5.Text}:{textBox13.Text}:{textBox2.Text}", "http");  //ip:端口:域名

                serverManager.CommitChanges();

                site.Start();
                MessageBox.Show("创建网站成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建网站过程出错，原因：{ex.Message}");
            }
        }

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ServerManager serverManager = new ServerManager();
            if (textBox4.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入应用程序池名称");
                return;
            }
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("请选择托管模式");
                return;
            }
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("请选择应用程序标识");
                return;
            }
            var appool = serverManager.ApplicationPools.Add(textBox4.Text);
            appool.ManagedPipelineMode = (ManagedPipelineMode)Enum.Parse(typeof(ManagedPipelineMode), comboBox2.SelectedItem.ToString());
            appool.ProcessModel.IdentityType = (ProcessModelIdentityType)Enum.Parse(typeof(ProcessModelIdentityType), comboBox1.SelectedItem.ToString());
            serverManager.CommitChanges();

            MessageBox.Show("创建应用程序池成功");
        }

        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入虚拟目录名称");
                return;
            }
            if (textBox7.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入虚拟目录路径");
                return;
            }
            if (!Directory.Exists(textBox7.Text))
            {
                MessageBox.Show("不要乱输,虚拟目录必须存在!");
                return;
            }
            try
            {
                ServerManager serverManager = new ServerManager();
                var app = serverManager.Sites[comboBox4.SelectedText].Applications[0];
                app.VirtualDirectories.Add($"/{textBox3.Text}", textBox7.Text);

                serverManager.CommitChanges();

                MessageBox.Show("虚拟目录创建成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建虚拟目录出错，原因：{ex.Message}");
            }
        }

        /// <summary>
        /// 虚拟目录下创建应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox4.SelectedText))
            {
                MessageBox.Show("请选择隶属网站，无可选项时请先添加一个网站！");
                return;
            }
            if (textBox3.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入虚拟目录名称！");
                return; 
            }
            if (textBox14.Text.IsNullOrWhiteSpace())
            {
                MessageBox.Show("请输入应用程序名称！");
                return;
            }
            if (!Directory.Exists(textBox15.Text))
            {
                MessageBox.Show("不要乱输，应用程序目录必须存在！");
                return;
            }

            try
            {
                ServerManager serverManager = new ServerManager();
                var site = serverManager.Sites[comboBox4.SelectedText];
                var app = site.Applications.Add($"/{textBox3.Text}/{textBox14.Text}", textBox15.Text);
                app.ApplicationPoolName = textBox4.Text;  //设置应用程序池
                serverManager.CommitChanges();

                MessageBox.Show("虚拟目录下创建应用程序池成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"虚拟目录下创建应用程序池出错，原因：{ex.Message}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox5.SelectedText))
            {
                MessageBox.Show("请选择隶属网站，无可选项时请先添加一个网站！");
                return;
            }
            try
            {
                ServerManager serverManager = new ServerManager();
                var site = serverManager.Sites[comboBox5.SelectedText];
                var app = site.Applications.Add($"/{textBox12.Text}", textBox11.Text);
                app.ApplicationPoolName = textBox4.Text;  //设置应用程序池
                serverManager.CommitChanges();

                MessageBox.Show("站点根目录下创建应用程序池成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"站点根目录下创建应用程序池出错，原因：{ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox6.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServerManager serverManager = new ServerManager();
            if (tabControl1.SelectedIndex == 2)
            {
                comboBox3.Items.Clear();
                foreach (var item in serverManager.ApplicationPools)
                {
                    comboBox3.Items.Add(item.Name);
                }
            }
            if (tabControl1.SelectedIndex == 3)
            {
                comboBox4.Items.Clear();
                foreach (var item in serverManager.Sites)
                {
                    comboBox4.Items.Add(item.Name);
                }
            }
            if (tabControl1.SelectedIndex == 4)
            {
                comboBox5.Items.Clear();
                foreach (var item in serverManager.Sites)
                {
                    comboBox5.Items.Add(item.Name);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox7.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox15.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox11.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}