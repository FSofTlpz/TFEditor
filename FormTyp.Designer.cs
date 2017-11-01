namespace TFEditor {
   partial class FormTyp {
      /// <summary>
      /// Erforderliche Designervariable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Verwendete Ressourcen bereinigen.
      /// </summary>
      /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Vom Windows Form-Designer generierter Code

      /// <summary>
      /// Erforderliche Methode für die Designerunterstützung.
      /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
      /// </summary>
      private void InitializeComponent() {
         this.label1 = new System.Windows.Forms.Label();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.button_OK = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.button_Cancel = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.textBox2 = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.textBox3 = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 9);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(36, 17);
         this.label1.TabIndex = 0;
         this.label1.Text = "Typ:";
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(113, 6);
         this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(37, 22);
         this.textBox1.TabIndex = 2;
         this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
         // 
         // button_OK
         // 
         this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.button_OK.Location = new System.Drawing.Point(12, 144);
         this.button_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.button_OK.Name = "button_OK";
         this.button_OK.Size = new System.Drawing.Size(75, 30);
         this.button_OK.TabIndex = 8;
         this.button_OK.Text = "OK";
         this.button_OK.UseVisualStyleBackColor = true;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(85, 9);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(22, 17);
         this.label2.TabIndex = 1;
         this.label2.Text = "0x";
         this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // button_Cancel
         // 
         this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.button_Cancel.Location = new System.Drawing.Point(110, 144);
         this.button_Cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.button_Cancel.Name = "button_Cancel";
         this.button_Cancel.Size = new System.Drawing.Size(75, 30);
         this.button_Cancel.TabIndex = 9;
         this.button_Cancel.Text = "Abbruch";
         this.button_Cancel.UseVisualStyleBackColor = true;
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(85, 37);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(22, 17);
         this.label3.TabIndex = 4;
         this.label3.Text = "0x";
         this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // textBox2
         // 
         this.textBox2.Location = new System.Drawing.Point(113, 34);
         this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.textBox2.Name = "textBox2";
         this.textBox2.Size = new System.Drawing.Size(37, 22);
         this.textBox2.TabIndex = 5;
         this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(12, 37);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(56, 17);
         this.label4.TabIndex = 3;
         this.label4.Text = "Subtyp:";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(12, 71);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(49, 17);
         this.label5.TabIndex = 6;
         this.label5.Text = "Name:";
         // 
         // textBox3
         // 
         this.textBox3.Location = new System.Drawing.Point(15, 99);
         this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.textBox3.Name = "textBox3";
         this.textBox3.Size = new System.Drawing.Size(170, 22);
         this.textBox3.TabIndex = 7;
         // 
         // FormTyp
         // 
         this.AcceptButton = this.button_OK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.button_Cancel;
         this.ClientSize = new System.Drawing.Size(205, 207);
         this.ControlBox = false;
         this.Controls.Add(this.textBox3);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.textBox2);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.button_Cancel);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.button_OK);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.Name = "FormTyp";
         this.Text = "Typ eingeben";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.Button button_OK;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button button_Cancel;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox textBox2;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox textBox3;
   }
}