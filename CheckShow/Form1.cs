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

        private DataBase _DataBase = new DataBase();
        private Container _Container = new Container();        
        public Form1()
        {
            InitializeComponent();

            _Container.LpnResult += InsertLpn;//车牌结果事件

            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.Value = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            radioTimeButton.Checked = true;


            //var mes = _DataBase.InsertData(DateTime.Parse("2018-12-19 13:12:22"), new string[] {  "2", "3", "4", "5", "6", "7", "8" ,"9"});
            //toolStripStatusLabel2.Text = string.Format("插入 [{0:d}] 条数据成功!", mes);
        }

        /// <summary>
        /// 车牌结果触发插入数据库
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void InsertLpn(DateTime arg1, string arg2)
        {
            var mes = _DataBase.InsertData(arg1.ToUniversalTime().AddHours(8), new string[] {  arg2, "3", "4", "5", "6", "7", "8" ,"9"});
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            string cmdText = string.Empty;
            if(radioTimeButton.Checked)
            {
                cmdText = string.Format("SELECT * FROM Picture WHERE Date BETWEEN '{0}' AND '{1}'", dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss"), dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if(radioPlateButton.Checked)
            {
                cmdText = string.Format("SELECT * FROM Picture WHERE  Plate='{0}'",textBox1.Text);
            }
            bindingSource1.DataSource = _DataBase.Select(cmdText).Tables["Picture"];
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        }
    }
}
