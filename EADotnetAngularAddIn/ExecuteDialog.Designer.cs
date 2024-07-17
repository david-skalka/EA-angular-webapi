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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.invertEntities = new System.Windows.Forms.Button();
            this.invertParts = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(547, 246);
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
            this.listEntities.Location = new System.Drawing.Point(6, 45);
            this.listEntities.Name = "listEntities";
            this.listEntities.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listEntities.Size = new System.Drawing.Size(298, 173);
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
            this.listPart.Location = new System.Drawing.Point(6, 45);
            this.listPart.Name = "listPart";
            this.listPart.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listPart.Size = new System.Drawing.Size(285, 173);
            this.listPart.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.invertEntities);
            this.groupBox1.Controls.Add(this.listEntities);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 231);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entities";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.invertParts);
            this.groupBox2.Controls.Add(this.listPart);
            this.groupBox2.Location = new System.Drawing.Point(328, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(297, 228);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parts";
            // 
            // invertEntities
            // 
            this.invertEntities.Location = new System.Drawing.Point(280, 16);
            this.invertEntities.Name = "invertEntities";
            this.invertEntities.Size = new System.Drawing.Size(24, 23);
            this.invertEntities.TabIndex = 3;
            this.invertEntities.Text = "I";
            this.invertEntities.UseVisualStyleBackColor = true;
            this.invertEntities.Click += new System.EventHandler(this.invertEntities_Click);
            // 
            // invertParts
            // 
            this.invertParts.Location = new System.Drawing.Point(264, 16);
            this.invertParts.Name = "invertParts";
            this.invertParts.Size = new System.Drawing.Size(27, 23);
            this.invertParts.TabIndex = 4;
            this.invertParts.Text = "I";
            this.invertParts.UseVisualStyleBackColor = true;
            this.invertParts.Click += new System.EventHandler(this.invertParts_Click);
            // 
            // ExecuteDialog
            // 
            this.ClientSize = new System.Drawing.Size(632, 276);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnExecute);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExecuteDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Execute generator";
            this.Load += new System.EventHandler(this.ExecuteDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ListBox listEntities;
        private System.Windows.Forms.ListBox listPart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button invertEntities;
        private System.Windows.Forms.Button invertParts;
    }
}
