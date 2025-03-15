using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core; 
using Newtonsoft.Json;              
using Newtonsoft.Json.Linq;
using VirtualesPrueba;






namespace VirtualesPrueba
{
    public partial class backlot : Form
    {
        private Dictionary<string, string> requestData = new Dictionary<string, string>();
        private Timer inactivityTimer = new Timer();
        private const int inactivityThreshold = 2 * 60 * 1000; // 2 minutos
        private Point lastMousePosition;
        private bool isActivityDetected = false;

        // Constantes y hooks
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc mouseProc;
        private IntPtr hookId = IntPtr.Zero;


        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Constructor
        public backlot()
        {
            InitializeComponent();   // Si usas un diseñador, llama aquí a la inicialización

            // Configuración del timer
            inactivityTimer.Interval = inactivityThreshold;
            inactivityTimer.Tick += InactivityTimer_Tick;

            // Manejadores de eventos del formulario
            this.Load += backlot_Load;
            this.Resize += backlot_Resize;
            this.FormClosed += backlot_FormClosed;
            this.Deactivate += backlot_Deactivate;
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT mouseInfo = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                Point cursorPosition = mouseInfo.pt;

                if (!this.Bounds.Contains(cursorPosition))
                {
                    this.Close();
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private void AjustarFormulario()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            double baseWidth = 1585;
            double baseHeight = 790;

            double scaleFactor = Math.Min(screenWidth / baseWidth, screenHeight / baseHeight);

            int newWidth = (int)(baseWidth * scaleFactor);
            int newHeight = (int)(baseHeight * scaleFactor);

            if (newWidth > screenWidth) newWidth = screenWidth;
            if (newHeight > screenHeight) newHeight = screenHeight;

            int offsetFromBottom = 0;
            int extraHeight = 0;

            if (screenWidth <= 800)
            {
                offsetFromBottom = 70; extraHeight = 60;
            }
            else if (screenWidth <= 1024)
            {
                offsetFromBottom = 50; extraHeight = 130;
            }
            else if (screenWidth <= 1280)
            {
                if (screenHeight <= 720)
                {
                    offsetFromBottom = 5; extraHeight = 10;
                }
                else if (screenHeight <= 800)
                {
                    offsetFromBottom = 20; extraHeight = 70;
                }
                else if (screenHeight <= 1024)
                {
                    offsetFromBottom = 10; extraHeight = 300;
                }
            }
            else if (screenWidth <= 1366)
            {
                offsetFromBottom = 10; extraHeight = 5;
            }
            else if (screenWidth <= 1440)
            {
                offsetFromBottom = 40; extraHeight = 60;
            }
            else if (screenWidth <= 1600)
            {
                offsetFromBottom = 30; extraHeight = 5;
            }
            else if (screenWidth <= 1680)
            {
                offsetFromBottom = 40; extraHeight = 100;
            }
            else if (screenWidth <= 1920)
            {
                offsetFromBottom = 0; extraHeight = 40;
            }
            else
            {
                offsetFromBottom = 0; extraHeight = 0;
            }

            newHeight += extraHeight;

            if (newHeight > screenHeight)
            {
                newHeight = screenHeight;
            }

            this.Width = newWidth;
            this.Height = newHeight;

            this.Left = (screenWidth - this.Width) / 2;
            this.Top = screenHeight - this.Height - offsetFromBottom;
        }

        private void backlot_Resize(object sender, EventArgs e)
        {
            AjustarFormulario();
        }

        private async void backlot_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            
            if (!string.IsNullOrEmpty(VirtualesPrueba.Properties.Settings.Default.SelectedPrinter))
            {
               
            }
            else
            {
               
            }

            inactivityTimer.Start();
            lastMousePosition = Cursor.Position;
            AjustarFormulario();

            try
            {
                // Asegúrate de haber agregado un control WebView2 (webView21) al formulario
                await webView21.EnsureCoreWebView2Async();
                ConfigureWebView2PopupBlocking();

                webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

                webView21.CoreWebView2.WebResourceRequested += WebView21_WebResourceRequested;
                webView21.CoreWebView2.WebResourceResponseReceived += WebView21_WebResourceResponseReceived;

                this.MouseMove += OnMouseMove;
                this.KeyPress += ResetInactivityTimer;

                await InjectActivityDetectionScriptAsync();
                await InjectPrintInterceptionScriptAsync();
                await InjectAutofillScriptAsync();
                SetupWebViewActivityListener();

                webView21.CoreWebView2.Navigate("https://www.caja.backlot.bet/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando WebView2: " + ex.Message);
            }
        }

