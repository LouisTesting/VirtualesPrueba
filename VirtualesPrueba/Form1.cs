using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Linq;
using System.Drawing; 

namespace VirtualesPrueba
{
    public partial class Form1 : Form
    {
     
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int HOTKEY_ID = 9000; 
        const uint MOD_NONE = 0x0000;
        const uint VK_F2 = 0x71;      

        private ChromiumWebBrowser browser;
        private backlot _backlotForm;
        private bool _loginOpen = false;
        private FormPrintSettings _printSettingsForm;
        private string marcadorArchivo = @"C:\MOHIO-DescargaCompleta.txt";
        private DateTime lastHotkeyTime = DateTime.MinValue;

        
        private NotifyIcon notifyIcon;
        private bool allowExit = false; 

        public Form1()
        {
            InitializeComponent();

           
            try
            {
                this.Icon = new Icon("Logos/icono.ico");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el icono: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.KeyPreview = true;
            this.Load += Form1_Load;

            try
            {
                if (!ArchivoDeMarcadorExiste())
                {
                    DescargarYEjecutar();
                }

                string extensionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensiones", "MOHIO");
                var settings = new CefSettings();
                settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");

                if (Directory.Exists(extensionPath))
                {
                    settings.CefCommandLineArgs.Add("load-extension", extensionPath);
                }
                else
                {
                    MessageBox.Show("No se encontró la extensión en: " + extensionPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (!Cef.Initialize(settings))
                {
                    MessageBox.Show("Error al inicializar CefSharp", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string urlGuardado = Properties.Settings.Default.DefaultUrl;
                if (string.IsNullOrEmpty(urlGuardado))
                {
                    urlGuardado = "https://games.mohiogaming.com/cashier/integration/login/77569"; 
                }

                browser = new ChromiumWebBrowser(urlGuardado)
                {
                    Dock = DockStyle.Fill
                };

                this.Controls.Add(browser);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar la aplicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool registered = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NONE, VK_F2);
            if (!registered)
            {
                MessageBox.Show("No se pudo registrar la hotkey global F2.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateButtonVisibility(Properties.Settings.Default.IsBacklot);

            
            notifyIcon = new NotifyIcon();
            try
            {
                notifyIcon.Icon = new Icon("Logos/icono.ico");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el icono del NotifyIcon: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            notifyIcon.Text = "Mi Aplicación - Haga clic derecho para opciones";
            notifyIcon.Visible = true;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem cerrarItem = new ToolStripMenuItem("Cerrar");
            cerrarItem.Click += (s, args) =>
            {
               
                allowExit = true;
                notifyIcon.Visible = false;
                Application.Exit();
            };
            contextMenu.Items.Add(cerrarItem);
            
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.MouseDoubleClick += (s, args) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };
        }

       
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Cef.Shutdown();
            notifyIcon.Visible = false;

            if (!allowExit)
            {
                try
                {
                    string exePath = Application.ExecutablePath;
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C timeout /t 5 && start \"\" \"{exePath}\"",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al reiniciar la aplicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            base.OnFormClosed(e);
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    OpenPrintSettingsForm();
                }
            }
            base.WndProc(ref m);
        }

        public async Task DescargarYEjecutar()
        {
            try
            {
                string url = "https://games.mohiogaming.com/files/cashier/MOHIO-Installer.exe";
                string rutaDestino = @"C:\MOHIO-Installer.exe";

                using (HttpClient cliente = new HttpClient())
                {
                    var response = await cliente.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(rutaDestino, fileBytes);
                }

                MessageBox.Show("Descarga completada y guardada en: " + rutaDestino, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EjecutarComoAdministrador(rutaDestino);
                CrearArchivoDeMarcador();
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Error de conexión: {httpEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarComoAdministrador(string archivo)
        {
            try
            {
                if (!File.Exists(archivo))
                {
                    MessageBox.Show("El archivo no se encontró en la ubicación: " + archivo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (IsRunningAsAdministrator())
                {
                    Process.Start(archivo);
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = archivo,
                        Verb = "runas",
                        UseShellExecute = true
                    };

                    Process proceso = Process.Start(startInfo);
                    proceso.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar el archivo como administrador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsRunningAsAdministrator()
        {
            try
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private bool ArchivoDeMarcadorExiste()
        {
            return File.Exists(marcadorArchivo);
        }

        private void CrearArchivoDeMarcador()
        {
            try
            {
                File.WriteAllText(marcadorArchivo, "La descarga ya se ha realizado.");
                MessageBox.Show("Se creó el archivo de marcador.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el archivo de marcador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateButtonVisibility(bool isVisible)
        {
            buttonAnimalitos.Visible = isVisible;
        }

        public void OpenPrintSettingsForm()
        {
            if ((DateTime.Now - lastHotkeyTime).TotalMilliseconds < 1000)
            {
                return;
            }
            lastHotkeyTime = DateTime.Now;

            UnregisterHotKey(this.Handle, HOTKEY_ID);
            try
            {
                FormLogin login = new FormLogin();
                DialogResult result = login.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (_printSettingsForm == null || _printSettingsForm.IsDisposed)
                    {
                        _printSettingsForm = new FormPrintSettings();
                        _printSettingsForm.Show();
                    }
                    else
                    {
                        _printSettingsForm.BringToFront();
                    }
                }

                login.Dispose();
            }
            finally
            {
                RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NONE, VK_F2);
            }
        }

        public void ActualizarUrl()
        {
            string nuevaUrl = Properties.Settings.Default.DefaultUrl;
            if (browser != null)
            {
                browser.Load(nuevaUrl);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (_backlotForm == null || _backlotForm.IsDisposed)
            {
                _backlotForm = new backlot();
                _backlotForm.FormClosed += (s, args) => _backlotForm = null;
                _backlotForm.Show();
            }
            else
            {
                _backlotForm.BringToFront();
            }
        }
    }
}
