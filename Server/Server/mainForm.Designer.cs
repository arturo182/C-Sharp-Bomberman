namespace Server
{
  partial class mainForm
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
      if(disposing && (components != null)) {
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
      this.components = new System.ComponentModel.Container();
      this.logTextBox = new System.Windows.Forms.RichTextBox();
      this.startButton = new System.Windows.Forms.Button();
      this.roundTmer = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // logTextBox
      // 
      this.logTextBox.Location = new System.Drawing.Point(12, 12);
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.Size = new System.Drawing.Size(354, 284);
      this.logTextBox.TabIndex = 0;
      this.logTextBox.Text = "";
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(291, 302);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 1;
      this.startButton.Text = "Start!";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // roundTmer
      // 
      this.roundTmer.Tick += new System.EventHandler(this.roundTmer_Tick);
      // 
      // mainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(378, 337);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.logTextBox);
      this.Name = "mainForm";
      this.Text = "Bomberman - Serwer";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox logTextBox;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Timer roundTmer;
  }
}

