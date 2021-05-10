using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VideoRemoteController;
using Zack.ComObjectHelpers;

namespace PhoneAsPrompter
{
    public partial class Form1 : Form
    {
        private dynamic pptApp;
        private dynamic presentation;
        private IWebHost host;

        public Form1()
        {
            InitializeComponent();

            this.miOpen.Click += MiOpen_Click;
            this.miShowUrl.Click += MiShowUrl_Click;

            host = new WebHostBuilder()
               .UseKestrel()
               .UseUrls("http://*:80")
               .Configure(Configure)
               .Build();
            host.RunAsync();
            this.Closed += Form1_Closed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                           
        }
        private void Form1_Closed(object sender, EventArgs e)
        {
            host.StopAsync();
            host.WaitForShutdown();
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
                if (path == "/getNote")
                {
                    response.StatusCode = 200;
                    string notesText = null;                   
                    this.Invoke(new Action(()=> {
                        if (pptApp == null)
                        {
                            return;
                        }
                        
                        using (COMReferenceTracker ct = new COMReferenceTracker())
                        {
                            // dynamic activeWindow = ct.T(pptApp.ActiveWindow);
                            //dynamic activeSlide = ct.T(ct.T(activeWindow.View)).Slide;
                            // dynamic notesPage = ct.T(activeSlide.NotesPage);
                            try
                            {
                                dynamic notesPage = ct.T(presentation.SlideShowWindow.View.Slide.NotesPage);
                                notesText = GetInnerText(notesPage, ct);
                            }
                            catch(COMException ex)
                            {
                                notesText = "";
                            }
                        }
                    }));
                    await response.WriteAsync(notesText);
                }
                else
                {
                    response.StatusCode = 404;
                }
            });
        }

        private static string GetInnerText(dynamic part, COMReferenceTracker t)
        {
            StringBuilder sb = new StringBuilder();
            dynamic shapes = t.T(t.T(part).Shapes);
            int shapesCount = shapes.Count;
            for (int i = 0; i < shapesCount; i++)
            {
                dynamic shape = t.T(shapes[i + 1]);
                var textFrame = t.T(shape.TextFrame);
                if (textFrame.HasText == -1)//MsoTriState.msoTrue==-1
                {
                    string text = t.T(textFrame.TextRange).Text;
                    sb.AppendLine(text);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void MiShowUrl_Click(object sender, System.EventArgs e)
        {
            FormUrl form = new FormUrl();
            form.ShowDialog();
        }

        private void MiOpen_Click(object sender, System.EventArgs e)
        {
            using (COMReferenceTracker ct = new COMReferenceTracker())
            {
                pptApp = PowerPointHelper.CreatePowerPointApplication();//don't ct.T(pptApp)
                //because it will be reused
                pptApp.Visible = true;
                dynamic presentations = pptApp.Presentations;
                this.presentation = presentations.Open(@"E:\主同步盘\我的坚果云\UoZ\SE101-玩着学编程\Part3课件\5-搞“对象”.pptx");
                //string s = ComHelper.GetComObjectDescription(this.presentation);
                ct.T(this.presentation.SlideShowSettings).Run();
            }
        }
    }
}
