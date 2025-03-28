namespace VirtualesPrueba
{
    partial class Form2
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controles del formulario
        internal Microsoft.Web.WebView2.WinForms.WebView2 WebView21;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Timer Timer1;
        internal System.Windows.Forms.Button btnHipicas;

        /// <summary>
        /// Limpia los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; de lo contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.WebView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnHipicas = new System.Windows.Forms.Button();
            this.btnVirtuales = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.WebView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // WebView21
            // 
            this.WebView21.AllowExternalDrop = true;
            this.WebView21.CreationProperties = null;
            this.WebView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebView21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebView21.Location = new System.Drawing.Point(0, 0);
            this.WebView21.Name = "WebView21";
            this.WebView21.Size = new System.Drawing.Size(1001, 518);
            this.WebView21.TabIndex = 0;
            this.WebView21.ZoomFactor = 1D;
            // 
            // PictureBox1
            // 
            this.PictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(14)))));
            this.PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox1.Image")));
            this.PictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("PictureBox1.InitialImage")));
            this.PictureBox1.Location = new System.Drawing.Point(637, 0);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(191, 52);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox1.TabIndex = 1;
            this.PictureBox1.TabStop = false;
            // 
            // Timer1
            // 
            this.Timer1.Interval = 3000;
            // 
            // btnHipicas
            // 
            this.btnHipicas.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnHipicas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnHipicas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHipicas.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHipicas.Location = new System.Drawing.Point(908, 10);
            this.btnHipicas.Margin = new System.Windows.Forms.Padding(0);
            this.btnHipicas.Name = "btnHipicas";
            this.btnHipicas.Size = new System.Drawing.Size(93, 42);
            this.btnHipicas.TabIndex = 3;
            this.btnHipicas.Text = "Hipicas";
            this.btnHipicas.UseVisualStyleBackColor = false;
            this.btnHipicas.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btnVirtuales
            // 
            this.btnVirtuales.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnVirtuales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnVirtuales.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVirtuales.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnVirtuales.Location = new System.Drawing.Point(813, 10);
            this.btnVirtuales.Name = "btnVirtuales";
            this.btnVirtuales.Size = new System.Drawing.Size(93, 42);
            this.btnVirtuales.TabIndex = 4;
            this.btnVirtuales.Text = "Virtuales";
            this.btnVirtuales.UseVisualStyleBackColor = false;
            this.btnVirtuales.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 518);
            this.Controls.Add(this.btnVirtuales);
            this.Controls.Add(this.btnHipicas);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.WebView21);
            this.Name = "Form2";
            this.Text = "BetsDeportes v1.5 Cashier";
            ((System.ComponentModel.ISupportInitialize)(this.WebView21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnVirtuales;
    }
}
