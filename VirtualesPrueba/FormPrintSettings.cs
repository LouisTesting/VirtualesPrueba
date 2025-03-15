using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualesPrueba
{
    public partial class FormPrintSettings : Form
    {
        public string SelectedLogo { get; set; }
        public string SelectedUser { get; set; }
        public string FinalMessage { get; set; }
        public string SelectedPrinter { get; set; }
        public int PaperWidth { get; set; }

        public FormPrintSettings()
        {
            InitializeComponent();
            this.Load += FormPrintSettings_Load;
        }

        private void FormPrintSettings_Load(object sender, EventArgs e)
        {
            txtUrl.Text = Properties.Settings.Default.DefaultUrl;

            this.ShowInTaskbar = false;

            cmbMode.Items.Add("Cajero");
            cmbMode.Items.Add("Terminal");

            cmbBacklot.Items.Add("Activado");
            cmbBacklot.Items.Add("Desactivado");

            // Si tienes una propiedad para logo, se puede agregar su estado aquí (comentado en VB)
            // chkImprimirLogo.Checked = Properties.Settings.Default.ImprimirLogo;
            chkImprimirUsuario.Checked = Properties.Settings.Default.ImprimirUsuario;
            chkImprimirMensajeFinal.Checked = Properties.Settings.Default.ImprimirMensajeFinal;

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cmbPrinters.Items.Add(printer);
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedPrinter))
            {
                cmbPrinters.SelectedItem = Properties.Settings.Default.SelectedPrinter;
            }
            else
            {
                PrinterSettings defaultPrinter = new PrinterSettings();
                if (PrinterSettings.InstalledPrinters.Cast<string>().Contains(defaultPrinter.PrinterName))
                {
                    cmbPrinters.SelectedItem = defaultPrinter.PrinterName;
                }
                else if (cmbPrinters.Items.Count > 0)
                {
                    cmbPrinters.SelectedIndex = 0;
                }
            }

            if (Properties.Settings.Default.IsTerminal)
            {
                cmbMode.SelectedItem = "Terminal";
            }
            else
            {
                cmbMode.SelectedItem = "Cajero";
            }

            if (Properties.Settings.Default.IsBacklot)
            {
                cmbBacklot.SelectedItem = "Activado";
            }
            else
            {
                cmbBacklot.SelectedItem = "Desactivado";
            }

            UpdateControlsBasedOnMode();

            if (Properties.Settings.Default.PaperWidth == 58)
            {
                rb58mm.Checked = true;
            }
            else if (Properties.Settings.Default.PaperWidth == 80)
            {
                rb80mm.Checked = true;
            }
            else
            {
                rb80mm.Checked = true;
            }

            string defaultLogoPath = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png");

            if (string.IsNullOrEmpty(Properties.Settings.Default.SelectedLogo) || !File.Exists(Properties.Settings.Default.SelectedLogo))
            {
                if (File.Exists(defaultLogoPath))
                {
                    Properties.Settings.Default.SelectedLogo = defaultLogoPath;
                }
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedLogo) && File.Exists(Properties.Settings.Default.SelectedLogo))
            {
                picLogo.Image = Image.FromFile(Properties.Settings.Default.SelectedLogo);
            }
            else
            {
                picLogo.Image = null;
            }

            txtUsuario.Text = Properties.Settings.Default.SelectedUser;
            txtMensajeFinal.Text = Properties.Settings.Default.FinalMessage;
        }

        private void UpdateControlsBasedOnMode()
        {
            if (cmbMode.SelectedItem.ToString() == "Terminal")
            {
                cmbPrinters.Enabled = false;
                rb58mm.Enabled = false;
                rb80mm.Enabled = false;

                cmbPrinters.SelectedItem = null;
                rb58mm.Checked = false;
                rb80mm.Checked = false;
            }
            else if (cmbMode.SelectedItem.ToString() == "Cajero")
            {
                cmbPrinters.Enabled = true;
                rb58mm.Enabled = true;
                rb80mm.Enabled = true;
            }
        }

        private void LoadPrinters()
        {
            cmbPrinters.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cmbPrinters.Items.Add(printer);
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedPrinter) &&
                cmbPrinters.Items.Contains(Properties.Settings.Default.SelectedPrinter))
            {
                cmbPrinters.SelectedItem = Properties.Settings.Default.SelectedPrinter;
            }
            else if (cmbPrinters.Items.Count > 0)
            {
                cmbPrinters.SelectedIndex = 0;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones de selección mínima
                if (!(chkImprimirUsuario.Checked || chkImprimirMensajeFinal.Checked))
                {
                    MessageBox.Show("Por favor, selecciona al menos un elemento para imprimir.", "Configuración Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar configuraciones de impresión
                Properties.Settings.Default.ImprimirUsuario = chkImprimirUsuario.Checked;
                Properties.Settings.Default.ImprimirMensajeFinal = chkImprimirMensajeFinal.Checked;

                // Configuración del modo de operación
                if (cmbMode.SelectedItem.ToString() == "Terminal")
                {
                    Properties.Settings.Default.IsTerminal = true;
                    Properties.Settings.Default.SelectedPrinter = string.Empty;
                    Properties.Settings.Default.PaperWidth = 0;
                }
                else if (cmbMode.SelectedItem.ToString() == "Cajero")
                {
                    Properties.Settings.Default.IsTerminal = false;

                    // Validar selección de impresora
                    if (cmbPrinters.SelectedItem == null)
                    {
                        MessageBox.Show("Por favor, selecciona una impresora.", "Selección de Impresora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Properties.Settings.Default.SelectedPrinter = cmbPrinters.SelectedItem.ToString();

                    // Validar selección de tamaño de papel
                    if (!rb58mm.Checked && !rb80mm.Checked)
                    {
                        MessageBox.Show("Por favor, selecciona un tipo de impresión válido.", "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Properties.Settings.Default.PaperWidth = rb58mm.Checked ? 58 : 80;
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona un modo válido (Terminal o Cajero).", "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Configuración del Backlot
                Properties.Settings.Default.IsBacklot = (cmbBacklot.SelectedItem.ToString() == "Activado");

                // Validaciones de usuario y mensaje final
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show("Por favor, ingresa un nombre de usuario.", "Usuario Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.SelectedUser = txtUsuario.Text.Trim();

                if (string.IsNullOrWhiteSpace(txtMensajeFinal.Text))
                {
                    MessageBox.Show("Por favor, ingresa un mensaje final.", "Mensaje Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.FinalMessage = txtMensajeFinal.Text.Trim();

                // Guardar la URL dinámica ingresada
                string nuevaUrl = txtUrl.Text.Trim();
                if (string.IsNullOrEmpty(nuevaUrl) || !Uri.IsWellFormedUriString(nuevaUrl, UriKind.Absolute))
                {
                    MessageBox.Show("Ingrese una URL válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.DefaultUrl = nuevaUrl;

                // Configuración del logo
                string defaultLogoPath = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png");
                if (!string.IsNullOrEmpty(SelectedLogo) && File.Exists(SelectedLogo))
                {
                    Properties.Settings.Default.SelectedLogo = SelectedLogo;
                }
                else if (File.Exists(defaultLogoPath))
                {
                    Properties.Settings.Default.SelectedLogo = defaultLogoPath;
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona un logo.", "Logo No Seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar todas las configuraciones
                Properties.Settings.Default.Save();

                // Aplicar cambios en `Form1`
                if (Application.OpenForms["Form1"] is Form1 mainForm)
                {
                    mainForm.UpdateButtonVisibility(Properties.Settings.Default.IsBacklot);
                    mainForm.ActualizarUrl(); 
                }

                MessageBox.Show("Configuración guardada correctamente.", "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar la configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSeleccionarLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedLogo = openFileDialog.FileName;
                    picLogo.Image = Image.FromFile(SelectedLogo);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedImagePath = openFileDialog.FileName;
                    // Si deseas mostrar una vista previa, descomenta la siguiente línea:
                    // picBoxPreview.Image = Image.FromFile(selectedImagePath);
                    Properties.Settings.Default.TerminalImagePath = selectedImagePath;
                    Properties.Settings.Default.Save();

                    MessageBox.Show("Imagen guardada correctamente.", "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsBasedOnMode();
        }

       
    }
}
