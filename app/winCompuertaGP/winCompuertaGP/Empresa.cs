using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winCompuertaGP
{
    public struct Empresa
    {
        private string idbd;
        private string nombreBd;
        private string metadataIntegra;
        private string metadataGP;
        private string metadataUIIntegra;

        public string Idbd
        {
            get
            {
                return idbd;
            }

            set
            {
                idbd = value;
            }
        }

        public string NombreBd
        {
            get
            {
                return nombreBd;
            }

            set
            {
                nombreBd = value;
            }
        }

        /// <summary>
        /// metadata de la bd Integra del servicio de integración
        /// </summary>
        public string MetadataIntegra
        {
            get
            {
                return metadataIntegra;
            }

            set
            {
                metadataIntegra = value;
            }
        }

        /// <summary>
        /// metadata de la bd GP del servicio de integración
        /// </summary>
        public string MetadataGP
        {
            get
            {
                return metadataGP;
            }

            set
            {
                metadataGP = value;
            }
        }

        /// <summary>
        /// metadata de la bd Integra de la aplicación winForms
        /// </summary>
        public string MetadataUIIntegra
        {
            get
            {
                return metadataUIIntegra;
            }

            set
            {
                metadataUIIntegra = value;
            }
        }
    }
}
