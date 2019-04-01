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
        public Cabeca Cabecalho { get; set; }
        public RPSCompleto RPS { get; set; }
        public PedidoEnvioLoteRPS() { }
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

                //Cabecalho
                Pedido.Cabecalho = new Cabeca();
                {
                    Pedido.Cabecalho.CPFCNPJRemetente = new Cabeca.CPFCNPJRemet();

                    Pedido.Cabecalho.CPFCNPJRemetente.CNPJ = "02195059000108";
                    Pedido.Cabecalho.transacao = "false";
                    Pedido.Cabecalho.dtInicio = documentoGP.DocVenta.fechaEmision.ToString(("yyyy-MM-dd"));
                    Pedido.Cabecalho.dtFim = documentoGP.DocVenta.fechaEmision.ToString(("yyyy-MM-dd"));
                    Pedido.Cabecalho.QtdRPS = "1";
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
                             "3" + //documentoGP.DocVenta.indicadorTomador +
                             documentoGP.DocVenta.CPFCNPJTomador +
                             "3" + //documentoGP.DocVenta.indicadorIntermediario +
                             "00000000000000" + //documentoGP.DocVenta.CPFCNPJIntermediario +
                             "N"
                             ; //documentoGP.DocVenta.ISSRetidoIntermediario
                               //PAsar a Base64 el string 64

                    Pedido.RPS.Assinatura = GetHash1(Assinatura);
                    Pedido.RPS.ChaveRPS = new RPSCompleto.Chave();
                    {
                        Pedido.RPS.ChaveRPS.InscricaoPrestador = documentoGP.DocVenta.InscricaoPrestador;
                        Pedido.RPS.ChaveRPS.SerieRPS = documentoGP.DocVenta.serie;
                        Pedido.RPS.ChaveRPS.NumeroRPS = documentoGP.DocVenta.numero;
                    }
                    Pedido.RPS.TipoRPS = "RPS";
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
            catch (Exception)
            {
                return null;

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

        public string EnviarDatosArchivo(PedidoEnvioLoteRPS Pedido, string Archivo)
        {
            string Cabecera, Detalle;
            string Errores="";

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
            Detalle += Pedido.RPS.TributacaoRPS;
            Detalle += Pedido.RPS.ValorServicos.Replace(".", "").PadLeft(15, '0');
            Detalle += Pedido.RPS.ValorDeducoes.Replace(".", "").PadLeft(15, '0');
            Detalle += Pedido.RPS.CodigoServico.PadRight(5);

            //Detalle += "TEST " + Pedido.RPS.AliquotaServicos + "FIN TEST";
            Detalle += (Convert.ToDecimal(Pedido.RPS.AliquotaServicos, nfi) * 100).ToString("0").PadLeft(4, '0');
            if (Pedido.RPS.ISSRetido == "true") Detalle += "1"; else Detalle += "2";

            //Indicador longitud CPF
            if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 14) Detalle += "1";
            else if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 11) Detalle += "2";
            else if (Pedido.RPS.CPFCNPJTomador.CPF.Length == 0) Detalle += "3";
            else
            {
                Errores = Errores += "Error: EL Numero de CPF/CNPJ no tiene una longitud valida\n\r";

            }

            // Indicador Tomador
            Detalle += Pedido.RPS.CPFCNPJTomador.CPF;
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
            Detalle += Pedido.RPS.EnderecoTomador.Logradouro.PadRight(50).Substring(0,50);
            Detalle += Pedido.RPS.EnderecoTomador.NumeroEndereco.PadRight(10).Substring(0,10);
            Detalle += "".PadLeft(30);
            Detalle += Pedido.RPS.EnderecoTomador.Bairro.PadRight(30).Substring(0,30);
            Detalle += Pedido.RPS.EnderecoTomador.Cidade.PadRight(50).Substring(0,50);
            Detalle += Pedido.RPS.EnderecoTomador.UF.PadRight(2).Substring(0,2);
            Detalle += Pedido.RPS.EnderecoTomador.CEP.PadRight(8).Substring(0,8);
            Detalle += Pedido.RPS.EmailTomador.PadRight(75).Substring(0,75);
            Detalle += Utiles.Izquierda( Pedido.RPS.Discriminacao,1000);
            Detalle += "\n\r";

            StreamWriter ArchSalida = new StreamWriter(Archivo);

            if (Errores != "")
            {
                return Errores;
            }
            
            ArchSalida.WriteLine(Cabecera);
            ArchSalida.WriteLine(Detalle);

            ArchSalida.Close();

            return "Archivo Generado OK" + "Arch: " + Errores;
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

