namespace VirtualesPrueba
{
    partial class FormPrintSettings
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ComboBox cmbPrinters;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.RadioButton rb58mm;
        private System.Windows.Forms.RadioButton rb80mm;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.ComboBox cmbBacklot;

        /// <summary>
        /// Método necesario para el Diseñador de Windows Forms.
        /// No se debe modificar con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbPrinters = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.rb58mm = new System.Windows.Forms.RadioButton();
            this.rb80mm = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.cmbBacklot = new System.Windows.Forms.ComboBox();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxVirtuales = new System.Windows.Forms.ComboBox();
            this.chkImprimirUsuario = new System.Windows.Forms.CheckBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtMensajeFinal = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.chkImprimirMensajeFinal = new System.Windows.Forms.CheckBox();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.comboBoxHipicas = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbPrinters
            // 
            this.cmbPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrinters.FormattingEnabled = true;
            this.cmbPrinters.Location = new System.Drawing.Point(87, 29);
            this.cmbPrinters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbPrinters.Name = "cmbPrinters";
            this.cmbPrinters.Size = new System.Drawing.Size(115, 21);
            this.cmbPrinters.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(21, 32);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Impresora";
            // 
            // rb58mm
            // 
            this.rb58mm.AutoSize = true;
            this.rb58mm.Location = new System.Drawing.Point(21, 66);
            this.rb58mm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rb58mm.Name = "rb58mm";
            this.rb58mm.Size = new System.Drawing.Size(101, 17);
            this.rb58mm.TabIndex = 2;
            this.rb58mm.TabStop = true;
            this.rb58mm.Text = "Impresion 58mm";
            this.rb58mm.UseVisualStyleBackColor = true;
            // 
            // rb80mm
            // 
            this.rb80mm.AutoSize = true;
            this.rb80mm.Location = new System.Drawing.Point(21, 95);
            this.rb80mm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rb80mm.Name = "rb80mm";
            this.rb80mm.Size = new System.Drawing.Size(101, 17);
            this.rb80mm.TabIndex = 3;
            this.rb80mm.TabStop = true;
            this.rb80mm.Text = "Impresion 80mm";
            this.rb80mm.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(209, 328);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 19);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Guardar";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(355, 328);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 19);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(259, 38);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(79, 13);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "Logo Impresión";
            this.Label2.Visible = false;
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(355, 18);
            this.picLogo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(111, 50);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 7;
            this.picLogo.TabStop = false;
            this.picLogo.Visible = false;
            // 
            // cmbBacklot
            // 
            this.cmbBacklot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBacklot.FormattingEnabled = true;
            this.cmbBacklot.Location = new System.Drawing.Point(15, 151);
            this.cmbBacklot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbBacklot.Name = "cmbBacklot";
            this.cmbBacklot.Size = new System.Drawing.Size(79, 21);
            this.cmbBacklot.TabIndex = 23;
            this.cmbBacklot.Visible = false;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(24, 277);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(610, 20);
            this.txtUrl.TabIndex = 25;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "URL";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(12, 127);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(54, 13);
            this.Label5.TabIndex = 24;
            this.Label5.Text = "Animalitos";
            this.Label5.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Virtuales";
            // 
            // comboBoxVirtuales
            // 
            this.comboBoxVirtuales.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVirtuales.FormattingEnabled = true;
            this.comboBoxVirtuales.Location = new System.Drawing.Point(15, 217);
            this.comboBoxVirtuales.Name = "comboBoxVirtuales";
            this.comboBoxVirtuales.Size = new System.Drawing.Size(129, 21);
            this.comboBoxVirtuales.TabIndex = 27;
            // 
            // chkImprimirUsuario
            // 
            this.chkImprimirUsuario.AutoSize = true;
            this.chkImprimirUsuario.Location = new System.Drawing.Point(237, 86);
            this.chkImprimirUsuario.Name = "chkImprimirUsuario";
            this.chkImprimirUsuario.Size = new System.Drawing.Size(15, 14);
            this.chkImprimirUsuario.TabIndex = 21;
            this.chkImprimirUsuario.UseVisualStyleBackColor = true;
            this.chkImprimirUsuario.Visible = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(269, 87);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(43, 13);
            this.Label3.TabIndex = 9;
            this.Label3.Text = "Usuario";
            this.Label3.Visible = false;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(355, 87);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(112, 20);
            this.txtUsuario.TabIndex = 10;
            this.txtUsuario.Visible = false;
            // 
            // txtMensajeFinal
            // 
            this.txtMensajeFinal.Location = new System.Drawing.Point(355, 127);
            this.txtMensajeFinal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMensajeFinal.Name = "txtMensajeFinal";
            this.txtMensajeFinal.Size = new System.Drawing.Size(112, 20);
            this.txtMensajeFinal.TabIndex = 11;
            this.txtMensajeFinal.Visible = false;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(269, 129);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(72, 13);
            this.Label4.TabIndex = 12;
            this.Label4.Text = "Mensaje Final";
            this.Label4.Visible = false;
            // 
            // chkImprimirMensajeFinal
            // 
            this.chkImprimirMensajeFinal.AutoSize = true;
            this.chkImprimirMensajeFinal.Location = new System.Drawing.Point(237, 129);
            this.chkImprimirMensajeFinal.Name = "chkImprimirMensajeFinal";
            this.chkImprimirMensajeFinal.Size = new System.Drawing.Size(15, 14);
            this.chkImprimirMensajeFinal.TabIndex = 22;
            this.chkImprimirMensajeFinal.UseVisualStyleBackColor = true;
            this.chkImprimirMensajeFinal.Visible = false;
            // 
            // cmbMode
            // 
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(477, 192);
            this.cmbMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(115, 21);
            this.cmbMode.TabIndex = 17;
            this.cmbMode.Visible = false;
            this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(417, 194);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(43, 13);
            this.Label6.TabIndex = 18;
            this.Label6.Text = "Usuario";
            this.Label6.Visible = false;
            // 
            // comboBoxHipicas
            // 
            this.comboBoxHipicas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHipicas.FormattingEnabled = true;
            this.comboBoxHipicas.Location = new System.Drawing.Point(163, 217);
            this.comboBoxHipicas.Name = "comboBoxHipicas";
            this.comboBoxHipicas.Size = new System.Drawing.Size(129, 21);
            this.comboBoxHipicas.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(160, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "Hipicas";
            // 
            // FormPrintSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 368);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBoxHipicas);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxVirtuales);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.cmbBacklot);
            this.Controls.Add(this.chkImprimirMensajeFinal);
            this.Controls.Add(this.chkImprimirUsuario);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.txtMensajeFinal);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rb80mm);
            this.Controls.Add(this.rb58mm);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.cmbPrinters);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormPrintSettings";
            this.Text = "FormPrintSettings";
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxVirtuales;
        private System.Windows.Forms.CheckBox chkImprimirUsuario;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.TextBox txtMensajeFinal;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.CheckBox chkImprimirMensajeFinal;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.ComboBox comboBoxHipicas;
        private System.Windows.Forms.Label label9;
    }
}