using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MaquinaDeEstados;

namespace notaFiscalCsvHelper
{
    public class NFSe
    {
        [Index(0)] 
        public string TipodeRegistro { get; set; }
        [Index(1)] 
        public string numNFSe { get; set; }
        [Index(2)] 
        public string DataHoraNFE { get; set; }
        [Index(3)] 
        public string CodigodeVerificacaodaNFSe { get; set; }
        [Index(4)] 
        public string TipodeRPS { get; set; }
        [Index(5)] 
        public string SeriedoRPS { get; set; }
        [Index(6)] 
        public string NumerodoRPS { get; set; }
        [Index(7)] 
        public string DatadoFatoGerador { get; set; }
        [Index(8)] 
        public string InscricaoMunicipaldoPrestador { get; set; }
        [Index(9)] 
        public string IndicadordeCPFCNPJdoPrestador { get; set; }
        [Index(10)] 
        public string CPFCNPJdoPrestador { get; set; }
        [Index(11)] 
        public string RazaoSocialdoPrestador { get; set; }
        [Index(12)] 
        public string TipodoEnderecodoPrestador { get; set; }
        [Index(13)] 
        public string EnderecodoPrestador { get; set; }
        [Index(14)] 
        public string NumerodoEnderecodoPrestador { get; set; }
        [Index(15)] 
        public string ComplementodoEnderecodoPrestador { get; set; }
        [Index(16)] 
        public string BairrodoPrestador { get; set; }
        [Index(17)] 
        public string CidadedoPrestador { get; set; }
        [Index(18)] 
        public string UFdoPrestador { get; set; }
        [Index(19)] 
        public string CEPdoPrestador { get; set; }
        [Index(20)] 
        public string EmaildoPrestador { get; set; }
        [Index(21)] 
        public string OpcaoPeloSimples { get; set; }
        [Index(22)] 
        public string SituacaodaNotaFiscal { get; set; }
        [Index(23)] 
        public string DatadeCancelamento { get; set; }
        [Index(24)] 
        public string NdaGuia { get; set; }
        [Index(25)] 
        public string DatadeQuitacaodaGuiaVinculadaaNotaFiscal { get; set; }
        [Index(26)] 
        public string ValordosServicos { get; set; }
        [Index(27)] 
        public string ValordasDeducoes { get; set; }
        [Index(28)] 
        public string CodigodoServicoPrestadonaNotaFiscal { get; set; }
        [Index(29)] 
        public string Aliquota { get; set; }
        [Index(30)] 
        public string ISSdevido { get; set; }
        [Index(31)] 
        public string ValordoCredito { get; set; }
        [Index(32)] 
        public string ISSRetido { get; set; }
        [Index(33)] 
        public string IndicadordeCPFCNPJdoTomador { get; set; }
        [Index(34)] 
        public string CPFCNPJdoTomador { get; set; }
        [Index(35)] 
        public string InscricaoMunicipaldoTomador { get; set; }
        [Index(36)] 
        public string InscricaoEstadualdoTomador { get; set; }
        [Index(37)] 
        public string RazaoSocialdoTomador { get; set; }
        [Index(38)] 
        public string TipodoEnderecodoTomador { get; set; }
        [Index(39)] 
        public string EnderecodoTomador { get; set; }
        [Index(40)] 
        public string NumerodoEnderecodoTomador { get; set; }
        [Index(41)] 
        public string ComplementodoEnderecodoTomador { get; set; }
        [Index(42)] 
        public string BairrodoTomador { get; set; }
        [Index(43)] 
        public string CidadedoTomador { get; set; }
        [Index(44)]
        public string UFdoTomador { get; set; }
        [Index(45)] 
        public string CEPdoTomador { get; set; }
        [Index(46)] 
        public string EmaildoTomador { get; set; }
        [Index(47)] 
        public string DiscriminacaodosServicos { get; set; }

        public Maquina CicloDeVida { get; set; }

        public string Sopnumbe
        {
            get { return this.SeriedoRPS + this.NumerodoRPS; }
            set { }
        }
    }

}
