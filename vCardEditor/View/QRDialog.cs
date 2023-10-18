﻿using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace vCardEditor.View
{
    public partial class QRDialog : Form
    {
        public QRDialog(string content)
        {
            InitializeComponent();
            RenderQrCode(content);
        }

        public void RenderQrCode(string code)
        {
            //TODO Change ECCLevel
            QRCodeGenerator.ECCLevel eccLevel = 0;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, eccLevel))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                pictureBoxQRCode.BackgroundImage = qrCode.GetGraphic(20, Color.Black, Color.White, null, 1);
                pictureBoxQRCode.Size = new System.Drawing.Size(pictureBoxQRCode.Width, pictureBoxQRCode.Height);
                pictureBoxQRCode.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp|PNG Image|*.png|JPeg Image|*.jpg|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile())
                {
                    ImageFormat imageFormat = null;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            imageFormat = ImageFormat.Bmp;
                            break;
                        case 2:
                            imageFormat = ImageFormat.Png;
                            break;
                        case 3:
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case 4:
                            imageFormat = ImageFormat.Gif;
                            break;
                        default:
                            throw new NotSupportedException("File extension is not supported");
                    }

                    pictureBoxQRCode.BackgroundImage.Save(fs, imageFormat);
                }
            }
        }
    }
}
