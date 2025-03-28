using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace VirtualesPrueba
{
    public partial class SettingsUniversal : Form
    {
        public SettingsUniversal()
        {
            InitializeComponent();
        }

        private void SettingsUniversal_Load(object sender, EventArgs e)
        {
            // Cargar el logo si existe
            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                pbLogo.Image = Image.FromFile(logoPath);
            }
            else
            {
                pbLogo.Image = null;
            }
            // llena el cmoboXo de impresoras llen
            // Llenar el ComboBox de impresoras
            ComboBoxPrinters.Items.Clear();
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                ComboBoxPrinters.Items.Add(printerName);
            }

            string savedPrinter = Properties.Settings.Default.SelectedPrinter;
            if (!string.IsNullOrEmpty(savedPrinter))
            {
                ComboBoxPrinters.SelectedItem = savedPrinter;
            }
            else if (ComboBoxPrinters.Items.Count > 0)
            {
                ComboBoxPrinters.SelectedIndex = 0;
            }

            

            // Llenar el ComboBox de tamaño de papel
            ComboBoxPaperWidth.Items.Clear();
            ComboBoxPaperWidth.Items.Add("80 mm");
            ComboBoxPaperWidth.Items.Add("58 mm");

            string savedPaperWidth = Properties.Settings.Default.PaperWidthRetail;
            if (!string.IsNullOrEmpty(savedPaperWidth))
            {
                ComboBoxPaperWidth.SelectedItem = savedPaperWidth;
            }
            else
            {
                ComboBoxPaperWidth.SelectedItem = "80 mm";
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            ofdLogo.Filter = "Imágenes PNG|*.png|Imágenes JPG|*.jpg|Todos los archivos|*.*";
            if (ofdLogo.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = ofdLogo.FileName;
                pbLogo.Image = Image.FromFile(selectedPath);
                // Nota: Dependiendo de tu implementación de settings, puede ser "LogoPathUniversal" u otra propiedad.
                Properties.Settings.Default.LogoPathUniversal = selectedPath;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ComboBoxPrinters.SelectedItem != null)
            {
                Properties.Settings.Default.SelectedPrinter = ComboBoxPrinters.SelectedItem.ToString();
            }

            if (ComboBoxPaperWidth.SelectedItem != null)
            {
                Properties.Settings.Default.PaperWidthRetail = ComboBoxPaperWidth.SelectedItem.ToString();
            }

            Properties.Settings.Default.Save();
            MessageBox.Show("¡Configuración guardada correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
