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

        private Action<string[]> ShowPicture;
        private string ImagePath = Properties.Settings.Default.ImagePath;
        private string Container_Lane = Properties.Settings.Default.Container_Lane;
        private string Container_Plate_Name = Properties.Settings.Default.Container_Plate_Name;

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
        }

        /// <summary>
        /// 车牌结果触发插入数据库
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void InsertLpn(DateTime arg1, string arg2)
        {
            var mes = _DataBase.InsertData(arg1.ToUniversalTime().AddHours(8), ReturnImagePath(arg1.ToUniversalTime().AddHours(8), arg2));
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            string Plate = string.Empty;
            if(radioPlateButton.Checked)
            {
                Plate = textBox1.Text;
            }
            bindingSource1.DataSource = _DataBase.Select(dateTimePicker1.Value,dateTimePicker2.Value, Plate).Tables["Picture"];
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// 双击单元格显示图片窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string[] str = new string[dataGridView1.Columns.Count];
            int row = dataGridView1.CurrentRow.Index;
            for(int i=0;i<dataGridView1.Columns.Count;i++)
            {
                str[i] = dataGridView1.Rows[row].Cells[i].Value.ToString();
            }
            Picture _Picture = new Picture();
            ShowPicture += _Picture.ShowPicture;
            ShowPicture?.Invoke(str);
            _Picture.Show();
        }

        /// <summary>
        /// 返回图片路径
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Plate"></param>
        /// <returns></returns>
        private string[] ReturnImagePath(DateTime dt,string Plate)
        {
            string[] Message = new string[9] {Plate,null,null,null,null,null,null,null,null};
            string Path = string.Format(@"{0}\{1}\0{2}\0{3}\",ImagePath,dt.Year.ToString(),dt.Month.ToString(),dt.Day.ToString());
            for(int i=1;i<7;i++)
            {                
                Message[i] = string.Format(@"{0}{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane, i);
                if (!System.IO.File.Exists(Message[i]))
                {
                    Message[i] = null;
                }
            }
            Message[8]= string.Format(@"{0}{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane, Container_Plate_Name);
            return Message;
        }
    }
}
