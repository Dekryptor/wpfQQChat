using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDLL
{
    [Serializable]
    public class Message
    {
        //发送消息的目的 
        /*
         * 000 服务器发送给客户端的消息
         * 001 客户端发送飞服务器的消息
         * 101 文字聊天
         * 201 发送的文件
         * 301 客户端对客户端的请求 1为窗口抖动 2语音请求 3为视频请求
         */
        private string _sign;
        //发来消息的用户Id
        private string _fromClient;
        //接受消息的用户id
        private string _toClient;
        //发送的消息
        private string _msg;
        //直接发送buffer流，文件
        private byte[] _bufferFile;
        private int _request;

        //发送消息的构造函数
        public Message(string sign, string fromClient, string toClient, string msg)
        {
            this._sign = sign;
            this._fromClient = fromClient;
            this._toClient = toClient;
            this._msg = msg;
        }
        //发送文件的构造函数
        public Message(string sign,string fromClient,string toClient,byte[] buffer)
        {
            this.Sign = sign;
            this.FromClient = fromClient;
            this.ToClient = toClient;
            this.BufferFile = buffer;
        } 
        //客户端对客户端的请求
        public Message(string sign,string fromClient,string toClient,int n)
        {
            this.Sign = sign;
            this.FromClient = fromClient;
            this.ToClient = toClient;
            this.Request = n;
        }

        public string Sign { get => _sign; set => _sign = value; }
        public string FromClient { get => _fromClient; set => _fromClient = value; }
        public string ToClient { get => _toClient; set => _toClient = value; }
        public string Msg { get => _msg; set => _msg = value; }
        public byte[] BufferFile { get => _bufferFile; set => _bufferFile = value; }
        public int Request { get => _request; set => _request = value; }
    }
}
