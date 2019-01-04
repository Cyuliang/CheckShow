using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CheckShow
{
    public partial class Picture : Form
    {

        private Action<Image ,string> BigShowPictureFunc=null;
        public Picture()
        {
            InitializeComponent();

            label1.Text = "车底图片";
            label1.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(10, 10);

            label2.Text = "前图片";
            label2.Parent = pictureBox2;
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(10, 10);

            label3.Text = "后图片";
            label3.Parent = pictureBox3;
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(10, 10);

            label4.Text = "车牌图片";
            label4.Parent = pictureBox4;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(10, 10);

            label5.Text = "左图片";
            label5.Parent = pictureBox5;
            label5.BackColor = Color.Transparent;
            label5.Location = new Point(10, 10);

            label6.Text = "右图片";
            label6.Parent = pictureBox6;
            label6.BackColor = Color.Transparent;
            label6.Location = new Point(10, 10);

            label7.Text = "预留";
            label7.Parent = pictureBox7;
            label7.BackColor = Color.Transparent;
            label7.Location = new Point(10, 10);

            pictureBox1.Tag = 1;
            pictureBox2.Tag = 2;
            pictureBox3.Tag = 3;
            pictureBox4.Tag = 4;
            pictureBox5.Tag = 5;
            pictureBox6.Tag = 6;
            pictureBox7.Tag = 7;     
        }

        public void ShowPicture(string[] str)
        {
            try
            {
                this.Text = str[2]+"-（单击放大图片）";
                if (str[3] != null && System.IO.File.Exists(str[3]))
                {
                    pictureBox2.Image = ImageFromStream(str[3]);
                }
                if (str[4] != null && System.IO.File.Exists(str[4]))
                {
                    pictureBox5.Image = ImageFromStream(str[4]);
                }
                if (str[5] != null && System.IO.File.Exists(str[5]))
                {
                    pictureBox6.Image = ImageFromStream(str[5]);
                }
                if (str[6] != null && System.IO.File.Exists(str[6]))
                {
                    pictureBox3.Image = ImageFromStream(str[6]);
                }
                if (str[7] != null && System.IO.File.Exists(str[7]))
                {
                    pictureBox4.Image = ImageFromStream(str[7]);
                }

                if (str[8] != null && System.IO.File.Exists(str[8]))
                {
                    pictureBox1.Image = ImageFromStream(str[8]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("image error");
            }
        }

        /// <summary>
        /// 单击放大图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Click(object sender, EventArgs e)
        {
            Image image = null;
            string lable = string.Empty;
            PictureBox picture = (PictureBox)sender;
            if(picture.Image==null)
            {
                return;
            }
            switch(picture.Tag)
            {
                case 1:
                    image = pictureBox1.Image;
                    lable = label1.Text;
                    break;
                case 2:
                    image = pictureBox2.Image;
                    lable = label2.Text;
                    break;
                case 3:
                    image = pictureBox3.Image;
                    lable = label3.Text;
                    break;
                case 4:
                    image = pictureBox4.Image;
                    lable = label4.Text;
                    break;
                case 5:
                    image = pictureBox5.Image;
                    lable = label5.Text;
                    break;
                case 6:
                    image = pictureBox6.Image;
                    lable = label6.Text;
                    break;
                case 7:
                    image = pictureBox7.Image;
                    lable = label7.Text;
                    break;
            }
            BigShow _BigShow = new BigShow();
            BigShowPictureFunc += _BigShow.ShowPicture;
            BigShowPictureFunc?.Invoke(image,lable);
            _BigShow.WindowState = FormWindowState.Maximized;
            _BigShow.ShowDialog();
            BigShowPictureFunc -= _BigShow.ShowPicture;
            _BigShow.Dispose();
        }

        private void Picture_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach(Control control in Controls)
            {
                if(control is PictureBox)
                {
                    PictureBox p = (PictureBox)control;
                    p.Image = null;                                        
                    p.Dispose();
                }
            }
        }

        private Image ImageFromStream(string image)
        {
            FileStream stream = new FileStream(image,FileMode.Open,FileAccess.Read);
            Image img= Image.FromStream(stream);
            stream.Close();
            //stream.Dispose();
            return (img);
        }

        private void Picture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
