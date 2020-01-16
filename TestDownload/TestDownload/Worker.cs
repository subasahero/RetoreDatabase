using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestDownload
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Vòng lặp vô hạn
            while (!stoppingToken.IsCancellationRequested)
            {

                List<string> pathFolders = new List<string> {
                    @"D:\Demo\DemoBackUp\Account",
                    @"D:\Demo\DemoBackUp\Test",
                };
                List<string> ftpPaths = new List<string>
                {
                    "",
                    @"\Test",
                };
                try
                {
                    for (int i = 0; i < pathFolders.Count; i++)
                    {
                        
                        _logger.LogInformation("Connect! Ok");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Fail! Ok");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        private async Task<FileInfo> DownloadFile(string pathFolder, string ftpPath)
        {
            //Get file from local
            DirectoryInfo info = new DirectoryInfo(pathFolder);
            FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
            FileInfo need = files[0]; // file newest in folder local

            // Get file from ftp
            FtpClient client = new FtpClient();
            client.Host = "ftp://pmbk.vn";
            //client.Credentials = new NetworkCredential("david", "pass123");
            client.Connect();
            FtpListItem file = client.GetListing(ftpPath)[0];
            foreach (FtpListItem item in client.GetListing(ftpPath))
            {
                if (file.Created < item.Created)
                {
                    file = item;
                }
            }

            if (!need.Name.Equals(file.Name))
            {
                await client.DownloadFileAsync(pathFolder + file.Name, file.FullName);
                //Get file from local
                DirectoryInfo infoNew = new DirectoryInfo(pathFolder);
                FileInfo[] fileNews = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                need = fileNews[0]; // file newest in folder local
            }

            return need;
        }
    }
}
