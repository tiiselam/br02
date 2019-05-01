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
        string FacturaSopSerieYNumbeSeparados { get; set; }
        int FacturaSopSerie { get; set; }
        int FacturaSopReferencia { get; set; }

        int FacturaSopnumbe { get; set; }
        int FacturaSopDocdate { get; set; }
        int FacturaSopDuedate { get; set; }
        string FormatoFechaXL { get; set; }
        int FacturaSopTXRGNNUM { get; set; }
        int FacturaSopUNITPRCE { get; set; }
        int FacturaSopDeUNITPRCE { get; set; }
        int FacturaSopCUSTNAME { get; set; }
        int FacturaSopCliDireccion1 { get; set; }
        int FacturaSopCliDireccion2 { get; set; }
        int FacturaSopCliDireccion3 { get; set; }
        int FacturaSopCliCiudad { get; set; }
        int FacturaSopCliEstado { get; set; }
        int FacturaSopCliZipCode { get; set; }
        int FacturaSopCliEmail { get; set; }

        int FacturaSopItemnmbr { get; set; }
        int FacturaSopItemnmbrDescr { get; set; }
        int FacturaSopCodServicio { get ; set ; }

        int FacturaSopDeReqShipDate { get; set; }
        int FacturaSopDeActlShipDate { get; set; }
        int FacturaSopDeCmmttext { get; set; }

        bool IncluirUserDef { get; set; }
        string Usrtab01_predetValue { get; set; }
        string Usrtab02_predetValue { get; set; }


    }
}
