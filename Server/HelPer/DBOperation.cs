using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace Server
{
    class DBOperation
    {
        public static bool updataUserLoginIP(SqlConnection sqlConnection,string userID, IPEndPoint userLoginIP)
        {
            string ip = userLoginIP.ToString();
            try
            {
                sqlConnection.Open();
                string str = "update loging set userLoginIP = '@userLoginIP' where userID = @userID";
                SqlParameter[] parameters = new SqlParameter[2]{
                    new SqlParameter("@userID", userID),
                    new SqlParameter("@userLoginIP", userLoginIP) };
                SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.StoredProcedure, str, parameters);
                sqlConnection.Close();
                return true;
               
            }
            catch
            {
                if (sqlConnection.State == ConnectionState.Open) sqlConnection.Close();
                return false;
            }
        }
    }
}
