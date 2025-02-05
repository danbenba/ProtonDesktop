using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProtonDesktop
{
    public partial class DownloadProgressForm : Form
    {
        private CoreWebView2DownloadOperation _downloadOperation;
        private DownloadInfo _downloadInfo;

        // Timers for fade-in/fade-out animations
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.Timer? fadeOutTimer;

        // Import the CreateRoundRectRgn function from gdi32.dll
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,      // x-coordinate of upper-left corner
            int nTopRect,       // y-coordinate of upper-left corner
            int nRightRect,     // x-coordinate of lower-right corner
            int nBottomRect,    // y-coordinate of lower-right corner
            int nWidthEllipse,  // width of ellipse
            int nHeightEllipse  // height of ellipse
        );

        public DownloadProgressForm(CoreWebView2DownloadOperation downloadOperation, DownloadInfo info)
        {
            InitializeComponent();

            // Define rounded region
            this.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20)); // 20px rounded corners

            _downloadOperation = downloadOperation;
            _downloadInfo = info;

            // Initialize label with file name
            lblFileName.Text = info.FileName;

            // Subscribe to download progress events
            _downloadOperation.BytesReceivedChanged += DownloadOperation_BytesReceivedChanged;
            _downloadOperation.StateChanged += DownloadOperation_StateChanged;

            // Configure buttons
            btnCancel.Click += (s, e) => CancelDownload();
            btnLogs.Click += (s, e) => ShowLogs();

            // Configure ProgressBar
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.ForeColor = Color.DodgerBlue;
            progressBar1.Height = 10; // Make ProgressBar thinner

            // Setup fade-in animation
            this.Opacity = 0;
            fadeInTimer = new System.Windows.Forms.Timer { Interval = 20 }; // every 20ms
            fadeInTimer.Tick += (s, e) =>
            {
                this.Opacity += 0.05; // increase opacity by 5%
                if (this.Opacity >= 1)
                {
                    this.Opacity = 1;
                    fadeInTimer.Stop();
                }
            };
            fadeInTimer.Start();
        }

        private void CancelDownload()
        {
            try
            {
                _downloadOperation.Cancel();
                _downloadInfo.Status = "Annulé";
            }
            catch
            {
                // Ignore exceptions on cancellation
            }

            // Close popup with fade-out animation
            FadeOutAndClose();
        }

        // Event handler with correct signature
        private void DownloadOperation_BytesReceivedChanged(object sender, object e)
        {
            // Retrieve bytes received and total bytes
            ulong totalBytes = _downloadOperation.TotalBytesToReceive ?? 0UL;

            // Calculate progress percentage
            int percent = 0;
            if (totalBytes > 0UL)
            {
            }

            // Update ProgressBar on UI thread
            progressBar1.Invoke((Action)(() =>
            {
                progressBar1.Value = Math.Min(percent, 100);
            }));
        }

        // Event handler with correct signature
        private void DownloadOperation_StateChanged(object sender, object e)
        {
            if (_downloadOperation.State == CoreWebView2DownloadState.Completed)
            {
                // Download completed
                _downloadInfo.Status = "Terminé";

                // Offer to open the file or folder
                // Must be done on UI thread
                Invoke((Action)(() => ShowCompletionDialog()));
            }
            else if (_downloadOperation.State == CoreWebView2DownloadState.Interrupted)
            {
                // Download interrupted (error or cancellation)
                if (_downloadOperation.InterruptReason == CoreWebView2DownloadInterruptReason.UserCanceled)
                {
                    _downloadInfo.Status = "Annulé";
                }
                else
                {
                    _downloadInfo.Status = $"Erreur: {_downloadOperation.InterruptReason}";
                }

                // Close popup with fade-out animation
                Invoke((Action)(() => FadeOutAndClose()));
            }
        }

        private void ShowCompletionDialog()
        {
            var result = MessageBox.Show(
                "Téléchargement terminé.\n\nVoulez-vous ouvrir le fichier ?\nSinon, ouvrir le dossier.",
                "Terminé",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                // Open the downloaded file
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _downloadInfo.ResultFilePath,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Ignore exceptions when opening file
                }
            }
            else if (result == DialogResult.No)
            {
                // Open the folder containing the downloaded file
                var folder = Path.GetDirectoryName(_downloadInfo.ResultFilePath);
                if (!string.IsNullOrEmpty(folder))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = folder,
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // Ignore exceptions when opening folder
                    }
                }
            }

            // Close popup with fade-out animation
            FadeOutAndClose();
        }

        private void ShowLogs()
        {
            // Show the download history window
            LogForm logForm = new LogForm();
            logForm.Show();
        }

        private void FadeOutAndClose()
        {
            // Initialize fade-out timer
            fadeOutTimer = new System.Windows.Forms.Timer { Interval = 20 };
            fadeOutTimer.Tick += (s, e) =>
            {
                this.Opacity -= 0.05; // Decrease opacity by 5%
                if (this.Opacity <= 0)
                {
                    this.Opacity = 0;
                    fadeOutTimer.Stop();
                    this.Close();
                }
            };
            fadeOutTimer.Start();
        }
    }
}
