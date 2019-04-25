using System.Windows.Forms;
using System;
using System.Drawing;
using System.Data;
using System.IO;

namespace CheckShow
{
    public partial class Form1 : Form
    {
        private GreateDataBase _GreateDataBase = new GreateDataBase();

        private Picture _Picture = new Picture();
        private DataBase _DataBase = new DataBase();
        private Uvss _Uvss = new Uvss();

        private delegate void UpdateUiBInvok(object state);
        private delegate void UpdateUIMessageInvok(string Message);
        private delegate void UpdateUIUVSS(bool status);
        private delegate void UpdateContianer(bool status);
        //清除图片委托
        private delegate void ClearnPictureDelegate();
        private ClearnPictureDelegate clearnPicture;

        private Action<string[]> FromShowPicture;//主窗口显示图片
        private Action<string[]> ShowPicture;//弹窗显示图片

        private System.Threading.Timer _TimerDateStatus=null;
        //延时显示图片
        System.Threading.Timer _TimerShowPicture = null;


        private DateTime LpnDt = DateTime.Now;

        private string Container_ImagePath = Properties.Settings.Default.Container_ImagePath;
        private string Container_Lane = Properties.Settings.Default.Container_Lane;
        private string Container_Plate_Name = Properties.Settings.Default.Container_Plate_Name;
        private string Container_ChediPath = Properties.Settings.Default.Container_ChediPath;

        private TabPage PictureTable = new TabPage("图像");

        /// <summary>
        /// 远端箱号服务器
        /// </summary>
        private Container_socket_DLL  _Container_socket_ConNum_DLL= new Container_socket_DLL(
            Properties.Settings.Default.ContainerDLL_Ip,
            Properties.Settings.Default.ContainerDLL_Port,
            Properties.Settings.Default.ContainerDLL_Intervals,
            Properties.Settings.Default.LocalIp_bing,
            Properties.Settings.Default.LocalPort_bing_1);

        /// <summary>
        /// 本地箱号服务器
        /// </summary>
        private Container_socket_DLL _Container_socket_Lpn_DLL = new Container_socket_DLL(
            Properties.Settings.Default.Container_Ip,
            Properties.Settings.Default.Container_Port,
            Properties.Settings.Default.ContainerDLL_Intervals,
            Properties.Settings.Default.LocalIp_bing,
            Properties.Settings.Default.LocalPort_bing_2);

        public Form1()
        {
            InitializeComponent();

            _Container_socket_Lpn_DLL.MessageCallBack += RunMessage;                            //运行消息
            _Container_socket_Lpn_DLL.SocketStatusCallBack += _Container_socket_Lpn_DLLStatus;  //箱号链接状态            
            _Container_socket_Lpn_DLL.LpnCallBack += _Container_socket_Lpn_DLLLpn;              //车牌结果

            _Container_socket_ConNum_DLL.MessageCallBack += RunMessage;
            _Container_socket_ConNum_DLL.SocketStatusCallBack += _Container_socket_ConNum_DLLStatus;
            _Container_socket_ConNum_DLL.ConNumCallBack += _Container_socket_ConNum_DLLConNum;
            _Container_socket_ConNum_DLL.LpnCallBack += _Container_socket_ConNum_DLLLpn;

            FromShowPicture += _Picture.ShowPicture;
            clearnPicture += _Picture.PictureClear;

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

            //添加tabPage页面
            SetTablePage( _Picture);
        }

        #region 远端箱号
        private void _Container_socket_ConNum_DLLConNum(DateTime arg1, string Con,string Check)
        {
            if (!string.IsNullOrEmpty(Con))
            {
                if (_DataBase.InsertContainer(Con,Check) == 1)
                {
                    Lognet.Log.Info("插入集装箱数据成功");
                }
            }
            else
            {
                Lognet.Log.Debug("没有识别到集装箱号码");
            }
        }

        private void _Container_socket_ConNum_DLLLpn(DateTime arg1, string arg2)
        {
            //Nothing
        }

