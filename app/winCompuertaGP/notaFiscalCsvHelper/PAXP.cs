using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace notaFiscalCsvHelper
{
    class PAXP
    {
        [Index(0)]
        public string Prefixo { get; set; }
        [Index(1)]
        public string InvNo { get; set; }
        [Index(2)]
        public DateTime InvDate { get; set; }

        [Index(3)]
        public string FixedT { get; set; }
        [Index(4)]
        public string Amount { get; set; }

        [Index(5)]
        public string CodigoServicio1 { get; set; }

        [Index(6)]
        public string CodigoServicio2 { get; set; }

        [Index(7)]
        public decimal AliquotaIss { get; set; }

        [Index(8)]
        public string cnpj_cp { get; set; }

        [Index(9)]
        public string Desconocido1 { get; set; }

        [Index(10)]
        public string RazaoSocial { get; set; }

        [Index(11)]
        public string TipoLogradouro { get; set; }

        [Index(12)]
        public string Logradouro { get; set; }

        [Index(13)]
        public string Numero { get; set; }

        [Index(14)]
        public string AdditionalInfo { get; set; }

        [Index(15)]
        public string Bairro { get; set; }

        [Index(16)]
        public string Cidade { get; set; }

        [Index(17)]
        public string Estado { get; set; }

        [Index(18)]
        public string Cep { get; set; }

        [Index(19)]
        public string email { get; set; }

        [Index(20)]
        public string Descricao { get; set; }

        [Index(21)]
        public DateTime DataVencimento { get; set; }

        [Index(22)]
        public string ImageDescription { get; set; }

        [Index(23)]
        public string ImageDescription2 { get; set; }

        [Index(24)]
        public string ValorUnitario { get; set; }

        [Index(25)]
        public string CCM { get; set; }

    }
}
