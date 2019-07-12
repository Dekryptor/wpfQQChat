using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDLL
{
    public class UserInfo
    {
        private static string _userID;
        private static string _userName;
        private static string _userPhone;
        private static string _userMail;
        private static string _userProfession;
        public static string UserID { get => _userID; set => _userID = value; }
        public static string UserName { get => _userName; set => _userName = value; }
        public static string UserPhone { get => _userPhone; set => _userPhone = value; }
        public static string UserMail { get => _userMail; set => _userMail = value; }
        public static string UserProfession { get => _userProfession; set => _userProfession = value; }
    }
}
