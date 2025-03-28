namespace VirtualesPrueba
{
    partial class SettingsUniversal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controles del formulario:
        internal System.Windows.Forms.PictureBox pbLogo;
        internal System.Windows.Forms.Button btnSeleccionar;
        internal System.Windows.Forms.Label LabelPrinter;
        internal System.Windows.Forms.ComboBox ComboBoxPrinters;
        internal System.Windows.Forms.Label LabelPaper;
        internal System.Windows.Forms.ComboBox ComboBoxPaperWidth;
        internal System.Windows.Forms.Button btnGuardar;
        internal System.Windows.Forms.OpenFileDialog ofdLogo;

        /// <summary>
        /// Limpia los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; de lo contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No modifique
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsUniversal));
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.LabelPrinter = new System.Windows.Forms.Label();
            this.ComboBoxPrinters = new System.Windows.Forms.ComboBox();
            this.LabelPaper = new System.Windows.Forms.Label();
            this.ComboBoxPaperWidth = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.ofdLogo = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLogo
            // 
            this.pbLogo.Location = new System.Drawing.Point(20, 20);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(200, 100);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLogo.TabIndex = 0;
            this.pbLogo.TabStop = false;
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Location = new System.Drawing.Point(230, 20);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(120, 30);
            this.btnSeleccionar.TabIndex = 1;
            this.btnSeleccionar.Text = "Seleccionar logo";
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // LabelPrinter
            // 
            this.LabelPrinter.AutoSize = true;
            this.LabelPrinter.Location = new System.Drawing.Point(20, 140);
            this.LabelPrinter.Name = "LabelPrinter";
            this.LabelPrinter.Size = new System.Drawing.Size(62, 15);
            this.LabelPrinter.TabIndex = 2;
            this.LabelPrinter.Text = "Impresora:";
            // 
            // ComboBoxPrinters
            // 
            this.ComboBoxPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPrinters.FormattingEnabled = true;
            this.ComboBoxPrinters.Location = new System.Drawing.Point(90, 137);
            this.ComboBoxPrinters.Name = "ComboBoxPrinters";
            this.ComboBoxPrinters.Size = new System.Drawing.Size(240, 23);
            this.ComboBoxPrinters.TabIndex = 3;
            // 
            // LabelPaper
            // 
            this.LabelPaper.AutoSize = true;
            this.LabelPaper.Location = new System.Drawing.Point(20, 170);
            this.LabelPaper.Name = "LabelPaper";
            this.LabelPaper.Size = new System.Drawing.Size(90, 15);
            this.LabelPaper.TabIndex = 4;
            this.LabelPaper.Text = "Tamaño papel:";
            // 
            // ComboBoxPaperWidth
            // 
            this.ComboBoxPaperWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPaperWidth.FormattingEnabled = true;
            this.ComboBoxPaperWidth.Location = new System.Drawing.Point(120, 167);
            this.ComboBoxPaperWidth.Name = "ComboBoxPaperWidth";
            this.ComboBoxPaperWidth.Size = new System.Drawing.Size(210, 23);
            this.ComboBoxPaperWidth.TabIndex = 5;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(20, 210);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // SettingsUniversal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 260);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.ComboBoxPaperWidth);
            this.Controls.Add(this.LabelPaper);
            this.Controls.Add(this.ComboBoxPrinters);
            this.Controls.Add(this.LabelPrinter);
            this.Controls.Add(this.btnSeleccionar);
            this.Controls.Add(this.pbLogo);
            this.Name = "SettingsUniversal";
            this.Text = "Configuración";
            this.Load += new System.EventHandler(this.SettingsUniversal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
