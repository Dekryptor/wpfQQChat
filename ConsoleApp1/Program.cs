using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [Serializable()] class Message 
    {
        public string fromClient { get; set; }
        public string toClient { get; set; }
        public string message { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Message send = new Message();
            send.fromClient = "583175160";
            send.toClient = "123456789";
            send.message = "你好";



            return;

        }

    }


}
