using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using Client.DBUtility;
using System.Net;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void BtUserLogin_Click(object sender, RoutedEventArgs e)
        {
            #region 取来账号和密码的信息
            string userID = TextUserID.Text.Trim().ToString();
            IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.TextUserPassword.SecurePassword);
            string userPassword= System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
            #endregion

            #region 连接数据库 完成登录验证
            SqlConnection sqlConnection = new SqlConnection(ConfigManager.ConnectStr);
            try
            {

                sqlConnection.Open();
                string sql = "select * from userInfo where userID=@userID and userPassword=@userPassword ";
                SqlParameter[] parameters = new SqlParameter[2] 
                { new SqlParameter("@userID", userID),
                    new SqlParameter("@userPassword", userPassword) };
                DataTable dataSet = SqlHelper.ExcuteTable(sql, CommandType.Text, parameters);
                sqlConnection.Close();

                #region 对返回的数据进行判断用户是否成功登录
                if (dataSet.Rows.Count == 0)
                {
                    MessageBox.Show("登录失败，请检查账号密码是否正确");
                }
                else
                {
                    #region 完成用户信息的读取
                    UserInfo.UserID = dataSet.Rows[0][0].ToString();
                    UserInfo.UserName = dataSet.Rows[0][2].ToString();
                    #endregion

                    #region 跳转用户用户界面，关闭登录窗口
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                    this.Close();//关闭当前窗口
                    #endregion

                    //写入日志
                    string log = string.Format("用户：{0}，登录该系统成功，登录ip:{1}\n", userID, IPAddress.Loopback);
                    WriteLog(log);
                }
                #endregion

            }
            catch
            {
                if (sqlConnection.State == ConnectionState.Open) sqlConnection.Close();
                WriteLog("登录系统异常\n");
            }
            #endregion


        }




        #region 关闭窗口，窗口最小化
        private void BtnWindowMini_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch { }
        }
        private void BtnWindowClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
            
        }
        #endregion
        #region 写系统日志的方法
        public void WriteLog(string msg)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("消息：" + msg);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (IOException e)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("异常：" + e.Message);
                    sw.WriteLine("时间：" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    sw.WriteLine("**************************************************");
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        #endregion

    }
}
