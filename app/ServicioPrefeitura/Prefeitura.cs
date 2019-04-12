using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using cfdiEntidadesGP;
using Prefeitura.WService;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Text;
using Comun;


namespace Web_Service
{
    public class Cabeca
    {
        public class CPFCNPJRemet
        {
            public string CNPJ { get; set; }
            public CPFCNPJRemet() { }
        }

        public CPFCNPJRemet CPFCNPJRemetente { get; set; }
        public string transacao { get; set; }
        public string dtInicio { get; set; }
        public string dtFim { get; set; }
        public string QtdRPS { get; set; }
        public string ValorTotalServicos { get; set; }
        public string ValorTotalDeducoes { get; set; }

        public Cabeca() { }
    }

    public class RPSCompleto
    {
        public class Chave
        {
            public string InscricaoPrestador { get; set; }
            public string SerieRPS { get; set; }
            public string NumeroRPS { get; set; }
            public Chave() { }
        }

        public class CPFCNP
        {
            public string CPF { get; set; }
            public CPFCNP() { }
        }

        public class Endereco
        {
            public string TipoLogradouro { get; set; }
            public string Logradouro { get; set; }
            public string NumeroEndereco { get; set; }
            public string Bairro { get; set; }
            public string Cidade { get; set; }
            public string UF { get; set; }
            public string CEP { get; set; }
            public Endereco() { }
        }

        public string Assinatura { get; set; }
        public Chave ChaveRPS { get; set; }
        public string TipoRPS { get; set; }
        public string DataEmissao { get; set; }
        public string StatusRPS { get; set; }
        public string TributacaoRPS { get; set; }
        public string ValorServicos { get; set; }
        public string ValorDeducoes { get; set; }
        public string ValorPIS { get; set; }
        public string ValorCOFINS { get; set; }
        public string ValorINSS { get; set; }
        public string ValorIR { get; set; }
        public string ValorCSLL { get; set; }
        public string CodigoServico { get; set; }
        public string AliquotaServicos { get; set; }
        public string ISSRetido { get; set; }
        public CPFCNP CPFCNPJTomador { get; set; }
        public string InscricaoMunicipalTomador { get; set; }
        public string InscricaoEstadualTomador { get; set; }
        public string RazaoSocialTomador { get; set; }
        public Endereco EnderecoTomador { get; set; }
        public string EmailTomador { get; set; }
        public string Discriminacao { get; set; }
        public string ValorCargaTributaria { get; set; }
        public string PercentualCargaTributaria { get; set; }
        public string FonteCargaTributaria { get; set; }
        public string ValorTotalRecebido { get; set; }
        

    }

    public class PedidoEnvioLoteRPS
    {
        public int Soptype { get; set; }
        public string SopNumbe { get; set; }
        public Cabeca Cabecalho { get; set; }
        public RPSCompleto RPS { get; set; }
        public PedidoEnvioLoteRPS() { }
    }

    public class FacturasProcesadas
    {
        public FacturasProcesadas()
        {

        }
        public int SopType { get; set; }
        public string SopNumbe { get; set; }
        public bool Status { get; set; }
        public string Archivo { get; set; }
        public string MsjError { get; set; }
    }



    public class WebServicesNfe
    {
        // cfdiGettyBrasilPrefeituraWS.Nfe_WebService.TesteEnvioLoteRPSRequest();
        public System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;

        public WebServicesNfe()
        {
            //ServicioWS.Endpoint.Address = new System.ServiceModel.EndpointAddress(URLwebServPAC) ;
        }

