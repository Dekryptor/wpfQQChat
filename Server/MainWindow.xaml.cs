using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 定义，初始化
        //创建负责监听的socket
        IPAddress ipa = IPAddress.Loopback;
        IPEndPoint point = new IPEndPoint(IPAddress.Loopback, 6000);
        #region 声明回调函数
        private delegate void SetTextValueCallBack(string strValue);
        private SetTextValueCallBack setCallBack;
        private delegate void ReceiveMsgCallBack(string strReceive);
        private ReceiveMsgCallBack receiveCallBack;
        private delegate void SetCmbCallBack(string strItem);
        private SetCmbCallBack setCmbCallBack;
        private delegate void SendFileCallBack(byte[] bf);
        private SendFileCallBack sendCallBack;
        #endregion
        //将远程连接的客户端的IP地址和Socket存入集合中
        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();
        //创建监听连接的线程
        Thread AcceptSocketThread;
        //接收客户端发送消息的线程
        Thread ReceiveThread;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            #region  绑定回调函数
            setCallBack = new SetTextValueCallBack(SetTextValue);
            receiveCallBack = new ReceiveMsgCallBack(ReceiveMsg);
            setCmbCallBack = new SetCmbCallBack(AddCmbItem);
            sendCallBack = new SendFileCallBack(SendFile);
            #endregion
            //监听
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketWatch.Bind(point);
            socketWatch.Listen(100);
            SetTextValue("开启监听");

            //创建监听线程
            AcceptSocketThread = new Thread(new ParameterizedThreadStart(StartListen));
            AcceptSocketThread.IsBackground = true;
            AcceptSocketThread.Start(socketWatch);
        }
        /// <summary>
        /// 等待客户端的连接，并且创建与之通信用的Socket
        /// </summary>
        void StartListen(object o)
        {
            Socket socketWatch = o as Socket;
            //等待客户端的连接，并且创建一个负责通信的Socket
            while (true)
            {
                try
                {
                    Socket socketSend = socketWatch.Accept();
                    string strIP = socketSend.RemoteEndPoint.ToString();
                    dicSocket.Add(strIP, socketSend);
                    //UI控制
                    this.cmbSocket.Dispatcher.Invoke(setCallBack, strIP);
                    this.txtLog.Dispatcher.Invoke(setCallBack,
                        socketSend.RemoteEndPoint.ToString() + ":连接成功,时间：" + DateTime.Now.ToString());
                    //定义接受客户端消息的线程
                    ReceiveThread = new Thread(new ParameterizedThreadStart(Receive));
                    ReceiveThread.IsBackground = true;
                    ReceiveThread.Start(socketSend);
                }
                catch { }





                //将成功连接的客户端的ip地址和用户名，写入数据库，并保存在内存中,

            }

        }

        #region 接受客户端的消息，并进行处理
        private void Receive(Object o)
        {
            Socket socketReceive = o as Socket;
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    //实际接收到的字节数
                    int r = socketReceive.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    else
                    {
                        //自定义传输协议

                            string str = Encoding.UTF8.GetString(buffer, 1, r);
                            this.txtLog.Dispatcher.Invoke(receiveCallBack, "接收远程服务器:" + socketReceive.RemoteEndPoint + "发送的消息:" + str + "\n");
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
            }
        }

        #endregion


        #region 实例化回调函数
        private void SetTextValue(string strValue)
        {
            this.txtLog.AppendText(strValue + " \r \n");
        }
        private void ReceiveMsg(string strMsg)
        {

            this.txtLog.AppendText(strMsg + " \r \n");
        }
        private void AddCmbItem(string strItem)
        {
        }
        private void SendFile(byte[] sendBuffer)
        {
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


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
