using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfdiEntidadesGP
{
    public class GBRADocumentoVentaGP
    {
            public vwCfdiGeneraDocumentoDeVentaBRA DocVenta
            {
                get
                {
                    return _DocVenta;
                }

                set
                {
                    _DocVenta = value;
                }
            }

            vwCfdiGeneraDocumentoDeVentaBRA _DocVenta;

            public GBRADocumentoVentaGP()
            {
                _DocVenta = new vwCfdiGeneraDocumentoDeVentaBRA();
            }

            //public void GetDatosDocumentoVenta(string Sopnumbe, short Soptype)
            //{

            //    using (GBRAEntities dv = new GBRAEntities())
            //    {
            //        _DocVenta = dv.vwCfdiGeneraDocumentoDeVentaBRA
            //                            .Where(v => v.SOPNUMBE == Sopnumbe && v.soptype == Soptype)
            //                            .First();

            //    }
            //}
    }
}
