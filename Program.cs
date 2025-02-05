using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProtonDesktop
{
    internal static class Program
    {
        private static readonly string currentVersion = "1.0.0";
        
        // URL pour récupérer la version distante (un fichier texte, par ex.)
        private static readonly string versionUrl = "https://raw.githubusercontent.com/danbenba/ProtonDesktop/refs/heads/project/version";

        // URL du fichier .exe à télécharger pour la mise à jour
        private static readonly string installerBaseUrl = "https://votre-url-serveur-ou-github/ProtonDesktop_Update.exe";
        
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Vérifie si l'argument "/nogui" est présent
            bool noGuiMode = args.Contains("/nogui", StringComparer.OrdinalIgnoreCase);

            // Si mode GUI, on vérifie d’abord s’il y a une mise à jour
            if (!noGuiMode)
            {
                CheckForUpdate();
            }

            if (noGuiMode)
            {
                Application.Run(new HiddenAppContext());
            }
            else
            {
                Application.Run(new Form1());
            }
        }

        /// <summary>
        /// Vérifie s'il existe une version plus récente sur le serveur.
        /// Si oui, propose à l'utilisateur d'effectuer la mise à jour.
        /// </summary>
        private static void CheckForUpdate()
        {
            string logFilePath = Path.Combine(Path.GetTempPath(), "ProtonDesktop_UpdateLog.txt");
            
            try
            {
                using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                {
                    logWriter.WriteLine("------------------------------------------------------------");
                    logWriter.WriteLine($"[CheckForUpdate] {DateTime.Now}");

                    using (WebClient webClient = new WebClient())
                    {
                        logWriter.WriteLine($"Téléchargement de la version en ligne depuis : {versionUrl}");
                        string onlineVersion = webClient.DownloadString(versionUrl).Trim();

                        logWriter.WriteLine($"Version actuelle : {currentVersion}");
                        logWriter.WriteLine($"Version disponible : {onlineVersion}");

                        if (!string.IsNullOrEmpty(onlineVersion) 
                            && !onlineVersion.Equals(currentVersion, StringComparison.OrdinalIgnoreCase))
                        {
                            logWriter.WriteLine("Nouvelle version détectée. Demande de confirmation à l'utilisateur.");

                            DialogResult dr = MessageBox.Show(
                                $"Une nouvelle version ({onlineVersion}) est disponible.\nVoulez-vous mettre à jour maintenant ?",
                                "Mise à jour disponible",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (dr == DialogResult.Yes)
                            {
                                logWriter.WriteLine("L'utilisateur a accepté la mise à jour.");

                                // Ouvre la fenêtre d'update avec barre de progression + logs
                                using (UpdateForm updateForm = new UpdateForm(installerBaseUrl))
                                {
                                    // Montre la fenêtre d'update en mode modal.
                                    // Quand le téléchargement sera fini (ou annulé), la fenêtre se ferme.
                                    updateForm.ShowDialog();
                                }

                                logWriter.WriteLine("Fenêtre de mise à jour terminée. Fermeture de l'appli principale.");
                                logWriter.WriteLine("------------------------------------------------------------");

                                // On ferme tout (c'est la fin de l'appli, l'updater va faire le reste)
                                Application.Exit();
                            }
                            else
                            {
                                logWriter.WriteLine("Utilisateur a refusé la mise à jour.");
                                logWriter.WriteLine("------------------------------------------------------------");
                            }
                        }
                        else
                        {
                            logWriter.WriteLine("Aucune mise à jour disponible ou version identique.");
                            logWriter.WriteLine("------------------------------------------------------------");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                {
                    logWriter.WriteLine("ERREUR lors de la vérification de mise à jour : " + ex);
                    logWriter.WriteLine("------------------------------------------------------------");
                }
            }
        }
    }
}
