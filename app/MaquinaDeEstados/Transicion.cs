using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaquinaDeEstados
{
    public class Transicion
    {
        //public static string[] eventos = { "ensamblaLote", "anulaFolioEnSII", "enviaAlSII", "recibeDelSIIAceptado", "recibeDelSIIConReparos",
        //                                   "recibeDelSIIRechazado", "enviaAlCliente"};
        //ATENCION! agregar condición de guarda para cada evento

        private int _evento;
        private string _accion;
        private int _origen;
        private int _destino;
        private String _tipo;

        public int iErr;
        public string sMsj;

        public Transicion()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evto"></param>
        /// <param name="acc"></param>
        /// <param name="tipo">std: transición estándar
        ///                     sco: transición a subcomponente</param>
        /// <param name="org"></param>
        /// <param name="dest"></param>
        public Transicion(int evto, string acc, String tipo, int org, int dest)
        {
            _evento = evto;
            _accion = acc;
            _origen = org;
            _destino = dest;
            _tipo = tipo;
        }

        public String Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        public int evento
        {
            get { return _evento; }
            set { _evento = value; }
        }

        public string accion
        {
            get { return _accion; }
            set { _accion= value; }
        }
        public int origen
        {
            get { return _origen; }
            set { _origen = value; }
        }
        public int destino
        {
            get { return _destino; }
            set { _destino = value; }
        }

        public bool CondicionDeGuarda(int anulado, int conAcceso)
        {
            bool ok = false;
            iErr = 0;
            if (_evento == Maquina.eventoGeneraTxt)   //ensambla archivo txt
            {
                if (anulado == 0 && conAcceso == 1)
                    ok = true;

                if (anulado == 1)
                    sMsj = "El documento está anulado en GP.";

                if (conAcceso == 0)
                    sMsj = "No tiene permisos para generar este documento. Verifique los permisos en la ventana de mantenimiento de Certificados de GP.";
            }

            if (_evento == Maquina.eventoPrefecturaAcepta )  
            {
                if (anulado == 0 && conAcceso == 1)
                    ok = true;

                if (anulado == 1)
                    sMsj = "El documento está anulado en GP. [Transicion.CondicionDeGuarda]";

                if (conAcceso == 0)
                    sMsj = "No tiene permisos para emitir documentos. Verifique los permisos en la ventana de mantenimiento de Certificados de GP. [Transicion.CondicionDeGuarda]";

            }

            if (!ok) iErr++;

            return ok;
        }
    }
}