        private void _Container_socket_ConNum_DLLStatus(bool obj)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new UpdateContianer(_Container_socket_ConNum_DLLStatus), new object[] { obj });
            }
            else
            {
                if (obj)
                {
                    toolStripStatusLabel8.BackColor = Color.DarkGreen;
                    _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
                }
                else
                {
                    toolStripStatusLabel8.BackColor = Color.DarkRed;
                }
            }
        }
    
        #endregion

        /// <summary>
        /// 运行状态提示
        /// </summary>
        /// <param name="Message"></param>
        private void RunMessage(string Message)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1?.Invoke(new UpdateUIMessageInvok(RunMessage), new object[] { Message });
            }
            else
            {
                toolStripStatusLabel4.Text = Message;
                Lognet.Log.Info(Message);
            }
        }

        #region 本地箱号动态库
        /// <summary>
        /// 箱号数据结果链接状态
        /// </summary>
        /// <param name="obj"></param>
        private void _Container_socket_Lpn_DLLStatus(bool obj)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new UpdateContianer(_Container_socket_Lpn_DLLStatus), new object[] { obj });
            }
            else
            {
                if (obj)
                {
                    toolStripStatusLabel3.BackColor = Color.DarkGreen;
                    _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
                }
                else
                {
                    toolStripStatusLabel3.BackColor = Color.DarkRed;
                }
            }
        }

        /// <summary>
        /// 车牌结果
        /// </summary>
        /// <param name="obj"></param>
        private void _Container_socket_Lpn_DLLLpn(DateTime TriggerTime, string Lpn)
        {
            _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));

            if(string.IsNullOrEmpty(Lpn))
            {
                Lpn = "nul";
            }

            string[] ImagePath = ReturnImagePath(TriggerTime, Lpn);

            if (_DataBase.InsertData(TriggerTime, ImagePath) == 1)
            {
                Lognet.Log.Info("插入车牌数据成功");

                //延时显示图片
                _TimerShowPicture= new System.Threading.Timer(ShowPictureCallBack, ImagePath, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0));
                RunMessage("开始显示图片");
            }

            LpnDt = TriggerTime;//车底图片保位置
        }

        private void ShowPictureCallBack(object state)
        {
            _TimerShowPicture.Dispose();

            string[] ImagePath = (string[])state;
            //触发实时显示箱体图片
            string[] rows = { null, null, null, null, ImagePath[1], ImagePath[3], ImagePath[2], ImagePath[4], ImagePath[5], ImagePath[6] };
            clearnPicture?.Invoke();
            FromShowPicture?.Invoke(rows);
        }

        /// <summary>
        /// 箱号结果
        /// </summary>
        /// <param name="ConNum"></param>
        private void _Container_socket_Lpn_DLLConNum(DateTime TriggerTime, string ConNum)
        {
            //Nothing
        }
        #endregion


        /// <summary>
        /// 添加窗口到tabPage
        /// </summary>
        /// <param name="form"></param>
        private void SetTablePage(Form form)
        {
            form.TopLevel = false;
            form.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Show();
            tabPage1.Controls.Add(form);
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
            RunMessage(string.Format("车底触发：{0}",obj));
            _TimerDateStatus.Change(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
            string CheDiPath = string.Format(@"{0}\{1}\{2}\{3}\{4}_chedi.jpg", Container_ChediPath, LpnDt.Year.ToString(), LpnDt.Month.ToString().PadLeft(2, '0'), LpnDt.Day.ToString().PadLeft(2, '0'), LpnDt.ToString("yyyyMMddHHmmss"));

            try
            {
                if (File.Exists(obj))
                {
                    //Image.FromFile(obj).Save(CheDiPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    FileStream fs = new FileStream(obj, FileMode.Open, FileAccess.Read);
                    Image image= Image.FromStream(fs);
                    image.Save(CheDiPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                    image.Dispose();
                }
                if (_DataBase.InsertData(CheDiPath) == 1)
                {
                    Lognet.Log.Info("插入车底数据成功");

                    //触发实时显示车底图片
                    string[] rows = { null, null, null, null, null, null, null, null,null, CheDiPath };
                    //ShowPicture += _Picture.ShowPicture;
                    FromShowPicture?.Invoke(rows);
                }
            }
            catch (Exception)
            {
                Lognet.Log.Warn("车底图片保存路径不存在！");
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
                toolStripStatusLabel4.Text = "就绪";
            }
        }


        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindButton_Click(object sender, EventArgs e)
        {
            bool onlyContainer = false;//帅选箱号
            bool onlyPlate = false;//筛选车牌
            string CheckNum = "ALL";//筛选正确结果
            string Plate = string.Empty;
            string Container = string.Empty;

            //筛选车牌
            if(checkBox2.Checked)
            {
                onlyPlate = true;
            }
            //只查询集装箱
            if (checkBox1.Checked)
            {
                onlyContainer = true;
            }
            //只查正确结果
            if(radioButton2.Checked)
            {
                CheckNum = "Y";
            }
            //只查错误结果
            if (radioButton3.Checked)
            {
                CheckNum = "N";
            }

            if(radioPlateButton.Checked)
            {
                Plate = textBox1.Text;
            }
            if(radioContainerButton.Checked)
            {
                Container = textBox2.Text;
            }
            bindingSource1.DataSource = _DataBase.Select(dateTimePicker1.Value,dateTimePicker2.Value, Plate,Container, onlyContainer, onlyPlate, CheckNum).Tables["Picture"];
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            //切换tabpage
            tabControl1.SelectedIndex = 1;
        }

        /// <summary>
        /// 双击单元格显示图片窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.Rows.Count==0)
            {
                return;
            }
            string[] str = new string[dataGridView1.Columns.Count];
            int row = dataGridView1.CurrentRow.Index;
            for(int i=0;i<dataGridView1.Columns.Count;i++)
            {
                str[i] = dataGridView1.Rows[row].Cells[i].Value.ToString();
            }
            Picture _Picture = new Picture();
            ShowPicture += _Picture.ShowPicture;
            ShowPicture?.Invoke(str);
            _Picture.ShowDialog();
            ShowPicture -= _Picture.ShowPicture;
            _Picture.Dispose();
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

            //前相机，右相机，右相机
            for (int i=1;i<4;i++)
            {
                Message[i] = string.Format(@"{0}\{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane, i);
                //if (!System.IO.File.Exists(Message[i]))
                //{
                //    Message[i] = "nul";
                //}
            }
            //后相机
            Message[4] = string.Format(@"{0}\{1}{2}10.jpg", Path, dt.ToString("yyyyMMddHHmmss"), Container_Lane);
            //if (!System.IO.File.Exists(Message[4]))
            //{
            //    Message[4] = "nul";
            //}
            //车牌
            Message[5] = string.Format(@"{0}\{1}{2}{3}.jpg", Path, dt.ToString("yyyyMMddHHmmss"),Container_Lane ,Container_Plate_Name);
            //if (!System.IO.File.Exists(Message[5]))
            //{
            //    Message[5] = "nul";
            //}
            Message[6] = "nul";
            //Path = string.Format(@"{0}\{1}\{2}\{3}", Container_ChediPath, dt.Year.ToString(), dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'));
            //Message[6] = string.Format(@"{0}\{1}.jpg", Path, dt.ToString("yyyyMMddHHmmss"));//车底图片
            //if (!System.IO.File.Exists(Message[6]))
            //{
            //    Message[6] = "nul";
            //}
            return Message;
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogButton_Click(object sender, EventArgs e)
        {
            LogForm _logForm = new LogForm();
            _logForm.Show();
        }

        /// <summary>
        /// 显示图片按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPictureButton_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count==0)
            {
                return;
            }
            string[] str = new string[dataGridView1.Columns.Count];
            int row = dataGridView1.CurrentRow.Index;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                str[i] = dataGridView1.Rows[row].Cells[i].Value.ToString();
            }
            Picture _Picture = new Picture();
            ShowPicture += _Picture.ShowPicture;
            ShowPicture?.Invoke(str);
            _Picture.ShowDialog();
            ShowPicture -= _Picture.ShowPicture;
            _Picture.Dispose();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //    DialogResult result = MessageBox.Show("确认要关闭程序吗?","提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question); 
            //    if(result==DialogResult.No)
            //    {
            //        e.Cancel = true;
            //        this.WindowState = FormWindowState.Minimized;
            //        this.notifyIcon1.Visible = true;
            //    }
            //}
            ExitPasswordForm _PasswordForm = new ExitPasswordForm();
            _PasswordForm.PasswordAction += Exit_Password;
            _PasswordForm.ShowDialog();
            if (EXIT)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }

            //DialogResult result = MessageBox.Show("确认关闭程序吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (result == DialogResult.No)
            //{
            //    e.Cancel = true;
            //    this.WindowState = FormWindowState.Minimized;
            //    this.notifyIcon1.Visible = true;
            //}
        }

        private bool EXIT = false;
        /// <summary>
        /// 密码正确关闭程序
        /// </summary>
        /// <param name="obj"></param>
        private void Exit_Password(bool obj)
        {
            EXIT = obj;
        }

        /// <summary>
        /// 双击任务栏图标显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.Visible = true;//这个也可以            
            //this.Show();
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false; 
        }

        /// <summary>
        /// 清除数据表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = (DataTable)dataGridView1.DataSource;
                dt.Rows.Clear();
                dataGridView1.DataSource = dt;
            }
            catch (Exception)
            {
                ;//数据表不存在
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }
    }
}
