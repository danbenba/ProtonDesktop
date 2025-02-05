using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Net;
using System.Windows.Forms;

namespace ProtonDesktop
{
    public partial class Form1 : Form
    {
        private WebView2? webView;
        private const string SettingsFile = "window_settings.txt";
        
        public Form1()
        {
            InitializeComponent();
            LoadWindowSettings();
            InitializeWebView2Async();
            this.Text = "Proton Desktop - Version 0.5 - GitHub.com/danbenba/ProtonDesktop";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream iconStream = assembly.GetManifestResourceStream("ProtonDesktop.icon.ico"))
                {
                    if (iconStream != null)
                    {
                        this.Icon = new Icon(iconStream);
                    }
                    else
                    {
                        // Gestion des erreurs si l'icône n'est pas trouvée
                        MessageBox.Show("Icône de l'application non trouvée dans les ressources intégrées.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors du chargement de l'icône
                MessageBox.Show("Erreur lors du chargement de l'icône : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void InitializeWebView2Async()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(webView);

            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: Path.Combine(Application.StartupPath, "WebView2UserData"));

            await webView.EnsureCoreWebView2Async(env);
            webView.CoreWebView2.Navigate("https://mail.protonmail.com/");
            webView.CoreWebView2.NavigationStarting += CoreWebView2OnNavigationStarting;
            webView.CoreWebView2.DownloadStarting += CoreWebView2OnDownloadStarting;
        }

        private void CoreWebView2OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var uri = new Uri(e.Uri);
            if (!IsProtonMailUrl(uri))
            {
                e.Cancel = true;
                MessageBox.Show("Seul ProtonMail est autorisé dans ce navigateur.", "Navigation Bloquée",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CoreWebView2OnDownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = Path.GetFileName(e.ResultFilePath);
                sfd.Filter = "Tous les fichiers (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    e.ResultFilePath = sfd.FileName;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private bool IsProtonMailUrl(Uri uri)
        {
            var host = uri.Host.ToLowerInvariant();
            return host.EndsWith("protonmail.com") || host.EndsWith("proton.me");
        }

        private void LoadWindowSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    var settings = File.ReadAllText(SettingsFile).Split(',');
                    if (settings.Length == 2 && int.TryParse(settings[0], out int width) && int.TryParse(settings[1], out int height))
                    {
                        this.Size = new Size(width, height);
                    }
                }
                catch { }
            }
            else
            {
                this.Size = new Size(1376, 822);
            }
        }

        private void SaveWindowSettings()
        {
            File.WriteAllText(SettingsFile, $"{this.Width},{this.Height}");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveWindowSettings();
            base.OnFormClosing(e);
        }

    }
}
