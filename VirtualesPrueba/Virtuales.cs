using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Linq;

namespace VirtualesPrueba
{
    public partial class Virtuales : Form
    {
        

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

        public Virtuales()
        {
            InitializeComponent();
            // Configuramos el formulario sin bordes y posición manual
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            try { this.Icon = new Icon("Logos/icono.ico"); }
            catch (Exception ex) { MessageBox.Show("No se pudo cargar el icono: " + ex.Message); }
            this.KeyPreview = true;
            this.Load += Virtuales_Load;
        }

        private void AjustarFormulario()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            double baseWidth = 1585, baseHeight = 785;
            double scaleFactor = Math.Min(screenWidth / baseWidth, screenHeight / baseHeight);
            int newWidth = (int)(baseWidth * scaleFactor);
            int newHeight = (int)(baseHeight * scaleFactor);
            if (newWidth > screenWidth) newWidth = screenWidth;
            if (newHeight > screenHeight) newHeight = screenHeight;
            int offsetFromBottom = 0, extraHeight = 0;
            if (screenWidth <= 800) { offsetFromBottom = 70; extraHeight = 60; }
            else if (screenWidth <= 1024) { offsetFromBottom = 50; extraHeight = 130; }
            else if (screenWidth <= 1280)
            {
                if (screenHeight <= 720) { offsetFromBottom = 5; extraHeight = 10; }
                else if (screenHeight <= 800) { offsetFromBottom = 20; extraHeight = 70; }
                else if (screenHeight <= 1024) { offsetFromBottom = 10; extraHeight = 300; }
            }

            else if (screenWidth <= 1366) { offsetFromBottom = 10; extraHeight = 5; }
            else if (screenWidth <= 1440) { offsetFromBottom = 40; extraHeight = 60; }
            else if (screenWidth <= 1600) { offsetFromBottom = 30; extraHeight = 5; }
            else if (screenWidth <= 1680) { offsetFromBottom = 40; extraHeight = 100; }
            else if (screenWidth <= 1920) { offsetFromBottom = 0; extraHeight = 40; }
            else { offsetFromBottom = 0; extraHeight = 0; }
            newHeight += extraHeight;
            if (newHeight > screenHeight) newHeight = screenHeight;
            this.Width = newWidth;
            this.Height = newHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = screenHeight - this.Height - offsetFromBottom;
            Debug.WriteLine($"Formulario ajustado: {this.Width}x{this.Height} en ({this.Left}, {this.Top})");
        }

        private async void Virtuales_Load(object sender, EventArgs e)
        {
           
            AjustarFormulario();

            this.ShowInTaskbar = false;
            notifyIcon = new NotifyIcon();
            try { notifyIcon.Icon = new Icon("Logos/icono.ico"); }
            catch (Exception ex) { MessageBox.Show("No se pudo cargar el icono del NotifyIcon: " + ex.Message); }
            notifyIcon.Text = "Mi Aplicación - Haga clic derecho para opciones";
            notifyIcon.Visible = true;
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem cerrarItem = new ToolStripMenuItem("Cerrar");
            cerrarItem.Click += (s, args) => { allowExit = true; notifyIcon.Visible = false; Application.Exit(); };
            contextMenu.Items.Add(cerrarItem);
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.MouseDoubleClick += (s, args) => { this.Show(); };

            
            if (!ArchivoDeMarcadorExiste())
            {
                await DescargarYEjecutar();
            }

           

            if (Cef.IsInitialized != true)
            {
                var settings = new CefSettings();
                settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
                string extensionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensiones", "MOHIO");

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
            }


            await Task.Run(() =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    
                    if (browser == null)
                    {
                        string urlGuardado = Properties.Settings.Default.DefaultUrl;
                        if (string.IsNullOrEmpty(urlGuardado))
                            urlGuardado = "https://games.mohiogaming.com/cashier/integration/login/77569";

                        browser = new ChromiumWebBrowser(urlGuardado) { Dock = DockStyle.Fill };
                        this.Controls.Add(browser);

                        
                        browser.FrameLoadStart += (senderFrame, eFrame) =>
                        {
                            if (eFrame.Frame.IsMain)
                            {
                                this.Invoke((MethodInvoker)(() => EliminarLogoDesdeCef()));
                            }
                        };
                    }
                    else
                    {
                        browser.Load(Properties.Settings.Default.DefaultUrl);
                    }
                }));
            });
        }

        private void EliminarLogoDesdeCef()
        {
            try
            {
                if (browser != null)
                {
                    string script = @"
                    (function() {
                        console.log('Iniciando overlay blanco antes de la carga.');
                        let overlay = document.createElement('div');
                        overlay.id = 'customOverlay';
                        overlay.style.position = 'fixed';
                        overlay.style.top = '0';
                        overlay.style.left = '0';
                        overlay.style.width = '100vw';
                        overlay.style.height = '100vh';
                        overlay.style.backgroundColor = 'white';
                        overlay.style.zIndex = '999999';
                        overlay.style.transition = 'opacity 1s ease-out';
                        document.documentElement.appendChild(overlay);
                        function removeElements() {
                            let elementsToRemove = [
                                '.logoImg',
                                'div.font-large',
                                'div.font-small',
                                'div.headerLogin',
                                'img.infoIcon'
                            ];
                            elementsToRemove.forEach(selector => {
                                let el = document.querySelector(selector);
                                if (el) { el.remove(); }
                            });
                            setTimeout(() => {
                                let overlay = document.getElementById('customOverlay');
                                if (overlay) {
                                    overlay.style.opacity = '0';
                                    setTimeout(() => { overlay.remove(); }, 1000);
                                }
                            }, 3000);
                        }
                        if (document.readyState === 'complete') { removeElements(); }
                        else { window.addEventListener('load', removeElements, { once: true }); }
                    })();
                    ";
                    browser.ExecuteScriptAsync(script);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar el script: " + ex.Message);
            }
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
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarComoAdministrador(string archivo)
        {
            try
            {
                if (!File.Exists(archivo))
                {
                    MessageBox.Show("El archivo no se encontró en: " + archivo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (IsRunningAsAdministrator())
                {
                    Process.Start(archivo);
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
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
                MessageBox.Show("Error al ejecutar como administrador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch { return false; }
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
                MessageBox.Show("Error al crear el marcador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OpenPrintSettingsForm()
        {
            if ((DateTime.Now - lastHotkeyTime).TotalMilliseconds < 1000) return;
            lastHotkeyTime = DateTime.Now;
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
    }
}
