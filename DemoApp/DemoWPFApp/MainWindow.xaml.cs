using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using SkiaSharp.QrCode.Image;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DemoWPFApp
{
    public partial class MainWindow : Window
    {
        private IWebHost host;

        public MainWindow()
        {
            InitializeComponent();
            ShowUrl();

            host = new WebHostBuilder()
               .UseKestrel()
               .UseUrls("http://*:80")
               .Configure(Configure)
               .Build();
            host.RunAsync();
            this.Closed += MainWindow_Closed;
        }

        private void ShowUrl()
        {
            string url = "http://" + Environment.MachineName;
            this.labelUrl.Content = url;

            var qrCode = new QrCode(url, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCode.GenerateImage(stream);
                stream.Position = 0;

                imgQRCode.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
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
                if (path == "/report")
                {
                    response.StatusCode = 200;
                    string value = request.Query["value"];
                    await this.Dispatcher.BeginInvoke(new Action(() => {
                        this.labelMsg.Content = value;
                    }));
                    await response.WriteAsync("OK");
                }
                else
                {
                    response.StatusCode = 404;
                }
            });
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            host.StopAsync();
            host.WaitForShutdown();
        }
    }
}