        public PedidoEnvioLoteRPS GeneraDatosRPS(GBRADocumentoVentaGP documentoGP)
        {
            string Assinatura;
            //Defino la cultura para el punto decimal a hacer el ToString. ESto define . como separador decimal.

            try
            {
                // Inicializo el objeto donde voy a guardar todos los datos del RPS
                PedidoEnvioLoteRPS Pedido = new PedidoEnvioLoteRPS();

                //Esto se agrega para tener rastro de la factura para los errores.
                Pedido.Soptype = documentoGP.DocVenta.soptype;
                Pedido.SopNumbe = documentoGP.DocVenta.SOPNUMBE;
                
                //Cabecalho
                Pedido.Cabecalho = new Cabeca();
                {
                    Pedido.Cabecalho.CPFCNPJRemetente = new Cabeca.CPFCNPJRemet();

                    Pedido.Cabecalho.CPFCNPJRemetente.CNPJ = documentoGP.DocVenta.CPFCNPJRemetente;//"02195059000108";
                    if (documentoGP.DocVenta.transacao == 0) { Pedido.Cabecalho.transacao = "false"; }
                    Pedido.Cabecalho.dtInicio = documentoGP.DocVenta.fechaEmision.ToString(("yyyy-MM-dd"));
                    Pedido.Cabecalho.dtFim = documentoGP.DocVenta.fechaEmision.ToString(("yyyy-MM-dd"));
                    Pedido.Cabecalho.QtdRPS = documentoGP.DocVenta.QtdRPS.ToString();
                    Pedido.Cabecalho.ValorTotalServicos = documentoGP.DocVenta.montoTotalVenta.ToString();
                    Pedido.Cabecalho.ValorTotalDeducoes = documentoGP.DocVenta.descuentoGlobalMonto.ToString();
                }

                // RPS
                Pedido.RPS = new RPSCompleto();
                {
                    Assinatura = documentoGP.DocVenta.InscricaoPrestador +
                             documentoGP.DocVenta.serie +
                             documentoGP.DocVenta.numero +
                             documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd") +
                             documentoGP.DocVenta.TributacaoRPS +
                             documentoGP.DocVenta.StatusRPS +
                             documentoGP.DocVenta.ISSRetido +
                             documentoGP.DocVenta.ValorTotalRecebido.ToString("0.00", nfi) +
                             documentoGP.DocVenta.ValorCargaTributaria.ToString("0.00", nfi) +
                             documentoGP.DocVenta.CodigoServico +
                             documentoGP.DocVenta.indicadorTomador +
                             documentoGP.DocVenta.CPFCNPJTomador +
                             documentoGP.DocVenta.indicadorIntermediario +
                             documentoGP.DocVenta.CPFCNPJIntermediario +
                             documentoGP.DocVenta.ISSRetidoIntermediario
                             ; //documentoGP.DocVenta.ISSRetidoIntermediario
                               //PAsar a Base64 el string 64

                    Pedido.RPS.Assinatura = GetHash1(Assinatura);
                    Pedido.RPS.ChaveRPS = new RPSCompleto.Chave();
                    {
                        Pedido.RPS.ChaveRPS.InscricaoPrestador = documentoGP.DocVenta.InscricaoPrestador;
                        Pedido.RPS.ChaveRPS.SerieRPS = documentoGP.DocVenta.serie;
                        Pedido.RPS.ChaveRPS.NumeroRPS = documentoGP.DocVenta.numero;
                    }
                    if (documentoGP.DocVenta.TipoRPS == "1") { Pedido.RPS.TipoRPS = "RPS"; }
                    Pedido.RPS.DataEmissao = documentoGP.DocVenta.fechaEmision.ToString("yyyy-MM-dd");
                    Pedido.RPS.StatusRPS = documentoGP.DocVenta.StatusRPS;
                    Pedido.RPS.TributacaoRPS = documentoGP.DocVenta.TributacaoRPS;
                    Pedido.RPS.ValorServicos = documentoGP.DocVenta.montoTotalVenta.ToString("0.00", nfi);
                    Pedido.RPS.ValorDeducoes = documentoGP.DocVenta.descuentoGlobalMonto.ToString("0.00", nfi);
                    Pedido.RPS.ValorPIS = documentoGP.DocVenta.ValorPIS.ToString();
                    Pedido.RPS.ValorCOFINS = documentoGP.DocVenta.ValorCOFINS.ToString();
                    Pedido.RPS.ValorINSS = documentoGP.DocVenta.ValorINSS.ToString();
                    Pedido.RPS.ValorIR = documentoGP.DocVenta.ValorIR.ToString();
                    Pedido.RPS.ValorCSLL = documentoGP.DocVenta.ValorCSLL.ToString();
                    Pedido.RPS.CodigoServico = documentoGP.DocVenta.CodigoServico;
                    Pedido.RPS.AliquotaServicos = documentoGP.DocVenta.AliquotaServicos;
                    Pedido.RPS.ISSRetido = documentoGP.DocVenta.ISSRetido;
                    Pedido.RPS.CPFCNPJTomador = new RPSCompleto.CPFCNP();
                    {
                        Pedido.RPS.CPFCNPJTomador.CPF = documentoGP.DocVenta.CPFCNPJTomador.Replace("-", "").Replace("/", "").Replace(".", "");
                    }
                    Pedido.RPS.InscricaoMunicipalTomador = documentoGP.DocVenta.InscricaoMunicipalTomador;
                    Pedido.RPS.InscricaoEstadualTomador = documentoGP.DocVenta.InscricaoEstadualTomador;
                    Pedido.RPS.RazaoSocialTomador = documentoGP.DocVenta.RazaoSocialTomador;
                    Pedido.RPS.EnderecoTomador = new RPSCompleto.Endereco();
                    {
                        Pedido.RPS.EnderecoTomador.TipoLogradouro = documentoGP.DocVenta.Emisor_TipoLogradouro;
                        Pedido.RPS.EnderecoTomador.Logradouro = documentoGP.DocVenta.Emisor_Logradouro;
                        Pedido.RPS.EnderecoTomador.NumeroEndereco = documentoGP.DocVenta.Emisor_NumeroEndereco;
                        Pedido.RPS.EnderecoTomador.Bairro = documentoGP.DocVenta.Emisor_Bairro;
                        Pedido.RPS.EnderecoTomador.Cidade = documentoGP.DocVenta.Emisor_Cidade;
                        Pedido.RPS.EnderecoTomador.UF = documentoGP.DocVenta.Emisor_UF;
                        Pedido.RPS.EnderecoTomador.CEP = documentoGP.DocVenta.Emisor_CEP;
                    }
                    Pedido.RPS.EmailTomador = documentoGP.DocVenta.EmailTomador;

                    Pedido.RPS.Discriminacao = documentoGP.DocVenta.Concepto;
                    Pedido.RPS.ValorCargaTributaria = documentoGP.DocVenta.ValorCargaTributaria.ToString("0.00", nfi);
                    Pedido.RPS.PercentualCargaTributaria = documentoGP.DocVenta.PercentualCargaTributaria.ToString("0.00", nfi);
                    Pedido.RPS.FonteCargaTributaria = documentoGP.DocVenta.FonteCargaTributaria;
                    Pedido.RPS.ValorTotalRecebido = documentoGP.DocVenta.ValorTotalRecebido.ToString("0.00", nfi);
                }

                return Pedido;


            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);

            }
        }

