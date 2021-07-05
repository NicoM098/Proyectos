using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPFinal_Kiosco.Clases
{
    class Pedido
    {
        private int nroPedido;

        private string finPedido;

        private double tiempoRestante;

        private bool recienCreado;

        public Pedido()
        {
        }

        public Pedido(int nroPedido, string finPedido, double tiempoRestante, bool recienCreado)
        {
            this.nroPedido = nroPedido;
            this.finPedido = finPedido;
            this.tiempoRestante = tiempoRestante;
            this.recienCreado = recienCreado;
        }

        public int NroPedido { get => nroPedido; set => nroPedido = value; }

        public string FinPedido { get => finPedido; set => finPedido = value; }

        public double TiempoRestante { get => tiempoRestante; set => tiempoRestante = value; }

        public bool RecienCreado { get => recienCreado; set => recienCreado = value; }
    }
}
