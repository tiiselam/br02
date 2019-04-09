
using System.Collections.Generic;


namespace IntegradorDeGP
{
    public interface IParametrosDB: IParametros
    {
        string ConnStringSource { get; set; }
        string ConnectionStringSourceEF { get; set; }
        string FormatoFechaDB { get; set; }
        Dictionary<string, string> IdsDocumento { get; set; } //= new Dictionary<string, Int16>();

        bool IncluirUserDef { get; set; }
        string Usrtab01_predetValue { get; set; }

        int intEstadoCompletado { get; }
        int intEstadosPermitidos { get; }

//        bool emite { get; set; }
//        bool envia { get; set; }
//        bool imprime { get; set; }
//        bool publica { get; set; }
//        bool zip { get; set; }
//        bool anula { get; }

    }

}