        public string EnviaDatosPrefeitura(PedidoEnvioLoteRPS Pedido)
        {
            var XML_Prefeitura = new EnvioLoteRPSRequest();
            EnvioLoteRPSResponse Servicio_response = new EnvioLoteRPSResponse();

            XmlSerializer serializador = new XmlSerializer(typeof(PedidoEnvioLoteRPS));
            StringWriter XMLescritor = new StringWriter();

            serializador.Serialize(XMLescritor, Pedido);



            LoteNFeSoapClient Servicio = new LoteNFeSoapClient();

            X509Certificate2 cert = new X509Certificate2();
            cert = GetCertificate();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = GetXMLSigned(XMLescritor.ToString(), cert);

            XML_Prefeitura.VersaoSchema = 3;
            XML_Prefeitura.MensagemXML = xmlDoc.OuterXml;

            //StreamWriter ArchSalida = new StreamWriter("C:\\DESA\\TIISELAM\\PREFEITURA\\ARCHIVO_PREFEITURA.xml");

            xmlDoc.Save("C:\\DESA\\TIISELAM\\PREFEITURA\\ARCHIVO_PREFEITURA.xml");

            ///return XML_Prefeitura.MensagemXML;
            try
            {
                X509Store store = new X509Store(StoreLocation.CurrentUser);
                Servicio.ClientCredentials.ClientCertificate.Certificate = cert;

                //   Servicio.ClientCredentials.ClientCertificate.SetCertificate(cert.SubjectName.Name,  store.Location,StoreName.TrustedPeople);
                //Servicio.ClientCredentials.ClientCertificate.Certificate = cert;



                Servicio_response.RetornoXML = Servicio.TesteEnvioLoteRPS(XML_Prefeitura.VersaoSchema, XML_Prefeitura.MensagemXML);

                ;

                return XML_Prefeitura.MensagemXML;



                //       var response = Servicio.TesteEnvioLoteRPS(XML_Prefeitura.VersaoSchema, XML_Prefeitura.MensagemXML);
                //        return response;
                //      if (response == "O")
                //      {
                //          ;
                //byte[] converbyte = Convert.FromBase64String(response.xml.ToString());
                //return System.Text.Encoding.UTF8.GetString(converbyte);
                //return response.xml.ToString();

                /*return "Mensaje XML: " + response.mensaje + Environment.NewLine +
                                                   "Código error: " + response.codigo + Environment.NewLine +
                                                   "Estatus: " + response.estatus + Environment.NewLine +
                                                   "Hora: " + response.hora + Environment.NewLine +
                                                   "Id Transacción: " + response.idtransaccion + Environment.NewLine +
                                                   "Numeración: " + response.numeracion + Environment.NewLine +
                                                    "CRC: " + response.crc + Environment.NewLine +
                                                   "DebugXML: " + debug_xml + Environment.NewLine  + 
                                                   "XML: " + converbyte.ToString();*/


                //    }
                //    else
                //    {

                //throw new Exception("Mensaje: " + response.mensaje + Environment.NewLine +
                /*return "Mensaje Error XML: " + response.mensaje + Environment.NewLine +
                                                     "Código error: " + response.codigo + Environment.NewLine +
                                                    "Estatus: " + response.estatus + Environment.NewLine +
                                                    "Hora: " + response.hora + Environment.NewLine +
                                                    "Id Transacción: " + response.idtransaccion + Environment.NewLine +
                                                    "Numeración: " + response.numeracion + Environment.NewLine +
                                                    "CRC: " + response.crc + Environment.NewLine + 
                                                    "DebugXML: " + debug_xml + Environment.NewLine;*/
                //  }
            }
            //catch (Exception ex)
            catch (Exception ex)
            {
                return "EX: " + ex.Message + Servicio_response.RetornoXML;
            }

        }

