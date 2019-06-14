using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Services
{
    public class Downloader
    {
        public async Task<string> DownloadRawJsonDataAsync(string url)
        {
            using (var client = new WebClient())
            {
                try
                {
                    return await client.DownloadStringTaskAsync(url);
                }
                catch (WebException exception)
                {
                    return $"{exception.Message}";
                }
            }
        }
    }
}
