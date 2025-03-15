namespace VirtualesPrueba
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonAnimalitos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAnimalitos
            // 
            this.buttonAnimalitos.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAnimalitos.Location = new System.Drawing.Point(214, 12);
            this.buttonAnimalitos.Name = "buttonAnimalitos";
            this.buttonAnimalitos.Size = new System.Drawing.Size(81, 37);
            this.buttonAnimalitos.TabIndex = 0;
            this.buttonAnimalitos.Text = "Animalitos";
            this.buttonAnimalitos.UseVisualStyleBackColor = true;
            this.buttonAnimalitos.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonAnimalitos);
            this.Name = "Form1";
            this.Text = "El Inmejorable v2.5";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAnimalitos;
    }
}
