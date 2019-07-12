
using MessageDLL;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using ViewModes;

namespace Client
{
    /// <summary>
    /// UserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserWindow : Window
    {

        public void aaa()
        {
        }
        
        public void bb()
        {

        }

        #region 定义
        Dictionary<string, UserChatWindow> dicChatWindow = new Dictionary<string, UserChatWindow>();

        //声明一个设置窗口值的回调
        private delegate void SetTextCallBack(string strValue);
        private SetTextCallBack setCallBack;

        //定义一个委托，判断聊天窗口是否关闭
        private delegate void CloseChatWindow(string friendID);
        private event CloseChatWindow closeChatWindow;

        //声明Socket
        Socket socketLine;
        //声明服务器的ip地址
        static IPAddress ipa = IPAddress.Parse("127.0.0.1");
        IPEndPoint ip = new IPEndPoint(ipa, 6666);
        //声明客户端接受服务器发来消息的线程
        private Thread threadReceive;
        #endregion

        public UserWindow()
        {

            //一个无关的东西
            //测试github



            InitializeComponent();

            #region 实例化回调

            #endregion

            //连接服务器
            try
            {
                socketLine=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketLine.Connect(ip);
                //开启一个新的线程不停的接收服务器发送消息的线程
                threadReceive = new Thread(new ParameterizedThreadStart(Receive));
                //设置为后台线程
                threadReceive.IsBackground = true;
                threadReceive.Start(socketLine);
                WriteLog(string.Format("用户：{0},连接远程服务器成功", UserInfo.UserID));

            }
            catch { WriteLog(string.Format("用户：{0},连接服务器失败", UserInfo.UserID)); }

            
            this.DataContext = new MainWindowViewModel();//初始化测试数据


        }

        private void NavBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            MainWindowViewModel.addFriend();
        }


        //创建聊天对象窗口
        private void BtnOpenChatWindow_Click(object sender, RoutedEventArgs e)
        {
            //得到要与之聊天的用户id
            string friendID = this.txtFriendID.Text.Trim().ToString();
            string friendName = this.txtFriendName.Text.Trim().ToString();
            //创建聊天对象，并保存窗体对象           
            UserChatWindow userChatWindow = new UserChatWindow
            {
                Owner = this,
                Title = string.Format("{0}({1})", friendName, friendID),
                FriendID = friendID//保存该聊天对象的id
            };

            #region 绑定子窗口与父窗口的事件
            userChatWindow.sendMessageEvent += this.SendMessage;
            userChatWindow.sendFlieEvent += this.SendFile;
            userChatWindow.sendRequest += this.SendRequest;
            #endregion

            //保存该窗口对象
            dicChatWindow.Add(friendID, userChatWindow);

            userChatWindow.Show();//打开该窗口

        }

