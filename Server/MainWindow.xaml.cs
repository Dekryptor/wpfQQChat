

using MessageDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        IPEndPoint point = new IPEndPoint(IPAddress.Loopback, 6666);
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
            SetTextValue(string.Format("服务器{0}:开启监听",socketWatch.LocalEndPoint.ToString()));

            //创建监听线程
            AcceptSocketThread = new Thread(new ParameterizedThreadStart(StartListen));
            AcceptSocketThread.IsBackground = true;
            AcceptSocketThread.Start(socketWatch);

            this.cmbSocket.ItemsSource = dicSocket.Keys;

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
                    //dicSocket.Add(strIP, socketSend);
                    //UI控制
                    this.txtLog.Dispatcher.Invoke(setCallBack, socketSend.RemoteEndPoint.ToString() + "连接成功");
                    WriteLog(socketSend.RemoteEndPoint.ToString() + "接入服务器");

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
            string userid="";
            try
            {
                while (true)
                {
                    //将接收到额buffer转为Message
                    byte[] buffer = new byte[1024*1024*2];
                    int len = socketReceive.Receive(buffer);

                    if (len == 0)
                    {
                        break;
                    }
                    else
                    {
                        //反序列化，将buffer转化为类
                        WriteLog(string.Format("接收到来自客户端:{0} 的数据", socketReceive.RemoteEndPoint));

                        byte[] bufferReceive = new byte[len];
                        for(int i = 0; i < len; i++)
                        {
                            bufferReceive[0] = buffer[0];
                        }
                        Message messageReceive = SerializationUnit.DeserializeObject(buffer) as Message;
                        string fromClient = messageReceive.FromClient; //从哪里发过来
                        string toClient = messageReceive.ToClient;//要发到哪去
                        userid = messageReceive.Sign;//做一个标记

                        #region 根据协议处理数据
                        switch (messageReceive.Sign)
                        {
                            #region 001 完成用户ID与端口的绑定
                            case "001":
                                //数据逻辑处理
                                dicSocket.Add(messageReceive.FromClient, socketReceive);//将用户id与ip地址绑定

                                //UI处理以及日志
                                this.txtLog.Dispatcher.Invoke(receiveCallBack, string.Format("登录用户ID:{0}\n登录的ip地址:{1}",fromClient, socketReceive.RemoteEndPoint.ToString()));
                                //this.cmbSocket.Dispatcher.Invoke(setCmbCallBack, messageReceive.FromClient);//ui控制
                                WriteLog(string.Format("登录用户ID:{0}\n登录的ip地址:{1}", fromClient, socketReceive.RemoteEndPoint.ToString()));
                                break;
                            #endregion
                            #region 101 完成客户端数据的转发
                            case "101":
                                if (dicSocket.ContainsKey(fromClient))
                                {
                                    //将对象重新序列化
                                    byte[] sendByte = SerializationUnit.SerializeObject(messageReceive);
                                    dicSocket[toClient].Send(sendByte);
                                    //写日志
                                    this.txtLog.Dispatcher.Invoke(receiveCallBack,
                                        string.Format("fromClient:{0}\ntoClient:{1}\nMessage:{2}\n",
                                        fromClient, toClient, messageReceive.Msg));
                                    WriteLog(string.Format("fromClient:{0}\ntoClient:{1}\nMessage:{2}\n",
                                        fromClient, toClient, messageReceive.Msg));
                                }
                                else
                                {
                                    MessageBox.Show("服务器找不到该用户");
                                }
                                break;
                            #endregion
                            #region 201 发送文件的事件
                            case "201":
                                if (dicSocket.ContainsKey(fromClient))
                                {
                                    //将对象重新序列化
                                    byte[] sendByte = SerializationUnit.SerializeObject(messageReceive);
                                    dicSocket[toClient].Send(sendByte);
                                    //写日志
                                    this.txtLog.Dispatcher.Invoke(receiveCallBack,
                                        string.Format("fromClient:{0}\ntoClient:{1}\nMessage:{2}\n",
                                        fromClient, toClient, "转发文件成功"));
                                    WriteLog(string.Format("fromClient:{0}\ntoClient:{1}\nMessage:{2}\n",
                                        fromClient, toClient, "转发文件成功"));
                                }
                                else
                                {
                                    MessageBox.Show("服务器找不到该用户");
                                }
                                break;
                            #endregion
                            case "301":
                                if (dicSocket.ContainsKey(fromClient))
                                {
                                    //将对象重新序列化
                                    byte[] sendByte = SerializationUnit.SerializeObject(messageReceive);
                                    //转发数据
                                    dicSocket[toClient].Send(sendByte);
                                    //写日志
                                    this.txtLog.Dispatcher.Invoke(receiveCallBack,
                                        string.Format("fromClient:{0}\ntoClient:{1}\nRequest:{2}\n",
                                        fromClient, toClient, messageReceive.Request));
                                    WriteLog(string.Format("fromClient:{0}\ntoClient:{1}\nRequest:{2}\n",
                                        fromClient, toClient, messageReceive.Request));
                                }
                                else
                                {
                                    MessageBox.Show("服务器找不到该用户");
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion

                        //进行内存回收
                        GC.Collect();
                    }


                }
            }
            catch (Exception ex)
            {
                dicSocket.Remove(userid);
                //MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
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

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void BtnSendMsg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.cmbSocket.Text.Trim() == "")
                {
                    MessageBox.Show("请选择目标EndPoint");
                    return;
                }

                string ipSend = this.cmbSocket.Text.Trim();
                #region 对消息的处理
                string strMsg = txtMsg.Text.Trim().ToString();//得到需要发送的消息
                Message messageSend = new Message("000", "127.0.0.1:6666", ipSend, strMsg);
                byte[] bufferSend = SerializationUnit.SerializeObject(messageSend);
                dicSocket[ipSend].Send(bufferSend);
                //发送完成，清空文本输入窗口
                txtMsg.Text = "";
                WriteLog(string.Format("{0} To {1} 发送:{2} 成功", messageSend.FromClient, ipSend, messageSend.Msg));
                #endregion

            }
            catch
            {
                MessageBox.Show("给客户端发送消息错误.");

            }
        }
    }
}
