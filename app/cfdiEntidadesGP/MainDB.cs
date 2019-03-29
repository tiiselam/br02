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
        public IList<vwCfdiTransaccionesDeVenta> getFacturas(bool filtrarNumPF, string numPFDesde, string numPFHasta,
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

                // Filtrado por número de prefactura
                if (filtrarNumPF)
                {
                    if (numPFDesde != "")
                    {
                        //int npfDesde = Convert.ToInt32(numPFDesde);
                        datos = datos.Where(m => m.sopnumbe.CompareTo(numPFDesde) >= 0);
                    }

                    if (numPFHasta != "")
                    {
                        //int npfHasta = Convert.ToInt32(numPFHasta);
                        datos = datos.Where(m => m.sopnumbe.CompareTo(numPFHasta) <= 0);
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

                //if (filtrarId) {
                //    datos = datos.Where(m => m.ID_PACIENTE.Equals(id));
                //}

                // Filtrado por id de cliente
                if (filtrarCliente && idCliente != "")
                {
                    datos = datos.Where(m => m.CUSTNMBR.Contains(idCliente));
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

    }
}
