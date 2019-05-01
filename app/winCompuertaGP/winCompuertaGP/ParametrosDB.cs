using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegradorDeGP;
using System.Xml;

namespace winCompuertaGP
{
 
    public class ParametrosDB:IParametrosDB,IParametrosXL
    {
        private List<Empresa> _empresas;
        private string nombreArchivoParametros = "ParametrosCompuertaGP.xml";
        private string targetGPDB = "";
        private string _servidor = "";
        private string _seguridadIntegrada = "0";
        private string _usuarioSql = "";
        private string _passwordSql = "";
        private string connStringSourceEFUI = string.Empty;
        private string connectionStringSourceEF = string.Empty;
        private string connectionStringTargetEF = string.Empty;
        private string connStringSource = string.Empty;
        private string connStringTarget = string.Empty;
        private string formatoFechaDB;
        private string formatoFechaXL;
        private string _culturaParaMontos;
        private string rutaLog;
        Dictionary<string, string> idsDocumento;
        private string _rutaCarpeta;
        private string _clienteDefaultCUSTCLAS;
        private int _facturaSopFilaInicial;
        private int _facturaSopColumnaMensajes;
        private int _facturaSopSerie;
        private int _facturaSopSopnumbe;
        private string _facturaSopSerieYNumbeSeparados;
        private int _facturaSopDocdate;
        private int _facturaSopDuedate;
        private int _facturaSopTXRGNNUM;
        private int _facturaSopCUSTNAME;
        private int _facturaSopReferencia;
        private int _facturaSopCliDireccion1;
        private int _facturaSopCliDireccion2;
        private int _facturaSopCliDireccion3;
        private int _facturaSopCliCiudad;
        private int _facturaSopCliEstado;
        private int _facturaSopCliZipCode;
        private int _facturaSopCliEmail;
        private int _facturaSopUNITPRCE;
        private int _facturaSopDeUNITPRCE;
        private int _facturaSopItemnmbr;
        private int _facturaSopItemnmbrDescr;
        private int _facturaSopCodServicio;
        private int _facturaSopDeReqShipDate;
        private int _facturaSopDeActlShipDate;
        private int _facturaSopDeCmmttext;
        private string _incluirUserDef;
        private string _usrtab01_predetValue;
        private string _usrtab02_predetValue;
        private string _intEstadoCompletado;
        private string _intEstadosPermitidos;

        public ParametrosDB()
        {
            //try
            //{
                XmlDocument listaParametros = new XmlDocument();
                listaParametros.Load(new XmlTextReader(nombreArchivoParametros));

                this._servidor = listaParametros.DocumentElement.SelectSingleNode("/listaParametros/servidor/text()").Value;
                this.DefaultDB = listaParametros.DocumentElement.SelectSingleNode("/listaParametros/servidor").Attributes["defaultDB"].Value;
                this._seguridadIntegrada = listaParametros.DocumentElement.SelectSingleNode("/listaParametros/seguridadIntegrada/text()").Value;
                this._usuarioSql = listaParametros.DocumentElement.SelectSingleNode("/listaParametros/usuariosql/text()").Value;
                this._passwordSql = listaParametros.DocumentElement.SelectSingleNode("/listaParametros/passwordsql/text()").Value;

                XmlNodeList empresasNodes = listaParametros.DocumentElement.SelectNodes("/listaParametros/compannia");

                this._empresas = new List<Empresa>();
                foreach (XmlNode empresaNode in empresasNodes)
                {
                    this._empresas.Add(new Empresa()
                    {
                        Idbd = empresaNode.Attributes["bd"].Value,
                        NombreBd = empresaNode.Attributes["nombre"].Value,
                        MetadataIntegra = empresaNode.Attributes["metadataIntegra"].Value,
                        MetadataGP = empresaNode.Attributes["metadataGP"].Value,
                        MetadataUIIntegra = empresaNode.Attributes["metadataUI"].Value
                    });
                }

            //}
            //catch (Exception eprm)
            //{
            //    ultimoMensaje = "Contacte al administrador. No se pudo obtener la configuración general. [Parametros()]" + eprm.Message;
            //}
        }

