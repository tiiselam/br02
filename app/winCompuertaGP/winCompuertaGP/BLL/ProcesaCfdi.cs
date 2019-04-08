using Comun;
using MaquinaDeEstados;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using cfdiEntidadesGP;
using Prefeitura;

namespace cfd.FacturaElectronica
{
    public class ProcesaCfdi
    {
        private string _usuario;
        private String nroTicket=String.Empty;
        private String _mensajeSunat = String.Empty;

        //vwCfdTransaccionesDeVenta trxVenta;

        //internal vwCfdTransaccionesDeVenta TrxVenta
        //{
        //    get
        //    {
        //        return trxVenta;
        //    }

        //    set
        //    {
        //        trxVenta = value;
        //    }
        //}
        public delegate void LogHandler(int iAvance, string sMsj);
        public event LogHandler Progreso;

        /// <summary>
        /// Dispara el evento para actualizar la barra de progreso
        /// </summary>
        /// <param name="iProgreso"></param>
        public void OnProgreso(int iAvance, string sMsj)
        {
            if (Progreso != null)
                Progreso(iAvance, sMsj);
        }

        public ProcesaCfdi(string usuario)
        {
            _usuario = usuario;
        }

        /// <summary>
        /// Genera archivo texto para la prefectura de Sao Paulo
        /// </summary>
        public void GeneraDocumentoTxt(IList<vwCfdiTransaccionesDeVenta> listaTransaccionesVenta, MainDB DocumentosGP, Web_Service.WebServicesNfe serviciosPrefectura)
        {
            //string xmlFactura = string.Empty;
            string rutaYNom = string.Empty;
            try
            {
                String msj = String.Empty;
                int errores = 0; int i = 1;
                //ReglasME_old maquina = new ReglasME_old(true, false, false, false);
                OnProgreso(1, "INICIANDO GENERACION DE ARCHIVO TEXTO...");
                IList<Web_Service.PedidoEnvioLoteRPS> documentosRps = new List<Web_Service.PedidoEnvioLoteRPS>();
                string sTimeStamp = System.DateTime.Now.ToString("yyMMddHHmmss");
                string nombreArchivo = "NFPrefeitura_" + sTimeStamp;
                string extension = ".txt";

                foreach (vwCfdiTransaccionesDeVenta trxVenta in listaTransaccionesVenta)
                {
                    rutaYNom = Path.Combine(trxVenta.rutaXml.Trim(), nombreArchivo + extension);
                    msj = String.Empty;
                    //String accion = "EMITE TXT";
                    try
                    {
                        string tipoMEstados = "DOCVENTA-"+ trxVenta.estadoContabilizado;
                        trxVenta.CicloDeVida = new Maquina(trxVenta.estadoActual, trxVenta.regimen, trxVenta.voidstts, "emisor", tipoMEstados);
                        if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoGeneraTxt, 1))
                        {
                            var docGpBrasil = DocumentosGP.GetDatosDocumentoVenta(trxVenta.sopnumbe, trxVenta.soptype);
                            documentosRps.Add(serviciosPrefectura.GeneraDatosRPS(docGpBrasil));

                            DocumentosGP.CreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, rutaYNom, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _usuario, string.Empty, trxVenta.CicloDeVida.targetSingleStatus,
                                                        trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));

                            DocumentosGP.ActualizaOCreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, rutaYNom, trxVenta.CicloDeVida.idxTargetSingleStatus.ToString(), _usuario, string.Empty,
                                                        Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor,
                                                        trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                        }
                        else
                        {
                            msj += trxVenta.CicloDeVida.sMsj;
                        }
                        //{
                        //    if (trxVenta.voidstts.Equals(1))
                        //    { 
                        //        msj = "Anulado en GP y marcado como emitido.";
                        //        OnProgreso(1, msj);
                        //        DocumentosGP.CreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, "Anulado en GP", Maquina.idxBaseAnulado, _usuario, "", Maquina.estadoBaseAnulado, Maquina.binStatusBaseAnulado, msj.Trim());

