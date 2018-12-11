using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using log4net;
using System;
using log4net.Config;
using System.Reflection;

namespace CheckShow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string Connent = @"Data Source=Data.db;Pooling = true;FaillfMissing=false";
            SQLiteConnection conn = new SQLiteConnection(Connent);
            conn.Open();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Picture";
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.FillSchema(dataSet1, System.Data.SchemaType.Source, "Picture");
            da.Fill(dataSet1, "Picture");
            bindingSource1.DataSource = dataSet1.Tables["Picture"];
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView1.DataSource = bindingSource1;       
            conn.Close();

        }

        private void bindingNavigatorAddNewItem_Click(object sender, System.EventArgs e)
        {
            string Connent = @"Data Source=Data.db;Pooling = true;FaillfMissing=false";
            SQLiteConnection conn = new SQLiteConnection(Connent);
            conn.Open();

            DataRow dataRow = dataSet1.Tables["Picture"].NewRow();
            //dataRow["ID"] = 1;
            dataRow["Plate"] = "粤S01234";
            dataSet1.Tables["Picture"].Rows.Add(dataRow);
            dataGridView1.Invalidate();
            SQLiteDataAdapter da = new SQLiteDataAdapter();
            da.InsertCommand = new SQLiteCommand("INSERT INTO Picture( Plate)VALUES( '"+ dataRow["Plate"] + "'); ",conn);
            da.Update(dataSet1, "Picture");
            conn.Close();
        }

        private void toolStripButton1_Click(object sender, System.EventArgs e)
        {
            
            //string Connent = @"Data Source=Data.db;Pooling = true;FaillfMissing=false";
            //SQLiteConnection conn = new SQLiteConnection(Connent);
            //conn.Open();
            
            //DataRow dataRow = dataSet1.Tables["Picture"].NewRow();
            ////dataRow["ID"] = 1;
            //dataRow["Plate"] = "粤S01234";


            XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Error("Error message", new Exception("Error message generated"));

            //dataSet1.Tables["Picture"].Rows.Add(dataRow);
            //dataGridView1.Invalidate();
            //SQLiteDataAdapter da = new SQLiteDataAdapter();
            //da.InsertCommand = new SQLiteCommand("INSERT INTO Picture( Plate)VALUES( '" + dataRow["Plate"] + "'); ", conn);
            //da.Update(dataSet1, "Picture");
            //conn.Close();

        }
    }
}
