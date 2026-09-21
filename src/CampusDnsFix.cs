using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyTitle("北工大校园网 DNS 修复")]
[assembly: System.Reflection.AssemblyDescription("一键恢复 Wi-Fi 自动 DNS 并打开北工大校园网登录页")]
[assembly: System.Reflection.AssemblyCompany("国产林林漆")]
[assembly: System.Reflection.AssemblyProduct("北工大校园网 DNS 修复")]
[assembly: System.Reflection.AssemblyCopyright("Copyright © 2026 国产林林漆")]
[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("1.0.0.0")]

namespace CampusDnsFix
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    internal sealed class MainForm : Form
    {
        private readonly Button repairButton;
        private readonly Label statusLabel;

        public MainForm()
        {
            Text = "北工大校园网 DNS 修复";
            ClientSize = new Size(470, 315);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            Label title = new Label();
            title.Text = "北工大校园网 DNS 修复";
            title.Font = new Font("Microsoft YaHei UI", 17F, FontStyle.Bold, GraphicsUnit.Point);
            title.ForeColor = Color.FromArgb(24, 74, 135);
            title.AutoSize = true;
            title.Location = new Point(32, 26);
            Controls.Add(title);

            Label description = new Label();
            description.Text = "将当前 Wi-Fi 的 DNS 恢复为自动（DHCP），清空 DNS 缓存，\r\n然后打开 lgn.bjut.edu.cn。不会关闭或修改 VPN。";
            description.AutoSize = true;
            description.Location = new Point(34, 78);
            description.ForeColor = Color.FromArgb(55, 55, 55);
            Controls.Add(description);

            repairButton = new Button();
            repairButton.Text = "一键修复并打开登录页";
            repairButton.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            repairButton.Size = new Size(280, 52);
            repairButton.Location = new Point(94, 137);
            repairButton.BackColor = Color.FromArgb(24, 113, 190);
            repairButton.ForeColor = Color.White;
            repairButton.FlatStyle = FlatStyle.Flat;
            repairButton.FlatAppearance.BorderSize = 0;
            repairButton.Click += RepairButtonClick;
            Controls.Add(repairButton);

            statusLabel = new Label();
            statusLabel.Text = "准备就绪";
            statusLabel.AutoSize = false;
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Location = new Point(24, 205);
            statusLabel.Size = new Size(422, 34);
            statusLabel.ForeColor = Color.FromArgb(80, 80, 80);
            Controls.Add(statusLabel);

            Label note = new Label();
            note.Text = "提示：启动时出现管理员确认属于正常现象。";
            note.AutoSize = true;
            note.Location = new Point(94, 249);
            note.ForeColor = Color.FromArgb(120, 120, 120);
            note.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            Controls.Add(note);

            Label author = new Label();
            author.Text = "作者：国产林林漆";
            author.AutoSize = true;
            author.Location = new Point(176, 278);
            author.ForeColor = Color.FromArgb(90, 90, 90);
            author.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            Controls.Add(author);
        }

        private void RepairButtonClick(object sender, EventArgs e)
        {
            repairButton.Enabled = false;
            statusLabel.ForeColor = Color.FromArgb(80, 80, 80);
            statusLabel.Text = "正在查找 Wi-Fi 并恢复自动 DNS……";
            Refresh();

            try
            {
                NetworkInterface wifi = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    .OrderByDescending(n => n.OperationalStatus == OperationalStatus.Up)
                    .FirstOrDefault();

                if (wifi == null)
                    throw new InvalidOperationException("没有找到 Wi-Fi 网卡。请确认无线网卡已启用。");

                IPv4InterfaceProperties ipv4 = wifi.GetIPProperties().GetIPv4Properties();
                if (ipv4 == null)
                    throw new InvalidOperationException("无法读取 Wi-Fi 的 IPv4 配置。");

                int exitCode = RunPowerShell(ipv4.Index);
                if (exitCode != 0)
                    throw new InvalidOperationException("系统未能修改 DNS，错误代码：" + exitCode);

                statusLabel.ForeColor = Color.FromArgb(25, 125, 65);
                statusLabel.Text = "修复完成，正在打开校园网登录页……";
                Process.Start("http://lgn.bjut.edu.cn/");
            }
            catch (Exception ex)
            {
                statusLabel.ForeColor = Color.FromArgb(180, 45, 45);
                statusLabel.Text = "修复失败";
                MessageBox.Show(
                    ex.Message + "\r\n\r\n如果仍无法访问，请检查 FlClash 是否把 *.bjut.edu.cn 设为直连。",
                    "北工大校园网 DNS 修复",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                repairButton.Enabled = true;
            }
        }

        private static int RunPowerShell(int interfaceIndex)
        {
            string command = string.Format(
                "Set-DnsClientServerAddress -InterfaceIndex {0} -ResetServerAddresses -ErrorAction Stop; " +
                "Clear-DnsClientCache -ErrorAction SilentlyContinue",
                interfaceIndex);

            string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(command));
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "powershell.exe";
            info.Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + encoded;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            using (Process process = Process.Start(info))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }
    }
}