        public string GuardaArchivoTxt(string ruta, string nombreArchivo, string extension, string contenido)
        {
            string rutaYNomArchivo = ruta + nombreArchivo + extension;

            try
            {
                if (!Directory.Exists(ruta) && !string.IsNullOrEmpty(ruta))
                {
                    DirectoryInfo di = Directory.CreateDirectory(ruta);
                }

                using (StreamWriter Writer = new StreamWriter(rutaYNomArchivo))
                {
                    Writer.WriteLine(contenido);
                }

                return rutaYNomArchivo;

            }
            catch (DirectoryNotFoundException)
            {
                string smsj = "Verifique la existencia de la carpeta indicada en la configuración de Ruta de archivos Xml de GP. La ruta de la carpeta no existe: " + rutaYNomArchivo;
                throw new DirectoryNotFoundException(smsj);
            }
            catch (IOException)
            {
                string smsj = "Verifique permisos de escritura en la carpeta: " + rutaYNomArchivo + ". No se pudo guardar el archivo xml.";
                throw new IOException(smsj);
            }
        }

        public Tuple<string, string, decimal, decimal> PreparaDatosArchivoTxt(Web_Service.PedidoEnvioLoteRPS documentoRps) 
        {
            string Cabecera, Detalle;
            string Errores = ""+ Environment.NewLine;
            decimal Total_Servicios = 0, Total_Deducciones = 0;
          

         
                    Errores = "";
                    Cabecera = "1"; //Tipo de Registro
                    Cabecera += "001"; // Version de ARchivo
                    if (documentoRps.RPS.ChaveRPS.InscricaoPrestador.Length > 8)
                    {
                        Errores += "\tError: Longitud de Campo InscipcaoPesestador es mayor al permitido"+Environment.NewLine;
                    }
                    else
                    {
                        Cabecera += documentoRps.RPS.ChaveRPS.InscricaoPrestador.Substring(0, 8); // Inscripcion Municipal Prestador
                    }
                    Cabecera += documentoRps.Cabecalho.dtInicio.Replace("-", "");
                    Cabecera += documentoRps.Cabecalho.dtFim.Replace("-", "");
                    // Cabecera += "\n\r";

                    Detalle = "2"; //Tipo de Registro
                    Detalle += documentoRps.RPS.TipoRPS.PadRight(5);
                    if (documentoRps.RPS.ChaveRPS.SerieRPS.Length > 5)
                    {
                        Errores += "\tError: Longitud de Campo SerieRPS es mayor al permitido\n\r";
                    }
                    else
                    {
                        Detalle += documentoRps.RPS.ChaveRPS.SerieRPS.PadRight(5);
                    }
                    Detalle += documentoRps.RPS.ChaveRPS.NumeroRPS.PadLeft(12, '0');
                    Detalle += documentoRps.RPS.DataEmissao.Replace("-", "");
                    if (documentoRps.RPS.TributacaoRPS == null ||
                         documentoRps.RPS.TributacaoRPS.Length == 0)
                    {
                        Errores += "\tError: El campo TributacaoRPS no puede ser nulo" + Environment.NewLine;
                    }
                    else
                    {
                        Detalle += documentoRps.RPS.TributacaoRPS;
                    }
                    Detalle += documentoRps.RPS.ValorServicos.Replace(".", "").PadLeft(15, '0');
                    Detalle += documentoRps.RPS.ValorDeducoes.Replace(".", "").PadLeft(15, '0');

            Total_Servicios = Convert.ToDecimal(documentoRps.RPS.ValorServicos.Replace(".", ""));
            Total_Deducciones = Convert.ToDecimal(documentoRps.RPS.ValorDeducoes.Replace(".", ""));

            //Agregar Contro e Codigo de SErvicio
            if (documentoRps.RPS.CodigoServico == null ||
                        documentoRps.RPS.CodigoServico.Length == 0)
                    {
                        Errores += "\tError: El campo CodigoCodigoServico no puede ser nulo o vacio" + Environment.NewLine;
            }
                    else if (documentoRps.RPS.CodigoServico.Length > 5)
                    {
                        Errores += "\tError: La longitud del campo CodigoServico es mayor al permitido. 5 posiciones." + Environment.NewLine;
            }
                    else Detalle += documentoRps.RPS.CodigoServico.PadRight(5);

                    //Detalle += "TEST " + documentoRps.RPS.AliquotaServicos + "FIN TEST";
                    Detalle += (Convert.ToDecimal(documentoRps.RPS.AliquotaServicos, nfi) * 100).ToString("0").PadLeft(4, '0');

                    //Agregar control de ISS RETido
                    if (documentoRps.RPS.ISSRetido == null ||
                              documentoRps.RPS.ISSRetido.Length == 0)
                    {
                        Errores += "\tError: El campo ISSRetido no puede ser nulo o vacio" + Environment.NewLine;
                    }
                    else //if (documentoRps.RPS.ISSRetido == "true") Detalle += "1"; else Detalle += "2";
                        Detalle += documentoRps.RPS.ISSRetido;

                    //Indicador longitud CPF
                    if (documentoRps.RPS.CPFCNPJTomador.CPF.Length == 14) Detalle += "1";
                    else if (documentoRps.RPS.CPFCNPJTomador.CPF.Length == 11) Detalle += "2";
                    else if (documentoRps.RPS.CPFCNPJTomador.CPF.Length == 0) Detalle += "3";
                    else
                    {
                        Errores += "\tError: EL Numero de CPF/CNPJ no tiene una longitud valida" + Environment.NewLine;

                    }
                    Detalle += documentoRps.RPS.CPFCNPJTomador.CPF;

                    // Indicador Tomador
                    if (documentoRps.RPS.InscricaoMunicipalTomador.Length > 8)
                    {
                        Errores += "\tError: La longitud del campo InscricaoMunicipalTomador es mayor al permitido" + Environment.NewLine;
                    }
                    else
                    {
                        Detalle += documentoRps.RPS.InscricaoMunicipalTomador.PadLeft(8);
                    }
                    if (documentoRps.RPS.InscricaoEstadualTomador.Length > 12)
                    {
                        Errores += "\tError: La longitud del campo InscricaoEstadualTomador es mayor al permitido" + Environment.NewLine;
            }
                    else
                    {
                        Detalle += documentoRps.RPS.InscricaoEstadualTomador.PadLeft(12);
                    }
                    if (documentoRps.RPS.RazaoSocialTomador.Length > 75)
                    {
                        Errores += "\tError: La longitud del campo RazaoSocialTomador es mayor al permitido" + Environment.NewLine;
            }
                    else
                    {
                        Detalle += documentoRps.RPS.RazaoSocialTomador.PadRight(75);
                    }
                    Detalle += documentoRps.RPS.EnderecoTomador.TipoLogradouro.PadRight(3); //?????
                    Detalle += documentoRps.RPS.EnderecoTomador.Logradouro.PadRight(50).Substring(0, 50);
                    Detalle += documentoRps.RPS.EnderecoTomador.NumeroEndereco.PadRight(10).Substring(0, 10);
                    Detalle += "".PadLeft(30);
                    Detalle += documentoRps.RPS.EnderecoTomador.Bairro.PadRight(30).Substring(0, 30);
                    Detalle += documentoRps.RPS.EnderecoTomador.Cidade.PadRight(50).Substring(0, 50);
                    Detalle += documentoRps.RPS.EnderecoTomador.UF.PadRight(2).Substring(0, 2);
                    Detalle += documentoRps.RPS.EnderecoTomador.CEP.PadRight(8).Substring(0, 8);
                    Detalle += documentoRps.RPS.EmailTomador.PadRight(75).Substring(0, 75);
                    if (documentoRps.RPS.Discriminacao == null ||
                        documentoRps.RPS.Discriminacao.Length == 0)
                    {
                        Errores += "\tError: El campo Discriminacao no puede ser nulo o vacio" + Environment.NewLine;
                    }
                    else Detalle += Utiles.Izquierda(documentoRps.RPS.Discriminacao, 1000);

                    //Detalle += "\n\r";
                  

                    if (Errores != "")
                    {
                        throw new ArgumentException(Errores);
                       
                    }
                   

                return new Tuple<string, string, decimal, decimal>(Cabecera, Detalle,Total_Servicios,Total_Deducciones);

          

        }