        #region 接受服务器消息的方法
        private void Receive(object o)
        {
            Socket socketReceive = o as Socket;
            try
            {
                while (true)
                {
                    //判断接受buffer是否为空
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int len = socketReceive.Receive(buffer);

                    if (len == 0)
                    {   //如果得到字符为空，则代表没有接收到数据
                        break;
                    }
                    else
                    {

                        WriteLog("接收到来自服务器的数据");
                        byte[] bufferReceive = new byte[len];
                        for (int i = 0; i < len; i++)
                        {
                            bufferReceive[0] = buffer[0];
                        }
                        Message messageReceive = SerializationUnit.DeserializeObject(buffer) as Message;
                        string fromFriendID = messageReceive.FromClient;//消息来自哪一个客户端

                        #region 根据返回的类型进行消息操作
                        switch (messageReceive.Sign)
                        {
                            #region 000为来自服务器的数据
                            case "000":
                                break;
                            #endregion
                            #region 101客户端与客户端之间的普通聊天
                            case "101":
                                //p判断该聊天窗口是否存在，存在则直接调用
                                if (this.dicChatWindow.ContainsKey(fromFriendID))
                                {
                                    dicChatWindow[fromFriendID].Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                dicChatWindow[fromFriendID].ChangeTxtMsgLog(
                                                    string.Format("{0}: {1}\n{2}\n", dicChatWindow[fromFriendID].Title.ToString(),DateTime.Now.ToString() ,messageReceive.Msg));
                                            }
                                            )
                                        );
                                    //写日志
                                    WriteLog(string.Format("fromClient:{1}\nMessage:{1}", fromFriendID, messageReceive.Msg));
                                }
                                else
                                {
                                    //该窗口不存在，则打开新的窗口，在进行操作
                                    MessageBox.Show("未完成的工作");
                                }
                                break;
                            #endregion
                            #region 201 接受图片的事件
                            case "201":
                                if (this.dicChatWindow.ContainsKey(fromFriendID))
                                {
                                    dicChatWindow[fromFriendID].Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                dicChatWindow[fromFriendID].ChangeTxtMsgLog(
                                                string.Format("{0}:\n{1}\n", dicChatWindow[fromFriendID].Title.ToString(), "接受图片的事件"));
                                            }
                                            )
                                        );

                                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                                    saveFileDialog.InitialDirectory = @"C:\Users\58317\Desktop";
                                    saveFileDialog.Title = "请选择要保存的文件";
                                    saveFileDialog.Filter = "所有文件|*.*";
                                    saveFileDialog.ShowDialog();
                                    string path = saveFileDialog.FileName;
                                    using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        fileStream.Write(messageReceive.BufferFile,0,messageReceive.BufferFile.Length);
                                    }

                                    WriteLog(string.Format("fromClient:{1}\nMessage:{1}", fromFriendID, "接受图片的事件"));
                                }
                                else
                                {
                                    //该窗口不存在，则打开新的窗口，在进行操作
                                    MessageBox.Show("未完成的工作");
                                }
                                break;
                            #endregion
                            #region 301 客户端对客户端的请求事件
                            case "301":
                                if (this.dicChatWindow.ContainsKey(fromFriendID))
                                {
                                    #region 判断请求的类型，分别执行不同的操作
                                    switch (messageReceive.Request)
                                    {
                                        case 1:
                                            dicChatWindow[fromFriendID].Dispatcher.Invoke(
                                                new Action(
                                                    delegate
                                                    {
                                                        dicChatWindow[fromFriendID].ChangeTxtMsgLog(
                                                            string.Format("来自{0}: {1}\n请求震动事件\n", dicChatWindow[fromFriendID].Title.ToString(), DateTime.Now.ToString()));
                                                        dicChatWindow[fromFriendID].ZD();
                                                    }
                                                    )
                                                );
                                            //写日志
                                           // WriteLog(string.Format("fromClient:{1}\n请求震动事件", fromFriendID));
                                            break;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //该窗口不存在，则打开新的窗口，在进行操作
                                    MessageBox.Show("未完成的工作");
                                }
                                break;
                            #endregion
                            default:
                                break;

                        }
                    }
                    #endregion

                    //进行内存回收
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收服务端发送的消息出错:" + ex.ToString());
            }
        }
        #endregion

        #region 给好友发送消息的方法
        public void SendMessage(string toClient,string msg)
        {
            //包装消息
            Message messageSend = new Message("101", UserInfo.UserID, toClient, msg);
            byte[] bufferSend = SerializationUnit.SerializeObject(messageSend);
            socketLine.Send(bufferSend);
        }
        #endregion
        #region 给好友发送文件的方法
        public void SendFile(string toClient,byte[] buffer,int r)
        {
            //包装消息
            Message messageSendFile = new Message("201", UserInfo.UserID, toClient, buffer);
            byte[] bufferSend = SerializationUnit.SerializeObject(messageSendFile);
            socketLine.Send(bufferSend);
        }
        #endregion
        #region 给好友发送请求的方法
        public void SendRequest(string toClient,int n)
        {
            Message messageSendRequest = new Message("301", UserInfo.UserID, toClient, n);
            byte[] bufferSend = SerializationUnit.SerializeObject(messageSendRequest);
            socketLine.Send(bufferSend);
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
        #region 窗口的最大化、最小化
        private void BtnWindowMini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnWindowMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

        }

        private void BtnWindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        //在窗口加载的时候,像服务器发送第一次数据
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string userId = UserInfo.UserID;
            Message messageSend = new Message("001", userId, socketLine.LocalEndPoint.ToString(), "绑定用户ip地址");
            byte[] bufferSend = SerializationUnit.SerializeObject(messageSend);
            socketLine.Send(bufferSend);
        }

    }
}
