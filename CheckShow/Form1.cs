using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using log4net;
using System;
using log4net.Config;
using System.Reflection;
using System.Drawing;

namespace CheckShow
{
    public partial class Form1 : Form
    {
        private DataBase _DataBase = new DataBase();
        private Container _Container = new Container();

        private System.Threading.Timer _TimerDateStatus;
        private Action<string[]> ShowPicture;
        private string Container_ImagePath = Properties.Settings.Default.Container_ImagePath;
        private string Container_Lane = Properties.Settings.Default.Container_Lane;
        private string Container_Plate_Name = Properties.Settings.Default.Container_Plate_Name;
        private string Container_ChediPath = Properties.Settings.Default.Container_ChediPath;

        public Form1()
        {
            InitializeComponent();

            _Container.LpnResult += InsertLpn;//车牌结果事件
            _Container.GetStatusAction += ContainerStatus;//链接状态

            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.Value = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            radioTimeButton.Checked = true;

            _TimerDateStatus= new System.Threading.Timer(DataCallBack, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0));
        }

        /// <summary>
        /// 定时更新状态
        /// </summary>
        /// <param name="state"></param>
        private void DataCallBack(object state)
        {
            toolStripStatusLabel4.Text = "等待车辆数据！";
        }

        /// <summary>
        /// 箱号链接状态
        /// </summary>
        /// <param name="obj"></param>
        private void ContainerStatus(bool obj)
        {
            if(obj)
            {
                toolStripStatusLabel3.BackColor=Color.DarkGreen;
                _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
            }
            else
            {
                toolStripStatusLabel3.BackColor = Color.DarkRed;
            }
        }

        /// <summary>
        /// 车牌结果触发插入数据库
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void InsertLpn(DateTime arg1, string arg2)
        {
            toolStripStatusLabel4.Text = string.Format("Date：{0} 车辆通过：{1}",arg1.ToString("yyyy-MM-DD HH:mm:ss"),arg2);
            _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));

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
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
        /// 如果第4张图片为空，证明为小箱4张图片
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Plate"></param>
        /// <returns></returns>
        private string[] ReturnImagePath(DateTime dt,string Plate)
        {
            string[] Message = new string[9] {Plate,null,null,null,null,null,null,null,null};
            string Path = string.Format(@"{0}\{1}\0{2}\0{3}",Container_ImagePath,dt.Year.ToString(),dt.Month.ToString(),dt.Day.ToString());
            for(int i=1;i<7;i++)
            {
                Message[i] = string.Format(@"{0}\{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane, i);
                if (!System.IO.File.Exists(Message[i]))
                {
                    Message[i] = null;
                }
            }
            //车牌
            Message[7] = string.Format(@"{0}\{1}{2}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Plate_Name);
            if (!System.IO.File.Exists(Message[7]))
            {
                Message[7] = null;
            }
            Path = string.Format(@"{0}\{1}\0{2}\0{3}", Container_ChediPath, dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString());
            Message[8] = string.Format(@"{0}\{1}.jpg", Path, dt.ToString("yyyyMMddHHmmss"));//车底图片
            if (!System.IO.File.Exists(Message[8]))
            {
                Message[8] = null;
            }
            return Message;
        }
    }
}
