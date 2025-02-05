namespace ProtonDesktop
{
    partial class DownloadProgressForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLogs;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                _downloadOperation = null;
                _downloadInfo = null;
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Forms

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFileName = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnLogs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.lblFileName.Location = new System.Drawing.Point(20, 20);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(100, 23);
            this.lblFileName.Text = "NomFichier";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(20, 60);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 10);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.progressBar1.BackColor = System.Drawing.Color.LightGray;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(20, 90);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.Text = "Annuler";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.BackColor = System.Drawing.Color.LightCoral;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btnLogs
            // 
            this.btnLogs.Location = new System.Drawing.Point(140, 90);
            this.btnLogs.Name = "btnLogs";
            this.btnLogs.Size = new System.Drawing.Size(100, 35);
            this.btnLogs.Text = "Historique";
            this.btnLogs.UseVisualStyleBackColor = true;
            this.btnLogs.FlatStyle = FlatStyle.Flat;
            this.btnLogs.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnLogs.ForeColor = System.Drawing.Color.White;
            this.btnLogs.FlatAppearance.BorderSize = 0;
            this.btnLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // DownloadProgressForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(340, 140);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // pas de bordure
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name = "DownloadProgressForm";
            this.BackColor = System.Drawing.Color.White;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
