using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProtonDesktop
{
    public partial class UpdateForm : Form
    {
        private readonly string downloadUrl;
        private readonly HttpClient httpClient;
        private string tempFilePath;
        private string logFilePath;

        public UpdateForm(string url)
        {
            InitializeComponent();
            downloadUrl = url;
            httpClient = new HttpClient();
        }

        private async void UpdateForm_Load(object sender, EventArgs e)
        {
            logFilePath = Path.Combine(Path.GetTempPath(), "ProtonDesktop_UpdateLog.txt");
            tempFilePath = Path.Combine(Path.GetTempPath(), "ProtonDesktop_Update.exe");

            AddLog($"Démarrage du téléchargement depuis : {downloadUrl}");
            try
            {
                using (var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                TriggerProgressChanged(totalBytes, totalRead);
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            if (canReportProgress)
                            {
                                TriggerProgressChanged(totalBytes, totalRead);
                            }

                        } while (isMoreToRead);
                    }
                }

                AddLog("Téléchargement terminé avec succès !");
                AddLog("Lancement de l'installateur...");

                Process.Start(new ProcessStartInfo(tempFilePath)
                {
                    UseShellExecute = true
                });
                AddLog("Installeur lancé. L'application va se fermer.");
            }
            catch (Exception ex)
            {
                AddLog($"Erreur lors du téléchargement ou du lancement : {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }

        private void TriggerProgressChanged(long totalBytes, long totalRead)
        {
            if (totalBytes <= 0)
                return;

            int progress = (int)((double)totalRead / totalBytes * 100);
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = progress));
                textBoxLogs.Invoke(new Action(() => AddLog($"Téléchargement... {progress}%")));
            }
            else
            {
                progressBar1.Value = progress;
                AddLog($"Téléchargement... {progress}%");
            }
        }

        private void AddLog(string message)
        {
            textBoxLogs.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now:HH:mm:ss} - {message}");
                }
            }
            catch
            {
                // Ignorer les erreurs de log
            }
        }
    }
}
