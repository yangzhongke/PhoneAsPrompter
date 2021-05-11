﻿using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Windows.Forms;

namespace VideoRemoteController
{
    public partial class FormMain : Form
    {
        private LibVLC libVLC;
        private MediaPlayer mediaPlayer;
        private IWebHost host;
        public FormMain()
        {
            InitializeComponent();

            this.miOpen.Click += MiOpen_Click;
            this.miShowUrl.Click += MiShowUrl_Click;

            //https://codesailer.com/tutorials/simple_video_player/
            /*NuGet: LibVLCSharp.WinForms、VideoLAN.LibVLC.Windows 
             */
            Core.Initialize();
            this.libVLC = new LibVLC();
            this.mediaPlayer = new MediaPlayer(libVLC);

            VideoView videoView = new VideoView();
            videoView.Dock = DockStyle.Fill;
            this.Controls.Add(videoView);

            videoView.MediaPlayer = mediaPlayer;
            mediaPlayer.Play(new Media(libVLC, @"F:\珍贵音频视频\缝纫机乐队 不再犹豫.mp4"));

            host = new WebHostBuilder()
               .UseKestrel()
               .UseUrls("http://*:80")
               .Configure(Configure)
               .Build();
            host.RunAsync();
            this.Closed += Form1_Closed;
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
                if (path == "/play")
                {
                    response.StatusCode = 200;
                    this.BeginInvoke(new Action(() => {
                        mediaPlayer.Play();
                    }));
                    await response.WriteAsync("OK");
                }
                else if (path == "/pause")
                {
                    response.StatusCode = 200;
                    this.BeginInvoke(new Action(() => {
                        mediaPlayer.Pause();
                    }));
                    await response.WriteAsync("OK");
                }
                else if (path == "/setVolume")
                {
                    response.StatusCode = 200;
                    int value = Convert.ToInt32(request.Query["value"]);
                    this.BeginInvoke(new Action(() => {
                        mediaPlayer.Volume= value;
                    }));
                    await response.WriteAsync("OK");
                }
                else
                {
                    response.StatusCode = 404;
                }
            });
        }

        private void MiShowUrl_Click(object sender, System.EventArgs e)
        {
            FormUrl form = new FormUrl();
            form.ShowDialog(this);
        }

        private void MiOpen_Click(object sender, System.EventArgs e)
        {
            if(ofdOpenVideo.ShowDialog()!= DialogResult.OK)
            {
                return;
            }
            mediaPlayer.Play(new Media(libVLC, ofdOpenVideo.FileName));
        }
    }
}