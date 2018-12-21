using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckShow
{
    public partial class Picture : Form
    {

        private Action<Image> BigShowPictureFunc=null;
        public Picture()
        {
            InitializeComponent();

            label1.Text = "车底图片";
            label1.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(10, 10);

            label2.Text = "左前图片";
            label2.Parent = pictureBox2;
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(10, 10);

            label3.Text = "左后图片";
            label3.Parent = pictureBox3;
            label3.BackColor = Color.Transparent;
            label3.Location = new Point(10, 10);

            label4.Text = "右前图片";
            label4.Parent = pictureBox4;
            label4.BackColor = Color.Transparent;
            label4.Location = new Point(10, 10);

            label5.Text = "右后图片";
            label5.Parent = pictureBox5;
            label5.BackColor = Color.Transparent;
            label5.Location = new Point(10, 10);

            label6.Text = "前图片";
            label6.Parent = pictureBox6;
            label6.BackColor = Color.Transparent;
            label6.Location = new Point(10, 10);

            label7.Text = "后图片";
            label7.Parent = pictureBox7;
            label7.BackColor = Color.Transparent;
            label7.Location = new Point(10, 10);

            label8.Text = "车牌图片";
            label8.Parent = pictureBox8;
            label8.BackColor = Color.Transparent;
            label8.Location = new Point(10, 10);

            label9.Text = "预留位";
            label9.Parent = pictureBox9;
            label9.BackColor = Color.Transparent;
            label9.Location = new Point(10, 10);

            pictureBox1.Tag = 1;
            pictureBox2.Tag = 2;
            pictureBox3.Tag = 3;
            pictureBox4.Tag = 4;
            pictureBox5.Tag = 5;
            pictureBox6.Tag = 6;
            pictureBox7.Tag = 7;
            pictureBox8.Tag = 8;
            pictureBox9.Tag = 9;          
        }

        public void ShowPicture(string[] str)
        {
            try
            {
                if(str[3]!=null&& System.IO.File.Exists(str[3]))
                {
                    pictureBox1.Image = Image.FromFile(str[3]);
                }
                if(str[4]!=null && System.IO.File.Exists(str[4]))
                {
                    pictureBox2.Image = Image.FromFile(str[4]);
                }
                if(str[5]!=null && System.IO.File.Exists(str[5]))
                {
                    pictureBox3.Image = Image.FromFile(str[5]);
                }
                if(str[6]!=null && System.IO.File.Exists(str[6]))
                {
                    pictureBox4.Image = Image.FromFile(str[6]);
                }
                if(str[7]!=null && System.IO.File.Exists(str[7]))
                {
                    pictureBox5.Image = Image.FromFile(str[7]);
                }
                if(str[8]!=null && System.IO.File.Exists(str[8]))
                {
                    pictureBox6.Image = Image.FromFile(str[8]);
                }
                if(str[9]!=null && System.IO.File.Exists(str[9]))
                {
                    pictureBox7.Image = Image.FromFile(str[9]);
                }
                if(str[10]!=null && System.IO.File.Exists(str[10]))
                {
                    pictureBox8.Image = Image.FromFile(str[10]);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("error");
            }
        }

        /// <summary>
        /// 双击放大图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            Image image = null;
            PictureBox picture = (PictureBox)sender;
            switch(picture.Tag)
            {
                case 1:
                    image = pictureBox1.Image;
                    break;
                case 2:
                    image = pictureBox2.Image;
                    break;
                case 3:
                    image = pictureBox3.Image;
                    break;
                case 4:
                    image = pictureBox4.Image;
                    break;
                case 5:
                    image = pictureBox5.Image;
                    break;
                case 6:
                    image = pictureBox6.Image;
                    break;
                case 7:
                    image = pictureBox7.Image;
                    break;
                case 8:
                    image = pictureBox8.Image;
                    break;
                case 9:
                    image = pictureBox9.Image;
                    break;
            }
            BigShow _BigShow = new BigShow();
            BigShowPictureFunc += _BigShow.ShowPicture;
            BigShowPictureFunc?.Invoke(image);
            _BigShow.ShowDialog(); 
        }
    }
}
