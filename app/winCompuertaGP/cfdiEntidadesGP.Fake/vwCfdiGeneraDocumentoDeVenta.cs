using System;
using System.Collections.Generic;
using System.Text;

namespace cfdiEntidadesGP.Fake
{
    public class vwCfdiGeneraDocumentoDeVenta
    {
        public string correlativo { get; set; }

        public short soptype { get; set; }
        public string sopnumbe { get; set; }

        public string emisorTipoDoc { get; set; }
        public string emisorNroDoc { get; set; }
        public string emisorNombre { get; set; }
        public string InscricaoPrestador { get; set; }
        public string serie { get; set; }
        public string numero { get; set; }
        public string TipoRPS { get; set; }
        public System.DateTime fechaEmision { get; set; }
        public string horaEmision { get; set; }
        public string StatusRPS { get; set; }
        public string TributacaoRPS { get; set; }
        public decimal montoTotalVenta { get; set; }
        public decimal descuentoGlobalMonto { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorIR { get; set; }
        public decimal ValorCSLL { get; set; }
        public string CodigoServico { get; set; }
        public decimal AliquotaServicos { get; set; }
        public bool ISSRetido { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string InscricaoMunicipalTomador { get; set; }
        public string InscricaoEstadualTomador { get; set; }
        public string RazaoSocialTomador { get; set; }
        public string Emisor_TipoLogradouro { get; set; }
        public string Emisor_Logradouro { get; set; }
        public string Emisor_NumeroEndereco { get; set; }
        public string Emisor_Bairro { get; set; }
        public string Emisor_Cidade { get; set; }
        public string Emisor_CEP { get; set; }
        public string Emisor_UF { get; set; }
        public string EmailTomador { get; set; }
        public string Concepto { get; set; }
        public decimal ValorCargaTributaria { get; set; }
        public decimal PercentualCargaTributaria { get; set; }
        public string FonteCargaTributaria { get; set; }
        public decimal ValorTotalRecebido { get; set; }

    }

}
