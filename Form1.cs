using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using Ryadel.Components.Security;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;


namespace GitHubDemoTest
{
    public partial class Form1 : Form
    {
        private string Plaintext = null;//密码明文
        private string encryptedText = null;//加密后的文本
        private string sourceText = null;//解密后的文本

        #region 窗体初始化
        private void Form1_Load(object sender,EventArgs e)
        {
            this.Icon = new Icon("calendar.ico", new Size(32,32));
            this.Text = "码片打印";
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        #endregion


        #region 构造函数
        public Form1()
                {
                    InitializeComponent();
                    this.label5.Image = new Bitmap("calendar.ico");
                    this.label5.Text = "^_^";
                    this.monthCalendar1.Hide();
                }
        #endregion


        #region 码片生成并保存
        private void GenerateAndSaveCode_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length == 0 || this.textBox2.Text.Length == 0 || this.textBox3.Text.Length == 0 || this.textBox4.Text.Length==0)
            {
            MessageBox.Show("信息填写不完整");
            return;
            }
            string savePath = string.Empty;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.CheckFileExists = false;
            dlg.DefaultExt = "png";
            dlg.Title = "保存文件";
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                savePath = dlg.FileName;
            }

           
            GetPlaintext();
            AESCriptText();
            Image img = RenderQrCode();

            img.Save(savePath);
            img.Dispose();
        }
        #endregion
        

        #region 码片生成
            private Image RenderQrCode()
            {
            string level = "Q";
            QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(encryptedText, eccLevel);//生成QR码

            QRCode qrCode = new QRCode(qrCodeData);
            Image img = qrCode.GetGraphic(10, Color.Black, Color.White, null, 0);
            return img;
        }
        #endregion


        #region 产生明文
                public void GetPlaintext()
                {
                    Plaintext = string.Empty;
                    Plaintext += this.textBox1.Text;
                    Plaintext += " ";
                    Plaintext += this.textBox2.Text;  
                    Plaintext += " ";
                    Plaintext += this.textBox3.Text;
                    Plaintext += " ";
                    Plaintext += this.textBox4.Text;
                }
        #endregion
        

        #region 加密
            public void AESCriptText()
            {
                    string iv = "0987654321ABCDEF";
                    string passPhrase = "1234567890ABCDEF";
                    encryptedText = MyRijndaelManaged.AesEncrypt(Plaintext, passPhrase, iv);
            }
        #endregion


        #region 解密
                public void AESDecriptText()
                {
                    string iv = "0987654321ABCDEF";
                    string passPhrase = "1234567890ABCDEF";
                    sourceText = MyRijndaelManaged.AesDecrypt(encryptedText, passPhrase, iv);
                }
        #endregion


        #region 码片选择
                private void CodeChoose_Click(object sender, EventArgs e)
                {
                     this.textBox1.Text = string.Empty;
                     this.textBox2.Text = string.Empty;
                     this.textBox3.Text = string.Empty;
                     this.textBox4.Text = string.Empty;
                     OpenFileDialog openFileDialog = new OpenFileDialog();
                     openFileDialog.Title = "码片选择";
                     openFileDialog.FileName = string.Empty;
                     openFileDialog.Filter = "QrCode文件|*.png|所有文件|*.*";
                     openFileDialog.FilterIndex = 1;
                     openFileDialog.CheckFileExists = true;
                     openFileDialog.InitialDirectory = "C:\\Users\\Auto Serve\\Pictures\\QrCode\\";

                    if (openFileDialog.ShowDialog()==DialogResult.OK)
                    {
               
                       string localFilePath = openFileDialog.FileName;
                       Image showimage = Image.FromFile(localFilePath);
                        if(this.pictureBox1.BackgroundImage!=null)
                        this.pictureBox1.BackgroundImage.Dispose();//如果选择码片，图片已经显示，则销毁
                        this.pictureBox1.Width = showimage.Width;
                        this.pictureBox1.Height = showimage.Height;
                        this.pictureBox1.BackgroundImage = showimage;
                        DecodeQRCode();
                        AESDecriptText();
                        SplitSourceText();
                    }
                    else
                    {
                        MessageBox.Show("文件获取失败");
                        return;
                     }
                }
        #endregion


        #region 解析二维码
        public void DecodeQRCode()
        {
            encryptedText = string.Empty;
            QRCodeDecoder qrDecoder = new QRCodeDecoder();
            QRCodeImage qrImage = new QRCodeBitmapImage((Bitmap)this.pictureBox1.BackgroundImage);
            encryptedText = qrDecoder.decode(qrImage,Encoding.UTF8);
        }
        #endregion


        #region 明文还原
        public void SplitSourceText()
        {
            string[] stra = sourceText.Split(' ');
            this.textBox1.Text = stra[0];
            this.textBox2.Text = stra[1];
            this.textBox3.Text = stra[2];
            this.textBox4.Text = stra[3];
        }
        #endregion
        
       
        #region 码片预览
        private void CodePreview_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length==0 || textBox2.Text.Length==0 || textBox3.Text.Length==0 || textBox4.Text.Length==0)
            {
                MessageBox.Show("请将信息填写完整");
                return;
            }

            GetPlaintext();
            AESCriptText();
            Image img =RenderQrCode();
            this.pictureBox1.Width = img.Width;
            this.pictureBox1.Height = img.Height;
            this.pictureBox1.BackgroundImage = img;
           
        }
        #endregion


        #region 选择打印日期
        private void label5_Click(object sender, EventArgs e)
        {
            this.label5.Hide();
            this.monthCalendar1.Show();
        }
        #endregion
        

        #region 日历日期选择事件
        private void monthCalendar1_DateSelected(object sender,EventArgs e)
        {
            this.monthCalendar1.Hide();
            this.textBox3.Text= this.monthCalendar1.SelectionStart.ToString("yyyy/MM/dd");
            this.label5.Show();
        }
        #endregion

    }
}
