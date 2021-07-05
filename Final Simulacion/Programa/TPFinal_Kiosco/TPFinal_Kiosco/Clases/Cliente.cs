using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPFinal_Kiosco.Clases
{
    class Cliente
    {
        private int id;
        private string estado;

        public Cliente()
        {
            estado = "";
        }

        public Cliente(int id, string estado)
        {
            this.id = id;
            this.estado = estado;
        }

        public int Id { get => id; set => id = value; }
        public string Estado { get => estado; set => estado = value; }
    }
}
