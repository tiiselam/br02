using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notaFiscalCsvHelper
{
    public class SerieC_RR
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
        public DateTime? DataVencimento { get; set; }

        [Index(22)]
        public string ImageNumber { get; set; }



        [Index(23)]
        public string Usage { get; set; }
        [Index(24)]
        public DateTime? InicioDireitoDeUso { get; set; }

        [Index(25)]
        public DateTime? FinDireitoDeUso { get; set; }
        [Index(26)]
        public string ValorUnitario { get; set; }

        [Index(27)]
        public string CCM { get; set; }


    }

    public sealed class SerieC_RRMap : ClassMap<SerieC_RR>
    {
        public SerieC_RRMap()
        {
            Map(m => m.Prefixo).Index(0);
            Map(m => m.InvNo).Index(1);
            Map(m => m.InvDate).Index(2);
            Map(m => m.FixedT).Index(3);
            Map(m => m.Amount).Index(4);
            Map(m => m.CodigoServicio1).Index(5);
            Map(m => m.CodigoServicio2).Index(6);
            Map(m => m.AliquotaIss).Index(7);
            Map(m => m.cnpj_cp).Index(8);
            Map(m => m.Desconocido1).Index(9);
            Map(m => m.RazaoSocial).Index(10);
            Map(m => m.TipoLogradouro).Index(11);
            Map(m => m.Logradouro).Index(12);
            Map(m => m.Numero).Index(13);
            Map(m => m.AdditionalInfo).Index(14);
            Map(m => m.Bairro).Index(15);
            Map(m => m.Cidade).Index(16);
            Map(m => m.Estado).Index(17);
            Map(m => m.Cep).Index(18);
            Map(m => m.email).Index(19);
            Map(m => m.Descricao).Index(20);
            Map(m => m.DataVencimento).Index(21);
            Map(m => m.ImageNumber).Index(22);
            Map(m => m.Usage).Index(23);
            Map(m => m.InicioDireitoDeUso).Index(24).ConvertUsing(row =>
                    {
                        var field = row.GetField(24);
                        if (field == "00/00/0000")
                        {
                            return DateTime.Parse("01/01/1900");
                        }

                        return DateTime.Parse(field);
                    }
                );

            Map(m => m.FinDireitoDeUso).Index(25).ConvertUsing(row =>
                    {
                        var field = row.GetField(24);
                        if (field == "00/00/0000")
                        {
                            return DateTime.Parse("01/01/1900");
                        }

                        return DateTime.Parse(field);
                    }
                );
            Map(m => m.ValorUnitario).Index(26);
            Map(m => m.CCM).Index(27);
        }
    }
}
