using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfdiEntidadesGP.Fake
{
    public class DocumentoVentaGP
    {
        vwCfdiGeneraDocumentoDeVenta _DocVenta;

        public DocumentoVentaGP()
        {
            _DocVenta = new vwCfdiGeneraDocumentoDeVenta();
        }

        public void GetDatosDocumentoVenta(String Sopnumbe, short Soptype)
        {
            _DocVenta.AliquotaServicos = 2.9M;
            _DocVenta.CodigoServico = "02961";
            _DocVenta.Concepto = "Cessão de 999 Imagens para uso em Material Didático conforme doc 99999";
            _DocVenta.CPFCNPJTomador = "01.404.158/0001-90";
            _DocVenta.descuentoGlobalMonto =0;
            _DocVenta.EmailTomador = "leila.melo@tvglobo.com.br";
            _DocVenta.emisorNombre = "getty brasil";
            _DocVenta.emisorNroDoc = "";
            _DocVenta.emisorTipoDoc = "3";
            _DocVenta.Emisor_Bairro = "Água Branca";
            _DocVenta.Emisor_CEP = "05036-001";
            _DocVenta.Emisor_Cidade = "São Paulo";
            _DocVenta.Emisor_Logradouro = "Santa Marina";
            _DocVenta.Emisor_NumeroEndereco = "1193";
            _DocVenta.Emisor_TipoLogradouro = "Av.";
            _DocVenta.Emisor_UF = "SP";
            _DocVenta.fechaEmision = Convert.ToDateTime( "29/10/2018");
            _DocVenta.FonteCargaTributaria = "IBPT";
            _DocVenta.InscricaoEstadualTomador = "";
            _DocVenta.InscricaoMunicipalTomador = "";
            _DocVenta.InscricaoPrestador = "31000000";
            _DocVenta.ISSRetido = true;
            _DocVenta.montoTotalVenta = 1500;
            _DocVenta.numero = "52288";
            _DocVenta.serie = "B";
            _DocVenta.sopnumbe = "B-52288";
            _DocVenta.soptype = 3;
            _DocVenta.StatusRPS = "N";
            _DocVenta.TipoRPS = "1";
            _DocVenta.TributacaoRPS = "T";
            _DocVenta.ValorCargaTributaria =0;
            _DocVenta.ValorCOFINS = 0;
            _DocVenta.ValorCSLL = 0;
            _DocVenta.ValorINSS = 0;
            _DocVenta.ValorIR = 0;
            _DocVenta.ValorPIS = 0;
            _DocVenta.ValorTotalRecebido = 1500;


        }
    }
}
