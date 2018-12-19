using System;
using System.Data;
using System.Data.SQLite;

namespace CheckShow
{
    class DataBase
    {
        private SQLiteConnection connection = null;
        private SQLiteCommand command = null;
 
        public DataBase()
        {
            GreateDataBase();
            try
            {
                connection = new SQLiteConnection(@"Data Source=Data.db;Pooling = true;FaillfMissing=false");
                connection.Open();
                command = new SQLiteCommand(connection);
            }
            catch (System.Exception ex)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// 判断数据库是否存在，不存在则创建数据库
        /// </summary>
        private void GreateDataBase()
        {
            string DataBase = "Data.db";
            if (!System.IO.File.Exists(DataBase))
            {
                SQLiteDBHelper.CreateDB(DataBase);
                Lognet.Log.Warn("数据库不存在，重新创建数据库");
            }
        }

        public int InsertData(DateTime dt,string[] Message)
        {
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@Date",DbType.DateTime),
                    new SQLiteParameter("@Plate", DbType.String,10),
                    new SQLiteParameter("@P_1",DbType.String,10),
                    new SQLiteParameter("@P_2",DbType.String,10),
                    new SQLiteParameter("@P_3",DbType.String,10),
                    new SQLiteParameter("@P_4",DbType.String,10),
                    new SQLiteParameter("@P_5",DbType.String,10),
                    new SQLiteParameter("@P_6",DbType.String,10),
                    new SQLiteParameter("@P_7",DbType.String,10),
                    new SQLiteParameter("@P_8",DbType.String,10)
                };
            int result=-1;
            try
            {
                parameters[0].Value = dt;
                int i = 1;
                foreach (var v in Message)
                {
                    parameters[i].Value = v;
                    i++;
                }

                command.CommandText = "INSERT INTO Picture(Date,Plate,P_1,P_2,P_3,P_4,P_5,P_6,P_7,P_8)" +
                        " VALUES(@Date,@Plate,@P_1,@P_2,@P_3,@P_4,@P_5,@P_6,@P_7,@P_8)";

                command.Parameters.AddRange(parameters);
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Lognet.Log.Error("插入数据错误", ex);
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataSet Select(string cmdText)
        {
            DataSet ds = new DataSet();
            try
            {
                command.CommandText = cmdText;
                SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                da.FillSchema(ds, SchemaType.Source, "Picture");
                if (da.Fill(ds, "Picture") == 0)
                {
                    ds.Tables["Picture"].Clear();
                }
            }
            catch (Exception ex)
            {
                Lognet.Log.Error("查询数据错误", ex);
                connection.Close();
            }
            return ds;
        }
    }
}