                        //        DocumentosGP.ActualizaOCreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, "Anulado en GP", Maquina.idxBaseAnulado, _usuario, string.Empty,
                        //                                    Maquina.estadoBaseEmisor, Maquina.estadoBaseEmisor,
                        //                                    trxVenta.CicloDeVida.targetBinStatus, trxVenta.CicloDeVida.EstadoEnPalabras(trxVenta.CicloDeVida.targetBinStatus));
                        //    }
                        //}
                    }
                    catch (InvalidOperationException ae)
                    {
                        msj = ae.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (TimeoutException ae)
                    {
                        string imsj = ae.InnerException == null ? "" : ae.InnerException.ToString();
                        msj = ae.Message + " " + imsj + Environment.NewLine + ae.StackTrace;
                        errores++;
                    }
                    catch (DirectoryNotFoundException dnf)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.ruta_clave + " Verifique si existe la carpeta." + Environment.NewLine;
                        DocumentosGP.CreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, msj, "errCarpeta", _usuario, string.Empty, Maquina.estadoBaseError,
                                                    trxVenta.CicloDeVida.targetBinStatus, dnf.Message);
                        msj += dnf.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (IOException io)
                    {
                        msj = "El comprobante fue emitido, pero no se pudo guardar el archivo en: " + trxVenta.ruta_clave + " Verifique permisos a la carpeta." + Environment.NewLine;
                        DocumentosGP.CreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, msj, "errIO", _usuario, string.Empty, Maquina.estadoBaseError,
                                                    trxVenta.CicloDeVida.targetBinStatus, io.Message);
                        msj += io.Message + Environment.NewLine;
                        errores++;
                    }
                    catch (Exception lo)
                    {
                        string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
                        msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
                        DocumentosGP.CreaLogFactura(trxVenta.soptype, trxVenta.sopnumbe, msj, "errDesconocido", _usuario, string.Empty, Maquina.estadoBaseError,
                                                    trxVenta.CicloDeVida.targetBinStatus, lo.Message);
                        errores++;
                    }
                    finally
                    {
                        OnProgreso(i * 100 / listaTransaccionesVenta.Count, "Doc:" + trxVenta.sopnumbe + " " + msj.Trim() + Environment.NewLine);              //Notifica al suscriptor
                        i++;
                    }
                    if (errores > 10) break;
                }

                if (documentosRps.Count > 0)
                {
                    string ArchivoSal = serviciosPrefectura.EnviarDatosArchivo(documentosRps.FirstOrDefault(), rutaYNom);
                }
                else
                    OnProgreso(100, "No se generó el archivo porque ningún documento de venta tenía el status correcto.");
            }
            finally
            {
                OnProgreso(100, "-----------");
            }
            OnProgreso(100, "Proceso finalizado!");
        }

        //public async Task<string> ProcesaConsultaStatusAsync(ICfdiMetodosWebService servicioTimbre)
        //{
        //    string resultadoSunat = string.Empty;
        //    try
        //    {
        //        String msj = String.Empty;
        //        String eBinario = String.Empty;
        //        trxVenta.Rewind();                                                          //move to first record

        //        int errores = 0;
        //        int i = 1;
        //        cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(connString, _Param);     //log de facturas xml emitidas y anuladas
        //        ReglasME maquina = new ReglasME(_Param);
        //        String accion = "CONSULTA STATUS";

        //        OnProgreso(1, "INICIANDO CONSULTA DE STATUS...");              //Notifica al suscriptor
        //        do
        //        {
        //            msj = String.Empty;
        //            String claseDocumento = !trxVenta.Docid.Equals("RESUMEN") ? _Param.tipoDoc : trxVenta.Docid;
        //            try
        //            {
        //                String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });

        //                if (maquina.ValidaTransicion(claseDocumento, accion, trxVenta.EstadoActual))
        //                    if (trxVenta.Voidstts == 0 && trxVenta.EstadoContabilizado.Equals("contabilizado"))  //documento no anulado
        //                    {
        //                        string tipoDoc = string.Empty;
        //                        string serie = string.Empty;
        //                        string correlativo = string.Empty;

        //                        trxVenta.ArmarDocElectronico(string.Empty);
        //                        tipoDoc = trxVenta.DocGP.DocVenta.tipoDocumento;
        //                        serie = serieCorrelativo[0];
        //                        correlativo = serieCorrelativo[1];

        //                        resultadoSunat = await servicioTimbre.ConsultaStatusAlOSEAsync(trxVenta.DocGP.DocVenta.emisorNroDoc, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, tipoDoc, serie, correlativo);
        //                        String[] codigoYMensaje = resultadoSunat.Split(new char[] { '-' });
        //                        maquina.DestinoAceptado = codigoYMensaje[0] == "0" ? true : false;
        //                        maquina.ActualizarNodoDestinoStatusBase();
        //                        DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, codigoYMensaje[1], codigoYMensaje[0], connString.Usuario, accion, maquina.DestinoStatusBase, maquina.DestinoEBinario, accion + ":" + codigoYMensaje[0]);

        //                        if (codigoYMensaje[0].Equals("0") || int.Parse(codigoYMensaje[0])>1000)
        //                        {
        //                            DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, connString.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, codigoYMensaje[0]);
        //                        }
        //                        msj = "Mensaje del OCE: " + resultadoSunat;

        //                    }
        //            }
        //            catch (Exception lo)
        //            {
        //                string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
        //                msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
        //                errores++;
        //            }
        //            finally
        //            {
        //                OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
        //                i++;
        //            }
        //        } while (trxVenta.MoveNext() && errores < 10);
        //    }
        //    catch (Exception xw)
        //    {
        //        string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
        //        this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
        //    }
        //    finally
        //    {
        //        OnProgreso(100, ultimoMensaje);
        //    }
        //    OnProgreso(100, "PROCESO FINALIZADO!");
        //    return resultadoSunat;
        //}

        //private Tuple<bool, string, string> ResultadoCDR(string tipoDocumento, string cdr)
        //{
        //    var xmlCdr = XElement.Parse(cdr);
        //    XNamespace nscac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        //    XNamespace nscbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        //    var responseCode = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "ResponseCode")?.First().Value;
        //    var description = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "Description")?.First().Value;
        //    //var refId = xmlCdr?.Elements(nscac + "DocumentResponse")?.Elements(nscac + "Response")?.Elements(nscbc + "ReferenceID")?.First().Value;

        //    if (string.IsNullOrEmpty(responseCode))
        //    {
        //        throw new ArgumentException("El archivo de respuesta no corresponde a un cdr. ");
        //    }

        //    return Tuple.Create(responseCode.Equals("0"), responseCode, description);
        //}

        //public async Task ProcesaObtienePDFAsync(ICfdiMetodosWebService servicioTimbre)
        //{
        //    try
        //    {
        //        String msj = String.Empty;
        //        String eBinario = String.Empty;
        //        trxVenta.Rewind();                                                          //move to first record

        //        int errores = 0;
        //        int i = 1;
        //        cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(connString, _Param);     //log de facturas xml emitidas y anuladas
        //        ReglasME maquina = new ReglasME(_Param);
        //        String accion = "IMPRIME PDF";

        //        OnProgreso(1, "INICIANDO CONSULTA DE PDFs...");              //Notifica al suscriptor
        //        do
        //        {
        //            msj = String.Empty;
        //            String rutaNombrePDF = String.Empty;
        //            String ticket = trxVenta.Regimen;
        //            try
        //            {
        //                String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });
        //                string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Docid + trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_CDR_" + accion.Substring(0, 2);

        //                if (maquina.ValidaTransicion(_Param.tipoDoc, accion, trxVenta.EstadoActual))
        //                    if (trxVenta.Voidstts == 0 && trxVenta.EstadoContabilizado.Equals("contabilizado"))  //no anulado y contabilizado
        //                    {
        //                        trxVenta.ArmarDocElectronico(string.Empty);
        //                        rutaNombrePDF = await servicioTimbre.ObtienePDFdelOSEAsync(trxVenta.Rfc, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, trxVenta.DocGP.DocVenta.tipoDocumento, serieCorrelativo[0], serieCorrelativo[1], trxVenta.RutaXml.Trim(), nombreArchivo, ".pdf");
        //                        DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, rutaNombrePDF, ticket, connString.Usuario, accion, maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

        //                        DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, connString.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, ticket);
        //                    }
        //                    else
        //                        msj = "No se puede generar porque no está Contabilizado o está Anulado.";

        //            }
        //            catch (ArgumentException ae)
        //            {
        //                msj = ae.Message + Environment.NewLine ;
        //                //DocVenta.LogDocumento(trxVenta, msj, maquina, ticket, _Param.tipoDoc, accion, false, rutaNombrePDF);
        //                //DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, _Conex.Usuario, "emitido", "emitido", maquina.eBinActualConError, maquina.EnLetras(maquina.eBinActualConError, _Param.tipoDoc), ticket);
        //                errores++;
        //            }
        //            catch (IOException io)
        //            {
        //                msj = "Excepción al revisar la carpeta/archivo: " + trxVenta.Ruta_clave + " Verifique su existencia y privilegios." + Environment.NewLine + io.Message + Environment.NewLine;
        //                errores++;
        //            }
        //            catch (Exception lo)
        //            {
        //                string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
        //                msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
        //                errores++;
        //            }
        //            finally
        //            {
        //                OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
        //                i++;
        //            }
        //        } while (trxVenta.MoveNext() && errores < 10);
        //    }
        //    catch (Exception xw)
        //    {
        //        string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
        //        this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
        //    }
        //    finally
        //    {
        //        OnProgreso(100, ultimoMensaje);
        //    }
        //    OnProgreso(100, "PROCESO FINALIZADO!");
        //}

        //public async Task ProcesaBajaComprobanteAsync(String motivoBaja, ICfdiMetodosWebService servicioTimbre)
        //{
        //    try
        //    {
        //        String msj = String.Empty;
        //        String eBinario = String.Empty;
        //        trxVenta.Rewind();                                                          //move to first record

        //        int errores = 0; int i = 1;
        //        cfdReglasFacturaXml DocVenta = new cfdReglasFacturaXml(connString, _Param);     //log de facturas xml emitidas y anuladas
        //        ReglasME maquina = new ReglasME(_Param);

        //        OnProgreso(1, "INICIANDO BAJA DE DOCUMENTO...");              //Notifica al suscriptor
        //        do
        //        {
        //            msj = String.Empty;
        //            try
        //            {
        //                String accion = "BAJA";
        //                if (maquina.ValidaTransicion(_Param.tipoDoc, accion, trxVenta.EstadoActual))
        //                {
        //                    eBinario = maquina.eBinarioNuevo;

        //                    trxVenta.ArmarBaja(motivoBaja);
        //                    String[] serieCorrelativo = trxVenta.Sopnumbe.Split(new char[] { '-' });
        //                    string numeroSunat = serieCorrelativo[0] + "-" + serieCorrelativo[1];
        //                    //string numeroSunat = serieCorrelativo[0] + "-" + long.Parse(serieCorrelativo[1]).ToString();

        //                    //validaciones
        //                    switch (trxVenta.DocumentoBaja.Bajas.First().TipoDocumento)
        //                    {
        //                        case "01":
        //                            if (!trxVenta.Sopnumbe.Substring(0, 1).Equals("F"))
        //                            {
        //                                msj = "El folio de la Factura debe empezar con la letra F. ";
        //                                throw new ApplicationException(msj);
        //                            }
        //                            break;
        //                        case "03":
        //                            if (!trxVenta.Sopnumbe.Substring(0, 1).Equals("B"))
        //                            {
        //                                msj = "El folio de la Boleta debe empezar con la letra B. ";
        //                                throw new ApplicationException(msj);
        //                            }
        //                            break;
        //                        default:
        //                            msj = "ok";
        //                            break;
        //                    }
        //                    string nombreArchivo = Utiles.FormatoNombreArchivo(trxVenta.Docid + trxVenta.Sopnumbe + "_" + trxVenta.s_CUSTNMBR, trxVenta.s_NombreCliente, 20) + "_" + accion.Substring(0, 4);

        //                    string resultadoBaja = await servicioTimbre.SolicitarBajaAsync(trxVenta.DocumentoBaja.Emisor.NroDocumento, trxVenta.Ruta_certificadoPac, trxVenta.Contrasenia_clavePac, string.Concat(trxVenta.DocumentoBaja.Bajas.First().TipoDocumento, "-", numeroSunat), Utiles.Izquierda(motivoBaja, 100));

        //                    //var rutaYNom = await DocVenta.GuardaArchivoAsync(trxVenta, resultado.Item2, nombreArchivo, ".xml", false);

        //                    DocVenta.RegistraLogDeArchivoXML(trxVenta.Soptype, trxVenta.Sopnumbe, resultadoBaja, "baja ok", connString.Usuario, string.Empty, maquina.DestinoStatusBase, maquina.DestinoEBinario, maquina.DestinoMensaje);

        //                    DocVenta.ActualizaFacturaEmitida(trxVenta.Soptype, trxVenta.Sopnumbe, connString.Usuario, "emitido", "emitido", maquina.DestinoEBinario, maquina.DestinoMensaje, "baja ok");

        //                }
        //            }
        //            catch (HttpRequestException he)
        //            {
        //                msj = string.Concat(he.Message, Environment.NewLine, he.StackTrace);
        //                errores++;
        //            }
        //            catch (ApplicationException ae)
        //            {
        //                msj = ae.Message + Environment.NewLine + ae.StackTrace;
        //                errores++;
        //            }
        //            catch (IOException io)
        //            {
        //                msj = "Excepción al revisar la carpeta/archivo: " + trxVenta.Ruta_clave + " Verifique su existencia y privilegios." + Environment.NewLine + io.Message + Environment.NewLine;
        //                errores++;
        //            }
        //            catch (Exception lo)
        //            {
        //                string imsj = lo.InnerException == null ? "" : lo.InnerException.ToString();
        //                msj = lo.Message + " " + imsj + Environment.NewLine + lo.StackTrace;
        //                errores++;
        //            }
        //            finally
        //            {
        //                OnProgreso(i * 100 / trxVenta.RowCount, "Doc:" + trxVenta.Sopnumbe + " " + msj.Trim() + " " + maquina.ultimoMensaje + Environment.NewLine);              //Notifica al suscriptor
        //                i++;
        //            }
        //        } while (trxVenta.MoveNext() && errores < 10 && i < 2); //Dar de baja uno por uno
        //    }
        //    catch (Exception xw)
        //    {
        //        string imsj = xw.InnerException == null ? "" : xw.InnerException.ToString();
        //        this.ultimoMensaje = xw.Message + " " + imsj + Environment.NewLine + xw.StackTrace;
        //    }
        //    finally
        //    {
        //        OnProgreso(100, ultimoMensaje);
        //    }
        //    OnProgreso(100, "Proceso finalizado!");
        //}

    }
}
