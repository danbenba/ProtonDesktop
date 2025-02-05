using System;
using System.Windows.Forms;

namespace ProtonDesktop
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void LoadLogs()
        {
            foreach (var download in DownloadManager.Downloads)
            {
                var row = new string[]
                {
                    download.FileName,
                    download.Status,
                    download.BytesReceived > 0 && download.TotalBytes > 0 
                        ? $"{(download.BytesReceived * 100) / download.TotalBytes}%"
                        : "N/A"
                };
                listViewLogs.Items.Add(new ListViewItem(row));
            }
        }
    }
}
