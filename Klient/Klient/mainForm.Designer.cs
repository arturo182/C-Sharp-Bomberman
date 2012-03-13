namespace Klient
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
      this.gamePictureBox = new System.Windows.Forms.PictureBox();
      this.connectButton = new System.Windows.Forms.Button();
      this.exitButton = new System.Windows.Forms.Button();
      this.connGroupBox = new System.Windows.Forms.GroupBox();
      this.nickTextBox = new System.Windows.Forms.TextBox();
      this.serverTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.controlTextBox = new System.Windows.Forms.TextBox();
      this.animTimer = new System.Windows.Forms.Timer(this.components);
      this.chatTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.gamePictureBox)).BeginInit();
      this.connGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // gamePictureBox
      // 
      this.gamePictureBox.Location = new System.Drawing.Point(9, 11);
      this.gamePictureBox.Name = "gamePictureBox";
      this.gamePictureBox.Size = new System.Drawing.Size(512, 512);
      this.gamePictureBox.TabIndex = 0;
      this.gamePictureBox.TabStop = false;
      // 
      // connectButton
      // 
      this.connectButton.Location = new System.Drawing.Point(531, 119);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(95, 27);
      this.connectButton.TabIndex = 0;
      this.connectButton.Text = "Połącz";
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // exitButton
      // 
      this.exitButton.Location = new System.Drawing.Point(636, 119);
      this.exitButton.Name = "exitButton";
      this.exitButton.Size = new System.Drawing.Size(95, 27);
      this.exitButton.TabIndex = 1;
      this.exitButton.Text = "Zakończ";
      this.exitButton.UseVisualStyleBackColor = true;
      this.exitButton.Click += new System.EventHandler(this.exitButton_clicked);
      // 
      // connGroupBox
      // 
      this.connGroupBox.Controls.Add(this.nickTextBox);
      this.connGroupBox.Controls.Add(this.serverTextBox);
      this.connGroupBox.Controls.Add(this.label2);
      this.connGroupBox.Controls.Add(this.label1);
      this.connGroupBox.Location = new System.Drawing.Point(531, 13);
      this.connGroupBox.Name = "connGroupBox";
      this.connGroupBox.Size = new System.Drawing.Size(200, 100);
      this.connGroupBox.TabIndex = 2;
      this.connGroupBox.TabStop = false;
      this.connGroupBox.Text = "Połączenie";
      // 
      // nickTextBox
      // 
      this.nickTextBox.Location = new System.Drawing.Point(57, 42);
      this.nickTextBox.Name = "nickTextBox";
      this.nickTextBox.Size = new System.Drawing.Size(137, 20);
      this.nickTextBox.TabIndex = 3;
      this.nickTextBox.Text = "arturo182";
      // 
      // serverTextBox
      // 
      this.serverTextBox.Location = new System.Drawing.Point(57, 16);
      this.serverTextBox.Name = "serverTextBox";
      this.serverTextBox.Size = new System.Drawing.Size(137, 20);
      this.serverTextBox.TabIndex = 2;
      this.serverTextBox.Text = "127.0.0.1";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(7, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Nick:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(43, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Serwer:";
      // 
      // controlTextBox
      // 
      this.controlTextBox.Location = new System.Drawing.Point(531, 153);
      this.controlTextBox.Name = "controlTextBox";
      this.controlTextBox.Size = new System.Drawing.Size(17, 20);
      this.controlTextBox.TabIndex = 3;
      this.controlTextBox.Visible = false;
      this.controlTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.controlTextBox_KeyDown);
      // 
      // animTimer
      // 
      this.animTimer.Tick += new System.EventHandler(this.animTimer_Tick);
      // 
      // chatTextBox
      // 
      this.chatTextBox.Location = new System.Drawing.Point(12, 500);
      this.chatTextBox.Name = "chatTextBox";
      this.chatTextBox.Size = new System.Drawing.Size(509, 20);
      this.chatTextBox.TabIndex = 4;
      this.chatTextBox.Visible = false;
      this.chatTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatTextBox_KeyDown);
      // 
      // mainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(741, 532);
      this.Controls.Add(this.chatTextBox);
      this.Controls.Add(this.controlTextBox);
      this.Controls.Add(this.connGroupBox);
      this.Controls.Add(this.exitButton);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.gamePictureBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "mainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Bomberman - Client";
      this.Shown += new System.EventHandler(this.mainForm_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.gamePictureBox)).EndInit();
      this.connGroupBox.ResumeLayout(false);
      this.connGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox gamePictureBox;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.Button exitButton;
    private System.Windows.Forms.GroupBox connGroupBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox nickTextBox;
    private System.Windows.Forms.TextBox serverTextBox;
    private System.Windows.Forms.TextBox controlTextBox;
    private System.Windows.Forms.Timer animTimer;
    private System.Windows.Forms.TextBox chatTextBox;
  }
}

