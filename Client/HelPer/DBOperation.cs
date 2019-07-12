using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Client.DBUtility;
using System.Data;
using System.Data.SqlClient;

namespace Client.DBUtility
{
    class DBOperation
    {
        public static bool updataUserLoginIP(SqlConnection sqlConnection,string userID, IPEndPoint userLoginIP)
        {
            string ip = userLoginIP.ToString();
            try
            {
                string str = "update loging set userLoginIP = '@userLoginIP' where userID = @userID";
                SqlParameter[] parameters = new SqlParameter[2]{
                    new SqlParameter("@userID", userID),
                    new SqlParameter("@userLoginIP", userLoginIP) };
                SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.StoredProcedure, str, parameters);
                return true;
               
            }
            catch
            {
                if (sqlConnection.State == ConnectionState.Open) sqlConnection.Close();
                return false;
            }
        }

        public static bool addNewUser(SqlConnection sqlConnection, string userID,string userPassWord,string userName,string userPhone="",string userMail="",string userProfession="")
        {
            try
            {
                string str = "insert into userInfo values(@userID, @userPassword, @userName, @userPhone, @userMail, @userProfession)";
                SqlParameter[] parameters = new SqlParameter[6] {
                    new SqlParameter("@userID",userID),
                    new SqlParameter("@userPassword",userPassWord),
                    new SqlParameter("@userName",userName),
                    new SqlParameter("@userPhone",userPhone),
                    new SqlParameter("@userMail",userMail),
                    new SqlParameter("@userProfession",userProfession)
                };
                int n = SqlHelper.ExecuteNonQuery(sqlConnection, CommandType.StoredProcedure, str, parameters);
                if (n == 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
            
        }


    }
}
