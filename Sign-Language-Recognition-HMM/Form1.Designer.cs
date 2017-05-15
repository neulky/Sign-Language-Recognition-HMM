namespace Sign_Language_Recognition_HMM
{
    partial class Result
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
            this.txtShowResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtShowResult
            // 
            this.txtShowResult.Font = new System.Drawing.Font("黑体", 150F);
            this.txtShowResult.Location = new System.Drawing.Point(1, 1);
            this.txtShowResult.Name = "txtShowResult";
            this.txtShowResult.Size = new System.Drawing.Size(469, 236);
            this.txtShowResult.TabIndex = 0;
            // 
            // Result
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 238);
            this.Controls.Add(this.txtShowResult);
            this.Name = "Result";
            this.Text = "Result";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtShowResult;
    }
}