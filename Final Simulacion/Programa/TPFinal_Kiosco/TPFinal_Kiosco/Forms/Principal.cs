using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using TPFinal_Kiosco.Clases;

namespace TPFinal_Kiosco
{
    public partial class Principal : Form
    {
        //DECLARACION DE VARIABLES GLOBALES...

        //Enteros...
        int nroIt;
        int cantIteraciones;
        int ColaDueño;
        int cantMaxPedidos;
        int cantPedidosAct;
        int ultimoNroPedido;
        int contClientes;

        //Double...
        double Reloj;
        double ACTiempoOcioso;
        double ACTiempoCoc;
        double ACTiempoMostr;
        double cteSolCom;
        double desde;
        double hasta;

        //String...
        string Evento;
        string RND1;
        string TiempoEntreLlegada;
        string ProxLlegada;
        string RND2;
        string Destino;
        string RND3;
        string TiempoAtencion;
        string FinAtencionMost;
        string FinSolicComida;
        string RND4;
        string TiempoPrep;
        string EstadoDueño;
        string InicioCocina;
        string InicioMostr;
        string EstadoAyudante;
        string InicioTiempoOc;
        
        //Boleanos...
        bool cambioEstadoDueño;

        //Listas, hashtable y objetos de clases...
        List<double> probAcDestino = new List<double>();

        List<Cliente> lisColaDueño = new List<Cliente>();

        List<Cliente> lisClientesConPedido = new List<Cliente>();

        List<Pedido> lisPedidos = new List<Pedido>();

        Cliente cliActual = new Cliente();

        Generador oGenerador = new Generador();

        Hashtable clientesHash = new Hashtable();

        public Principal()
        {
            InitializeComponent();

            CargarTablaDestino();

            hardcoding();

            deshabilitarBotonLimpiar();

            habilitarDoubleBuffer();
        }



        private void hardcoding()
        {
            txtTiempo.Text = 100.ToString();
            txtDesde.Text = 0.ToString();
            txtHasta.Text = 100.ToString();
            txtMedia.Text = 5.ToString();
            txtTiempoAtA.Text = (0.5f).ToString();
            txtTiempoAtB.Text = 2.ToString();
            txtTiempoCocA.Text = 5.ToString();
            txtTiempoCocB.Text = 10.ToString();

            pbrSimulacion.Visible = false;

            for (int i = 0; i < dgvProbDestino.Rows.Count; i++)
            {
                if (i == 0)
                {
                    dgvProbDestino.Rows[0].Cells["prob"].Value = 0.8f;
                }
                else if (i == 1)
                {
                    dgvProbDestino.Rows[1].Cells["prob"].Value = 0.2f;
                }
            }
        }



