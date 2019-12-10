using Comun;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfdiEntidadesGP
{
    public class MainDB
    {
        public string connectionString { get; set; }

        // Rango de fechas para generar prefacturas
        public DateTime fechaDesdePref { get; set; }
        public DateTime fechaHastaPref { get; set; }

        public MainDB(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #region Eventos
        public event EventHandler<ErrorEventArgsEntidadesGP> eventoErrDB;

        protected virtual void OnErrorDB(ErrorEventArgsEntidadesGP e)
        {
            //si no es null notificar
            eventoErrDB?.Invoke(this, e);
        }

        #endregion

        public void setConnectionString(string connection)
        {
            this.connectionString = connection;
        }

        //// Permite comprobar la conexión con la base de datos
        public bool probarConexion()
        {
            using (var db = this.getDbContext())
            {
                return db.Database.Exists();
            }
        }

        public GBRAEntities getDbContext()
        {
            if (string.IsNullOrEmpty(this.connectionString))
                return new GBRAEntities();

            return new GBRAEntities(this.connectionString);
        }

        //// Devuelve las prefacturas que cumplan con los criterios de filtrado seleccionados
        public IList<vwCfdiTransaccionesDeVenta> getFacturas(
                                                            bool filtrarNfse, string numNfseDesde, string numNfseHasta,
                                                            bool filtrarFecha, DateTime fechaDesde, DateTime fechaHasta,
                                                            bool filtrarEstado, string estado,
                                                            bool filtrarCliente, string idCliente,
                                                            bool filtrarReferencia, string referencia,
                                                            bool filtrarSopnumber, string sopnumberDesde, string sopnumberHasta)
        {
            using (var db = this.getDbContext())
            {
                // verificar la conexión con el servidor de bd
                if (!this.probarConexion())
                {
                    ErrorEventArgsEntidadesGP args = new ErrorEventArgsEntidadesGP();
                    args.mensajeError = "No se pudo establecer la conexión con el servidor al tratar de leer las pre-facturas.";
                    OnErrorDB(args);
                }

                var datos = db.vwCfdiTransaccionesDeVenta.AsQueryable();

                // Filtrado por número de nfse
                if (filtrarNfse)
                {
                    if (numNfseDesde != "")
                    {
                        datos = datos.Where(m => m.cstponbr.CompareTo(numNfseDesde) >= 0);
                    }
                    if (numNfseHasta != "")
                    {
                        datos = datos.Where(m => m.cstponbr.CompareTo(numNfseHasta) <= 0);
                    }
                }
                // Filtrado por fecha
                if (filtrarFecha)
                {
                    datos = datos.Where(m => DbFunctions.TruncateTime(m.fechahora) >= fechaDesde && DbFunctions.TruncateTime(m.fechahora) <= fechaHasta);
                }

                // Filtrado por estado
                if (filtrarEstado)
                {
                    datos = datos.Where(m => m.estado.Equals(estado));
                }

                // Filtrado por id de cliente
                if (filtrarCliente && idCliente != "")
                {
                    datos = datos.Where(m => m.idImpuestoCliente.Contains(idCliente));
                }

                // Filtrado por referencia
                if (filtrarReferencia && referencia != "")
                {
                    datos = datos.Where(m => m.mensajeEA.Contains(referencia));
                }

                // Filtrado por sopnumbe
                if (filtrarSopnumber && sopnumberDesde != "")
                {
                    //var c = Convert.ToInt32(sopnumberDesde);
                    datos = datos.Where(m => m.sopnumbe.CompareTo(sopnumberDesde) >= 0);
                }

                // Filtrado por sopnumbe
                if (filtrarSopnumber && sopnumberHasta != "")
                {
                    //var c = Convert.ToInt32(sopnumberHasta);
                    datos = datos.Where(m => m.sopnumbe.CompareTo(sopnumberHasta) <= 0);
                }

                return datos.ToList();
            }
        }

        public void CreaLogFactura(short soptype, string sopnumbe, string mensaje, string noAprobacion, string idusuario, string innerxml,
                                            string eBaseNuevo, string eBinarioActual, string mensajeBinActual)
        {
            using (var db = this.getDbContext())
            {
                // verificar la conexión con el servidor de bd
                if (!this.probarConexion())
                {
                    ErrorEventArgsEntidadesGP args = new ErrorEventArgsEntidadesGP();
                    args.mensajeError = "No se pudo establecer la conexión con el servidor. Verifique la configuración de conexión a la base de datos. [CreaLog]";
                    OnErrorDB(args);
                }

                var log = new cfdLogFacturaXML()
                {
                    soptype = soptype,
                    sopnumbe = sopnumbe,
                    mensaje = Utiles.Derecha(mensaje, 255),
                    estado = eBaseNuevo,
                    noAprobacion = noAprobacion,
                    fechaEmision = DateTime.Now,
                    idUsuario = Utiles.Derecha(idusuario, 10),
                    idUsuarioAnulacion = "-",
                    fechaAnulacion = new DateTime(1900, 1, 1),
                    archivoXML = !string.IsNullOrEmpty(innerxml) ? innerxml : string.Empty,
                    estadoActual = eBinarioActual,
                    mensajeEA = Utiles.Derecha(mensajeBinActual, 255),
                };
                db.cfdLogFacturaXML.Add(log);
                db.SaveChanges();
            }

        }

        public void ActualizaOCreaLogFactura(short soptype, string sopnumbe, string mensaje, string noAprobacion, string idusuario, string innerxml,
                                            string eBaseAnterior, string eBaseNuevo, string eBinarioActual, string mensajeEA)
        {
            using (var db = this.getDbContext())
            {
                // verificar la conexión con el servidor de bd
                if (!this.probarConexion())
                {
                    ErrorEventArgsEntidadesGP args = new ErrorEventArgsEntidadesGP();
                    args.mensajeError = "No se pudo establecer la conexión con el servidor. Verifique la configuración de conexión a la base de datos. [CreaLog]";
                    OnErrorDB(args);
                }
                //var docl = db.cfdLogFacturaXML.AsQueryable();

                try
                {
                    var doc = db.cfdLogFacturaXML.Where(x => x.sopnumbe.Equals(sopnumbe) && x.soptype.Equals(soptype) && x.estado.Equals(eBaseAnterior)).First();

                    if (doc != null)
                    {
                        if (!eBaseAnterior.Equals(eBaseNuevo))
                            doc.estado = eBaseNuevo;         // "anulado";
                        doc.fechaAnulacion = DateTime.Now;
                        doc.idUsuarioAnulacion = Utiles.Derecha(idusuario, 10);
                        doc.estadoActual = eBinarioActual;
                        doc.mensajeEA = Utiles.Derecha(mensajeEA, 255);
                        doc.noAprobacion = noAprobacion;
                    }
                }
                catch (Exception ex)
                {
                    var log = new cfdLogFacturaXML()
                    {
                        soptype = soptype,
                        sopnumbe = sopnumbe,
                        mensaje = Utiles.Derecha(mensaje, 255),
                        estado = eBaseNuevo,
                        noAprobacion = noAprobacion,
                        fechaEmision = DateTime.Now,
                        idUsuario = Utiles.Derecha(idusuario, 10),
                        idUsuarioAnulacion = "-",
                        fechaAnulacion = new DateTime(1900, 1, 1),
                        archivoXML = !string.IsNullOrEmpty(innerxml) ? innerxml : string.Empty,
                        estadoActual = eBinarioActual,
                        mensajeEA = Utiles.Derecha(mensajeEA, 255),
                    };
                    db.cfdLogFacturaXML.Add(log);

                }
                finally
                {
                    db.SaveChanges();
                }
            }

        }


        public GBRADocumentoVentaGP GetDatosDocumentoVenta(string Sopnumbe, short Soptype)
        {
            GBRADocumentoVentaGP _DocVenta = new GBRADocumentoVentaGP();
            using (var db = this.getDbContext())
            {
                // verificar la conexión con el servidor de bd
                if (!this.probarConexion())
                {
                    ErrorEventArgsEntidadesGP args = new ErrorEventArgsEntidadesGP();
                    args.mensajeError = "No se pudo establecer la conexión con el servidor al tratar de leer las pre-facturas.";
                    OnErrorDB(args);
                }
                _DocVenta.DocVenta = db.vwCfdiGeneraDocumentoDeVentaBRA
                    .Where(v => v.SOPNUMBE == Sopnumbe && v.soptype == Soptype)
                    .First();
            }

            return _DocVenta;
        }

        public void ActualizaNumeroFiscalElectronico(short soptype, string nsfe, string sopnumbe, out string mensaje)
        {
            System.Data.Entity.Core.Objects.ObjectParameter msj = new System.Data.Entity.Core.Objects.ObjectParameter("MENS", typeof(string)) ;
            using (var db = this.getDbContext())
            {
                if (!this.probarConexion())
                {
                    ErrorEventArgsEntidadesGP args = new ErrorEventArgsEntidadesGP();
                    args.mensajeError = "No se pudo establecer la conexión con el servidor al tratar de actualizar los NFS-e.";
                    OnErrorDB(args);
                }

                var ejecutaSp = db.spCfdiActualizaNumeroFiscalElectronico(soptype, nsfe, sopnumbe, msj);
                mensaje = msj.Value.ToString();

            }
        }
    }
}
