using CsvHelper;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notaFiscalCsvHelper
{
    public class LectorCSV
    {
        public Action<int, string> ProgressHandler;
        public void OnProgreso(int iAvance, string sMsj)
        {
            ProgressHandler?.Invoke(iAvance, sMsj);
        }

        public IEnumerable<ExcelPackage> ConvierteCsvAExcel(string carpetaOrigen, IEnumerable<string> lNombreArchivos, CultureInfo culInfo)
        {
            List<ExcelPackage> archivosXl = new List<ExcelPackage>();
            foreach (string archivoCsv in lNombreArchivos)
            {
                try
                {
                    using (var reader = new StreamReader(Path.Combine(carpetaOrigen, archivoCsv), Encoding.GetEncoding("windows-1254")))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            csv.Configuration.HasHeaderRecord = false;
                            csv.Configuration.MissingFieldFound = (headerNames, index, context) =>
                            {
                                OnProgreso(0, $"Los siguientes campos: ['{string.Join("', '", headerNames)}'] de la fila '{index}' no existen. ");
                            };

                            if (archivoCsv.ToUpper().Contains("RF"))
                            {
                                var records = csv.GetRecords<SerieB_RF>();
                                var xl = CreaExcel_RF(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                            else if (archivoCsv.ToUpper().Contains("RM"))
                            {
                                var records = csv.GetRecords<SerieC_RM>();
                                var xl = CreaExcel_RM(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                            else if (archivoCsv.ToUpper().Contains("RR"))
                            {
                                csv.Configuration.RegisterClassMap<SerieC_RRMap>();
                                //classMap.Map(row => row.InicioDireitoDeUso).ConvertUsing(row => Convert.ToDateTime(row.GetField(24).Replace("00/00/0000", "01/01/1900")));

                                var records = csv.GetRecords<SerieC_RR>();
                                
                                var xl = CreaExcel_RR(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                            else if (archivoCsv.ToUpper().Contains("ISTOCK"))
                            {
                                var records = csv.GetRecords<iStock>();
                                var xl = CreaExcel_iStock(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                            else if (archivoCsv.ToUpper().Contains("PAXP"))
                            {
                                var records = csv.GetRecords<PAXP>();
                                var xl = CreaExcel_paxp(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                            else if (archivoCsv.ToUpper().Contains("CUSTOM") || archivoCsv.ToUpper().Contains("PREMIUM"))
                            {
                                var records = csv.GetRecords<Premium>();
                                var xl = CreaExcel_Premium(records, archivoCsv, culInfo);
                                archivosXl.Add(xl);
                            }
                        }
                    }

                }
                catch (Exception csv)
                {
                    OnProgreso(0, csv.Message);
                }
            }
            return archivosXl;

        }

        private ExcelPackage CreaExcel_Premium(IEnumerable<Premium> records, string archivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Premium");
            int i = 1;
            try
            {
                foreach (Premium record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;
                    if (record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;

                    worksheet.Cells[i, 23].Value = record.ImageNumber;
                    worksheet.Cells[i, 24].Value = record.Usage;
                    worksheet.Cells[i, 25].Value = record.Industry;
                    worksheet.Cells[i, 26].Value = record.Protection;
                    worksheet.Cells[i, 27].Value = record.StartDate;
                    worksheet.Cells[i, 28].Value = record.EndDate;
                    worksheet.Cells[i, 29].Value = record.Territory;

                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 30].Value = unitprice;
                    worksheet.Cells[i, 31].Value = record.CCM;

                    worksheet.Cells[i, 32].Value = record.StartDate;
                    worksheet.Cells[i, 33].Value = record.EndDate;
                    worksheet.Cells[i, 34].Value = record.Usage;

                    //iStock, premium, paxp, RF, RR, RM deben tener unitprice, cod item, direccion1 en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat(record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(archivoCsv + ": Excepción en la fila " + i.ToString() + ", columna 5 o 30 " + " [CreaExcel] " + f.Message);
            }
            package.Workbook.Properties.Title = archivoCsv;
            return package;
        }

        private ExcelPackage CreaExcel_paxp(IEnumerable<PAXP> records, string archivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("paxp");
            int i = 1;
            try
            {
                foreach (PAXP record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;

                    if(record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;

                    worksheet.Cells[i, 23].Value = record.ImageDescription;
                    worksheet.Cells[i, 24].Value = record.ImageDescription2;

                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 25].Value = unitprice;
                    worksheet.Cells[i, 26].Value = record.CCM;

                    //iStock, premium, paxp, RF, RR, RM deben tener unitprice, cod item, direccion1 en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat(record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(archivoCsv + ": Excepción en la fila " + i.ToString() + ", columna 5 o 25 " + " [CreaExcel] " + f.Message);
            }
            package.Workbook.Properties.Title = archivoCsv;
            return package;
        }

        private ExcelPackage CreaExcel_iStock(IEnumerable<iStock> records, string archivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("iStock");
            int i = 1;
            try
            {
                foreach (iStock record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;
                    if (record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;

                    worksheet.Cells[i, 23].Value = record.ImageDescription;
                    worksheet.Cells[i, 24].Value = record.ImageDescription2;

                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 25].Value = unitprice;
                    worksheet.Cells[i, 26].Value = record.CCM;

                    //iStock, premium, paxp, RF, RR, RM deben tener unitprice, cod item, direccion1 en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat(record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(archivoCsv + ": Excepción en la fila " + i.ToString() + ", columna 5 o 25 " + " [CreaExcel] " + f.Message);
            }
            package.Workbook.Properties.Title = archivoCsv;
            return package;
        }

        private ExcelPackage CreaExcel_RF(IEnumerable<SerieB_RF> records, string nombreArchivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RF");
            int i = 1;
            try
            {
                foreach (SerieB_RF record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;
                    if (record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;
                    worksheet.Cells[i, 23].Value = record.ImageNumber;
                    worksheet.Cells[i, 24].Value = record.DescricaoColecao;

                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 25].Value = unitprice;
                    worksheet.Cells[i, 26].Value = record.CCM;

                    //RF, RR, RM deben tener unitprice en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat( record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(nombreArchivoCsv +": Excepción en la fila " + i.ToString() + ", columna 5 o 25 " + " [CreaExcel] "+ f.Message);
            }
            package.Workbook.Properties.Title = nombreArchivoCsv;
            return package;
        }

        private ExcelPackage CreaExcel_RM(IEnumerable<SerieC_RM> records, string nombreArchivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RM");
            int i = 1;
            try
            {
                foreach (SerieC_RM record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;
                    if (record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;
                    worksheet.Cells[i, 23].Value = record.ImageNumber;
                    worksheet.Cells[i, 24].Value = record.DescricaoColecao;

                    worksheet.Cells[i, 25].Value = record.Usage;
                    worksheet.Cells[i, 26].Value = record.Protecao;
                    worksheet.Cells[i, 27].Value = record.InicioDireitoDeUso;
                    worksheet.Cells[i, 28].Value = record.FinDireitoDeUso;
                    worksheet.Cells[i, 29].Value = record.Veiculacao;   //territorio

                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 30].Value = unitprice;
                    worksheet.Cells[i, 31].Value = record.CCM;
                    //RM y RR deben tener los mismos campos
                    worksheet.Cells[i, 32].Value = record.InicioDireitoDeUso;
                    worksheet.Cells[i, 33].Value = record.FinDireitoDeUso;
                    worksheet.Cells[i, 34].Value = record.Protecao +"-"+ record.Veiculacao + "-" + record.Usage;
                    //RF, RR, RM deben tener unitprice en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat(record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(nombreArchivoCsv + ": Excepción en la fila " + i.ToString() + ", columna 5 o 30 " + " [CreaExcel] " + f.Message);
            }
            package.Workbook.Properties.Title = nombreArchivoCsv;
            return package;
        }

        private ExcelPackage CreaExcel_RR(IEnumerable<SerieC_RR> records, string nombreArchivoCsv, CultureInfo culInfo)
        {
            var package = new ExcelPackage();
            Decimal unitprice = 0;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RR");
            int i = 1;
            try
            {
                foreach (SerieC_RR record in records)
                {
                    worksheet.Cells[i, 1].Value = record.Prefixo;
                    worksheet.Cells[i, 2].Value = record.InvNo;
                    worksheet.Cells[i, 3].Value = record.InvDate;
                    worksheet.Cells[i, 4].Value = record.FixedT;

                    unitprice = Convert.ToDecimal(record.Amount, culInfo);
                    //if (Decimal.TryParse(record.Amount, culInfo, out unitprice))
                    worksheet.Cells[i, 5].Value = unitprice;
                    //else
                    //    throw new FormatException("El monto es incorrecto en la fila " + i.ToString() + ", columna 5 " + " [CreaExcel]");

                    worksheet.Cells[i, 6].Value = record.CodigoServicio1;
                    worksheet.Cells[i, 7].Value = record.CodigoServicio2;
                    worksheet.Cells[i, 8].Value = record.AliquotaIss;
                    worksheet.Cells[i, 9].Value = record.cnpj_cp;
                    worksheet.Cells[i, 10].Value = record.Desconocido1;
                    worksheet.Cells[i, 11].Value = record.RazaoSocial;
                    worksheet.Cells[i, 12].Value = record.TipoLogradouro;
                    worksheet.Cells[i, 13].Value = record.Logradouro;
                    worksheet.Cells[i, 14].Value = record.Numero;
                    worksheet.Cells[i, 15].Value = record.AdditionalInfo;
                    worksheet.Cells[i, 16].Value = record.Bairro;
                    worksheet.Cells[i, 17].Value = record.Cidade;
                    worksheet.Cells[i, 18].Value = record.Estado;
                    worksheet.Cells[i, 19].Value = record.Cep;
                    worksheet.Cells[i, 20].Value = record.email;
                    worksheet.Cells[i, 21].Value = record.Descricao;
                    if (record?.DataVencimento != null)
                        worksheet.Cells[i, 22].Value = record.DataVencimento;
                    worksheet.Cells[i, 23].Value = record.ImageNumber;
                    worksheet.Cells[i, 24].Value = record.Usage;

                    worksheet.Cells[i, 25].Value = record.InicioDireitoDeUso;
                    worksheet.Cells[i, 26].Value = record.FinDireitoDeUso;
                    unitprice = Convert.ToDecimal(record.ValorUnitario, culInfo);
                    worksheet.Cells[i, 27].Value = unitprice;
                    worksheet.Cells[i, 28].Value = record.CCM;
                    //RM y RR deben tener los mismos campos:
                    worksheet.Cells[i, 32].Value = record.InicioDireitoDeUso;
                    worksheet.Cells[i, 33].Value = record.FinDireitoDeUso;
                    worksheet.Cells[i, 34].Value = record.Usage;
                    //RF, RR, RM deben tener unitprice en el mismo campo
                    worksheet.Cells[i, 35].Value = unitprice;
                    worksheet.Cells[i, 36].Value = record.Prefixo + record.CodigoServicio1 + record.CodigoServicio2;
                    worksheet.Cells[i, 37].Value = string.Concat(record.TipoLogradouro, " ", record.Logradouro);
                    worksheet.Cells[i, 38].Value = record.CCM;
                    i++;
                }

            }
            catch (Exception f)
            {
                throw new FormatException(nombreArchivoCsv+": Excepción en la fila " + i.ToString() + ", columna 5 o 27 " + " [CreaExcel]" + f.Message +" ie: "+ f.InnerException?.Message);
            }
            package.Workbook.Properties.Title = nombreArchivoCsv;
            return package;
        }
    }
}
