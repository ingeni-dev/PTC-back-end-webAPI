using System.Collections.Generic;
using System.Data;

namespace PTCwebApi.DataService {
    public static class ToDatable {
        public static DataTable ToDataTable (this List<dynamic> list, string tableName) {

            if (list == null || list.Count == 0) {
                return null;
            }
            //build columns
            var props = (IDictionary<string, object>) list[0];
            var t = new DataTable (tableName);
            foreach (var prop in props) {
                t.Columns.Add (new DataColumn (prop.Key, prop.Value.GetType ()));
            }
            //add rows
            foreach (var row in list) {
                var data = t.NewRow ();
                foreach (var prop in (IDictionary<string, object>) row) {
                    data[prop.Key] = prop.Value;
                }
                t.Rows.Add (data);
            }
            return t;
        }
    }

}