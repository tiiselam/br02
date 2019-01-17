using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegradorDeGP
{
    public interface IParametrosXL : IParametros
    {
        string ClienteDefaultCUSTCLAS { get; set; }
        string rutaCarpeta { get; set; }
        int FacturaSopFilaInicial { get; set; }
        int FacturaSopColumnaMensajes { get; set; }

        int FacturaSopnumbe { get; set; }
        int FacturaSopDocdate { get; set; }
        int FacturaSopDuedate { get; set; }
        string FormatoFechaXL { get; set; }
        int FacturaSopTXRGNNUM { get; set; }
        int FacturaSopUNITPRCE { get; set; }
        int FacturaSopCUSTNAME { get; set; }
    }
}