        public void GetParametros(int idxEmpresa)
        {
            string IdCompannia = this._empresas[idxEmpresa].Idbd;
                XmlDocument listaParametros = new XmlDocument();
                listaParametros.Load(new XmlTextReader(nombreArchivoParametros));
                XmlNode elemento = listaParametros.DocumentElement;


            targetGPDB = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/TargetGPDB/text()").Value;
            if (seguridadIntegrada)
            {
                connectionStringSourceEF = this._empresas[idxEmpresa].MetadataIntegra + "provider connection string='data source=" + _servidor + "; initial catalog = " + IdCompannia + "; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework'";
                connectionStringTargetEF = this._empresas[idxEmpresa].MetadataGP + "provider connection string='data source=" + _servidor + "; initial catalog = " + targetGPDB + "; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework'";
                connStringSource = "Initial Catalog=" + IdCompannia + ";Data Source=" + _servidor + ";Integrated Security=SSPI";
                connStringTarget = "Initial Catalog=" + targetGPDB + ";Data Source=" + _servidor + ";Integrated Security=SSPI";
                connStringSourceEFUI = this._empresas[idxEmpresa].MetadataUIIntegra + "provider connection string='data source=" + _servidor + "; initial catalog = " + IdCompannia + "; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework'"; 
            }
            else
            {
                connectionStringSourceEF = this._empresas[idxEmpresa].MetadataIntegra + "provider connection string='data source=" + _servidor + ";initial catalog=" + IdCompannia + ";user id=" + _usuarioSql + ";Password=" + _passwordSql + ";integrated security=False; MultipleActiveResultSets=True;App=EntityFramework'";
                connectionStringTargetEF = this._empresas[idxEmpresa].MetadataGP + "provider connection string='data source=" + _servidor + ";initial catalog=" + targetGPDB + ";user id=" + _usuarioSql + ";Password=" + _passwordSql + ";integrated security=False; MultipleActiveResultSets=True;App=EntityFramework'";
                connStringSource = "User ID=" + _usuarioSql + ";Password=" + _passwordSql + ";Initial Catalog=" + IdCompannia + ";Data Source=" + _servidor;
                connStringTarget = "User ID=" + _usuarioSql + ";Password=" + _passwordSql + ";Initial Catalog=" + targetGPDB + ";Data Source=" + _servidor;
                connStringSourceEFUI = this._empresas[idxEmpresa].MetadataUIIntegra + "provider connection string='data source=" + _servidor + "; initial catalog = " + IdCompannia + ";user id=" + _usuarioSql + ";Password=" + _passwordSql + ";integrated security=False; MultipleActiveResultSets=True;App=EntityFramework'";
            }

            FormatoFechaDB = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/formatoFechaDB/text()").Value;
            formatoFechaXL = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/formatoFechaXL/text()").Value;
            _culturaParaMontos = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/culturaParaMontos/text()").Value;
            
            RutaLog = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/RutaLog/text()").Value;
            _rutaCarpeta = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/rutaCarpeta/text()").Value;

            try
            {
                XmlNodeList idsDocumentoSOP = listaParametros.DocumentElement.SelectNodes("/listaParametros/compannia[@bd='" + IdCompannia + "']/idsDocumentoSOP");
                idsDocumento = new Dictionary<string, string>();
                foreach (XmlNode n in idsDocumentoSOP)
                {
                        idsDocumento.Add(n.Attributes["idAriane"].Value, n.Attributes["idGP"].Value);
                }
            }
            catch (Exception ae)
            {
                throw new ArgumentException("Excepción en los parámetros de la sección idsDocumentoSOP. [GetParametros(int)] " + ae.Message);
            }

            try
            {
                _clienteDefaultCUSTCLAS = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/Cliente/DefaultCUSTCLAS/text()").Value;
            }
            catch (Exception ae)
            {
                throw new ArgumentException("Excepción en los parámetros de la sección Cliente. [GetParametros(int)] " + ae.Message);
            }

            try
            {
                _facturaSopFilaInicial = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/filaInicial/text()").Value);
                _facturaSopColumnaMensajes = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/columnaMensajes/text()").Value);
                _facturaSopSerie = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/serie/text()").Value);
                _facturaSopSopnumbe = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/sopnumbe/text()").Value);
                _facturaSopSerieYNumbeSeparados = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/facturaSopSerieYNumbeSeparados/text()").Value;
                _facturaSopDocdate = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/docdate/text()").Value);
                _facturaSopDuedate = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/duedate/text()").Value);
                _facturaSopTXRGNNUM = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/TXRGNNUM/text()").Value);
                _facturaSopCUSTNAME = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/CUSTNAME/text()").Value);
                _facturaSopReferencia = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/referencia/text()").Value);
                _facturaSopUNITPRCE = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/UNITPRCE/text()").Value);

                _facturaSopCliDireccion1 = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliDireccion1/text()").Value);
                _facturaSopCliDireccion2 = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliDireccion2/text()").Value);
                _facturaSopCliDireccion3 = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliDireccion3/text()").Value);
                _facturaSopCliCiudad = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliCiudad/text()").Value);
                _facturaSopCliEstado = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliEstado/text()").Value);
                _facturaSopCliZipCode = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliZipCode/text()").Value);
                _facturaSopCliEmail = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopCa/cliEmail/text()").Value);

            }
            catch (Exception ae)
            {
                throw new ArgumentException("Excepción en alguno de los parámetros de la sección facturasSopCa. [GetParametros(int)] " + ae.Message);
            }

            try
            {
                _facturaSopDeUNITPRCE = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/UNITPRCE/text()").Value);
                _facturaSopItemnmbr = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/itemnmbr/text()").Value);
                _facturaSopItemnmbrDescr = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/itemnmbrDescr/text()").Value);
                _facturaSopCodServicio = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/codservicio/text()").Value);

                _facturaSopDeReqShipDate = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/ReqShipDate/text()").Value);
                _facturaSopDeActlShipDate = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/ActlShipDate/text()").Value);
                _facturaSopDeCmmttext = int.Parse(elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/facturaSopDe/CMMTTEXT/text()").Value);

            }
            catch (Exception ae)
            {
                throw new ArgumentException("Excepción en alguno de los parámetros de la sección facturasSopDe. [GetParametros(int)] " + ae.Message);
            }

            try
            {
                _incluirUserDef = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/sopUserDefined/incluirUserDef/text()").Value;
                _usrtab01_predetValue = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/sopUserDefined/usrtab01_predetValue/text()").Value;
                _usrtab02_predetValue = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/sopUserDefined/usrtab02_predetValue/text()").Value;

                //_intEstadoCompletado = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/intEstadoCompletado/text()").Value;
                //_intEstadosPermitidos = elemento.SelectSingleNode("//compannia[@bd='" + IdCompannia + "']/intEstadosPermitidos/text()").Value;

            }
            catch (Exception ae)
            {
                throw new ArgumentException("Excepción en alguno de los parámetros de la sección sopUserDefined. [GetParametros(int)] " + ae.Message);
            }

        }

        public string servidor
        {
            get { return _servidor; }
            set { _servidor = value; }
        }

        public bool seguridadIntegrada
        {
            get
            {
                return _seguridadIntegrada.Equals("1");
            }
            set
            {
                if (value)
                    _seguridadIntegrada = "1";
                else
                    _seguridadIntegrada = "0";
            }
        }

        public string usuarioSql
        {
            get { return _usuarioSql; }
            set { _usuarioSql = value; }
        }

        public string passwordSql
        {
            get { return _passwordSql; }
            set { _passwordSql = value; }
        }


        public string TargetGPDB
        {
            get
            {
                return targetGPDB;
            }
            set { targetGPDB = value; }

        }


        public List<Empresa> Empresas
        {
            get
            {
                return _empresas;
            }

            set
            {
                _empresas = value;
            }
        }

        public string ConnectionStringSourceEF
        {
            get
            {
                return connectionStringSourceEF;
            }

            set
            {
                connectionStringSourceEF = value;
            }
        }

        public string ConnectionStringTargetEF
        {
            get
            {
                return connectionStringTargetEF;
            }

            set
            {
                connectionStringTargetEF = value;
            }
        }
        public string DefaultDB { get; private set; }

        public string ConnStringSource
        {
            get
            {
                return connStringSource;
            }

            set
            {
                connStringSource = value;
            }
        }

        public string ConnStringTarget
        {
            get
            {
                return connStringTarget;
            }

            set
            {
                connStringTarget = value;
            }
        }

        public string FormatoFechaDB
        {
            get
            {
                return formatoFechaDB;
            }

            set
            {
                formatoFechaDB = value;
            }
        }

        public string FormatoFechaXL
        {
            get
            {
                return formatoFechaXL;
            }

            set
            {
                formatoFechaXL = value;
            }
        }
        public string CulturaParaMontos { get => _culturaParaMontos; set => _culturaParaMontos = value; }

        public string RutaLog
        {
            get
            {
                return rutaLog;
            }

            set
            {
                rutaLog = value;
            }
        }

        public string ConnStringSourceEFUI
        {
            get
            {
                return connStringSourceEFUI;
            }

            set
            {
                connStringSourceEFUI = value;
            }
        }

        public Dictionary<string, string> IdsDocumento
        {
            get
            {
                return idsDocumento;
            }

            set
            {
                idsDocumento = value;
            }
        }

        public string rutaCarpeta { get => _rutaCarpeta; set => _rutaCarpeta = value; }
        public string ClienteDefaultCUSTCLAS { get => _clienteDefaultCUSTCLAS; set => _clienteDefaultCUSTCLAS = value; }
        public int FacturaSopFilaInicial { get => _facturaSopFilaInicial; set => _facturaSopFilaInicial = value; }
        public int FacturaSopColumnaMensajes { get => _facturaSopColumnaMensajes; set => _facturaSopColumnaMensajes = value; }
        public int FacturaSopSerie { get => _facturaSopSerie; set => _facturaSopSerie = value; }
        public int FacturaSopnumbe { get => _facturaSopSopnumbe; set => _facturaSopSopnumbe = value; }
        public int FacturaSopDocdate { get => _facturaSopDocdate; set => _facturaSopDocdate = value; }
        public int FacturaSopDuedate { get => _facturaSopDuedate; set => _facturaSopDuedate = value; }
        public int FacturaSopTXRGNNUM { get => _facturaSopTXRGNNUM; set => _facturaSopTXRGNNUM = value; }
        public int FacturaSopCUSTNAME { get => _facturaSopCUSTNAME; set => _facturaSopCUSTNAME = value; }
        public int FacturaSopUNITPRCE { get => _facturaSopUNITPRCE; set => _facturaSopUNITPRCE = value; }
        /// <summary>
        /// Indica si Serie y Sopnumbe están separados
        /// </summary>
        public string FacturaSopSerieYNumbeSeparados { get => _facturaSopSerieYNumbeSeparados; set => _facturaSopSerieYNumbeSeparados = value; }
        public int FacturaSopReferencia { get => _facturaSopReferencia; set => _facturaSopReferencia = value; }
        public int FacturaSopItemnmbr { get => _facturaSopItemnmbr; set => _facturaSopItemnmbr = value; }
        public int FacturaSopItemnmbrDescr { get => _facturaSopItemnmbrDescr; set => _facturaSopItemnmbrDescr = value; }
        public int FacturaSopDeUNITPRCE { get => _facturaSopDeUNITPRCE; set => _facturaSopDeUNITPRCE = value; }
        public int FacturaSopDeReqShipDate { get => _facturaSopDeReqShipDate; set => _facturaSopDeReqShipDate = value; }
        public int FacturaSopDeActlShipDate { get => _facturaSopDeActlShipDate; set => _facturaSopDeActlShipDate = value; }
        public int FacturaSopDeCmmttext { get => _facturaSopDeCmmttext; set => _facturaSopDeCmmttext = value; }

        public int intEstadoCompletado { get => int.Parse(_intEstadoCompletado); set => _intEstadoCompletado = value.ToString(); }
        public int intEstadosPermitidos { get => int.Parse(_intEstadosPermitidos); set => _intEstadosPermitidos = value.ToString(); }
        public bool IncluirUserDef { get => _incluirUserDef.ToLower().Equals("true") || _incluirUserDef.Equals("1") ; set => _incluirUserDef = value.ToString(); }
        public string Usrtab01_predetValue { get => _usrtab01_predetValue; set => _usrtab01_predetValue = value; }
        public string Usrtab02_predetValue { get => _usrtab02_predetValue; set => _usrtab02_predetValue = value; }
        public int FacturaSopCodServicio { get => _facturaSopCodServicio; set => _facturaSopCodServicio = value; }
        public int FacturaSopCliDireccion1 { get => _facturaSopCliDireccion1; set => _facturaSopCliDireccion1 = value; }
        public int FacturaSopCliDireccion2 { get => _facturaSopCliDireccion2; set => _facturaSopCliDireccion2 = value; }
        public int FacturaSopCliDireccion3 { get => _facturaSopCliDireccion3; set => _facturaSopCliDireccion3 = value; }
        public int FacturaSopCliCiudad { get => _facturaSopCliCiudad; set => _facturaSopCliCiudad = value; }
        public int FacturaSopCliEstado { get => _facturaSopCliEstado; set => _facturaSopCliEstado = value; }
        public int FacturaSopCliZipCode { get => _facturaSopCliZipCode; set => _facturaSopCliZipCode = value; }
        public int FacturaSopCliEmail { get => _facturaSopCliEmail; set => _facturaSopCliEmail = value; }
    }

}

