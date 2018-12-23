using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckShow
{
    public partial class LogForm : Form
    {
        private DataBase _DataBase = new DataBase();
        private readonly string connectionString = @"Data Source=log4net.db;Pooling = true;FaillfMissing=false";
        public LogForm()
        {
            InitializeComponent();

            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.Value = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            radioTimeButton.Checked = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string Level = string.Empty;
            if (radioLevelButton.Checked)
            {
                Level = textBox1.Text;
            }
            bindingSource1.DataSource = Select(dateTimePicker1.Value, dateTimePicker2.Value, Level).Tables["Log"];
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        }

        public DataSet Select(DateTime dts, DateTime dte, string Level)
        {
            DataSet ds = new DataSet();
            string cmdText = string.Empty;
            SQLiteParameter[] parameter = {
                    new SQLiteParameter("@DateS",DbType.DateTime),
                    new SQLiteParameter("@DateE",DbType.DateTime),
                    new SQLiteParameter("@Level",DbType.String,10)
                };
            parameter[0].Value = dts;
            parameter[1].Value = dte;
            parameter[2].Value = Level;

            if (string.IsNullOrEmpty(Level))
            {
                cmdText = "SELECT * FROM Log WHERE Date BETWEEN @DateS AND @DateE";
            }
            else
            {
                cmdText = "SELECT * FROM Log WHERE  Level=@Level";
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(cmdText, connection))
                {
                    if (parameter != null)
                    {
                        command.Parameters.AddRange(parameter);
                    }
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                    da.FillSchema(ds, SchemaType.Source, "Log");
                    if (da.Fill(ds, "Log") == 0)
                    {
                        ds.Tables["Log"].Clear();
                    }
                    return ds;
                }
            }
        }
    }
}
