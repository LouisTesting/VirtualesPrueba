using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualesPrueba
{
    public class AppConfig
    {
        public string SelectedPrinter { get; set; }
        public int PaperWidth { get; set; }
        public string SelectedLogo { get; set; }
        public string SelectedUser { get; set; }
        public string FinalMessage { get; set; }
        public bool IsTerminal { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public bool IsBacklot { get; set; }

        public string UsuarioBacklot { get; set; }
        public string ContraseñaBacklot { get; set; }
    }
}
