namespace MazeMasters
{
    partial class GetSeed
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
            this.SeedValue = new System.Windows.Forms.TextBox();
            this.SeedLabel = new System.Windows.Forms.Label();
            this.SeedGo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SeedValue
            // 
            this.SeedValue.Location = new System.Drawing.Point(45, 17);
            this.SeedValue.Name = "SeedValue";
            this.SeedValue.Size = new System.Drawing.Size(172, 20);
            this.SeedValue.TabIndex = 0;
            // 
            // SeedLabel
            // 
            this.SeedLabel.AutoSize = true;
            this.SeedLabel.Location = new System.Drawing.Point(7, 20);
            this.SeedLabel.Name = "SeedLabel";
            this.SeedLabel.Size = new System.Drawing.Size(32, 13);
            this.SeedLabel.TabIndex = 1;
            this.SeedLabel.Text = "Seed";
            // 
            // SeedGo
            // 
            this.SeedGo.Location = new System.Drawing.Point(224, 17);
            this.SeedGo.Name = "SeedGo";
            this.SeedGo.Size = new System.Drawing.Size(48, 20);
            this.SeedGo.TabIndex = 2;
            this.SeedGo.Text = "Go";
            this.SeedGo.UseVisualStyleBackColor = true;
            this.SeedGo.Click += new System.EventHandler(this.SeedGo_Click);
            // 
            // GetSeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 53);
            this.Controls.Add(this.SeedGo);
            this.Controls.Add(this.SeedLabel);
            this.Controls.Add(this.SeedValue);
            this.Name = "GetSeed";
            this.Text = "Enter Seed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SeedValue;
        private System.Windows.Forms.Label SeedLabel;
        private System.Windows.Forms.Button SeedGo;
    }
}