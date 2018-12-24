using System.Windows.Forms;
using System;
using System.Drawing;

namespace CheckShow
{
    public partial class Form1 : Form
    {
        private DataBase _DataBase = new DataBase();
        private Container _Container = new Container();
        private Uvss _Uvss = new Uvss();

        private delegate void UpdateUiBInvok(object state);
        private delegate void UpdateUIMessageInvok(string Message);
        private delegate void UpdateUIUVSS(bool status);

        private System.Threading.Timer _TimerDateStatus;
        private DateTime LpnDt;

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
            _Uvss.MessageAction += UVSSMessage;
            _Uvss.LinkStatusAction += UVSSStatus;

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
        /// 车底链接状态
        /// </summary>
        /// <param name="obj"></param>
        private void UVSSStatus(bool obj)
        {
            if(statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new UpdateUIUVSS(UVSSStatus), new object[] { obj });
            }
            else
            {
                if (obj)
                {
                    toolStripStatusLabel5.BackColor = Color.DarkGreen;
                }
                else
                {
                    toolStripStatusLabel5.BackColor = Color.DarkRed;
                }
            }
        }

        /// <summary>
        /// 车底数据
        /// </summary>
        /// <param name="obj"></param>
        private void UVSSMessage(string obj)
        {
            UpdateUi(string.Format("车底触发：{0}",obj));
            _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
            string CheDiPath = string.Format(@"{0}\{1}\{2}\{3}\{4}_chedi.jpg", Container_ChediPath, LpnDt.Year.ToString(), LpnDt.Month.ToString().PadLeft(2, '0'), LpnDt.Day.ToString().PadLeft(2, '0'), LpnDt.ToString("yyyyMMddHHmmss"));

            if (System.IO.File.Exists(obj))
            {
                Image.FromFile(obj).Save(CheDiPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            if (_DataBase.InsertData(CheDiPath) ==1)
            {
                Lognet.Log.Info("插入车底数据成功");
            }
        }

        /// <summary>
        /// 定时更新状态
        /// </summary>
        /// <param name="state"></param>
        private void DataCallBack(object state)
        {
            if(statusStrip1.InvokeRequired)
            {
                statusStrip1?.Invoke(new UpdateUiBInvok(DataCallBack), new object[] { state });
            }
            else
            {
                toolStripStatusLabel4.Text = "等待车辆数据！";
            }
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
            UpdateUi(string.Format("Date：{0} 车辆通过：{1}", arg1.ToString("yyyy-MM-DD HH:mm:ss"), arg2));
            _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));

            if(_DataBase.InsertData(arg1, ReturnImagePath(arg1, arg2))==1)
            {
                Lognet.Log.Info("插入集装箱数据成功");
            }

            LpnDt = arg1;//车底图片保位置
        }

        /// <summary>
        /// 车辆触发更新状态提示
        /// </summary>
        /// <param name="Message"></param>
        private void UpdateUi(string Message)
        {
            if(statusStrip1.InvokeRequired)
            {
                statusStrip1?.Invoke(new UpdateUIMessageInvok(UpdateUi), new object[] { Message });
            }
            else
            {
                toolStripStatusLabel4.Text = Message;
            }
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
        /// 第4张图片是尾图尾号为10。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Plate"></param>
        /// <returns></returns>
        private string[] ReturnImagePath(DateTime dt,string Plate)
        {
            string[] Message = new string[7] {Plate,null,null,null,null,null,null};
            string Path = string.Format(@"{0}\{1}\{2}\{3}",Container_ImagePath,dt.Year.ToString(),dt.Month.ToString().PadLeft(2,'0'),dt.Day.ToString().PadLeft(2,'0'));
            for(int i=1;i<4;i++)
            {
                Message[i] = string.Format(@"{0}\{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane, i);
                if (!System.IO.File.Exists(Message[i]))
                {
                    Message[i] = "nul";
                }
            }
            //后相机
            Message[4] = string.Format(@"{0}\{1}{2}10.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane);
            if (!System.IO.File.Exists(Message[4]))
            {
                Message[4] = "nul";
            }
            //车牌
            Message[5] = string.Format(@"{0}\{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"),Container_Lane ,Container_Plate_Name);
            if (!System.IO.File.Exists(Message[5]))
            {
                Message[5] = "nul";
            }
            Path = string.Format(@"{0}\{1}\{2}\{3}", Container_ChediPath, dt.Year.ToString(), dt.Month.ToString().PadLeft(2,'0'), dt.Day.ToString().PadLeft(2,'0'));
            Message[6] = string.Format(@"{0}\{1}.jpg", Path, dt.ToString("yyyyMMddHHmmss"));//车底图片
            if (!System.IO.File.Exists(Message[6]))
            {
                Message[6] = "nul";
            }
            return Message;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LogForm _logForm = new LogForm();
            _logForm.Show();
        }
    }
}
