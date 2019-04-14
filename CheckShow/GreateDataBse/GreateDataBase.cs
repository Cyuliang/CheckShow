namespace CheckShow
{
    class GreateDataBase
    {
        public GreateDataBase()
        {
            string DataBase = "Log//Log.db";
            SQLiteDBHelper.CreateLogDB(DataBase);

            string DataPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Data";
            DataBase = "Data//Data.db";
            if (!System.IO.File.Exists(DataBase))
            {
                System.IO.Directory.CreateDirectory(DataPath);
                SQLiteDBHelper.CreateDB(DataBase);
                Lognet.Log.Warn("箱号数据库不存在，重新创建数据库");
            }
        }
    }
}