        public IList<FacturasProcesadas> EnviarDatosArchivo(IList<Web_Service.PedidoEnvioLoteRPS> documentosRps, string Archivo,bool ArchivoxFactura) //PedidoEnvioLoteRPS Pedido
        {
            string Cabecera, Detalle,Detalle_Archivo="",Cabecera_Archivo="";
            string Errores="";
            string MinFecha="9999-99-99", MaxFecha="00-00-0000";

            IList<FacturasProcesadas> Procesadas = new  List<FacturasProcesadas>();
            
            try
            {
                StreamWriter ArchSalidaGral = new StreamWriter(Archivo);

                foreach (Web_Service.PedidoEnvioLoteRPS Pedido in documentosRps)
                {
                    Errores = "";
                    Cabecera = "1"; //Tipo de Registro
                    Cabecera += "001"; // Version de ARchivo
                    if (Pedido.RPS.ChaveRPS.InscricaoPrestador.Length > 8)
                    {
                        Errores += "Error: Longitud de Campo InscipcaoPesestador es mayor al permitido\n\r";
                    }
                    else
                    {
                        Cabecera += Pedido.RPS.ChaveRPS.InscricaoPrestador.Substring(0, 8); // Inscripcion Municipal Prestador
                    }
                    Cabecera_Archivo = Cabecera;
                    Cabecera += Pedido.Cabecalho.dtInicio.Replace("-", "");
                    Cabecera += Pedido.Cabecalho.dtFim.Replace("-", "");
                    // Cabecera += "\n\r";

                    Detalle = "2"; //Tipo de Registro
                    Detalle += Pedido.RPS.TipoRPS.PadRight(5);
                    if (Pedido.RPS.ChaveRPS.SerieRPS.Length > 5)
                    {
                        Errores += "Error: Longitud de Campo SerieRPS es mayor al permitido\n\r";
                    }
                    else
                    {
                        Detalle += Pedido.RPS.ChaveRPS.SerieRPS.PadRight(5);
                    }
                    Detalle += Pedido.RPS.ChaveRPS.NumeroRPS.PadLeft(12, '0');
                    Detalle += Pedido.RPS.DataEmissao.Replace("-", "");
                    if ( Pedido.RPS.TributacaoRPS == null ||
                         Pedido.RPS.TributacaoRPS.Length ==0 )
                    {
                        Errores += "Error: El campo TributacaoRPS no puede ser nulo\n\r";
                    }
                    else
                    {
                        Detalle += Pedido.RPS.TributacaoRPS;
                    }
                    Detalle += Pedido.RPS.ValorServicos.Replace(".", "").PadLeft(15, '0');
                    Detalle += Pedido.RPS.ValorDeducoes.Replace(".", "").PadLeft(15, '0');

                    //Agregar Contro e Codigo de SErvicio
                    if( Pedido.RPS.CodigoServico == null ||
                        Pedido.RPS.CodigoServico.Length == 0)
                    {
                        Errores += "Error: El campo CodigoCodigoServico no puede ser nulo o vacio\n\r";
                    }
                    else if (Pedido.RPS.CodigoServico.Length > 5)
                    {
                        Errores += "Error: La longitud del campo CodigoServico es mayor al permitido. 5 posiciones.\n\r";
                    }
                    else  Detalle += Pedido.RPS.CodigoServico.PadRight(5);

                    //Detalle += "TEST " + Pedido.RPS.AliquotaServicos + "FIN TEST";
                    Detalle += (Convert.ToDecimal(Pedido.RPS.AliquotaServicos, nfi) * 100).ToString("0").PadLeft(4, '0');

                    //Agregar control de ISS RETido
                    if (Pedido.RPS.ISSRetido == null ||
                              Pedido.RPS.ISSRetido.Length == 0)
                    {
                        Errores += "Error: El campo ISSRetido no puede ser nulo o vacio\n\r";
                    }
                    else //if (Pedido.RPS.ISSRetido == "true") Detalle += "1"; else Detalle += "2";
                        Detalle += Pedido.RPS.ISSRetido;

                    //Indicador longitud CPF
                    if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 14) Detalle += "1";
                    else if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 11) Detalle += "2";
                    else if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 0) Detalle += "3";
                    else
                    {
                        Errores = Errores += "Error: EL Numero de CPF/CNPJ no tiene una longitud valida\n\r";

                    }
                    Detalle += Pedido.RPS.CPFCNPJTomador.CPF;

