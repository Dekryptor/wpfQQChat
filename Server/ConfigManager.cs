using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ConfigManager
    {
        #region 连接数据库信息
        static string dbServer = "DESKTOP-ANI8CV8";
        static string dbName = "chat";
        static string dbUserID = "sa";
        static string dbUserPassword = "123456";
        private static string _connectStr = string.Format("server={0};database={1};uid={2}; pwd={3}", dbServer, dbName, dbUserID, dbUserPassword);
        public static string ConnectStr { get => _connectStr; set => _connectStr = value; }
        #endregion

        #region SqlParameter数组

        #endregion
    }

}
