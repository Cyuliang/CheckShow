using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckShow
{
    public partial class BigShow : Form
    {
        public BigShow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="image"></param>
        public void ShowPicture(Image image,string lable)
        {
            this.Text = lable + "-（双击关闭窗口）";
            pictureBox1.Image = image;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Close();            
        }

        /// <summary>
        /// 放大图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image==null)
            {
                return;
            }
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                Title = "保存",
                OverwritePrompt = true,
                Filter = "JPEG文件(*.jpg)|*.jpg",
                ShowHelp = true
            };
            if (saveDlg.ShowDialog()==DialogResult.OK)
            {
                string filename = saveDlg.FileName;
                pictureBox1.Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void BigShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
