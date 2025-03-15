using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualesPrueba;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;

namespace VirtualesPrueba
{
    public class TicketPrinterBack
    {
        private TicketBack _ticket;
        private PrinterSettings _printerSettings;
        private int _paperWidth;
        private string _logoPath;
        
        public TicketPrinterBack(TicketBack ticket, int paperWidth = 80, string printerName = "", string logoPath = "")
        {
            _ticket = ticket;
            _paperWidth = paperWidth;
            _printerSettings = new PrinterSettings();

            // Ruta por defecto del logo: <CarpetaEjecutable>/Logos/InmejorableTicket.png
            string defaultLogoPath = Path.Combine(Application.StartupPath, "Logos", "InmejorableTicket.png");
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                _logoPath = logoPath;
            }
            else if (File.Exists(defaultLogoPath))
            {
                _logoPath = defaultLogoPath;
            }
            else
            {
                _logoPath = null;
            }

            // Reemplaza My.Settings.SelectedPrinter por Properties.Settings.Default.SelectedPrinter
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedPrinter) &&
                PrinterSettings.InstalledPrinters.Cast<string>().Contains(Properties.Settings.Default.SelectedPrinter))
            {
                _printerSettings.PrinterName = Properties.Settings.Default.SelectedPrinter;
            }
            else
            {
                // Usar la impresora predeterminada
                _printerSettings.PrinterName = (new PrinterSettings()).PrinterName;
                Debug.WriteLine("Usando impresora predeterminada del sistema.");
            }
        }

        public void PrintAutomatically()
        {
            try
            {
                if (PrinterSettings.InstalledPrinters.Count == 0)
                    throw new Exception("No hay impresoras instaladas.");

                if (!_printerSettings.IsValid)
                    throw new Exception("La impresora seleccionada no es válida.");

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings = _printerSettings;

                
                int paperWidthHundredths = (int)(_paperWidth / 25.4 * 100);
                pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", paperWidthHundredths, 3000);
                pd.DefaultPageSettings.Margins = new Margins(10, 10, 5, 5);

                
                pd.PrintController = new StandardPrintController();

                pd.PrintPage += PrintPageHandler;
                pd.Print();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al imprimir el ticket: {ex.Message}");
            }
        }


        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 10;
                int marginLeft = 10;
                int pageWidth = e.PageBounds.Width - marginLeft * 2;

                // Ajustar tamaño de fuente según ancho del papel (58 o 80 mm)
                Font fontNormal = new Font("Consolas", (_paperWidth == 58) ? 6 : 8, FontStyle.Regular);
                Font fontBold = new Font("Consolas", (_paperWidth == 58) ? 6 : 8, FontStyle.Bold);

                int logoWidth = (_paperWidth == 58) ? 140 : 220;
                int logoHeight = (_paperWidth == 58) ? 55 : 60;

                // Comprobamos si se imprime el logo
                // Reemplaza My.Settings.ImprimirLogo por Properties.Settings.Default.ImprimirLogo
                if (Properties.Settings.Default.ImprimirLogo &&
                    !string.IsNullOrEmpty(_logoPath) &&
                    File.Exists(_logoPath))
                {
                    using (Image logo = Image.FromFile(_logoPath))
                    {
                        float logoX = (pageWidth - logoWidth) / 2f;
                        g.DrawImage(logo, logoX, y, logoWidth, logoHeight);
                        y += logoHeight + 10;
                    }
                }

                // Encabezado
                g.DrawString("Ticket:", fontNormal, Brushes.Black, marginLeft, y);
                g.DrawString(SanitizeText(_ticket.Numero.ToString()), fontNormal, Brushes.Black, pageWidth - 150, y);
                y += 15;

                g.DrawString("Fecha:", fontNormal, Brushes.Black, marginLeft, y);
                g.DrawString(SanitizeText(_ticket.Fecha), fontNormal, Brushes.Black, pageWidth - 150, y);
                y += 15;

                g.DrawString("Hora:", fontNormal, Brushes.Black, marginLeft, y);
                g.DrawString(SanitizeText(_ticket.Hora), fontNormal, Brushes.Black, pageWidth - 150, y);
                y += 15;

                g.DrawString("HoraSorteo:", fontNormal, Brushes.Black, marginLeft, y);
                g.DrawString(SanitizeText(_ticket.HoraSorteo), fontNormal, Brushes.Black, pageWidth - 150, y);
                y += 15;

                g.DrawString(new string('-', 40), fontBold, Brushes.Black, marginLeft, y);
                y += 15;

                // Cabeceras de tabla
                g.DrawString("Evento", fontBold, Brushes.Black, marginLeft, y);
                g.DrawString("Apuesta", fontBold, Brushes.Black, marginLeft + pageWidth / 3, y);
                g.DrawString("Monto", fontBold, Brushes.Black, marginLeft + (pageWidth * 2) / 3, y);
                y += 15;

                // Mostrar filas
                int maxRows = Math.Max(Math.Max(_ticket.Evento.Count, _ticket.Apuesta.Count), _ticket.Opciones.Count);
                for (int i = 0; i < maxRows; i++)
                {
                    string evento = (i < _ticket.Evento.Count) ? _ticket.Evento[i] : "";
                    string apuesta = (i < _ticket.Apuesta.Count) ? _ticket.Apuesta[i] : "";
                    string monto = (i < _ticket.Opciones.Count) ? _ticket.Opciones[i] : "";

                    g.DrawString(evento, fontNormal, Brushes.Black, marginLeft, y);
                    g.DrawString(monto, fontNormal, Brushes.Black, marginLeft + pageWidth / 3, y);
                    g.DrawString(apuesta, fontNormal, Brushes.Black, marginLeft + (pageWidth * 2) / 3, y);
                    y += 15;
                }

                g.DrawString(new string('-', 40), fontBold, Brushes.Black, marginLeft, y);
                y += 15;

                // Total
                string totalText = $"Total: {_ticket.TotalMonto} BS.D";
                SizeF totalTextSize = g.MeasureString(totalText, fontBold);
                float totalTextX = marginLeft + 40 + (pageWidth - totalTextSize.Width) / 2f;
                g.DrawString(totalText, fontBold, Brushes.Black, totalTextX, y);
                y += 15;

                // Código de barras (seguridad)
                if (!string.IsNullOrEmpty(_ticket.Seguridad))
                {
                    var writer = new ZXing.OneD.Code128Writer();
                    BitMatrix matrix = writer.encode(
                        _ticket.Seguridad,
                        BarcodeFormat.CODE_128,
                        (_paperWidth == 58) ? 150 : 200,
                        (_paperWidth == 58) ? 40 : 50
                    );

                    using (Bitmap barcodeBitmap = new Bitmap(matrix.Width, matrix.Height))
                    {
                        for (int yPos = 0; yPos < matrix.Height; yPos++)
                        {
                            for (int x = 0; x < matrix.Width; x++)
                            {
                                barcodeBitmap.SetPixel(x, yPos, matrix[x, yPos] ? Color.Black : Color.White);
                            }
                        }

                        if (_paperWidth == 58)
                        {
                            using (Font securityFont = new Font("Consolas", 8, FontStyle.Bold))
                            {
                                float securityTextX = (pageWidth - g.MeasureString(_ticket.Seguridad, securityFont).Width) / 2f;
                                g.DrawString(_ticket.Seguridad, securityFont, Brushes.Black, securityTextX-15, y);
                                y += 20;
                            }
                        }

                        float barcodeX = (pageWidth - matrix.Width) / 2f;
                        g.DrawImage(barcodeBitmap, barcodeX - 20, y, matrix.Width, matrix.Height);
                        y += matrix.Height + 10;

                        if (_paperWidth == 80)
                        {
                            using (Font securityFont = new Font("Consolas", 8, FontStyle.Bold))
                            {
                                float securityTextX = (pageWidth - g.MeasureString(_ticket.Seguridad, securityFont).Width) / 2f;
                                g.DrawString(_ticket.Seguridad, securityFont, Brushes.Black, securityTextX-15, y);
                                y += 20;
                            }
                        }
                    }
                }

                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en PrintPageHandler: {ex.Message}");
            }
        }

        private string SanitizeText(string input)
        {
            if (input == null) return string.Empty;
            // Quita saltos de línea y espacio en blanco
            return input.Replace("\n", " ").Replace("\r", " ").Trim();
        }
    }

}
