using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VirtualesPrueba
{
    public partial class Form2 : Form
    {
        // Constantes para hook y hotkey
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int HOTKEY_ID = 1;
        private const int MOD_NONE = 0x0000;
        private const int VK_F2 = 0x71;
        private Form currentForm = null;
        // Variables para la hotkey y temporización
        private DateTime lastHotkeyTime = DateTime.MinValue;
        private FormPrintSettings _printSettingsForm;

        // Variables del WebView2 y configuración
        private string currentUrl = "https://shop.passowin.net/#/";
        private string usuarioActual = "User";

        // Variables para el hook de mouse
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc mouseProc;
        private IntPtr hookId = IntPtr.Zero;

        // Estructura para el hook
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // Importaciones de funciones nativas
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Form2()
        {
            InitializeComponent();

            // Aplicar configuración de botones desde Settings
            AplicarConfiguracionBotones();

            // Configuración de ventana
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form2_Load;
            this.FormClosing += Form2_FormClosing;
            // Asignar el delegado para el hook (si se requiere)
            mouseProc = MouseHookCallback;
            // Opcional: establecer el hook de mouse
            // hookId = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, GetModuleHandle(null), 0);

            // Registrar la hotkey F2
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NONE, VK_F2);
        }

       
        private void AplicarConfiguracionBotones()
        {
            Properties.Settings.Default.Reload();

            string estadoVirtuales = Properties.Settings.Default.EstadoBotonVirtuales?.Trim();
            string estadoHipicas = Properties.Settings.Default.EstadoBotonHipicas?.Trim();

            bool virtualesVisible = estadoVirtuales.Equals("Activado", StringComparison.OrdinalIgnoreCase);
            bool hipicasVisible = estadoHipicas.Equals("Activado", StringComparison.OrdinalIgnoreCase);

            Debug.WriteLine($"[Form2] EstadoBotonVirtuales = '{estadoVirtuales}', Visible = {virtualesVisible}");
            Debug.WriteLine($"[Form2] EstadoBotonHipicas = '{estadoHipicas}', Visible = {hipicasVisible}");

            btnVirtuales.Visible = virtualesVisible;
            btnHipicas.Visible = hipicasVisible;
        }

    
        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            using (FormPrintSettings configForm = new FormPrintSettings())
            {
                configForm.ConfigChanged += () =>
                {
                    AplicarConfiguracionBotones();
                };

                
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    AplicarConfiguracionBotones();
                }
            }
        }


        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT mouseInfo = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                if (!this.Bounds.Contains(mouseInfo.pt))
                {
                    this.Close();
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

  
        private async void Form2_Load(object sender, EventArgs e)
        {
            this.Icon = new Icon("logos/icono.ico");
    
            usuarioActual = Properties.Settings.Default.UltimoUsuario;
            if (string.IsNullOrEmpty(usuarioActual))
                usuarioActual = "User";
            this.Text = "BetsPlay v1.6 | Terminal | - " + usuarioActual;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = false;

            try
            {
                await WebView21.EnsureCoreWebView2Async();
                WebView21.KeyDown += WebView21_KeyDown;
                WebView21.CoreWebView2.NavigationStarting += WebView21_NavigationStarting;
                WebView21.CoreWebView2.NavigationCompleted += WebView21_NavigationCompleted;
               
                WebView21.CoreWebView2.NavigationCompleted += RemoveBackgroundScript;
                WebView21.CoreWebView2.WebMessageReceived += WebView21_WebMessageReceived;
                WebView21.CoreWebView2.Settings.IsStatusBarEnabled = false;
                WebView21.CoreWebView2.Settings.AreDevToolsEnabled = true;

                Timer1.Interval = 3000;
                Timer1.Tick += Timer1_Tick;

                WebView21.CoreWebView2.Navigate(currentUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando WebView2: " + ex.Message);
            }
        }

     
        private void WebView21_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string jsonData = e.WebMessageAsJson;
            string cleanedJson = JsonConvert.DeserializeObject<string>(jsonData);
            try
            {
                var userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(cleanedJson);
                if (userData != null && userData.ContainsKey("user"))
                {
                    string username = userData["user"];
                    if (usuarioActual != username)
                    {
                        usuarioActual = username;
                        Properties.Settings.Default.UltimoUsuario = usuarioActual;
                        Properties.Settings.Default.Save();
                        this.Invoke((MethodInvoker)(() =>
                        {
                            this.Text = "BetsPlay v1.6 | Terminal | " + usuarioActual;
                        }));
                    }
                }
                else
                {
                    MessageBox.Show("El JSON no contiene 'user'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

  
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            Properties.Settings.Default.UltimoUsuario = usuarioActual;
            Properties.Settings.Default.Save();
        }

    
        private void WebView21_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.F8)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F3)
            {
                currentUrl = "https://shop.passowin.net/#/config";
                WebView21.CoreWebView2.Navigate(currentUrl);
            }
            else if (e.KeyCode == Keys.F5)
            {
                currentUrl = "https://shop.passowin.net/#/";
                WebView21.CoreWebView2.Navigate(currentUrl);
                WebView21.CoreWebView2.Reload();
            }
        }

      
        private void WebView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri != currentUrl)
            {
                e.Cancel = true;
            }
        }

     
        private void RemoveBackgroundScript(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string script = @"
(function() {
        function applyChanges() {
            let logoElement = document.querySelector('.logo');
            if (logoElement) {
                logoElement.style.setProperty('background', 'none', 'important');
            }
            
            let logoElement2 = document.querySelector('.cpnContent');
            if (logoElement2) {
                logoElement2.style.setProperty('background', 'none', 'important');
                logoElement2.style.backgroundColor = '#B6B6BB'; 
            }

            let logoutButton = document.querySelector('li.logOut');
            if (logoutButton) {
                logoutButton.parentNode.removeChild(logoutButton);
            }

            let checkIcons = document.querySelectorAll('li.checkcppn');
            if (checkIcons.length > 3 && checkIcons[3].parentNode) {
                checkIcons[3].parentNode.removeChild(checkIcons[3]);
            }
            if (checkIcons[0] && checkIcons[0].parentNode) {
                checkIcons[0].parentNode.removeChild(checkIcons[0]);
            }

            let settings = document.querySelector('li.settings');
            if (settings && settings.parentNode) {
                settings.parentNode.removeChild(settings);
            }

            let paises = document.querySelector('li#langOpen');
            if (paises && paises.parentNode) {
                paises.parentNode.removeChild(paises);
            }

            let keyboardShortcut = document.querySelector('span.keyboardshortcut4');
            if (keyboardShortcut && keyboardShortcut.parentNode) {
                keyboardShortcut.parentNode.removeChild(keyboardShortcut);
            }

            let keyboardShortcut2 = document.querySelector('span.keyboardshortcut2');
            if (keyboardShortcut2 && keyboardShortcut2.parentNode) {
                keyboardShortcut2.parentNode.removeChild(keyboardShortcut2);
            }

            let keyboardShortcut3 = document.querySelector('span.keyboardshortcut');
            if (keyboardShortcut3 && keyboardShortcut3.parentNode) {
                keyboardShortcut3.parentNode.removeChild(keyboardShortcut3);
            }

            let usernameField = document.querySelector('input[ng-model=""data.username""]');

            if (usernameField) {
                console.log('✅ Campo de usuario detectado:', usernameField);

                usernameField.addEventListener('input', function() {
                    console.log('📩 Enviando usuario a VB.NET:', usernameField.value);
                    window.chrome.webview.postMessage(JSON.stringify({user: usernameField.value}));
                });
            } else {
                console.log('⚠️ No se encontró el campo de usuario');
            }

            }

        // Aplicar cambios inmediatamente
        applyChanges();

        // Observar cambios en el DOM y aplicar cambios dinámicamente
        let observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                applyChanges();
            });
        });

        observer.observe(document.body, { childList: true, subtree: true });

        
    })();
";
            WebView21.CoreWebView2.ExecuteScriptAsync(script);
        }

       
        private void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Timer1.Start();
        }

       
        private void Timer1_Tick(object sender, EventArgs e)
        {
            PictureBox1.Visible = true;
            Timer1.Stop();
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            var oldF = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (oldF != null)
            {
                oldF.Close();
                oldF.Dispose();
            }


            var f = new Form1();
            f.Show();
        }


        private void Button1_Click(object sender, EventArgs e)
        {

            if (currentForm != null && !currentForm.IsDisposed)
            {
                currentForm.Close();
            }

            currentForm = Application.OpenForms.OfType<Virtuales>().FirstOrDefault();

            if (currentForm == null)
            {
                currentForm = new Virtuales();
                currentForm.Show();
            }
            else
            {
                currentForm.BringToFront();
                currentForm.Focus();
            }
        }


        public void OpenPrintSettingsForm()
        {
            if ((DateTime.Now - lastHotkeyTime).TotalMilliseconds < 1000)
                return;

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
                        _printSettingsForm.ConfigChanged += () =>
                        {
                            AplicarConfiguracionBotones();
                        };
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