        private void habilitarDoubleBuffer()
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.SetProperty, null,
            dgvSimulacion, new object[] { true });
        }



        private string compareRandom(double random)
        {
            for (int i = 0; i < probAcDestino.Count(); i++)
            {
                if (random < probAcDestino.ElementAt(i))
                {
                    if (i == 0)
                    {
                        return "Golosinas o Bebidas";
                    }
                    else
                    {
                        return "Comida Rápida";
                    }
                }
            }
            return "";
        }



        private void CargarTablaDestino()
        {
            dgvProbDestino.Rows.Clear();

            dgvProbDestino.Rows.Add("Golosinas o Bebidas", 0);
            dgvProbDestino.Rows.Add("Comida Rápida", 0);
        }



        private List<double> ObtenerProbAcDestino()
        {
            List<double> listAux = new List<double>();

            for (int i = 0; i < dgvProbDestino.Rows.Count; i++)
            {
                listAux.Add(double.Parse(dgvProbDestino.Rows[i].Cells[2].Value.ToString()));
            }

            return listAux;
        }



        private void btnIniciar_Click(object sender, EventArgs e)
        {
            BorrarMensajesError();

            if (ValidarCampos())
            {
                //Medidores de tiempo...
                Stopwatch medidorTiempo = new Stopwatch();

                medidorTiempo.Start();

                pbrSimulacion.Visible = true;

                pbrSimulacion.Value = 0;

                int acumProgreso = 0;

                //Inicializacion del resto...
                probAcDestino = new List<double>();

                double tiempo = double.Parse(txtTiempo.Text);

                desde = double.Parse(txtDesde.Text);
                hasta = double.Parse(txtHasta.Text);

                double tiempoAtA = double.Parse(txtTiempoAtA.Text);
                double tiempoAtB = double.Parse(txtTiempoAtB.Text);

                double tiempoCocA = double.Parse(txtTiempoCocA.Text);
                double tiempoCocB = double.Parse(txtTiempoCocB.Text);

                double media = double.Parse(txtMedia.Text);

                cteSolCom = 0.1f;
                contClientes = 0;

                probAcDestino = ObtenerProbAcDestino();

                //INICIALIZACION DE VARIABLES....
                nroIt = 0;

                Evento = "Inicialización";

                Reloj = 0;

                //llegada_cliente
                RND1 = oGenerador.generadorUniforme().ToString();
                TiempoEntreLlegada = oGenerador.generadorExpNeg(media, double.Parse(RND1)).ToString();
                ProxLlegada = (double.Parse(TiempoEntreLlegada) + Reloj).ToString();

                //destino
                RND2 = "";
                Destino = "";

                //fin_atencion_mostrador
                RND3 = "";
                TiempoAtencion = "";
                FinAtencionMost = "";

                //fin_solicitud_comida
                FinSolicComida = "";

                //fin_preparacion_comida(i)
                RND4 = "";
                TiempoPrep = "";

                //dueño
                EstadoDueño = "Libre";
                ColaDueño = 0;
                InicioCocina = "";
                InicioMostr = "0";

                //ayudante
                EstadoAyudante = "Libre";
                InicioTiempoOc = "0";

                //Estadisticas
                ACTiempoOcioso = 0;
                ACTiempoCoc = 0;
                ACTiempoMostr = 0;

                //Otras Variables...
                cantMaxPedidos = 0;
                cantPedidosAct = 0;
                cantIteraciones = 0;
                cambioEstadoDueño = false;

                //CARGANDO LA FILA DE INICIALIZACION...
                dgvSimulacion.Rows.Add(nroIt,
                    Evento,
                    Reloj,
                    RND1,
                    TiempoEntreLlegada,
                    ProxLlegada,
                    RND2,
                    Destino,
                    RND3,
                    TiempoAtencion,
                    FinAtencionMost,
                    FinSolicComida,
                    RND4,
                    TiempoPrep,
                    EstadoDueño,
                    ColaDueño,
                    InicioCocina,
                    InicioMostr,
                    EstadoAyudante,
                    InicioTiempoOc,
                    ACTiempoOcioso,
                    ACTiempoCoc,
                    ACTiempoMostr);


                while (Reloj <= tiempo)
                {
                    //Se incrementa el nro de iteracion..
                    nroIt += 1;

                    //Se utiliza un contador secundario para los casos de "desde" y "hasta"
                    if (Reloj >= desde)
                    {
                        cantIteraciones += 1;
                    }

                    //Se incrementa el progreso de la barra..
                    acumProgreso = (int)(Reloj * 100 / tiempo);

                    if (acumProgreso <= 100)
                    {
                        pbrSimulacion.Value = acumProgreso;
                    }
                    else if (acumProgreso > 100)
                    {
                        pbrSimulacion.Value = 100;
                    }

                    //Limpieza de variables...
                    RND1 = "";
                    TiempoEntreLlegada = "";
                    RND2 = "";
                    Destino = "";
                    RND3 = "";
                    TiempoAtencion = "";
                    RND4 = "";
                    TiempoPrep = "";

                    cambioEstadoDueño = false;

                    //CALCULO DEL EVENTO...
                    Dictionary<string, Double> aComparar = new Dictionary<string, Double>();

                    List<string> eventos = new List<string>();
                    eventos.Add("llegada_cliente");
                    eventos.Add("fin_atencion_mostrador");
                    eventos.Add("fin_solicitud_comida");

                    List<string> columnas = new List<string>();
                    columnas.Add(ProxLlegada);
                    columnas.Add(FinAtencionMost);
                    columnas.Add(FinSolicComida);


                    //Si hay fines de preparacion, los agrega a ambas listas
                    if (cantMaxPedidos > 0)
                    {
                        for (int x = 0; x < cantMaxPedidos; x++)
                        {
                            Pedido pedTemp = lisPedidos.ElementAt(x);

                            string aux = "fin_preparacion_comida_" + pedTemp.NroPedido.ToString();
                            eventos.Add(aux);


                            string aux2 = pedTemp.FinPedido;
                            columnas.Add(aux2);
                        }
                    }


                    //Se recorre el array con los valores de las columnas que se corresponden con los proximos eventos
                    for (int indCol = 0; indCol < columnas.Count(); indCol++)
                    {
                        if (columnas.ElementAt(indCol) != "")
                        {
                            string key = indCol.ToString();

                            double value = double.Parse(columnas.ElementAt(indCol));

                            aComparar.Add(key, value);
                        }
                    }


                    //Se obtiene el minimo valor del diccionario
                    var min = aComparar.Min(kvp => kvp.Value);

                    int indice = columnas.IndexOf(min.ToString());


                    //Asignacion de evento....
                    Evento = eventos[indice];


                    //OPERACION CON LOS EVENTOS...

                    //Si el evento es del tipo "llegada_cliente"
                    if (Evento.Equals(eventos[0]))
                    {
                        llegadaCliente(media, tiempoAtA, tiempoAtB, cteSolCom);
                    }

                    //Si el evento es del tipo "fin_atencion_mostrador"
                    else if (Evento.Equals(eventos[1]))
                    {
                        finAtencionMostrador(tiempoAtA, tiempoAtB);
                    }

                    //Si el evento es del tipo "fin_solicitud_comida"
                    else if (Evento.Equals(eventos[2]))
                    {
                        finSolComida(tiempoAtA, tiempoAtB, tiempoCocA, tiempoCocB);
                    }

                    //Si el evento es del tipo "fin_preparacion_comida"
                    else if (cantMaxPedidos > 0)
                    {
                        for (int j = 1; j <= cantMaxPedidos; j++)
                        {
                            int nroFin = 2 + j;

                            if (Evento.Equals(eventos[nroFin]))
                            {
                                finPrepComida(j);
                                break;
                            }
                        }
                    }


                    if ((Reloj >= desde && cantIteraciones <= hasta) || Reloj >= tiempo)
                    {
                        string[] fila = cargarFila();
                        
                        dgvSimulacion.Rows.Add(fila);
                        
                        //Para cargar los estados de los clientes correspondientes...
                        foreach (DictionaryEntry item in clientesHash)
                        {
                            Cliente aux = (Cliente)item.Value;

                            if (dgvSimulacion.Columns["cliente" + aux.Id.ToString()] != null)
                            {
                                if (!(aux.Estado.Equals("")))
                                {
                                    dgvSimulacion.Rows[dgvSimulacion.Rows.Count - 1].Cells["cliente" + aux.Id.ToString()].Value = aux.Estado;
                                }
                            }
                        }
                    }



                    //Actualizar los fines de pedido en caso de cambio de estado del dueño...
                    if (cantMaxPedidos > 0)
                    {
                        foreach (Pedido pedido in lisPedidos)
                        {
                            if (pedido.FinPedido != "")
                            {
                                if (pedido.RecienCreado == false)
                                {
                                    if (cambioEstadoDueño == true)
                                    {
                                        if (EstadoDueño.Equals("AC"))
                                        {
                                            pedido.TiempoRestante = (double.Parse(pedido.FinPedido) - Reloj) / (double)2;
                                            pedido.FinPedido = (Reloj + pedido.TiempoRestante).ToString();
                                            pedido.RecienCreado = false;
                                        }

                                        else if (EstadoDueño.Equals("AM"))
                                        {
                                            pedido.TiempoRestante = (double.Parse(pedido.FinPedido) - Reloj) * (double)2;
                                            pedido.FinPedido = (Reloj + pedido.TiempoRestante).ToString();
                                            pedido.RecienCreado = false;
                                        }
                                    }
                                    else
                                    {
                                        pedido.TiempoRestante = double.Parse(pedido.FinPedido) - Reloj;
                                    }
                                }
                            }
                        }
                    }

                    //Para cargar los fin de pedido correspondientes...
                    if (cantMaxPedidos > 0)
                    {
                        foreach (Pedido pedido in lisPedidos)
                        {
                            dgvSimulacion.Rows[dgvSimulacion.Rows.Count - 1].Cells["finPreparacion" + pedido.NroPedido.ToString()].Value = pedido.FinPedido;

                            //De paso actualizamos el booleano para los pedidos recien creados..
                            pedido.RecienCreado = false;
                        }
                    }
                }

                pbrSimulacion.Value = 100;

                //Procesar los Acumuladores al final de la simulacion...
                if (InicioCocina.Equals(""))
                {
                    acumularTiempoMostr();
                }
                else
                {
                    acumularTiempoCocina();
                }

                if (InicioTiempoOc != "")
                {
                    acumularTiempoOcioso();
                }


                //Seteamos los porcentajes en los lbl correspondientes...
                double porcTiempoOcioso = Math.Round(((ACTiempoOcioso * 100) / Reloj), 2);
                porcTiempoOc_lbl.Text = porcTiempoOcioso.ToString() + "%";

                double porcTiempoCoc = Math.Round(((ACTiempoCoc * 100) / Reloj), 2);
                porcTiempoCoc_lbl.Text = porcTiempoCoc.ToString() + "%";

                double porcTiempoMost = Math.Round(((ACTiempoMostr * 100) / Reloj), 2);
                porcTiempoMost_lbl.Text = porcTiempoMost.ToString() + "%";


                //Deshabilitamos el botón de Iniciar Simulacion
                deshabilitarBotonIniciarSim();

                //Se habilita el boton limpiar
                habilitarBotonLimpiar();

                medidorTiempo.Stop();

                double tiempoTranscurrido = Math.Round(medidorTiempo.Elapsed.TotalSeconds, 4);

                if (tiempoTranscurrido > 60)
                {
                    tiempoTranscurrido = Math.Round((tiempoTranscurrido / (double)60), 4);

                    tiempo_lbl.Text = tiempoTranscurrido.ToString() + " min";
                }
                else
                {
                    tiempo_lbl.Text = tiempoTranscurrido.ToString() + " seg";
                }

                //Desvisibilizar la barra de progreso
                pbrSimulacion.Visible = false;
            }
        }



        //*********EVENTOS***********
        private void llegadaCliente(double media, double tiempoAtA, double tiempoAtB, double cteSolCom)
        {
            //Se crea un nuevo cliente, incrementando el contador...
            contClientes += 1;

            Cliente cliente = new Cliente();

            cliente.Id = contClientes;

            //Con respecto a la tabla...
            Reloj = double.Parse(ProxLlegada);

            generarProximaLlegada(media);

            //Si el estado del dueño era LIBRE
            if (EstadoDueño.Equals("Libre"))
            {
                generarDestino();

                if (Destino.Equals("Golosinas o Bebidas"))
                {
                    generarFinAtencionMostrador(tiempoAtA, tiempoAtB);
                }
                else if (Destino.Equals("Comida Rápida"))
                {
                    generarFinSolicitudComida(cteSolCom);
                }

                EstadoDueño = "AM";

                cliente.Estado = "SAM";

                cliActual = cliente;
            }

            //Si el estado del dueño era ATENDIENDO MOSTRADOR
            else if (EstadoDueño.Equals("AM"))
            {
                ColaDueño += 1;

                cliente.Estado = "EAM";

                lisColaDueño.Add(cliente);
            }

            //Si el estado del dueño era AYUDANDO COCINA
            else if (EstadoDueño.Equals("AC"))
            {
                generarDestino();

                if (Destino.Equals("Golosinas o Bebidas"))
                {
                    generarFinAtencionMostrador(tiempoAtA, tiempoAtB);
                }
                else if (Destino.Equals("Comida Rápida"))
                {
                    generarFinSolicitudComida(cteSolCom);
                }

                EstadoDueño = "AM";

                cambioEstadoDueño = true;

                acumularTiempoCocina();

                cliente.Estado = "SAM";

                cliActual = cliente;
            }

            if (Reloj >= desde && cantIteraciones <= hasta)
            {
                //Se agrega el cliente en la hashtable
                clientesHash.Add(cliente.Id, cliente);

                //Se agrega la columna de cliente..
                agregarColumnaCliente(cliente);
            }
        }



        private void finAtencionMostrador(double tiempoAtA, double tiempoAtB)
        {
            //Operacion del resto de las columnas...
            Reloj = double.Parse(FinAtencionMost);

            if (cliActual == null)
            {
                MessageBox.Show("'Cliente Actual' tiene valor null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            else if (cliActual.Estado.Equals("SAM"))
            {
                actualizarEstadoCliente();
            }


            //Se limpia el respectivo finAtencionMostrador....
            RND3 = "";
            TiempoAtencion = "";
            FinAtencionMost = "";


            if (ColaDueño == 0 && EstadoAyudante.Equals("PC"))
            {
                EstadoDueño = "AC";

                cambioEstadoDueño = true;

                acumularTiempoMostr();
            }

            else if (ColaDueño == 0)
            {
                EstadoDueño = "Libre";
            }

            else if (ColaDueño > 0)
            {
                EstadoDueño = "AM";

                Cliente cliente = lisColaDueño.ElementAt(0);

                ColaDueño -= 1;

                if (cliente.Estado.Equals("EAM"))
                {
                    cliente.Estado = "SAM";

                    generarDestino();

                    if (Destino.Equals("Golosinas o Bebidas"))
                    {
                        generarFinAtencionMostrador(tiempoAtA, tiempoAtB);
                    }
                    else if (Destino.Equals("Comida Rápida"))
                    {
                        generarFinSolicitudComida(cteSolCom);
                    }

                    if (Reloj >= desde && cantIteraciones <= hasta)
                    {
                        //Buscamos el respectivo cliente en la hash y cambiamos su estado..
                        clientesHash[cliente.Id] = cliente;
                    }

                    //Seteamos el cliente actual
                    cliActual = cliente;

                    //Lo eliminamos de la cola del dueño
                    lisColaDueño.RemoveAt(0);
                }

            }
        }



        private void finSolComida(double tiempoAtA, double tiempoAtB, double tiempoCocA, double tiempoCocB)
        {
            //Operacion del resto de las columnas...
            Reloj = double.Parse(FinSolicComida);

            Cliente cliTemp = cliActual;

            //Limpieza de fin_solicitud_comida
            FinSolicComida = "";


            //Con respecto al dueño...
            if (ColaDueño > 0)
            {
                EstadoDueño = "AM";

                Cliente cliente = lisColaDueño.ElementAt(0);

                ColaDueño -= 1;

                if (cliente.Estado.Equals("EAM"))
                {
                    cliente.Estado = "SAM";

                    generarDestino();

                    if (Destino.Equals("Golosinas o Bebidas"))
                    {
                        generarFinAtencionMostrador(tiempoAtA, tiempoAtB);
                    }
                    else if (Destino.Equals("Comida Rápida"))
                    {
                        generarFinSolicitudComida(cteSolCom);
                    }

                    if (Reloj >= desde && cantIteraciones <= hasta)
                    {
                        //Buscamos el respectivo cliente en la hash y cambiamos su estado..
                        clientesHash[cliente.Id] = cliente;
                    }
                    
                    //Seteamos el cliente actual
                    cliActual = cliente;

                    //Lo eliminamos de la cola del dueño
                    lisColaDueño.RemoveAt(0);
                }
            }

            else if (ColaDueño == 0)
            {
                cliActual = null;

                EstadoDueño = "AC";

                cambioEstadoDueño = true;

                acumularTiempoMostr();
            }


            //Con respecto al ayudante...
            if (EstadoAyudante.Equals("Libre"))
            {
                EstadoAyudante = "PC";

                acumularTiempoOcioso();

                generarFinPreparacionComida(tiempoCocA, tiempoCocB);

                //Se actualiza el estado del "cliente actual" y lo metemos en la cola de clientes con pedido...
                cliTemp.Estado = "EP (" + ultimoNroPedido.ToString() + ")";

                lisClientesConPedido.Add(cliTemp);

                if (Reloj >= desde && cantIteraciones <= hasta)
                {
                    //Buscamos el respectivo cliente en la hash y cambiamos su estado..
                    clientesHash[cliTemp.Id] = cliTemp;
                }
            }

            else if (EstadoAyudante.Equals("PC"))
            {
                EstadoAyudante = "PC";

                generarFinPreparacionComida(tiempoCocA, tiempoCocB);

                //Se actualiza el estado del "cliente actual" y lo metemos en la cola de clientes con pedido...
                cliTemp.Estado = "EP (" + ultimoNroPedido.ToString() + ")";

                lisClientesConPedido.Add(cliTemp);

                if (Reloj >= desde && cantIteraciones <= hasta)
                {
                    //Buscamos el respectivo cliente en la hash y cambiamos su estado..
                    clientesHash[cliTemp.Id] = cliTemp;
                }
            }
        }



        private void finPrepComida(int nroFin)
        {
            bool tienePedidos = false;

            //Obtenemos el valor del fin_preparacion_comida correspondiente...
            double FinPrepComida = double.Parse(dgvSimulacion.Rows[dgvSimulacion.Rows.Count - 1].Cells["finPreparacion" + nroFin.ToString()].Value.ToString());

            Reloj = FinPrepComida;

            //Limpiamos el fin preparacion correspondiente..
            foreach (Pedido pedido in lisPedidos)
            {
                if (pedido.NroPedido == nroFin)
                {
                    pedido.FinPedido = "";
                    pedido.TiempoRestante = 0;
                    pedido.RecienCreado = false;

                    cantPedidosAct -= 1;
                    break;
                }
            }

            //Buscamos el cliente correspondiente para eliminarlo de la lista de clientes con pedido y modificar su estado...
            foreach (Cliente cliente in lisClientesConPedido)
            {
                if (cliente.Estado.Equals("EP (" + nroFin.ToString() + ")"))
                {
                    int indice = lisClientesConPedido.IndexOf(cliente);

                    cliente.Estado = "";

                    //Lo eliminamos de la lista...
                    lisClientesConPedido.RemoveAt(indice);

                    if (Reloj >= desde && cantIteraciones <= hasta)
                    {
                        //Actualizamos el estado en la hash
                        clientesHash[cliente.Id] = cliente;
                    }

                    break;
                }
            }

            //Con respecto al ayudante...

            //Chequeamos si tiene fin preparacion pendientes...
            foreach (Pedido pedido in lisPedidos)
            {
                if (pedido.FinPedido.Equals(""))
                {
                    tienePedidos = false;
                }
                else
                {
                    tienePedidos = true;
                    break;
                }
            }

            if (tienePedidos == true)
            {
                EstadoAyudante = "PC";
            }
            else
            {
                EstadoAyudante = "Libre";

                InicioTiempoOc = Reloj.ToString();

                //Si el dueño estaba ayudando en la cocina, modificamos su estado ya que no hay pedidos en preparacion...
                if (EstadoDueño.Equals("AC"))
                {
                    EstadoDueño = "Libre";

                    acumularTiempoCocina();
                }
            }
        }



        //*********ACUMULADORES***********
        private void acumularTiempoCocina()
        {
            ACTiempoCoc += Math.Round((Reloj - double.Parse(InicioCocina)), 2);

            Math.Round(ACTiempoCoc, 2);

            InicioCocina = "";

            InicioMostr = Reloj.ToString();
        }



        private void acumularTiempoMostr()
        {
            ACTiempoMostr += Math.Round((Reloj - double.Parse(InicioMostr)), 2);

            Math.Round(ACTiempoMostr, 2);

            InicioMostr = "";

            InicioCocina = Reloj.ToString();
        }



        private void acumularTiempoOcioso()
        {
            ACTiempoOcioso += Math.Round((Reloj - double.Parse(InicioTiempoOc)), 2);

            Math.Round(ACTiempoOcioso, 2);

            InicioTiempoOc = "";
        }



        //*********GENERADORES***********
        private void generarDestino()
        {
            RND2 = oGenerador.generadorUniforme().ToString();

            Destino = compareRandom(double.Parse(RND2));
        }



        private void generarFinAtencionMostrador(double tiempoAtA, double tiempoAtB)
        {
            RND3 = oGenerador.generadorUniforme().ToString();

            TiempoAtencion = (oGenerador.generadorUniforme(tiempoAtA, tiempoAtB, double.Parse(RND3))).ToString();

            FinAtencionMost = (double.Parse(TiempoAtencion) + Reloj).ToString();
        }



        private void generarFinSolicitudComida(double cteSolCom)
        {
            FinSolicComida = Math.Round((Reloj + cteSolCom), 2).ToString();
        }



        private void generarFinPreparacionComida(double tiempoCocA, double tiempoCocB)
        {
            cantPedidosAct += 1;

            RND4 = oGenerador.generadorUniforme().ToString();

            //Si el dueño esta ayudando en la cocina, el tiempo de prep se reduce a la mitad...
            if (EstadoDueño.Equals("AC"))
            {
                TiempoPrep = ((oGenerador.generadorUniforme(tiempoCocA, tiempoCocB, double.Parse(RND4))) / (double) 2).ToString();
            }
            else
            {
                TiempoPrep = (oGenerador.generadorUniforme(tiempoCocA, tiempoCocB, double.Parse(RND4))).ToString();
            }

            if (cantPedidosAct > cantMaxPedidos)
            {
                agregarColFinPreparacion();
            }
            else
            {
                foreach (Pedido pedido in lisPedidos)
                {
                    if (pedido.FinPedido == "")
                    {
                        pedido.FinPedido = (Reloj + double.Parse(TiempoPrep)).ToString();
                        pedido.TiempoRestante = double.Parse(pedido.FinPedido) - Reloj;
                        pedido.RecienCreado = true;

                        ultimoNroPedido = pedido.NroPedido;
                        break;
                    }
                }
            }
        }



        private void generarProximaLlegada(double media)
        {
            RND1 = oGenerador.generadorUniforme().ToString();

            TiempoEntreLlegada = oGenerador.generadorExpNeg(media, double.Parse(RND1)).ToString();

            ProxLlegada = (double.Parse(TiempoEntreLlegada) + Reloj).ToString();
        }



        //*********OPERACIONES CON COLUMNAS***********
        private void agregarColFinPreparacion()
        {
            cantMaxPedidos += 1;

            DataGridViewColumn columna = new DataGridViewColumn();

            columna.Name = "finPreparacion" + cantMaxPedidos.ToString();
            columna.HeaderText = "Fin Preparacion " + cantMaxPedidos.ToString();
            columna.CellTemplate = new DataGridViewTextBoxCell();

            //Estilo de la columna
            columna.DefaultCellStyle.BackColor = Color.FromArgb(188, 9, 0);
            columna.DefaultCellStyle.ForeColor = Color.White;
            columna.DefaultCellStyle.SelectionBackColor = Color.FromArgb(125, 6, 0);
            columna.DefaultCellStyle.SelectionForeColor = Color.White;

            int ordenColumna = cantMaxPedidos - 1;

            dgvSimulacion.Columns.Insert(14 + ordenColumna, columna);

            double FinPreparacion = (double.Parse(TiempoPrep) + Reloj);

            //Creamos el objeto pedido...
            Pedido pedAux = new Pedido();

            pedAux.NroPedido = cantMaxPedidos;
            pedAux.FinPedido = FinPreparacion.ToString();
            pedAux.TiempoRestante = FinPreparacion - Reloj;
            pedAux.RecienCreado = true;

            ultimoNroPedido = pedAux.NroPedido;

            //Lo agregamos a la lista de pedidos..
            lisPedidos.Add(pedAux);
        }



        private void quitarColFinPreparacion(int nroFin)
        {
            if (cantMaxPedidos <= 0)
            {
                MessageBox.Show("No hay columnas de Fin Preparacion para borrar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                dgvSimulacion.Columns.Remove("finPreparacion" + nroFin.ToString());
            }
        }



        private void agregarColumnaCliente(Cliente cliente)
        {
            DataGridViewColumn columna = new DataGridViewColumn();

            columna.Name = "cliente" + cliente.Id.ToString();
            columna.HeaderText = "Estado Cliente " + cliente.Id.ToString();
            columna.CellTemplate = new DataGridViewTextBoxCell();
            columna.FillWeight = 10;
            columna.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgvSimulacion.Columns.Add(columna);
            //dgvSimulacion.Columns.Add("cliente" + cliente.Id.ToString(), "Estado Cliente " + cliente.Id.ToString());
        }



        private void eliminarColumnasCliente()
        {
            foreach (DictionaryEntry item in clientesHash)
            {

                Cliente cliente = (Cliente)item.Value;

                if (dgvSimulacion.Columns["cliente" + cliente.Id.ToString()] != null)
                {
                    dgvSimulacion.Columns.Remove("cliente" + cliente.Id.ToString());
                }
            }
        }



        private void eliminarColumnasFinPreparacion()
        {
            for (int i = 1; i <= cantMaxPedidos; i++)
            {
                quitarColFinPreparacion(i);
            }
        }



        private void actualizarEstadoCliente()
        {
            if (Reloj >= desde && cantIteraciones <= hasta)
            {
                int idAux = cliActual.Id;

                Cliente cliAux = (Cliente)clientesHash[idAux];

                if (cliAux != null)
                {
                    cliAux.Estado = "";

                    cliActual = null;

                    clientesHash[idAux] = cliAux;
                }
            }
        }



        //EVENTOS DE TEXTBOX Y DGV...
        private void dgvProbDestino_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            double aux = 0;

            for (int i = 0; i <dgvProbDestino.Rows.Count; i++)
            {
                aux += double.Parse(dgvProbDestino.Rows[i].Cells[1].Value.ToString());

                if (aux > 1)
                {
                    MessageBox.Show("La suma de las probabilidades no debe ser mayor a 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    break;
                }
                else
                {
                    dgvProbDestino.Rows[i].Cells[2].Value = aux;
                }
            }
        }



        private void validarEntero(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        private void validarDouble(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
               (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }



        private void txtIteraciones_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtDesde_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtHasta_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarEntero(sender, e);
        }

        private void txtMedia_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtTiempoAtA_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtTiempoAtB_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtTiempoCocA_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }

        private void txtTiempoCocB_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarDouble(sender, e);
        }



        //*********VALIDACIONES***********
        private void btnLimpiarCampos_Click(object sender, EventArgs e)
        {
            //Se limpian los dgv
            dgvSimulacion.Rows.Clear();
            dgvProbDestino.Rows.Clear();

            //Recargamos la tabla de prob destino
            CargarTablaDestino();

            //Seteamos los textbox a su manera original
            hardcoding();

            //Limpiamos las columnas de clientes..
            eliminarColumnasCliente();

            //Limpiamos las columnas de fin preparacion...
            eliminarColumnasFinPreparacion();

            //Volvemos a generar las respectivas listas y hashtables...
            probAcDestino = new List<double>();
            lisColaDueño = new List<Cliente>();
            lisClientesConPedido = new List<Cliente>();
            lisPedidos = new List<Pedido>();
            cliActual = new Cliente();
            oGenerador = new Generador();
            clientesHash = new Hashtable();

            //Limpiamos los labels de estadisticas
            porcTiempoCoc_lbl.Text = "- %";
            porcTiempoMost_lbl.Text = "- %";
            porcTiempoOc_lbl.Text = "- %";

            tiempo_lbl.Text = "-";

            //Se vuelve a habilitar el boton de iniciar simulacion
            habilitarBotonIniciarSim();

            //Se deshabilita el boton limpiar...
            deshabilitarBotonLimpiar();

            //Se borran los mensajes de error
            BorrarMensajesError();
        }



        private void habilitarBotonLimpiar()
        {
            btnLimpiarCampos.Enabled = true;
            btnLimpiarCampos.BackColor = Color.Black;
            btnLimpiarCampos.ForeColor = Color.White;
        }



        private void deshabilitarBotonLimpiar()
        {
            btnLimpiarCampos.Enabled = false;
            btnLimpiarCampos.BackColor = Color.White;
            btnLimpiarCampos.ForeColor = Color.Black;
        }



        private void habilitarBotonIniciarSim()
        {
            btnIniciar.Enabled = true;
            btnIniciar.BackColor = Color.Black;
            btnIniciar.ForeColor = Color.White;
        }



        private void deshabilitarBotonIniciarSim()
        {
            btnIniciar.Enabled = false;
            btnIniciar.BackColor = Color.White;
            btnIniciar.ForeColor = Color.Black;
        }



        private bool ValidarCampos()
        {
            bool ok = true;

            ok = validarValoresTabla();

            if (txtTiempo.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempo, "Ingrese algún valor");
            }
            else if (txtTiempo.Text.Equals("0"))
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtTiempo, "Debe ingresar un valor mayor a 0");
            }


            else if (txtDesde.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtDesde, "Ingrese algún valor");
            }
            else if (double.Parse(txtDesde.Text) > double.Parse(txtTiempo.Text))
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtDesde, "El valor j debe ser menor o igual al tiempo X ingresado");
            }


            if (txtHasta.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtHasta, "Ingrese algún valor");
            }
            else if (txtHasta.Text.Equals("0") || double.Parse(txtHasta.Text) > 500)
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtHasta, "Debe ingresar un valor mayor a 0 y <= 500");
            }


            if (txtMedia.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtMedia, "Ingrese algún valor");
            }
            else if (txtMedia.Text.Equals("0"))
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtMedia, "Debe ingresar un valor mayor a 0");
            }


            if (txtTiempoAtA.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoAtA, "Ingrese algún valor");
            }
            else if (txtTiempoAtB.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoAtB, "Ingrese algún valor");
            }
            else if (double.Parse(txtTiempoAtA.Text) >= double.Parse(txtTiempoAtB.Text))
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtTiempoAtA, "El Tiempo A no debe ser mayor o igual que el Tiempo B");
                errorDatoIncorrectoProvider.SetError(txtTiempoAtB, "El Tiempo A no debe ser mayor o igual que el Tiempo B");
            }

            if (txtTiempoAtB.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoAtB, "Ingrese algún valor");
            }


            if (txtTiempoCocA.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoCocA, "Ingrese algún valor");
            }
            else if (txtTiempoCocB.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoCocB, "Ingrese algún valor");
            }
            else if (double.Parse(txtTiempoCocA.Text) >= double.Parse(txtTiempoCocB.Text))
            {
                ok = false;
                errorDatoIncorrectoProvider.SetError(txtTiempoCocA, "El Tiempo A no debe ser mayor o igual que el Tiempo B");
                errorDatoIncorrectoProvider.SetError(txtTiempoCocB, "El Tiempo A no debe ser mayor o igual que el Tiempo B");
            }

            if (txtTiempoCocB.Text.Equals(""))
            {
                ok = false;
                errorNuloProvider.SetError(txtTiempoCocB, "Ingrese algún valor");
            }

            return ok;
        }



        private bool validarValoresTabla()
        {
            double aux = 0;

            for (int i = 0; i < dgvProbDestino.Rows.Count; i++)
            {
                aux += double.Parse(dgvProbDestino.Rows[i].Cells[1].Value.ToString());
            }

            if (aux < 1)
            {
                errorDatoIncorrectoProvider.SetError(dgvProbDestino, "La suma de las probabilidades debe dar 1");

                return false;
            }

            return true;
        }



        private void BorrarMensajesError()
        {
            errorNuloProvider.SetError(txtTiempo, "");
            errorNuloProvider.SetError(txtHasta, "");
            errorNuloProvider.SetError(txtMedia, "");
            errorNuloProvider.SetError(txtTiempoAtA, "");
            errorNuloProvider.SetError(txtTiempoAtB, "");
            errorNuloProvider.SetError(txtTiempoCocA, "");
            errorNuloProvider.SetError(txtTiempoCocB, "");
            errorNuloProvider.SetError(txtDesde, "");

            errorDatoIncorrectoProvider.SetError(txtTiempo, "");
            errorDatoIncorrectoProvider.SetError(txtHasta, "");
            errorDatoIncorrectoProvider.SetError(txtMedia, "");
            errorDatoIncorrectoProvider.SetError(txtTiempoAtA, "");
            errorDatoIncorrectoProvider.SetError(txtTiempoAtB, "");
            errorDatoIncorrectoProvider.SetError(txtTiempoCocA, "");
            errorDatoIncorrectoProvider.SetError(txtTiempoCocB, "");
            errorDatoIncorrectoProvider.SetError(txtDesde, "");
            errorDatoIncorrectoProvider.SetError(dgvProbDestino, "");
        }

        

        private string[] cargarFila()
        {
            string[] arrayAux = new string[23 + cantMaxPedidos];

            int indMax = 13 + cantMaxPedidos;

            for (int i = 0; i < arrayAux.Length; i++)
            {
                if (i == 0)
                {
                    arrayAux[i] = nroIt.ToString();
                }
                else if (i == 1)
                {
                    if (Evento.Equals("llegada_cliente"))
                    {
                        string aux = Evento + "_" + contClientes.ToString();

                        arrayAux[i] = aux;
                    }
                    else
                    {
                        arrayAux[i] = Evento;
                    }
                }
                else if (i == 2)
                {
                    arrayAux[i] = Reloj.ToString();
                }
                else if (i == 3)
                {
                    arrayAux[i] = RND1;
                }
                else if (i == 4)
                {
                    arrayAux[i] = TiempoEntreLlegada;
                }
                else if (i == 5)
                {
                    arrayAux[i] = ProxLlegada;
                }
                else if (i == 6)
                {
                    arrayAux[i] = RND2;
                }
                else if (i == 7)
                {
                    arrayAux[i] = Destino;
                }
                else if (i == 8)
                {
                    arrayAux[i] = RND3;
                }
                else if (i == 9)
                {
                    arrayAux[i] = TiempoAtencion;
                }
                else if (i == 10)
                {
                    arrayAux[i] = FinAtencionMost;
                }
                else if (i == 11)
                {
                    arrayAux[i] = FinSolicComida;
                }
                else if (i == 12)
                {
                    arrayAux[i] = RND4;
                }
                else if (i == 13)
                {
                    arrayAux[i] = TiempoPrep;
                }

                if (i >= indMax)
                {
                    arrayAux[i] = "";
                }

                if (i == (indMax + 1))
                {
                    arrayAux[i] = EstadoDueño;
                }
                else if (i == (indMax + 2))
                {
                    arrayAux[i] = ColaDueño.ToString();
                }
                else if (i == (indMax + 3))
                {
                    arrayAux[i] = InicioCocina;
                }
                else if (i == (indMax + 4))
                {
                    arrayAux[i] = InicioMostr;
                }
                else if (i == (indMax + 5))
                {
                    arrayAux[i] = EstadoAyudante;
                }
                else if (i == (indMax + 6))
                {
                    arrayAux[i] = InicioTiempoOc;
                }
                else if (i == (indMax + 7))
                {
                    arrayAux[i] = (Math.Round(ACTiempoOcioso, 2)).ToString();
                }
                else if (i == (indMax + 8))
                {
                    arrayAux[i] = (Math.Round(ACTiempoCoc, 2)).ToString();
                }
                else if (i == (indMax + 9))
                {
                    arrayAux[i] = (Math.Round(ACTiempoMostr, 2)).ToString();
                }
            }

            return arrayAux;
        }
    }
}
