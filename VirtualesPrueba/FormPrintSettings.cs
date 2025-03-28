using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VirtualesPrueba
{
    public partial class FormPrintSettings : Form
    {
        // Propiedades para la configuración (puedes usarlas en otros lugares)
        public string SelectedLogo { get; set; }
        public string SelectedUser { get; set; }
        public string FinalMessage { get; set; }
        public string SelectedPrinter { get; set; }
        public int PaperWidth { get; set; }

        // Evento que se dispara cuando cambia la configuración
        public event Action ConfigChanged;

        public FormPrintSettings()
        {
            InitializeComponent();
            this.Load += FormPrintSettings_Load;
        }

        private void FormPrintSettings_Load(object sender, EventArgs e)
        {
            // Forzamos a recargar los settings para obtener el valor guardado en disco
            Properties.Settings.Default.Reload();

            // Agregar opciones al ComboBox si aún no se han agregado.
            if (!comboBoxVirtuales.Items.Contains("Activado"))
            {
                comboBoxVirtuales.Items.Add("Activado");
                comboBoxVirtuales.Items.Add("Desactivado");
            }
            if (!comboBoxHipicas.Items.Contains("Activado"))
            {
                comboBoxHipicas.Items.Add("Activado");
                comboBoxHipicas.Items.Add("Desactivado");
            }

            // Cargar estados de botones individuales
            string estadoVirtuales = Properties.Settings.Default.EstadoBotonVirtuales;
            if (string.IsNullOrWhiteSpace(estadoVirtuales)) estadoVirtuales = "Activado";
            comboBoxVirtuales.SelectedItem = estadoVirtuales;

            string estadoHipicas = Properties.Settings.Default.EstadoBotonHipicas;
            if (string.IsNullOrWhiteSpace(estadoHipicas)) estadoHipicas = "Activado";
            comboBoxHipicas.SelectedItem = estadoHipicas;

            // Cargar otros controles según la configuración guardada.
            txtUrl.Text = Properties.Settings.Default.DefaultUrl;
            this.ShowInTaskbar = false;

            if (!cmbMode.Items.Contains("Cajero"))
            {
                cmbMode.Items.Add("Cajero");
                cmbMode.Items.Add("Terminal");
            }
            if (!cmbBacklot.Items.Contains("Activado"))
            {
                cmbBacklot.Items.Add("Activado");
                cmbBacklot.Items.Add("Desactivado");
            }

            // Cargar estados de impresión.
            chkImprimirUsuario.Checked = Properties.Settings.Default.ImprimirUsuario;
            chkImprimirMensajeFinal.Checked = Properties.Settings.Default.ImprimirMensajeFinal;

            // Cargar impresoras instaladas.
            cmbPrinters.Items.Clear();
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
                    cmbPrinters.SelectedItem = defaultPrinter.PrinterName;
                else if (cmbPrinters.Items.Count > 0)
                    cmbPrinters.SelectedIndex = 0;
            }

            // Configurar el modo (Terminal o Cajero).
            if (Properties.Settings.Default.IsTerminal)
                cmbMode.SelectedItem = "Terminal";
            else
                cmbMode.SelectedItem = "Cajero";

            // Configurar el Backlot.
            if (Properties.Settings.Default.IsBacklot)
                cmbBacklot.SelectedItem = "Activado";
            else
                cmbBacklot.SelectedItem = "Desactivado";

            UpdateControlsBasedOnMode();

            // Selección del ancho del papel.
            if (Properties.Settings.Default.PaperWidth == 58)
                rb58mm.Checked = true;
            else if (Properties.Settings.Default.PaperWidth == 80)
                rb80mm.Checked = true;
            else
                rb80mm.Checked = true;

            // Configurar el logo (se verifica que exista el archivo).
            string defaultLogoPath = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png");
            if (string.IsNullOrEmpty(Properties.Settings.Default.SelectedLogo) || !File.Exists(Properties.Settings.Default.SelectedLogo))
            {
                if (File.Exists(defaultLogoPath))
                    Properties.Settings.Default.SelectedLogo = defaultLogoPath;
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedLogo) && File.Exists(Properties.Settings.Default.SelectedLogo))
                picLogo.Image = Image.FromFile(Properties.Settings.Default.SelectedLogo);
            else
                picLogo.Image = null;

            txtUsuario.Text = Properties.Settings.Default.SelectedUser;
            txtMensajeFinal.Text = Properties.Settings.Default.FinalMessage;
        }

        private void comboBoxVirtuales_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EstadoBotonVirtuales = comboBoxVirtuales.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            ConfigChanged?.Invoke();
        }

        private void comboBoxHipicas_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EstadoBotonHipicas = comboBoxHipicas.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            ConfigChanged?.Invoke();
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que se haya seleccionado al menos un elemento para imprimir.
                if (!(chkImprimirUsuario.Checked || chkImprimirMensajeFinal.Checked))
                {
                    MessageBox.Show("Por favor, selecciona al menos un elemento para imprimir.",
                        "Configuración Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.ImprimirUsuario = chkImprimirUsuario.Checked;
                Properties.Settings.Default.ImprimirMensajeFinal = chkImprimirMensajeFinal.Checked;

                // Configurar el modo.
                if (cmbMode.SelectedItem.ToString() == "Terminal")
                {
                    Properties.Settings.Default.IsTerminal = true;
                    Properties.Settings.Default.SelectedPrinter = string.Empty;
                    Properties.Settings.Default.PaperWidth = 0;
                }
                else if (cmbMode.SelectedItem.ToString() == "Cajero")
                {
                    Properties.Settings.Default.IsTerminal = false;
                    if (cmbPrinters.SelectedItem == null)
                    {
                        MessageBox.Show("Por favor, selecciona una impresora.",
                            "Selección de Impresora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Properties.Settings.Default.SelectedPrinter = cmbPrinters.SelectedItem.ToString();
                    if (!rb58mm.Checked && !rb80mm.Checked)
                    {
                        MessageBox.Show("Por favor, selecciona un tipo de impresión válido.",
                            "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Properties.Settings.Default.PaperWidth = rb58mm.Checked ? 58 : 80;
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona un modo válido (Terminal o Cajero).",
                        "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Configurar el Backlot.
                Properties.Settings.Default.IsBacklot = (cmbBacklot.SelectedItem.ToString() == "Activado");

                // Validar y guardar usuario y mensaje final.
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show("Por favor, ingresa un nombre de usuario.",
                        "Usuario Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.SelectedUser = txtUsuario.Text.Trim();

                if (string.IsNullOrWhiteSpace(txtMensajeFinal.Text))
                {
                    MessageBox.Show("Por favor, ingresa un mensaje final.",
                        "Mensaje Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.FinalMessage = txtMensajeFinal.Text.Trim();

                // Validar y guardar la URL.
                string nuevaUrl = txtUrl.Text.Trim();
                if (string.IsNullOrEmpty(nuevaUrl) || !Uri.IsWellFormedUriString(nuevaUrl, UriKind.Absolute))
                {
                    MessageBox.Show("Ingrese una URL válida.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Properties.Settings.Default.DefaultUrl = nuevaUrl;

                // Configuración del logo.
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
                    MessageBox.Show("Por favor, selecciona un logo.",
                        "Logo No Seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (comboBoxVirtuales.SelectedItem == null || comboBoxHipicas.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, selecciona una opción para ambos botones.",
                        "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar los valores en Properties.Settings
                Properties.Settings.Default.EstadoBotonVirtuales = comboBoxVirtuales.SelectedItem.ToString();
                Properties.Settings.Default.EstadoBotonHipicas = comboBoxHipicas.SelectedItem.ToString();

                Properties.Settings.Default.Save(); // Guardar configuración en disco

                // Disparar el evento para notificar el cambio
                ConfigChanged?.Invoke();

                MessageBox.Show("Configuración guardada correctamente.",
                    "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar la configuración: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Properties.Settings.Default.TerminalImagePath = selectedImagePath;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Imagen guardada correctamente.",
                        "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsBasedOnMode();
        }
    }
}
