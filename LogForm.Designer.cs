namespace ProtonDesktop
{
    partial class LogForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listViewLogs;
        private System.Windows.Forms.ColumnHeader columnHeaderFileName;
        private System.Windows.Forms.ColumnHeader columnHeaderStatus;
        private System.Windows.Forms.ColumnHeader columnHeaderProgress;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                listViewLogs = null;
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Forms

        private void InitializeComponent()
        {
            this.listViewLogs = new System.Windows.Forms.ListView();
            this.columnHeaderFileName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderStatus = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderProgress = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewLogs
            // 
            this.listViewLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFileName,
            this.columnHeaderStatus,
            this.columnHeaderProgress});
            this.listViewLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLogs.FullRowSelect = true;
            this.listViewLogs.GridLines = true;
            this.listViewLogs.HideSelection = false;
            this.listViewLogs.Location = new System.Drawing.Point(0, 0);
            this.listViewLogs.Name = "listViewLogs";
            this.listViewLogs.Size = new System.Drawing.Size(600, 400);
            this.listViewLogs.TabIndex = 0;
            this.listViewLogs.UseCompatibleStateImageBehavior = false;
            this.listViewLogs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderFileName
            // 
            this.columnHeaderFileName.Text = "Nom du Fichier";
            this.columnHeaderFileName.Width = 250;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "Statut";
            this.columnHeaderStatus.Width = 150;
            // 
            // columnHeaderProgress
            // 
            this.columnHeaderProgress.Text = "Progression";
            this.columnHeaderProgress.Width = 100;
            // 
            // LogForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.listViewLogs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LogForm";
            this.Text = "Historique des Téléchargements";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
