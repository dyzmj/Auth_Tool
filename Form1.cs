using System;
using System.Data;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Auth_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 获取机器码
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 获取机器码
            string MachineCode = GetMachineCode.Value();

            if (string.IsNullOrEmpty(MachineCode))
            {
                MessageBox.Show("机器码获取失败，请联系管理员处理", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            textBox1.Text = MachineCode;
        }

        /// <summary>
        /// 导入证书
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == null || textBox2.Text.Length == 0)
            {
                MessageBox.Show("证书不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 写入注册表
            RegistryManager.WriteRegistryValue("AuthTool\\SDT", "MachineCode", textBox2.Text);

            // 导入证书
            MessageBox.Show("证书导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }



    /// <summary>
    /// Generates a 16 byte Unique Identification code of a computer
    /// Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
    /// </summary>
    public class GetMachineCode
    {
        private static string MachineCode = string.Empty;
        public static string Value()
        {
            if (string.IsNullOrEmpty(MachineCode))
            {
                // 根据机器名称、主机名称、MAC地址生成唯一证书
                string MachineName = Environment.MachineName;
                string ComputerName = SystemInformation.ComputerName;
                string MacAddress = GetMacAddr_Local();
                MachineCode = GetHash("Address >>" + MachineName + ComputerName + MacAddress);
            }
            return MachineCode;
        }

        /// <summary>
        /// 获取网卡物理地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddr_Local()
        {
            string madAddr = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc2 = mc.GetInstances();
            foreach (ManagementObject mo in moc2.Cast<ManagementObject>())
            {
                if (Convert.ToBoolean(mo["IPEnabled"]) == true)
                {
                    madAddr = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            return madAddr;
        }
        private static string GetHash(string s)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(MD5.Create().ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #region Original Device ID Getting Code
        //Return a hardware identifier
        #endregion
    }


    /// <summary>
    /// 提供与Windows注册表交互的功能。
    /// </summary>
    public class RegistryManager
    {
        /// <summary>
        /// 从Windows注册表中读取值。
        /// </summary>
        /// <param name="keyPath">注册表键的路径。</param>
        /// <param name="valueName">注册表值的名称。</param>
        /// <returns>从注册表中读取的值，如果未找到则为null。</returns>
        public static object ReadRegistryValue(string keyPath, string valueName)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath);
                if (key != null)
                {
                    return key?.GetValue(valueName);
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                //这个地方可按照业务自行处理
                Console.WriteLine($"读取注册表值时出错: {ex.Message}");
                return ex.Message;
            }
        }

        /// <summary>
        /// 向Windows注册表写入值。
        /// </summary>
        /// <param name="keyPath">注册表键的路径。</param>
        /// <param name="valueName">注册表值的名称。</param>
        /// <param name="value">要写入注册表的值。</param>
        public static void WriteRegistryValue(string keyPath, string valueName, object value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);
                key?.SetValue(valueName, value);
            }
            catch (Exception ex)
            {
                //这个地方可按照业务自行处理
                Console.WriteLine($"写入注册表值时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 从Windows注册表中删除值。
        /// </summary>
        /// <param name="keyPath">注册表键的路径。</param>
        /// <param name="valueName">注册表值的名称。</param>
        public static void DeleteRegistryValue(string keyPath, string valueName)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true);
                key?.DeleteValue(valueName);
            }
            catch (Exception ex)
            {
                //这个地方可按照业务自行处理
                Console.WriteLine($"删除注册表值时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查注册表键是否存在。
        /// </summary>
        /// <param name="keyPath">注册表键的路径。</param>
        /// <returns>如果注册表键存在，则为true；否则为false。</returns>
        public static bool RegistryKeyExists(string keyPath)
        {
            return Registry.CurrentUser.OpenSubKey(keyPath) != null;
        }
    }

}
