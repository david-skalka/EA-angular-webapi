namespace EADotnetAngularAddIn
{
    partial class ExecuteDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExecute = new System.Windows.Forms.Button();
            this.listEntities = new System.Windows.Forms.ListBox();
            this.listPart = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(434, 229);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // listEntities
            // 
            this.listEntities.FormattingEnabled = true;
            this.listEntities.Location = new System.Drawing.Point(12, 24);
            this.listEntities.Name = "listEntities";
            this.listEntities.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listEntities.Size = new System.Drawing.Size(225, 199);
            this.listEntities.TabIndex = 2;
            // 
            // listPart
            // 
            this.listPart.FormattingEnabled = true;
            this.listPart.Items.AddRange(new object[] {
            "initialize-solution",
            "initialize-angular",
            "db-context",
            "entity",
            "seeder",
            "global-mock-data",
            "app-component",
            "app-routes"});
            this.listPart.Location = new System.Drawing.Point(243, 24);
            this.listPart.Name = "listPart";
            this.listPart.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listPart.Size = new System.Drawing.Size(266, 199);
            this.listPart.TabIndex = 3;
            // 
            // ExecuteDialog
            // 
            this.ClientSize = new System.Drawing.Size(519, 260);
            this.Controls.Add(this.listPart);
            this.Controls.Add(this.listEntities);
            this.Controls.Add(this.btnExecute);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExecuteDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Execute generator";
            this.Load += new System.EventHandler(this.ExecuteDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ListBox listEntities;
        private System.Windows.Forms.ListBox listPart;
    }
}
