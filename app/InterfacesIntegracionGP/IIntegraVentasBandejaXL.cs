﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesDeIntegracionGP
{
    public interface IIntegraVentasBandejaXL
    {
        void ProcesaCarpetaEnTrabajo(string carpetaOrigen, List<string> archivosSeleccionados);

    }
}
