using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VideoRemoteController
{
    public partial class FormUrl : Form
    {
        public FormUrl()
        {
            InitializeComponent();
        }

        private void FormUrl_Load(object sender, EventArgs e)
        {
            string url = "http://" + Environment.MachineName;
            this.labelUrl.Text = url;

            var qrCode = new QrCode(url, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCode.GenerateImage(stream);
                stream.Position = 0;
                imgQRCode.SizeMode = PictureBoxSizeMode.Zoom;
                imgQRCode.Image = Image.FromStream(stream);
            }
        }
    }
}
