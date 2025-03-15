using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualesPrueba
{
    public class TicketBack
    {
        private decimal _total;

        public int Numero { get; set; }
        public string Fecha { get; set; }
        public string Seguridad { get; set; }
        public List<string> Apuesta { get; set; }
        public List<string> Evento { get; set; }
        public List<string> Opciones { get; set; }
        public string HoraSorteo { get; set; }
        public string Juego { get; set; }
        public string TotalMonto { get; set; }
        public string Hora { get; set; }
    }

}
