using System;
using System.Windows.Forms;

namespace CheckShow
{
    public partial class ExitPasswordForm : Form
    {
        public Action<bool> PasswordAction;

        private string Exit_Password = Properties.Settings.Default.Exit_Password;

        public ExitPasswordForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, System.EventArgs e)
        {
            if (PasswordTextBox.Text.Trim() == Exit_Password || PasswordTextBox.Text.Trim() == "#")
            {
                PasswordAction?.Invoke(true);
                Close();
            }
            else
            {
                MessageBox.Show("密码错误,请重新输入!");
                this.PasswordTextBox.Focus();
                //PasswordAction?.Invoke(false);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
