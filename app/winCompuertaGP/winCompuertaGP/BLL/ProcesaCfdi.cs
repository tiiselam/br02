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
            string rutaYNom = string.Empty;
            try
            {
                String msj = String.Empty;
                int errores = 0; int i = 1;
                OnProgreso(1, "INICIANDO GENERACION DE ARCHIVO TEXTO...");
                //IList<Web_Service.PedidoEnvioLoteRPS> documentosRps = new List<Web_Service.PedidoEnvioLoteRPS>();
                string detalleDocTxt = string.Empty;
                string cabeceraDocTxt = string.Empty;
                string sTimeStamp = System.DateTime.Now.ToString("yyMMddHHmmss");
                string nombreArchivo = "NFPrefeitura_" + sTimeStamp;
                string extension = ".txt";
                string ruta = string.Empty;
                string MinFecha = "99999999", MaxFecha = "00000000";

                foreach (vwCfdiTransaccionesDeVenta trxVenta in listaTransaccionesVenta)
                {
                    ruta = trxVenta.rutaXml.Trim();
                    rutaYNom = Path.Combine(trxVenta.rutaXml.Trim(), nombreArchivo + extension);
                    msj = String.Empty;
                    try
                    {
                        string tipoMEstados = "DOCVENTA-"+ trxVenta.estadoContabilizado;
                        trxVenta.CicloDeVida = new Maquina(trxVenta.estadoActual, trxVenta.regimen, trxVenta.voidstts, "emisor", tipoMEstados);
                        if (trxVenta.CicloDeVida.Transiciona(Maquina.eventoGeneraTxt, 1))
                        {
                            var docGpBrasil = DocumentosGP.GetDatosDocumentoVenta(trxVenta.sopnumbe, trxVenta.soptype);
                            //documentosRps.Add(serviciosPrefectura.GeneraDatosRPS(docGpBrasil));
                            var documentoRps = serviciosPrefectura.GeneraDatosRPS(docGpBrasil);
                            var documentoTxt = serviciosPrefectura.PreparaDatosArchivoTxt(documentoRps);
                            detalleDocTxt += documentoTxt.Item2+Environment.NewLine;
                            cabeceraDocTxt = documentoTxt.Item1;

                            if (String.Compare(cabeceraDocTxt.Substring(12, 8), MinFecha) < 0)
                            {
                                MinFecha = cabeceraDocTxt.Substring(12, 8);
                            }

                            if (String.Compare(cabeceraDocTxt.Substring(20, 8), MaxFecha) > 0)
                            {
                                MaxFecha = cabeceraDocTxt.Substring(20, 8);
                            }

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

                if (!string.IsNullOrEmpty( detalleDocTxt))
                {
                    cabeceraDocTxt = cabeceraDocTxt.Substring(0, 11) + MinFecha + MaxFecha;

                    string rn = serviciosPrefectura.GuardaArchivoTxt(ruta, nombreArchivo, extension, cabeceraDocTxt +Environment.NewLine+ detalleDocTxt);
                    OnProgreso(100, "Archivo guardado en: " + rn);
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


    }
}