                    // Indicador Tomador
                    if (Pedido.RPS.InscricaoMunicipalTomador.Length > 8)
                    {
                        Errores = Errores += "Error: La longitud del campo InscricaoMunicipalTomador es mayor al permitido\n\r";
                    }
                    else
                    {
                        Detalle += Pedido.RPS.InscricaoMunicipalTomador.PadLeft(8);
                    }
                    if (Pedido.RPS.InscricaoEstadualTomador.Length > 12)
                    {
                        Errores = Errores += "Error: La longitud del campo InscricaoEstadualTomador es mayor al permitido\n\r";
                    }
                    else
                    {
                        Detalle += Pedido.RPS.InscricaoEstadualTomador.PadLeft(12);
                    }
                    if (Pedido.RPS.RazaoSocialTomador.Length > 75)
                    {
                        Errores = Errores += "Error: La longitud del campo RazaoSocialTomador es mayor al permitido\n\r";
                    }
                    else
                    {
                        Detalle += Pedido.RPS.RazaoSocialTomador.PadRight(75);
                    }
                    Detalle += Pedido.RPS.EnderecoTomador.TipoLogradouro.PadRight(3); //?????
                    Detalle += Pedido.RPS.EnderecoTomador.Logradouro.PadRight(50).Substring(0, 50);
                    Detalle += Pedido.RPS.EnderecoTomador.NumeroEndereco.PadRight(10).Substring(0, 10);
                    Detalle += "".PadLeft(30);
                    Detalle += Pedido.RPS.EnderecoTomador.Bairro.PadRight(30).Substring(0, 30);
                    Detalle += Pedido.RPS.EnderecoTomador.Cidade.PadRight(50).Substring(0, 50);
                    Detalle += Pedido.RPS.EnderecoTomador.UF.PadRight(2).Substring(0, 2);
                    Detalle += Pedido.RPS.EnderecoTomador.CEP.PadRight(8).Substring(0, 8);
                    Detalle += Pedido.RPS.EmailTomador.PadRight(75).Substring(0, 75);
                    if (Pedido.RPS.Discriminacao == null ||
                        Pedido.RPS.Discriminacao.Length == 0)
                    {
                        Errores += "Error: El campo Discriminacao no puede ser nulo o vacio\n\r";
                    }
                    else  Detalle += Utiles.Izquierda(Pedido.RPS.Discriminacao, 1000);

