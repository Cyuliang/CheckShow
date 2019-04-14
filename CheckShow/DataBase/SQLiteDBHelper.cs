﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

/// <summary>
///MYSQLHelper 的摘要说明
/// </summary>
namespace CheckShow
{
    /// <summary> 
    /// 说明：这是一个针对System.Data.SQLite的数据库常规操作封装的通用类。 
    /// </summary> 
    public abstract class SQLiteDBHelper
    {
        private readonly string connectionString = string.Empty;

        /// <summary> 
        /// 创建SQLite数据库文件 
        /// </summary> 
        /// <param name="dbPath">要创建的SQLite数据库文件路径</param> 
        public static void CreateDB(string dbPath)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();                
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    try
                    {
                        command.CommandText = @"CREATE TABLE `Picture` (
	                        `ID`	    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        `Date`	    datetime,
	                        `Plate`	    TEXT DEFAULT 'nul',
                            `Container` TEXT DEFAULT 'nul',                            
	                        `P_1`	    TEXT DEFAULT 'nul',
	                        `P_2`	    TEXT DEFAULT 'nul',
	                        `P_3`	    TEXT DEFAULT 'nul',
	                        `P_4`	    TEXT DEFAULT 'nul',
	                        `P_5`	    TEXT DEFAULT 'nul',
	                        `P_6`	    TEXT DEFAULT 'nul',
                            `CheckNum`  TEXT DEFAULT 'nul'
                    )";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE INDEX `Plate` ON `Picture` (`Plate`	ASC)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE INDEX `Date` ON `Picture`  (`Date`	ASC)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE INDEX `Container` ON `Picture` (`Container`	ASC)";
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        ;
                    }
                }
            }
        }

        /// <summary>
        /// 创建Log数据卡
        /// </summary>
        /// <param name="dbPath"></param>
        public static void CreateLogDB(string dbPath)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    try
                    {
                        command.CommandText = @"CREATE TABLE `Log` (
	                    `LogId`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    `Date`	DATETIME NOT NULL,
	                    `Level`	VARCHAR ( 50 ) NOT NULL,
	                    `Logger`	VARCHAR ( 255 ) NOT NULL,
	                    `Message`	TEXT DEFAULT 'nul',
	                    `Exception`	TEXT DEFAULT 'nul');";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE INDEX `Level` ON `Log` (`Level`	ASC)";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE INDEX `Date` ON `Log`  (`Date`	ASC)";
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        ;
                    }                            
                }
            }
        }

        /// <summary> 
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。 
        /// </summary> 
        /// <param name="sql">要执行的增删改的SQL语句</param> 
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters)
        {
            int affectedRows = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        affectedRows = command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            return affectedRows;
        }
        /// <summary> 
        /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public SQLiteDataReader ExecuteReader(string sql, SQLiteParameter[] parameters)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public DataTable ExecuteDataTable(string sql, SQLiteParameter[] parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    return data;
                }
            }
        }
        /// <summary> 
        /// 执行一个查询语句，返回查询结果的第一行第一列 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public Object ExecuteScalar(string sql, SQLiteParameter[] parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    return data;
                }
            }
        }
        /// <summary> 
        /// 查询数据库中的所有数据类型信息 
        /// </summary> 
        /// <returns></returns> 
        public DataTable GetSchema()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                DataTable data = connection.GetSchema("TABLES");
                //connection.Close();
                //foreach (DataColumn column in data.Columns) 
                //{ 
                //  Console.WriteLine(column.ColumnName); 
                //} 
                return data;
            }
        }
    }
}