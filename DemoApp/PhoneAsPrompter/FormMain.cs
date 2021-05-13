using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Zack.ComObjectHelpers;

namespace PhoneAsPrompter
{
    public partial class FormMain : Form
    {
        private dynamic presentation;
        private COMReferenceTracker comRefTracker = new COMReferenceTracker();
        private IWebHost host;

        public FormMain()
        {
            InitializeComponent();

            ShowUrl();

            this.miOpen.Click += MiOpen_Click;
            host = new WebHostBuilder()
               .UseKestrel()
               .UseUrls("http://*:80")
               .Configure(Configure)
               .Build();
            host.RunAsync();
            this.Closed += Form1_Closed;
        }

        private void ShowUrl()
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

        private void Form1_Closed(object sender, EventArgs e)
        {
            ClearComRefs();
            host.StopAsync();
            host.WaitForShutdown();
            Process.GetCurrentProcess().Kill();//Forcely exit
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            //需要把wwwroot目录中的所有文件设置为“如果较新则复制”
            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                var request = context.Request;
                var response = context.Response;
                string path = request.Path.Value;
                bool hasRun = true;
                if (path == "/getNote")
                {
                    response.StatusCode = 200;
                    string notesText = null;                   
                    this.Invoke(new Action(()=> {
                        if (this.presentation == null)
                        {
                            return;
                        }

                        try
                        {
                            dynamic notesPage = T(T(T(T(presentation.SlideShowWindow).View).Slide).NotesPage);
                            notesText = GetInnerText(notesPage);
                        }
                        catch (COMException ex)
                        {
                            notesText = "";
                        }
                    }));
                    await response.WriteAsync(notesText);
                }
                else if (path == "/next")
                {
                    response.StatusCode = 200;
                    this.Invoke(new Action(() => {
                        if (this.presentation == null)
                        {
                            return;
                        }
                        try
                        {
                            T(T(this.presentation.SlideShowWindow).View).Next();
                            hasRun = true;
                        }
                        catch(COMException e)
                        {
                            hasRun = false;
                        }
                        
                    }));
                    if (hasRun)
                    {
                        await response.WriteAsync("OK");
                    }
                    else
                    {
                        await response.WriteAsync("NO");
                    }
                }
                else if (path == "/previous")
                {
                    response.StatusCode = 200;
                    this.Invoke(new Action(() => {
                        if (this.presentation == null)
                        {
                            return;
                        }
                        try
                        {
                            T(T(this.presentation.SlideShowWindow).View).Previous();
                            hasRun = true;
                        }
                        catch (COMException e)
                        {
                            hasRun = false;
                        }
                    }));
                    if (hasRun)
                    {
                        await response.WriteAsync("OK");
                    }
                    else
                    {
                        await response.WriteAsync("NO");
                    }
                    
                }
                else
                {
                    response.StatusCode = 404;
                }
            });
        }

        private string GetInnerText(dynamic part)
        {
            StringBuilder sb = new StringBuilder();
            dynamic shapes = T(T(part).Shapes);
            int shapesCount = shapes.Count;
            for (int i = 0; i < shapesCount; i++)
            {
                dynamic shape = T(shapes[i + 1]);
                var textFrame = T(shape.TextFrame);
                if (textFrame.HasText == -1)//MsoTriState.msoTrue==-1
                {
                    string text = T(textFrame.TextRange).Text;
                    sb.AppendLine(text);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }


        private void ClearComRefs()
        {
            try
            {
                if(this.presentation!=null)
                {
                    T(this.presentation.Application).Quit();
                    this.presentation = null;
                }                
            }
            catch(COMException ex)
            {
                Debug.WriteLine(ex);
            }
            this.comRefTracker.Dispose();
            this.comRefTracker = new COMReferenceTracker();
        }

        private dynamic T(dynamic comObj)
        {
            return this.comRefTracker.T(comObj);
        }

        private void MiOpen_Click(object sender, System.EventArgs e)
        {
            if(openFileDialog.ShowDialog()!= DialogResult.OK)
            {
                return;
            }
            //@"E:\主同步盘\我的坚果云\UoZ\SE101-玩着学编程\Part3课件\5-搞“对象”.pptx"
            string filename = openFileDialog.FileName;
            this.ClearComRefs();
            dynamic pptApp = T(PowerPointHelper.CreatePowerPointApplication());
            pptApp.Visible = true;
            dynamic presentations = T(pptApp.Presentations);
            this.presentation = T(presentations.Open(filename));
            T(this.presentation.SlideShowSettings).Run();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe",linkLabel.Text);
        }
    }
}
