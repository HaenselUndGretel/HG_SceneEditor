namespace MenuEditor
{
    partial class FormNewGame
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
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "1280x720",
            "2560x720",
            "3840x720",
            "5120x720",
            "3840x2160",
            "5120x2160",
            "5120x1440",
            "3840x1440",
            "1280x1440",
            "1280x2160",
            "1280x2880",
            "1280x3600",
            "1280x4320",
            "2560x2160",
            "2560x2880",
            "2560x3600",
            "2560x4320"});
      this.comboBox1.Location = new System.Drawing.Point(12, 29);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(201, 21);
      this.comboBox1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(57, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Auflösung:";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(15, 56);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(198, 23);
      this.button1.TabIndex = 2;
      this.button1.Text = "Neue Scene";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // FormNewGame
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(225, 88);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.comboBox1);
      this.Name = "FormNewGame";
      this.Text = "FormNewGame";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}