using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using PTCwebApi.DataService;
using PTCwebApi.Models.ProfilesModels;

namespace PTCwebApi.DataService {
    public class ConnectionProfile {
        public static string checkUsersALL () {
            DataTable dataTable = ConnectSQL.Execute ("SELECT AD_USER_ID, EMP_ID, EMP_FNAME, EMP_LNAME FROM  PORTAL_PROD.dbo.MV_ALL_USER WHERE AD_USER_ID IS NOT NULL");
            //Convert DataTable to Json
            string jsonMessage = JsonConvert.SerializeObject (dataTable);
            return jsonMessage;
        }
        public static List<UserProfileFromSQl> SetProfile (string user) {
            DataTable dataTable = ConnectSQL.Execute ("SELECT distinct ORG_ID,EMP_ID,EMP_FNAME,EMP_LNAME,POS_ID,ROLE_ID,IsNull(E_MAIL,'') AS E_MAIL,IsNull(EMP_NICKNAME,'') AS EMP_NICKNAME FROM  PORTAL_PROD.dbo.MV_ALL_USER WHERE AD_USER_ID = '" + user + "'");
            return ConvertDataTable<UserProfileFromSQl> (dataTable);
        }
        private static List<T> ConvertDataTable<T> (DataTable dt) {
            List<T> data = new List<T> ();
            foreach (DataRow row in dt.Rows) {
                T item = GetItem<T> (row);
                data.Add (item);
            }
            return data;
        }
        private static T GetItem<T> (DataRow dr) {
            Type temp = typeof (T);
            T obj = Activator.CreateInstance<T> ();

            foreach (DataColumn column in dr.Table.Columns) {
                foreach (PropertyInfo pro in temp.GetProperties ()) {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue (obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}