                    Detalle += "\n\r";
                    // Termine de armar los strings
                    
                    //Armo Salida
                    FacturasProcesadas FProc = new FacturasProcesadas();
                    FProc.SopType = Pedido.Soptype;
                    FProc.SopNumbe = Pedido.SopNumbe;

                    if (Errores != "")
                    {
                        FProc.Status = false;
                        FProc.MsjError = Errores;
                    }
                    else
                    {
                        if (ArchivoxFactura)
                        {
                            StreamWriter ArchSalida = new StreamWriter(Archivo.Substring(1, Archivo.LastIndexOf('.') - 1) + "_" + Pedido.SopNumbe.Trim() + Archivo.Substring(Archivo.LastIndexOf('.'), 4));
                            ArchSalida.WriteLine(Cabecera);
                            ArchSalida.WriteLine(Detalle);
                            ArchSalida.Close();
                            FProc.Archivo = Archivo.Substring(1, Archivo.LastIndexOf('.') - 1) + "_" + Pedido.SopNumbe.Trim() + Archivo.Substring(Archivo.LastIndexOf('.'), 4);
                        }
                        else
                        {
                            if (String.Compare(Pedido.Cabecalho.dtInicio, MinFecha) < 0)
                            {
                                MinFecha = Pedido.Cabecalho.dtInicio;
                            }

                            if(String.Compare(Pedido.Cabecalho.dtFim, MaxFecha) > 0 )
                            {
                                MaxFecha = Pedido.Cabecalho.dtFim;
                            }
                            
                            Detalle_Archivo += Detalle + "\n\r";
                            FProc.Archivo = Archivo;
                        }
                                               
                        FProc.Status = true;
                    }

