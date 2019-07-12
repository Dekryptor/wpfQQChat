using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ViewModes
{
    class MainWindowViewModel : BindableBase
    {
        public class Friend
        {
            public string FriendID { get; set; }
            public string Nickname { get; set; }
            public BitmapImage Head { get; set; }
            public string UserPhone { get; set; }
            public string UserMail { get; set; }
            public string UserProfession { get; set;}
        }

        #region 定义字段
        private static ObservableCollection<Friend> friends = new ObservableCollection<Friend>();
        public ObservableCollection<Friend> Friends
        {
            get { return friends; }
            set { friends = value; }
        }

        private string friendID;
        public string FriendID
        {
            get { return friendID; }
            set
            {
                SetProperty(ref friendID, value);
            }
        }

        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set { SetProperty(ref nickname, value); }
        }

        private BitmapImage head;
        public BitmapImage Head
        {
            get { return head; }
            set { SetProperty(ref head, value); }
        }

        private string userPhone;
        public string UserPhone
        {
            get { return userPhone; }
            set { SetProperty(ref userPhone, value); }
        }

        private string userMail;
        public string UserMail
        {
            get { return userMail; }
            set { SetProperty(ref userMail, value); }
        }

        private string userProfession;
        public string UserProfession
        {
            get { return userProfession; }
            set { SetProperty(ref userProfession, value); }
        }
        #endregion


        public DelegateCommand<object> SelectItemChangedCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        

        public MainWindowViewModel()
        {
            //测试用例
            friends.Add(new Friend() {
                FriendID = "127.0.0.1:6666",
                Nickname = "服务器",
                Head = new BitmapImage(new Uri("pack://application:,,,/Images/head1.jpg")),
                UserPhone = "00000000000",
                UserMail="",
                UserProfession= ""
            });

            friends.Add(new Friend() {
                FriendID ="001",
                Nickname = "好友一",
                Head = new BitmapImage(new Uri("pack://application:,,,/Images/head1.jpg")),
                UserPhone="11111111111",
                UserMail="001@qq.com",
                UserProfession="专科"
            });
            friends.Add(new Friend() {
                FriendID ="002",
                Nickname = "好友二",
                Head = new BitmapImage(new Uri("pack://application:,,,/Images/head2.jpg")),
                UserPhone="22222222222",
                UserMail="002@qq.com",
                UserProfession="硕士"
            });
            friends.Add(new Friend() {
                FriendID = "003",
                Nickname = "好友三",
                Head = new BitmapImage(new Uri("pack://application:,,,/Images/head3.jpg")),
                UserPhone="33333333333",
                UserMail="003@qq.com",
                UserProfession="本科"
            });


            CloseCommand = new DelegateCommand(() => {

                Application.Current.Shutdown();

            });


            SelectItemChangedCommand = new DelegateCommand<object>((p) => {
                ListView lv = p as ListView;
                Friend friend = lv.SelectedItem as Friend;
                FriendID = friend.FriendID;
                Head = friend.Head;
                Nickname = friend.Nickname;
                UserPhone = friend.UserPhone;
                UserMail = friend.UserMail;
                UserProfession = friend.UserProfession;
            });
        }

        public static void addFriend()
        {

            //这里用于连接数据库，判断好友是否存在，存在就添加，不存在就返回。
            friends.Add(new Friend() { Nickname = "新朋友", Head = new BitmapImage(new Uri("pack://application:,,,/Images/head6.jpg")) });

        }



    }
}
