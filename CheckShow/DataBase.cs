using System;
using System.Data;
using System.Data.SQLite;

namespace CheckShow
{
    class DataBase:IDisposable
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
                Lognet.Log.Error("打开数据库错误",ex);
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
                Lognet.Log.Warn("箱号数据库不存在，重新创建数据库");
            }
        }

        /// <summary>
        /// 插入车底图片路径
        /// </summary>
        /// <param name="UVSSPath"></param>
        /// <returns></returns>
        public int InsertData( string UVSSPath)
        {
            SQLiteParameter[] parameters = {
                //new SQLiteParameter("@dt",DbType.DateTime),
                new SQLiteParameter("@UVSSPath",DbType.String,10)
            };
            int result = -1;
            try
            {
                //parameters[0].Value = dt;
                parameters[0].Value = UVSSPath;
                //command.CommandText = @"UPDATE Picture SET P_6=@UVSSPath WHERE P_6='nul' order by ID desc limit 1";
                command.CommandText = "update Picture set p_6=@UVSSPath where ID =(select ID from Picture order by ID desc limit 1) and P_6='nul'";
                command.Parameters.AddRange(parameters);
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Lognet.Log.Error("插入数据错误", ex);
            }

            return result;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
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
                    new SQLiteParameter("@P_6",DbType.String,10)    
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

                command.CommandText = "INSERT INTO Picture(Date,Plate,P_1,P_2,P_3,P_4,P_5,P_6)" +
                        " VALUES(@Date,@Plate,@P_1,@P_2,@P_3,@P_4,@P_5,@P_6)";

                command.Parameters.AddRange(parameters);
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Lognet.Log.Error("插入数据错误", ex);
                //connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DataSet Select(DateTime dts,DateTime dte,string Plate)
        {
            DataSet ds = new DataSet();
            string cmdText = string.Empty;
            try
            {

                SQLiteParameter[] parameter = {
                    new SQLiteParameter("@DateS",DbType.DateTime),
                    new SQLiteParameter("@DateE",DbType.DateTime),
                    new SQLiteParameter("@Plate",DbType.String,10)
                };
                parameter[0].Value = dts;
                parameter[1].Value = dte;
                parameter[2].Value = Plate;
                
                if(string.IsNullOrEmpty(Plate))
                {
                    cmdText = "SELECT * FROM Picture WHERE Date BETWEEN @DateS AND @DateE";
                }
                else
                {
                    cmdText = "SELECT * FROM Picture WHERE  Plate=@Plate";
                }

                command.CommandText = cmdText;
                command.Parameters.AddRange(parameter);
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

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    connection.Dispose();
                    command.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DataBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
