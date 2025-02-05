using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace ProtonDesktop
{
    public class HiddenAppContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private WebView2 webView;
        private CoreWebView2? coreWebView2;

        public HiddenAppContext()
        {
            // 1) Créer une icône dans la zone de notification (system tray)
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                Text = "ProtonDesktop - Mode Fond"
            };

            // Menu contextuel de l'icône tray
            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Ouvrir ProtonDesktop", null, OpenMainForm);
            trayIcon.ContextMenuStrip.Items.Add("Quitter", null, Exit);

            // Permet d'ouvrir l'UI (Form1) si on double-clique sur l'icône
            trayIcon.DoubleClick += (s, e) => ShowMainForm();

            // 2) Initialiser WebView2 de manière invisible
            webView = new WebView2
            {
                Size = new Size(0, 0),
                Location = new Point(0, 0),
                Visible = false
            };

            // Ajouter le contrôle à un parent (nécessaire pour l'initialisation)
            Form hiddenForm = new Form
            {
                Size = new Size(0, 0),
                ShowInTaskbar = false,
                Opacity = 0,
                FormBorderStyle = FormBorderStyle.None
            };
            hiddenForm.Controls.Add(webView);
            hiddenForm.Show();

            // Initialisation asynchrone
            InitWebViewAsync();
        }

        private async void InitWebViewAsync()
        {
            try
            {
                // Chemin de stockage local des données (cookies, etc.)
                var env = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: Path.Combine(Application.StartupPath, "WebView2UserData"));

                await webView.EnsureCoreWebView2Async(env);
                coreWebView2 = webView.CoreWebView2;

                // On navigue vers ProtonMail
                coreWebView2.Navigate("https://mail.protonmail.com/");

                // Bloquer toute navigation hors domaine proton*
                coreWebView2.NavigationStarting += CoreWebView2OnNavigationStarting;

                // Gérer téléchargements
                coreWebView2.DownloadStarting += CoreWebView2OnDownloadStarting;

                // Injecter un script pour compter les mails non lus
                coreWebView2.DOMContentLoaded += (s, e) =>
                {
                    string script = @"
                        (function() {
                            setInterval(() => {
                                let unread = 0;
                                let badge = document.querySelector('.sidebarLink .badge');
                                if(badge) {
                                    unread = parseInt(badge.textContent) || 0;
                                }

                                window.chrome.webview.postMessage({ type: 'unreadCount', unreadCount: unread });
                            }, 30000);
                        })();
                    ";
                    coreWebView2.ExecuteScriptAsync(script);
                };

                // Récupérer le message posté par le JS ci-dessus
                coreWebView2.WebMessageReceived += (sender, e) =>
                {
                    try
                    {
                        using var jsonDoc = System.Text.Json.JsonDocument.Parse(e.WebMessageAsJson);
                        var root = jsonDoc.RootElement;
                        if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "unreadCount")
                        {
                            if (root.TryGetProperty("unreadCount", out var unreadProp))
                            {
                                int unread = unreadProp.GetInt32();
                                if (unread > 0)
                                {
                                    ShowBalloonNotification($"Vous avez {unread} mail(s) non lu(s) !");
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignorer les erreurs de parsing
                    }
                };

                // Indiquer que le programme est prêt
                ShowBalloonNotification("ProtonDesktop est lancé en arrière-plan.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation de WebView2: {ex.Message}", "Erreur",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit(this, EventArgs.Empty);
            }
        }

        private void ShowBalloonNotification(string message)
        {
            trayIcon.BalloonTipTitle = "ProtonDesktop";
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(3000); // durée en ms
        }

        private void CoreWebView2OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var uri = new Uri(e.Uri);
            // Vérifie si on est toujours sur un domaine ProtonMail
            if (!IsProtonMailUrl(uri))
            {
                e.Cancel = true;
                ShowBalloonNotification("Navigation bloquée hors domaine ProtonMail.");
            }
        }

        private bool IsProtonMailUrl(Uri uri)
        {
            var host = uri.Host.ToLowerInvariant();
            return host.EndsWith("protonmail.com") || host.EndsWith("proton.me");
        }

        private void CoreWebView2OnDownloadStarting(object? sender, CoreWebView2DownloadStartingEventArgs e)
        {
            // On peut proposer un chemin par défaut
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

            // Création d'un objet DownloadInfo pour référencer ce téléchargement
            var info = new DownloadInfo
            {
                FileName = Path.GetFileName(e.ResultFilePath),
                ResultFilePath = e.ResultFilePath,
                Status = "En cours"
            };

            // On sauvegarde dans une liste statique ou dans un manager
            DownloadManager.Downloads.Add(info);

            // On récupère l'opération en cours
            var op = e.DownloadOperation;

            // On affiche la popup
            var dlg = new DownloadProgressForm(op, info);
            dlg.Show();
        }

        private void OpenMainForm(object? sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void ShowMainForm()
        {
            Form1 form = new Form1();
            form.Show();
        }

        private void Exit(object? sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon.Dispose();
                webView?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