        private async System.Threading.Tasks.Task InjectPrintInterceptionScriptAsync()
        {
            string script = @"
                (function() {
                    window.print = function() {
                        try {
                            const ticketElement = document.querySelector('.ticket-content');
                            if (ticketElement) {
                                const message = {
                                    type: 'print',
                                    content: ticketElement.outerHTML
                                };
                                window.chrome.webview.postMessage(message);
                                console.log('Contenido del ticket enviado al host:', message);
                            }
                        } catch (error) {
                            console.error('Error en print interceptado:', error);
                        }
                    };
                })();
            ";
            await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);
        }

        private async System.Threading.Tasks.Task InjectActivityDetectionScriptAsync()
        {
            string script = @"
                (function() {
                    function notifyHost() {
                        try {
                            window.chrome.webview.postMessage({
                                type: 'userActivity',
                                message: 'User is active inside WebView'
                            });
                        } catch (e) {
                            console.error('Error posting activity message:', e);
                        }
                    }
                    document.addEventListener('mousemove', notifyHost);
                    document.addEventListener('keydown', notifyHost);
                    document.addEventListener('click', notifyHost);
                })();
            ";
            await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);
        }
        private string GetConfigFilePath()
        {
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BacklotApp");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            return Path.Combine(appDataFolder, "config.json");
        }

        private void SaveConfiguration(AppConfig config)
        {
            try
            {
                string configFilePath = GetConfigFilePath();
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configFilePath, json);
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private AppConfig LoadConfiguration()
        {
            try
            {
                string configFilePath = GetConfigFilePath();
                if (File.Exists(configFilePath))
                {
                    string json = File.ReadAllText(configFilePath);
                    AppConfig config = JsonConvert.DeserializeObject<AppConfig>(json);
                    return config;
                }
            }
            catch (Exception)
            {
                // Manejo de errores
            }
            return null;
        }

        private async System.Threading.Tasks.Task InjectAutofillScriptAsync()
        {
            try
            {
                AppConfig config = LoadConfiguration();
                if (config == null ||
                    string.IsNullOrEmpty(config.UsuarioBacklot) ||
                    string.IsNullOrEmpty(config.ContraseñaBacklot))
                {
                    return;
                }

                string script = $@"
                    (function() {{
                        function simulateTyping(element, textValue) {{
                            if (!element) return;
                            element.focus();
                            element.value = textValue;
                            element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        }}

                        function autofill() {{
                            const userField = document.querySelector('#usuario');
                            const passField = document.querySelector('#clave');

                            if (!userField || !passField) {{
                                console.log('Campos de usuario o contraseña no encontrados.');
                                return;
                            }}

                            simulateTyping(userField, '{config.UsuarioBacklot}');
                            simulateTyping(passField, '{config.ContraseñaBacklot}');
                            console.log('Autocompletado realizado.');
                        }}

                        document.addEventListener('DOMContentLoaded', autofill);
                    }})();
                ";
                await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private void SetupWebViewActivityListener()
        {
            webView21.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string message = e.WebMessageAsJson;
                var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                if (jsonData.ContainsKey("type") && jsonData["type"] == "userActivity")
                {
                    // Reinicia el temporizador
                    ResetInactivityTimer(null, null);
                }
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private void ConfigureWebView2PopupBlocking()
        {
            webView21.CoreWebView2.NewWindowRequested += HandleNewWindowRequested;
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            if (!isActivityDetected)
            {
                inactivityTimer.Stop();
                this.Close();
            }
            else
            {
                isActivityDetected = false;
            }
        }

        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            isActivityDetected = true;
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        private void HandleNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private void WebView21_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            try
            {
                var request = e.Request;
                if (request.Uri.Contains("https://www.caja.backlot.bet/controllers/login"))
                {
                    if (request.Method == "POST" && request.Content != null)
                    {
                        string content;
                        using (var memoryStream = new MemoryStream())
                        {
                            request.Content.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            content = new StreamReader(memoryStream).ReadToEnd();
                        }

                        var parsed = HttpUtility.ParseQueryString(content);
                        string user = parsed["usuario"];
                        string pass = parsed["contraseña"];

                        GuardarCredencialesBacklot(user, pass);
                    }
                }

                if (request.Uri.Contains("https://www.caja.backlot.bet/controllers/guardarTicket"))
                {
                    if (request.Method == "POST" && request.Content != null)
                    {
                        string content;
                        using (var memoryStream = new MemoryStream())
                        {
                            request.Content.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            content = new StreamReader(memoryStream).ReadToEnd();
                        }

                        var keyValuePairs = HttpUtility.ParseQueryString(content);

                        requestData["apuesta"] = string.Join(",", keyValuePairs.GetValues("apuesta[]"));
                        requestData["evento"] = string.Join(",", keyValuePairs.GetValues("evento[]"));
                        requestData["opcion"] = string.Join(",", keyValuePairs.GetValues("opcion[]"));
                        requestData["horaSorteo"] = keyValuePairs.GetValues("horaSorteo[]")?.FirstOrDefault();
                        requestData["juego"] = keyValuePairs.GetValues("juego[]")?.FirstOrDefault();
                        requestData["hora"] = keyValuePairs["hora"];
                        requestData["total"] = keyValuePairs["total"];
                    }
                }
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private void GuardarCredencialesBacklot(string usuario, string contraseña)
        {
            try
            {
                AppConfig config = LoadConfiguration() ?? new AppConfig();
                config.UsuarioBacklot = usuario;
                config.ContraseñaBacklot = contraseña;
                config.IsBacklot = true;

                SaveConfiguration(config);
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private async void WebView21_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            try
            {
                if (e.Request.Uri.Contains("https://www.caja.backlot.bet/controllers/guardarTicket"))
                {
                    var response = e.Response;
                    using (var responseStream = await response.GetContentAsync())
                    {
                        string responseBody;
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseBody = await reader.ReadToEndAsync();
                        }

                        var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                        var ticket = new TicketBack
                        {
                            Numero = jsonResponse.ContainsKey("ticket") ? Convert.ToInt32(jsonResponse["ticket"]) : 0,
                            Fecha = jsonResponse.ContainsKey("fecha") ? jsonResponse["fecha"].ToString() : "",
                            Seguridad = jsonResponse.ContainsKey("seguridad") ? jsonResponse["seguridad"].ToString() : "",
                            Apuesta = requestData["apuesta"].Split(',').ToList(),
                            Evento = requestData["evento"].Split(',').ToList(),
                            Opciones = requestData["opcion"].Split(',').ToList(),
                            HoraSorteo = requestData["horaSorteo"],
                            Juego = requestData["juego"],
                            Hora = requestData["hora"],
                            TotalMonto = requestData["total"]
                        };
                        var printer = new VirtualesPrueba.TicketPrinterBack(
                            ticket,
                            Properties.Settings.Default.PaperWidth,
                            Properties.Settings.Default.SelectedPrinter,
                            Properties.Settings.Default.SelectedLogo
                        );

                        printer.PrintAutomatically();

                        requestData.Clear();
                    }
                }
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }

        private string ExtractRequestData(Stream content, string key)
        {
            try
            {
                if (content == null) return "";
                string urlEncodedString = new StreamReader(content).ReadToEnd();
                var keyValuePairs = HttpUtility.ParseQueryString(urlEncodedString);
                return keyValuePairs.AllKeys.Contains(key) ? keyValuePairs[key] : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void OnMouseMove(object sender, EventArgs e)
        {
            Point currentMousePosition = Cursor.Position;
            if (!currentMousePosition.Equals(lastMousePosition))
            {
                lastMousePosition = currentMousePosition;
                ResetInactivityTimer(sender, e);
            }
        }

        private List<string> ExtractRequestDataList(Stream content, string key)
        {
            try
            {
                if (content == null) return new List<string>();
                string urlEncodedString = new StreamReader(content).ReadToEnd();
                var keyValuePairs = HttpUtility.ParseQueryString(urlEncodedString);
                if (keyValuePairs.AllKeys.Contains(key + "[]"))
                {
                    return keyValuePairs.GetValues(key + "[]").ToList();
                }
                return new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private void backlot_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backlot_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
            }

            this.MouseMove -= OnMouseMove;
            this.KeyPress -= ResetInactivityTimer;

            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            }
        }
    }
}