                    Procesadas.Add(FProc);
                }

                if (!ArchivoxFactura)
                {
                    Cabecera_Archivo += MinFecha.Replace("-", "");
                    Cabecera_Archivo += MaxFecha.Replace("-", "");
                    ArchSalidaGral.WriteLine(Cabecera_Archivo);
                    ArchSalidaGral.WriteLine(Detalle_Archivo);
                }

                ArchSalidaGral.Close();

                if (ArchivoxFactura)
                {
                    File.Delete(Archivo);
                }
                
                return Procesadas;

            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }

        }

        public string GetHash1(string stringtohash)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] inputBytes = (new UnicodeEncoding()).GetBytes(stringtohash);
            byte[] hash = sha1.ComputeHash(inputBytes);

            return Convert.ToBase64String(hash);
        }

        public X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert = new X509Certificate2();

            X509Store store = new X509Store(StoreLocation.CurrentUser);


            try
            {
                store.Open(OpenFlags.ReadOnly);

                // Place all certificates in an X509Certificate2Collection object.
                X509Certificate2Collection certCollection = store.Certificates;
                // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
                // currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, true);
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySerialNumber, "6d072cb1a718a8d0", false);
                if (signingCert.Count == 0)
                    return null;
                else
                    // Return the first certificate in the collection, has the right name and is current.
                    cert = signingCert[0];
                return cert;
                //return cert.Subject + cert.SubjectName;
            }
            finally
            {
                store.Close();
            }
        }

        public XmlDocument GetXMLSigned(string MensagemXML, X509Certificate2 cert)
        {
            //return XML_Prefeitura.MensagemXML;
            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(MensagemXML);

                RSA rsaKey = ((RSA)cert.PrivateKey);

                // Sign the XML document. 
                SignedXml signedXml = new SignedXml(xmlDoc);

                // Add the key to the SignedXml document.
                signedXml.SigningKey = rsaKey;

                // Create a reference to be signed.
                Reference reference = new Reference();
                reference.Uri = "";

                // Add an enveloped transformation to the reference.
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                KeyInfo keyInfo = new KeyInfo();
                KeyInfoX509Data keyInfoData = new KeyInfoX509Data(cert);
                //KeyInfoName kin = new KeyInfoName();
                //kin.Value = "Public key of certificate";
                //RSACryptoServiceProvider rsaprovider = (RSACryptoServiceProvider)cert.PublicKey.Key;
                //RSAKeyValue rkv = new RSAKeyValue(rsaprovider);
                // keyInfo.AddClause(kin);
                // keyInfo.AddClause(rkv);
                keyInfo.AddClause(keyInfoData);
                signedXml.KeyInfo = keyInfo;

                // Add the reference to the SignedXml object.
                signedXml.AddReference(reference);


                // Compute the signature.
                signedXml.ComputeSignature();



                // Get the XML representation of the signature and save
                // it to an XmlElement object.
                XmlElement xmlDigitalSignature = signedXml.GetXml();

                // Append the element to the XML document.
                xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

                return xmlDoc;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

