using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Client
{
    /// <summary>
    /// UserChatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserChatWindow : Window
    {
        #region 声明回调函数
        //用户在父窗口接收到数据时候，返回数据，对子窗体进行数据传递
        public delegate void ReceiveMsgCallBack(string strReceive);
        public ReceiveMsgCallBack receiveCallBack;
        //将子窗口的聊天信息传递给父窗口，进行转发
        public delegate void SendMessage(string toClient, string msg);
        public event SendMessage sendMessageEvent;
        //发送文件
        public delegate void SendFile(string toClient, byte[] buffer, int r);
        public event SendFile sendFlieEvent;
        //请求事件
        public delegate void SendRequest(string toClient, int n);
        public event SendRequest sendRequest;
        #endregion

        private string _friendID;
        public string FriendID { get => _friendID; set => _friendID = value; }

        public UserChatWindow()
        {
            #region 声明回调函数
            receiveCallBack = new ReceiveMsgCallBack(ReceiveMsg);
            #endregion

            InitializeComponent();

            
        }

        private void ReceiveMsg(string strMsg)
        {

            this.txtMsgLog.AppendText(strMsg + "\n");
        }

        public void ChangeTxtMsgLog(string msg)
        {
            this.txtMsgLog.Dispatcher.Invoke(receiveCallBack, msg);
        }

        public void ZD()
        {
            Point point = new Point(this.Left, this.Top);
            //this.WindowStartupLocation = WindowStartupLocation.Manual;

            for(int i = 0; i < 300; i++)
            {
                this.Left = this.Left + 30;
                this.Left = point.X;
            }
        }
        
        /// <summary>
        /// 101 聊天事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMsgSend_Click(object sender, RoutedEventArgs e)
        {
            string msg = this.txtMsg.Text.Trim().ToString();
            if (msg == "")
            {
                return;
            }
            else
            {
                this.txtMsgLog.Dispatcher.Invoke(receiveCallBack, string.Format("我：{0}\n  {1}\n", DateTime.Now.ToString(), msg));
                sendMessageEvent(FriendID, msg);
                this.txtMsg.Text = "";
            }

        }

        /// <summary>
        /// 201 选择文件，并发送 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSendFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\Users\58317\Desktop";
            ofd.Title = "请选择要发送的文件";
            ofd.Filter = "所有文件|*.*";
            ofd.ShowDialog();

            string path = ofd.FileName.ToString();
            //获取文件字节数组
            using (FileStream fileStream=new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
            {

                byte[] buffer = new byte[1024 * 1024 * 5];
                int r = fileStream.Read(buffer, 0, buffer.Length);

                byte[] bufferFile = new byte[r];
                for (int i = 0; i < r; i++)
                {
                    bufferFile[i] = buffer[i];
                }
                sendFlieEvent(FriendID, bufferFile, r);
            }
            this.txtMsgLog.Dispatcher.Invoke(receiveCallBack, string.Format("我:{0}\n发送文件事件\n",DateTime.Now.ToString()));
        }

        /// <summary>
        /// 301 请求事件 1 震动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnZhenDong_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sendRequest(FriendID, 1);
            this.txtMsgLog.Dispatcher.Invoke(receiveCallBack, string.Format("我:{0}\n发送震动事件\n", DateTime.Now.ToString()));
        }

    }
}
