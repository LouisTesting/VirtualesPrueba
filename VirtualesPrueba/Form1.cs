using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace VirtualesPrueba
{
    public partial class Form1 : Form
    {
        private const string AppName = "RetailHorseApp";
        private bool closingFromNotify = false;
        private NotifyIcon notifyIcon;
        private static Mutex appMutex;
        private JObject mainObj = null;
        private float currentScaleFactor = 1.0f;

        private const int HOTKEY_ID = 100;
        private const int MOD_NOREPEAT = 0x4000;
        private const int VK_F4 = 0x73;
        private bool eventosAsignados = false;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Load += Form1_Load;

            //this.Deactivate += Form1_Deactivate;

        }


               private void AjustarFormulario()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            double baseWidth = 1585;
            double baseHeight = 785;

            double scaleFactor = Math.Min(screenWidth / baseWidth, screenHeight / baseHeight);

            int newWidth = (int)(baseWidth * scaleFactor);
            int newHeight = (int)(baseHeight * scaleFactor);

            if (newWidth > screenWidth)
                newWidth = screenWidth;
            if (newHeight > screenHeight)
                newHeight = screenHeight;

            int offsetFromBottom = 0;
            int extraHeight = 0;

            if (screenWidth <= 800)
            {
                offsetFromBottom = 70;
                extraHeight = 60;
            }
            else if (screenWidth <= 1024)
            {
                offsetFromBottom = 50;
                extraHeight = 130;
            }
            else if (screenWidth <= 1280)
            {
                if (screenHeight <= 720)
                {
                    offsetFromBottom = 5;
                    extraHeight = 10;
                }
                else if (screenHeight <= 800)
                {
                    offsetFromBottom = 20;
                    extraHeight = 70;
                }
                else if (screenHeight <= 1024)
                {
                    offsetFromBottom = 10;
                    extraHeight = 300;
                }
            }
            else if (screenWidth <= 1366)
            {
                offsetFromBottom = 10;
                extraHeight = 5;
            }
            else if (screenWidth <= 1440)
            {
                offsetFromBottom = 40;
                extraHeight = 60;
            }
            else if (screenWidth <= 1600)
            {
                offsetFromBottom = 30;
                extraHeight = 5;
            }
            else if (screenWidth <= 1680)
            {
                offsetFromBottom = 40;
                extraHeight = 100;
            }
            else if (screenWidth <= 1920)
            {
                offsetFromBottom = 0;
                extraHeight = 40;
            }
            else
            {
                offsetFromBottom = 0;
                extraHeight = 0;
            }

            newHeight += extraHeight;
            if (newHeight > screenHeight)
                newHeight = screenHeight;

            this.Width = newWidth;
            this.Height = newHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = screenHeight - this.Height - offsetFromBottom;

            Debug.WriteLine($"Formulario ajustado: {this.Width}x{this.Height} en ({this.Left}, {this.Top})");
        }




        private async void Form1_Load(object sender, EventArgs e)
        {

            AjustarFormulario();
           

            this.Icon = new Icon("logos/icono.ico");
           
            this.TopMost = false;
            this.ShowInTaskbar = false;

          
            string lastUser = string.IsNullOrEmpty(Properties.Settings.Default.LastUser) ? "Usuario" : Properties.Settings.Default.LastUser;
            this.Text = "Retail Horse v1.3.7 | " + lastUser;

            try
            {

                if (webView21.CoreWebView2 == null)
                {
                    Debug.WriteLine("Inicializando WebView2...");
                    await webView21.EnsureCoreWebView2Async();
                }

                if (webView21.CoreWebView2 != null)
                {
                    Debug.WriteLine("✅ WebView2 Inicializado correctamente");


                    string script2 = @"
                        setTimeout(() => {
                            console.log('⚡ Script inyectado en WebView2');
                            function enviarValorInput() {
                                let input = document.getElementById('LInputEmail1');
                                if (input) {
                                    console.log('📩 Enviando valor del input:', input.value);
                                    window.chrome.webview.postMessage(input.value);
                                }
                            }
                            enviarValorInput();
                            setInterval(enviarValorInput, 1000);
                        }, 2000);
                    ";

                    string script = @"
                        setTimeout(() => {
                            console.log('⚡ Script inyectado en WebView2');
                            window.addEventListener('message', function(e) {
                                console.log('📩 Mensaje recibido:', e);
                                let jsonData = (typeof e.data === 'object') ? JSON.stringify(e.data) : e.data;
                                console.log('✅ Enviando a C#:', jsonData);
                                window.chrome.webview.postMessage(jsonData);
                            });
                        }, 2000);
                    ";

                    await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script2);
                    await webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script);

                    // Asignar eventos solo una vez
                    if (!eventosAsignados)
                    {
                        webView21.WebMessageReceived += WebView21_WebMessageReceived;
                        webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
                        eventosAsignados = true;
                    }

                    // Verificar si ya hay una URL cargada antes de navegar
                    if (string.IsNullOrEmpty(webView21.Source?.AbsoluteUri) || webView21.Source.AbsoluteUri == "about:blank")
                    {
                        Debug.WriteLine("🌐 Navegando a: https://retailhorse.shop/");
                        webView21.CoreWebView2.Navigate("https://retailhorse.shop/");
                    }
                    else
                    {
                        Debug.WriteLine("🔄 WebView2 ya tiene una URL cargada: " + webView21.Source.AbsoluteUri);
                    }

                    RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NOREPEAT, VK_F4);
                }
                else
                {
                    MessageBox.Show("WebView2 no se pudo inicializar correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("❌ WebView2 no se pudo inicializar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando WebView2: " + ex.Message);
                Debug.WriteLine("❌ Error inicializando WebView2: " + ex.Message);
            }


        }

  

        private PrintDocument GetConfiguredPrintDocument()
        {
            PrintDocument pd = new PrintDocument();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedPrinter))
            {
                pd.PrinterSettings.PrinterName = Properties.Settings.Default.SelectedPrinter;
            }

            string paperWidthSetting = Properties.Settings.Default.PaperWidthRetail;
            int paperWidth = 300;
            float scaleFactor = 1.1f;
            if (paperWidthSetting == "58 mm")
            {
                paperWidth = 200;
                scaleFactor = 0.7f;
            }
            pd.DefaultPageSettings.PaperSize = new PaperSize("TicketPaper", paperWidth, 1000);
            currentScaleFactor = scaleFactor;
            return pd;
        }

        
        private void PrintTicketCobroRetiro(string imprData)
        {
            try
            {
                PrintDocument pd = GetConfiguredPrintDocument();
                pd.PrintPage += (sender, e) => PrintTicketCobroRetiroPage(sender, e, imprData);
                pd.DefaultPageSettings.Landscape = false;
                pd.Print();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir cobro/retiro: " + ex.Message);
            }
        }

        private async void PrintTicketRecarga(JObject ticketData)
        {
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += (sender, e) => PrintTicketRecargaPage(sender, e, ticketData);
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir ticket recarga: " + ex.Message);
            }
        }

        private async void PrintTicketDevuelto(JObject ticketData)
        {
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += (sender, e) => PrintTicketDevueltoPage(sender, e, ticketData);
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir ticket devuelto: " + ex.Message);
            }
        }

        private async void PrintTicketGanado(JObject ticketData)
        {
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += (sender, e) => PrintTicketGanadoPage(sender, e, ticketData);
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir ticket ganado: " + ex.Message);
            }
        }

        private async void PrintTicketAnulado(JObject ticketData)
        {
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += (sender, e) => PrintTicketAnuladoPage(sender, e, ticketData);
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir ticket anulado: " + ex.Message);
            }
        }

        private async Task PrintReport(JObject reportData)
        {
            try
            {
                if (!reportData.ContainsKey("texto"))
                {
                    Debug.WriteLine("No se encontró 'texto' en el JSON.");
                    return;
                }
                JObject textoObj = reportData["texto"].ToObject<JObject>();
                Debug.WriteLine("Contenido de 'texto': " + textoObj.ToString());

                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += (sender, e) => PrintReportPage(sender, e, textoObj);
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                    // En C# no se remueve el handler anónimo.
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir reporte: " + ex.Message);
            }
        }

        private async void PrintTicket()
        {
            try
            {
                await Task.Run(() =>
                {
                    PrintDocument pd = GetConfiguredPrintDocument();
                    pd.PrintPage += PrintDocument_PrintPage;
                    pd.DefaultPageSettings.Landscape = false;
                    pd.Print();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }
        }

        private void PrintTicketCobroRetiroPage(object sender, PrintPageEventArgs e, string imprData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            // Fuentes escaladas
            Font titleFont = new Font("Arial", 18 * scale, FontStyle.Bold);
            Font boldFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 12 * scale);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posY = 30 * scale;
            float lineHeight = 30 * scale;

            // Campos a extraer
            string usuario = "";
            string pin = "";
            string fecha = "";
            string hora = "";
            string monto = "";

            // Dividir imprData por <br>
            string[] lineas = imprData.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ln in lineas)
            {
                string lineaLimpia = QuitarCodigosCortos(ln).Trim();

                Debug.WriteLine("Línea bruta: [" + ln + "]");
                Debug.WriteLine("Línea limpia: [" + lineaLimpia + "]");

                if (lineaLimpia.Contains("USUARIO:"))
                {
                    usuario = lineaLimpia.Replace("USUARIO:", "").Trim();
                }
                else if (lineaLimpia.Contains("PIN"))
                {
                    string tempPin = lineaLimpia;
                    tempPin = tempPin.Replace("Nï¿½", "").Replace("Nº", "");
                    tempPin = tempPin.Replace(":", "");
                    pin = tempPin.Trim();
                }
                else if (lineaLimpia.Contains("FECHA:"))
                {
                    fecha = lineaLimpia.Replace("FECHA:", "").Trim();
                }
                else if (lineaLimpia.Contains("Hora"))
                {
                    hora = lineaLimpia.Replace("Hora:", "").Trim();
                }
                else if (lineaLimpia.Contains("Monto:"))
                {
                    monto = lineaLimpia.Replace("Monto:", "").Trim();
                }
            }

            Debug.WriteLine("Usuario detectado: " + usuario);
            Debug.WriteLine("PIN detectado: " + pin);
            Debug.WriteLine("Fecha detectada: " + fecha);
            Debug.WriteLine("Hora detectada: " + hora);
            Debug.WriteLine("Monto detectada: " + monto);

            // Logo opcional
            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(190 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (10 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            // Título
            string ticketTitle = "Pago de Retiro";
            SizeF ticketSize = g.MeasureString(ticketTitle, titleFont);
            float ticketX = (pageWidth - ticketSize.Width) / 2;
            g.DrawString(ticketTitle, titleFont, brush, ticketX, posY);
            posY += lineHeight * 2;

            if (!string.IsNullOrEmpty(usuario))
            {
                g.DrawString("Usuario: " + usuario, normalFont, brush, marginLeft, posY);
                posY += lineHeight;
            }
            if (!string.IsNullOrEmpty(pin))
            {
                string pinTxt = pin;
                SizeF pinSize = g.MeasureString(pinTxt, boldFont);
                float pinX = (pageWidth - pinSize.Width) / 2;
                g.DrawString(pinTxt, boldFont, brush, pinX, posY);
                posY += lineHeight;
            }
            if (!string.IsNullOrEmpty(fecha))
            {
                g.DrawString("Fecha: " + fecha, normalFont, brush, marginLeft, posY);
                posY += lineHeight;
            }
            if (!string.IsNullOrEmpty(hora))
            {
                g.DrawString("Hora: " + hora, normalFont, brush, marginLeft, posY);
                posY += lineHeight;
            }
            if (!string.IsNullOrEmpty(monto))
            {
                string montoTxt = "Monto: " + monto;
                SizeF montoSize = g.MeasureString(montoTxt, boldFont);
                float montoX = (pageWidth - montoSize.Width) / 2;
                g.DrawString(montoTxt, boldFont, brush, montoX, posY);
                posY += lineHeight;
            }
            if (!string.IsNullOrEmpty(pin))
            {
                string pinNumber = pin.Replace("PIN", "").Trim();
                Bitmap pinBarcode = GenerateBarcodeBitmap(pinNumber, (int)(200 * scale), (int)(50 * scale));
                if (pinBarcode != null)
                {
                    float barcodeX = (pageWidth - pinBarcode.Width) / 2;
                    g.DrawImage(pinBarcode, barcodeX, posY);
                    posY += pinBarcode.Height + (5 * scale);
                }
            }
        }

        private void PrintTicketRecargaPage(object sender, PrintPageEventArgs e, JObject ticketData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 18 * scale, FontStyle.Bold);
            Font subtitleFont = new Font("Arial", 16 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 12 * scale);
            Font boldFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            string serialRef = mainObj?["codigo"]?.ToString();
            float posY = 30 * scale;
            float lineHeight = 30 * scale;

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(190 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (10 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            string ticketTitle = "Ticket Recarga";
            SizeF ticketSize = g.MeasureString(ticketTitle, titleFont);
            float ticketX = (pageWidth - ticketSize.Width) / 2;
            g.DrawString(ticketTitle, titleFont, brush, ticketX, posY);
            posY += lineHeight * 2;

            string serial = "PIN: " + ticketData["codigo"]?.ToString();
            SizeF serialSize = g.MeasureString(serial, boldFont);
            string horaPago = DateTime.Now.ToString("HH:mm:ss");
            string horaPagoTexto = "Hora: " + horaPago;
            g.DrawString(horaPagoTexto, boldFont, brush, (pageWidth - serialSize.Width) / 2, posY);
            posY += lineHeight;
            g.DrawString(serial, boldFont, brush, (pageWidth - serialSize.Width) / 2, posY);
            posY += lineHeight;

            Bitmap impSerialBarcode = null;
            if (!string.IsNullOrEmpty(serialRef))
            {
                impSerialBarcode = GenerateBarcodeBitmap(serialRef, (int)(200 * scale), (int)(50 * scale));
            }
            if (impSerialBarcode != null)
            {
                float impX = marginLeft + (usableWidth - impSerialBarcode.Width) / 2 - 15;
                g.DrawImage(impSerialBarcode, impX, posY);
                posY += impSerialBarcode.Height + (5 * scale);
            }
            posY += lineHeight;
            posY += lineHeight;

            // Mensaje final en forma de cono centrado
            string mensajeLinea1 = "Conserve su ticket (PIN),";
            string mensajeLinea2 = "le servirá para acreditar su saldo a la terminal";
            Font normalFont2 = new Font("Arial", 9 * scale);
            SizeF mensaje1Size = g.MeasureString(mensajeLinea1, normalFont2);
            SizeF mensaje2Size = g.MeasureString(mensajeLinea2, normalFont2);
            float mensaje1X = (pageWidth - mensaje1Size.Width) / 2;
            float mensaje2X = (pageWidth - mensaje2Size.Width) / 2;
            g.DrawString(mensajeLinea1, normalFont2, brush, mensaje1X, posY);
            posY += lineHeight;
            g.DrawString(mensajeLinea2, normalFont2, brush, mensaje2X, posY);
            posY += lineHeight;
        }

        private void PrintTicketGanadoPage(object sender, PrintPageEventArgs e, JObject ticketData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 18 * scale, FontStyle.Bold);
            Font subtitleFont = new Font("Arial", 16 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 12 * scale);
            Font boldFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posY = 30 * scale;
            float lineHeight = 30 * scale;

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(150 * scale);
                    int logoHeight = (int)(50 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (10 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            string ticketTitle = "¡Ticket Premiado!";
            SizeF ticketSize = g.MeasureString(ticketTitle, titleFont);
            float ticketX = (pageWidth - ticketSize.Width) / 2;
            g.DrawString(ticketTitle, titleFont, brush, ticketX, posY);
            posY += lineHeight * 2;

            string serial = "Serial N° " + ticketData["serial"]?.ToString();
            SizeF serialSize = g.MeasureString(serial, boldFont);
            g.DrawString(serial, boldFont, brush, (pageWidth - serialSize.Width) / 2, posY);
            posY += lineHeight;

            string horaEpoch = ticketData["hora"]?.ToString();
            Dictionary<string, string> fechaHora = ConvertirHoraEpochAFechaToAnulado(horaEpoch);
            string fechaTexto = "Fecha: " + fechaHora["fecha"];
            SizeF fechaSize = g.MeasureString(fechaTexto, normalFont);
            g.DrawString(fechaTexto, normalFont, brush, (pageWidth - fechaSize.Width) / 2, posY);
            posY += lineHeight;

            string horaTexto = "Hora: " + fechaHora["hora"];
            SizeF horaSize = g.MeasureString(horaTexto, normalFont);
            g.DrawString(horaTexto, normalFont, brush, (pageWidth - horaSize.Width) / 2, posY);
            posY += lineHeight * 2;

            RectangleF rectPagado = new RectangleF(marginLeft, posY, 100 * scale, 25 * scale);
            g.FillRectangle(new SolidBrush(Color.Black), rectPagado);
            SizeF textPagadoSize = g.MeasureString("PAGADO", boldFont);
            float textPagadoX = marginLeft + ((rectPagado.Width - textPagadoSize.Width) / 2);
            float textPagadoY = posY + ((rectPagado.Height - textPagadoSize.Height) / 2);
            g.DrawString("PAGADO", boldFont, new SolidBrush(Color.White), textPagadoX, textPagadoY);

            string monto = ticketData["total"]?.ToString() + " " + ticketData["simbolo"]?.ToString();
            SizeF montoSize = g.MeasureString(monto, boldFont);
            float montoX = pageWidth - montoSize.Width - marginRight;
            g.DrawString(monto, boldFont, brush, montoX, posY);
            posY += lineHeight * 2;

            string horaPago = DateTime.Now.ToString("HH:mm:ss");
            string horaPagoTexto = "Hora: " + horaPago;
            g.DrawString(horaPagoTexto, normalFont, brush, marginLeft, posY);
        }

        private void PrintTicketDevueltoPage(object sender, PrintPageEventArgs e, JObject ticketData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 18 * scale, FontStyle.Bold);
            Font subtitleFont = new Font("Arial", 16 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 12 * scale);
            Font boldFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posY = 30 * scale;
            float lineHeight = 30 * scale;

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(190 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (10 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            string ticketTitle = "Ticket Devuelto";
            SizeF ticketSize = g.MeasureString(ticketTitle, subtitleFont);
            float ticketX = (pageWidth - ticketSize.Width) / 2;
            g.DrawString(ticketTitle, subtitleFont, brush, ticketX, posY);
            posY += lineHeight;
            string serial = "Serial N°: " + ticketData["serial"]?.ToString();
            SizeF serialSize = g.MeasureString(serial, boldFont);
            float serialX = (pageWidth - serialSize.Width) / 2;
            g.DrawString(serial, boldFont, brush, serialX, posY);
            posY += lineHeight * 2;

            string monto = ticketData["total"]?.ToString() + " " + ticketData["simbolo"]?.ToString();
            g.DrawString("Monto:", boldFont, brush, marginLeft, posY);
            g.DrawString(monto, boldFont, brush, marginLeft + (150 * scale), posY);
            posY += lineHeight * 2;

            string horaEpoch = ticketData["hora"]?.ToString();
            Dictionary<string, string> fechaHora = ConvertirHoraEpochAFechaToAnulado(horaEpoch);
            g.DrawString("Fecha:", boldFont, brush, marginLeft, posY);
            g.DrawString(fechaHora["fecha"], normalFont, brush, marginLeft + (55 * scale), posY);
            g.DrawString("Hora:", boldFont, brush, marginLeft + (150 * scale), posY);
            g.DrawString(fechaHora["hora"], normalFont, brush, marginLeft + (200 * scale), posY);
        }

        private void PrintTicketAnuladoPage(object sender, PrintPageEventArgs e, JObject ticketData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 16 * scale, FontStyle.Bold);
            Font subtitleFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10 * scale);
            Font boldFont = new Font("Arial", 12 * scale, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posY = 30 * scale;
            float lineHeight = 30 * scale;

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(200 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (5 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            string ticketTitle = "Ticket Anulado";
            SizeF ticketSize = g.MeasureString(ticketTitle, subtitleFont);
            float ticketX = (pageWidth - ticketSize.Width) / 2;
            g.DrawString(ticketTitle, subtitleFont, brush, ticketX, posY);
            posY += lineHeight;
            string serial = "Serial N°: " + ticketData["serial"]?.ToString();
            SizeF serialSize = g.MeasureString(serial, boldFont);
            float serialX = (pageWidth - serialSize.Width) / 2;
            g.DrawString(serial, boldFont, brush, serialX, posY);
            posY += lineHeight * 2;

            string monto = ticketData["total"]?.ToString() + " " + ticketData["simbolo"]?.ToString();
            g.DrawString("Monto:", boldFont, brush, marginLeft, posY);
            g.DrawString(monto, boldFont, brush, marginLeft + (150 * scale), posY);
            posY += lineHeight * 2;

            string horaEpoch = ticketData["hora"]?.ToString();
            Dictionary<string, string> fechaHora = ConvertirHoraEpochAFechaToAnulado(horaEpoch);
            g.DrawString("Fecha:", boldFont, brush, marginLeft, posY);
            g.DrawString(fechaHora["fecha"], normalFont, brush, marginLeft + (55 * scale), posY);
            g.DrawString("Hora:", boldFont, brush, marginLeft + (150 * scale), posY);
            g.DrawString(fechaHora["hora"], normalFont, brush, marginLeft + (200 * scale), posY);
            posY += lineHeight;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (mainObj == null)
                return;

            float scale = currentScaleFactor; // 1.0 para 80 mm; 0.8 para 58 mm
            Graphics g = e.Graphics;
            float marginLeft = 10 * scale;
            float marginRight = 10 * scale;
            float usableWidth = e.PageBounds.Width - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 12 * scale, FontStyle.Bold);
            Font subTitleFont = new Font("Arial", 10 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 9 * scale);
            Font boldFont = new Font("Arial", 9 * scale, FontStyle.Bold);
            Font boldJugada = new Font("Arial", 7 * scale);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posX = marginLeft;
            float posY = 10 * scale;
            float lineHeight = 20 * scale;

            string companyName = mainObj["nom_us"]?.ToString();
            string serialRef = mainObj["serial"]?.ToString();
            string impSerial = mainObj["valida"]?.ToString();
            string totalStr = mainObj["total"]?.ToString();
            string moneda = mainObj["moneda"]?.ToString();
            string finalMessage = mainObj["notePrint"]?.ToString();
            JArray tckArray = mainObj["tck"] as JArray;
            string hora = mainObj["hora"]?.ToString();
            string horaFormateada = ConvertirHoraEpochAFecha(hora);
            string fecha = mainObj["fecha"]?.ToString();
            string numero = mainObj["numero"]?.ToString();

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(270 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (e.PageBounds.Width - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX - 15, posY, logoWidth, logoHeight);
                    posY += logoHeight + (5 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            Bitmap impSerialBarcode = null;
            if (!string.IsNullOrEmpty(serialRef))
            {
                impSerialBarcode = GenerateBarcodeBitmap(serialRef, (int)(150 * scale), (int)(30 * scale));
            }

            Bitmap serialBarcode = null;
            if (!string.IsNullOrEmpty(impSerial))
            {
                serialBarcode = GenerateBarcodeBitmap(impSerial, (int)(200 * scale), (int)(40 * scale));
            }

            List<string> tracks = new List<string>();
            Dictionary<string, Dictionary<string, List<JObject>>> dataByTrack = new Dictionary<string, Dictionary<string, List<JObject>>>();

            if (tckArray != null)
            {
                foreach (JToken betToken in tckArray)
                {
                    JObject betObj = betToken as JObject;
                    if (betObj != null)
                    {
                        string pista = betObj["pista"]?.ToString();
                        if (string.IsNullOrEmpty(pista))
                        {
                            pista = "HIPÓDROMO DESCONOCIDO";
                        }
                        string carrera = betObj["carrera"]?.ToString();
                        if (string.IsNullOrEmpty(carrera))
                        {
                            carrera = "Carrera Desconocida";
                        }
                        if (!dataByTrack.ContainsKey(pista))
                        {
                            dataByTrack[pista] = new Dictionary<string, List<JObject>>();
                            tracks.Add(pista);
                        }
                        if (!dataByTrack[pista].ContainsKey(carrera))
                        {
                            dataByTrack[pista][carrera] = new List<JObject>();
                        }
                        dataByTrack[pista][carrera].Add(betObj);
                    }
                }
            }

            if (impSerialBarcode != null)
            {
                float impX = marginLeft + (usableWidth - impSerialBarcode.Width) / 2 - 15;
                g.DrawImage(impSerialBarcode, impX, posY);
                posY += impSerialBarcode.Height + (5 * scale);
            }

            string titleText = string.IsNullOrEmpty(companyName) ? "HORSE RACING" : companyName;
            SizeF titleSize = g.MeasureString(titleText, titleFont);
            float titleX = marginLeft + (usableWidth - titleSize.Width) / 2;
            posY += lineHeight + (5 * scale);

            g.DrawString("Serial/ referencia: ", boldFont, brush, posX, posY);
            g.DrawString(string.IsNullOrEmpty(serialRef) ? "N/A" : serialRef, normalFont, brush, posX + (110 * scale), posY);
            posY += (lineHeight - (5 * scale));

            g.DrawString("Usuario: ", boldFont, brush, posX, posY);
            g.DrawString(string.IsNullOrEmpty(companyName) ? "N/A" : companyName, normalFont, brush, posX + (50 * scale), posY);
            posY += (lineHeight - (5 * scale));

            g.DrawString("Fecha / Hora: ", boldFont, brush, posX, posY);
            g.DrawString(string.IsNullOrEmpty(horaFormateada) ? "N/A" : horaFormateada, normalFont, brush, posX + (85 * scale), posY);
            posY += (lineHeight - (5 * scale));

            g.DrawLine(Pens.Black, marginLeft, posY, e.PageBounds.Width - marginRight, posY);
            posY += lineHeight;

            foreach (string pistaName in tracks)
            {
                g.DrawString(pistaName, subTitleFont, brush, posX, posY);
                posY += lineHeight - (5 * scale);
                g.DrawLine(Pens.Black, posX, posY, e.PageBounds.Width - marginRight, posY);
                posY += lineHeight - (5 * scale);

                Dictionary<string, List<JObject>> carrerasDict = dataByTrack[pistaName];
                foreach (string carreraKey in carrerasDict.Keys)
                {
                    g.DrawString("Carrera: " + carreraKey, normalFont, brush, posX, posY);
                    posY += lineHeight - (5 * scale);

                    float colX1 = posX;
                    float colX2 = posX + (150 * scale);
                    float colX3 = posX + (220 * scale);

                    g.DrawString("N° Caballo", boldFont, brush, colX1, posY);
                    g.DrawString("Jugada", boldFont, brush, colX2, posY);
                    g.DrawString("Monto", boldFont, brush, colX3, posY);
                    posY += lineHeight;

                    List<JObject> listApuestas = carrerasDict[carreraKey];
                    foreach (JObject betObj in listApuestas)
                    {
                        string caballos = betObj["caballos"]?.ToString();
                        string tipoApuesta = betObj["apuesta"]?.ToString();
                        string montoStr = betObj["monto"]?.ToString();

                        g.DrawString(caballos, normalFont, brush, colX1, posY);
                        g.DrawString(tipoApuesta, boldJugada, brush, colX2, posY);
                        g.DrawString(montoStr, normalFont, brush, colX3, posY);
                        posY += lineHeight + (2 * scale);
                    }
                    posY += 10 * scale;
                }
            }

            g.DrawLine(Pens.Black, marginLeft, posY, e.PageBounds.Width - marginRight, posY);
            posY += lineHeight - (5 * scale);

            if (!string.IsNullOrEmpty(totalStr))
            {
                string totalFinal = totalStr;
                if (!string.IsNullOrEmpty(moneda))
                {
                    totalFinal += " " + moneda;
                }
                string totalTxt = "TOTAL " + totalFinal;
                SizeF totalSize = g.MeasureString(totalTxt, titleFont);
                float totalX = marginLeft + (usableWidth - totalSize.Width) / 2;
                g.DrawString(totalTxt, titleFont, brush, totalX, posY);
                posY += lineHeight + (5 * scale);
            }

            g.DrawLine(Pens.Black, marginLeft, posY, e.PageBounds.Width - marginRight, posY);
            posY += lineHeight;

            Font normalFont2 = new Font("Arial", 9 * scale);
            float lineHeight2 = 20 * scale;
            float centerShift = 0;
            string msgOffice = finalMessage;
            float maxLineWidth = usableWidth - marginLeft - 10 * scale;

            List<string> lines = new List<string>();
            string[] words = msgOffice.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                SizeF testSize = g.MeasureString(testLine, normalFont2);
                if (testSize.Width > maxLineWidth)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            float posY2 = posY;
            for (int i = 0; i < lines.Count; i++)
            {
                SizeF msgSize2 = g.MeasureString(lines[i], normalFont2);
                float msgX2 = marginLeft + (usableWidth - msgSize2.Width + centerShift) / 2 - 15;
                g.DrawString(lines[i], normalFont2, brush, msgX2, posY2);
                posY2 += lineHeight2;
                centerShift += 5;
            }
            posY += lineHeight + 60;

            if (serialBarcode != null)
            {
                float serialX = marginLeft + (usableWidth - serialBarcode.Width) / 2 - 15;
                g.DrawImage(serialBarcode, serialX - (5 * scale), posY);
                posY += serialBarcode.Height + (5 * scale);

                SizeF serialTextSize = g.MeasureString(serialRef, normalFont);
                float serialTextX = marginLeft + (usableWidth - serialTextSize.Width) / 2 - 15;
                g.DrawString(impSerial, normalFont, brush, serialTextX - (5 * scale), posY);
                posY += serialTextSize.Height + (5 * scale);
            }
        }

        private void PrintReportPage(object sender, PrintPageEventArgs e, JObject reportData)
        {
            Graphics g = e.Graphics;
            float scale = currentScaleFactor;
            float pageWidth = e.PageBounds.Width;
            float marginLeft = 20 * scale;
            float marginRight = 20 * scale;
            float usableWidth = pageWidth - marginLeft - marginRight;

            Font titleFont = new Font("Arial", 16 * scale, FontStyle.Bold);
            Font subtitleFont = new Font("Arial", 14 * scale, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10 * scale);
            Font boldFont = new Font("Arial", 10 * scale, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            float posY = 30 * scale;
            float lineHeight = 25 * scale;

            string logoPath = Properties.Settings.Default.LogoPathUniversal;
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    Image logoImg = Image.FromFile(logoPath);
                    int logoWidth = (int)(200 * scale);
                    int logoHeight = (int)(70 * scale);
                    float logoX = (pageWidth - logoWidth) / 2;
                    g.DrawImage(logoImg, logoX, posY, logoWidth, logoHeight);
                    posY += logoHeight + (5 * scale);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("⚠ Error al cargar el logo: " + ex.Message);
                }
            }

            string cuadre = "Cuadre de Caja";
            SizeF cuadreSize = g.MeasureString(cuadre, subtitleFont);
            float cuadreX = (pageWidth - cuadreSize.Width) / 2;
            g.DrawString(cuadre, subtitleFont, brush, cuadreX, posY);
            posY += lineHeight;

            string usuario = "Usuario: " + reportData["usuario"]?.ToString();
            SizeF usuarioSize = g.MeasureString(usuario, boldFont);
            float usuarioX = (pageWidth - usuarioSize.Width) / 2;
            g.DrawString(usuario, boldFont, brush, usuarioX, posY);
            posY += lineHeight * 2;

            string desde = reportData["desde"]?.ToString();
            string hasta = reportData["hasta"]?.ToString();
            g.DrawString("Desde:", boldFont, brush, marginLeft, posY);
            g.DrawString("Hasta:", boldFont, brush, marginLeft + (130 * scale), posY);
            posY += lineHeight;
            g.DrawString(desde, normalFont, brush, marginLeft, posY);
            g.DrawString(hasta, normalFont, brush, marginLeft + (130 * scale), posY);
            posY += lineHeight * 2;

            string resultados = "Resultados";
            SizeF resultadosSize = g.MeasureString(resultados, subtitleFont);
            float resultadosX = (pageWidth - resultadosSize.Width) / 2;
            g.DrawString(resultados, subtitleFont, brush, resultadosX, posY);
            posY += lineHeight;

            float col1X = marginLeft;
            float col2X = col1X + (200 * scale);
            Dictionary<string, string> datos = new Dictionary<string, string>()
            {
                {"Cantidad de tickets:", reportData["cant_tck"]?.ToString()},
                {"Ventas:", reportData["venta"]?.ToString()},
                {"Devueltos:", reportData["devolucion"]?.ToString()},
                {"Anulados:", reportData["nulo"]?.ToString()},
                {"Ventas Netas:", reportData["venta_neta"]?.ToString()},
                {"Pagos:", reportData["pagado"]?.ToString()},
                {"Saldo:", reportData["saldo"]?.ToString()}
            };

            foreach (var dato in datos)
            {
                g.DrawString(dato.Key, boldFont, brush, col1X, posY);
                g.DrawString(dato.Value, normalFont, brush, col2X, posY);
                posY += lineHeight;
            }

            posY += lineHeight;

            string horaStr = reportData["hora"]?.ToString();
            g.DrawString("Fecha:", boldFont, brush, marginLeft, posY);
            g.DrawString(desde, normalFont, brush, marginLeft + (50 * scale), posY);
            g.DrawString("Hora:", boldFont, brush, marginLeft + (130 * scale), posY);
            g.DrawString(horaStr, normalFont, brush, marginLeft + (180 * scale), posY);
        }

       

        private string QuitarCodigosCortos(string linea)
        {
            string patron = "#[^#]{1,3}#";
            string resultado = Regex.Replace(linea, patron, "");
            resultado = resultado.Replace("#", "").Trim();
            return resultado;
        }

        private Dictionary<string, string> ConvertirHoraEpochAFechaToAnulado(string horaEpoch)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>
            {
                { "fecha", "N/A" },
                { "hora", "N/A" }
            };

            if (string.IsNullOrEmpty(horaEpoch))
                return resultado;

            if (long.TryParse(horaEpoch, out long epoch))
            {
                DateTime dt = DateTimeOffset.FromUnixTimeSeconds(epoch).LocalDateTime;
                resultado["fecha"] = dt.ToString("dd/MM/yyyy");
                resultado["hora"] = dt.ToString("HH:mm:ss");
            }
            return resultado;
        }

        private string ConvertirHoraEpochAFecha(string horaEpoch)
        {
            if (string.IsNullOrEmpty(horaEpoch))
                return "N/A";

            if (long.TryParse(horaEpoch, out long epoch))
            {
                DateTime dt = DateTimeOffset.FromUnixTimeSeconds(epoch).LocalDateTime;
                return dt.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
                return "N/A";
        }

        private string StripOuterQuotes(string input)
        {
            if (input.Length >= 2 && input.StartsWith("\"") && input.EndsWith("\""))
            {
                input = input.Substring(1, input.Length - 2);
            }
            return input;
        }

        private JObject TryParseObject(string jsonString)
        {
            try
            {
                var token = JToken.Parse(jsonString);
                if (token.Type == JTokenType.Object)
                {
                    return (JObject)token;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        private Bitmap GenerateBarcodeBitmap(string data, int width, int height)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(data);
            Bitmap bmp = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                              ImageLockMode.WriteOnly,
                                              PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

     

        private void WebView21_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string rawData = e.WebMessageAsJson;
                if (string.IsNullOrEmpty(rawData))
                    return;

                rawData = StripOuterQuotes(rawData);
                string cleaned = Regex.Unescape(rawData);

                if (!cleaned.Contains("{") && !cleaned.Contains(":"))
                {
                    string newTitle = "Retail Horse v1.3.7 | " + cleaned;
                    this.Text = newTitle;
                    Properties.Settings.Default.LastUser = cleaned;
                    Properties.Settings.Default.Save();
                    return;
                }

                mainObj = TryParseObject(cleaned);
                if (mainObj == null)
                {
                    MessageBox.Show("Error: JSON inválido después de TryParseObject.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (mainObj.ContainsKey("cobrotck"))
                {
                    string imprData = mainObj["impr"]?.ToString();
                    PrintTicketCobroRetiro(imprData);
                    return;
                }

                if (mainObj.ContainsKey("rechargetck"))
                {
                    string asJsonString = JsonConvert.SerializeObject(mainObj);
                    JObject jObj = JObject.Parse(asJsonString);
                    PrintTicketRecarga(jObj);
                    return;
                }

                if (mainObj.ContainsKey("texto"))
                {
                    string asJsonString = JsonConvert.SerializeObject(mainObj);
                    JObject jObj = JObject.Parse(asJsonString);
                    PrintReport(jObj);
                    return;
                }

                if (mainObj.ContainsKey("sendStatus"))
                {
                    string tipoEstatus = mainObj["sendStatus"]?.ToString();
                    Debug.WriteLine("sendStatus encontrado: " + tipoEstatus);
                    if (tipoEstatus == "A")
                    {
                        PrintTicketAnulado(mainObj);
                        return;
                    }
                    if (tipoEstatus == "D")
                    {
                        PrintTicketDevuelto(mainObj);
                        return;
                    }
                    if (tipoEstatus == "P")
                    {
                        PrintTicketGanado(mainObj);
                        return;
                    }
                }
                else
                {
                    Debug.WriteLine("sendStatus no encontrado en JSON");
                }

                PrintTicket();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error procesando mensaje: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ShowSettingsForm()
        {
           
            Form existingForm = Application.OpenForms.OfType<SettingsUniversal>().FirstOrDefault();
            if (existingForm == null)
            {
                SettingsUniversal configForm = new SettingsUniversal();
                configForm.ShowDialog();
            }
            else
            {
                if (existingForm.WindowState == FormWindowState.Minimized)
                {
                    existingForm.WindowState = FormWindowState.Normal;
                }
                existingForm.BringToFront();
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    ShowSettingsForm();
                }
            }
            base.WndProc(ref m);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            base.OnFormClosing(e);
        }

    }
}
