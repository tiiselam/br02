using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using IntegradorDeGP;
using cfdiEntidadesGP;
using Web_Service;
using cfd.FacturaElectronica;
using notaFiscalCsvHelper;
using OfficeOpenXml;

namespace winCompuertaGP
{
    public partial class winFormCompuertaGPBase : Form
    {

        //DataGridView dGridActivo;

        private MainDB mainController;
        private ParametrosDB configuracion;
        private object[] idDetallePrefacturaSeleccionada;

        //private object celdaActual;
        private List<int> filasActualizadas;

        DateTime fechaIni = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

        int dePeriodo = DateTime.Now.Year * 100 + 01;
        int aPeriodo = DateTime.Now.Year * 100 + DateTime.Now.Month;

        IList<vwCfdiTransaccionesDeVenta> listaDeFacturas = null;
        List<vwCfdiTransaccionesDeVenta> LDocsNoSeleccionados = new List<vwCfdiTransaccionesDeVenta>();   //Docs no marcados del grid
        short idxChkBox = 0;                    //columna check box del grid
        short idxIdDoc = 1;                     //columna id de documento del grid
        short idxSoptype = 2;                   //columna soptype del grid
        short idxSopnumbe = 3;                  //columna sopnumbe del grid

        public winFormCompuertaGPBase()
        {
            try
            {
                InitializeComponent();

                //dgvFacturas.AutoGenerateColumns = false;

                cmbBEstado.SelectedIndex = 0;


                mainController = new MainDB("");
                mainController.eventoErrDB += MainController_eventoErrorDB;
                configuracion = new ParametrosDB();
                cargarEmpresas();
                
                var empresaDefault = configuracion.Empresas.Where(x => x.Idbd == configuracion.DefaultDB).First();
                cmbBxCompannia.SelectedIndex = configuracion.Empresas.IndexOf(empresaDefault);

                //celdaActual = null;
                filasActualizadas = new List<int>();

                lblFecha.Text = DateTime.Now.ToShortDateString();
                lblUsuario.Text = Environment.UserName;

                //this.idPrefacturaSeleccionada = -1;
                this.idDetallePrefacturaSeleccionada = new object[2];
                //this.filtrarDetallePrefactura = false;
            }
            catch (Exception exc)
            {
                txtbxMensajes.Text = string.Concat( exc.Message, Environment.NewLine, exc?.InnerException?.ToString(), Environment.NewLine, exc.StackTrace , Environment.NewLine);
            }

        }

        private void MainController_eventoErrorDB(object sender, ErrorEventArgsEntidadesGP e)
        {
            txtbxMensajes.Text += e.mensajeError + Environment.NewLine;
        }

        private void cargarEmpresas()
        {
            try
            {
                cmbBxCompannia.Items.Clear();
                foreach (Empresa e in configuracion.Empresas)
                {
                    cmbBxCompannia.Items.Add(e.Idbd + "->" + e.NombreBd);
                }
            }
            catch (Exception exc)
            {
                txtbxMensajes.AppendText(exc.Message + Environment.NewLine);
            }
        }

        private void cmbBxCompannia_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarDatosEmpresa(((ComboBox)sender).SelectedIndex);
        }

        private void winformPreFactura_Load(object sender, EventArgs e)
        {

        }

        private void cargarDatosEmpresa(int index)
        {

            // Limpiar los DataGridViews
            dgvFacturas.Rows.Clear();
            //dgvDetallesPrefactura.Rows.Clear();
            //dgvPrestaciones.Rows.Clear();

            // Limpiar los filtros y las cabeceras
            limpiarFiltrosPreFacturas();

            // Establecer el nuevo string de conexión
            //mainController.connectionString = this.connections[index];
            configuracion.GetParametros(index);
            mainController.connectionString = configuracion.ConnStringSourceEFUI;

            // Limpiar los mensajes
            txtbxMensajes.Text = "";

            // Verificar la conexión
            if (mainController.probarConexion())
            {
                //ActualizarStatus();

                // Recargar los datos del grid
                filtrarFacturas();
            }
            else
                txtbxMensajes.Text = "Contacte al administrador. No se pudo establecer la conexión para la compañía seleccionada. [cargarDatosEmpresa]";
        }


        #region Utiles UI
        private void reportaProgreso(int i, string s)
        {
            //iProgreso = i;
            tsProgressBar1.Increment(i);
            //tsProgressBar1.Refresh();

            if (tsProgressBar1.Value == tsProgressBar1.Maximum)
                tsProgressBar1.Value = 0;

            txtbxMensajes.AppendText(s + "\r\n");
            txtbxMensajes.Refresh();
        }

        private void reportaAlertas(string s)
        {
            textBoxAlertas.AppendText(s + "\r\n");
            textBoxAlertas.Refresh();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        #endregion

        #region Búsqueda UI
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                txtbxMensajes.Text = "";
                var errores = validarFiltrosPreFacturas();
                if (errores == "")
                {
                    var c = filtrarFacturas();
                    txtbxMensajes.Text = "";
                    txtbxMensajes.AppendText("Total de documentos encontrados: " + c + Environment.NewLine);
                }
                else
                    txtbxMensajes.Text = errores;
            }
            catch (Exception exc)
            {
                txtbxMensajes.Text = string.Concat(exc.Message, Environment.NewLine, exc?.InnerException.ToString(), Environment.NewLine);
            }
        }

        private void hoytsMenuItem4_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today;
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = hoytsMenuItem4.Text;
            filtrarFacturas();
        }

        private void ayertsMenuItem5_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-1);
            fechaFin = DateTime.Today.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ayertsMenuItem5.Text;
            filtrarFacturas();
        }

        private void ultimos7tsMenuItem6_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-6);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos7tsMenuItem6.Text;
            filtrarFacturas();
        }

        private void ultimos30tsMenuItem7_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-29);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos30tsMenuItem7.Text;
            filtrarFacturas();
        }

        private void ultimos60tsMenuItem8_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-59);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos60tsMenuItem8.Text;
            filtrarFacturas();
        }

        private void mesActualtsMenuItem9_Click(object sender, EventArgs e)
        {
            fechaIni = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fechaFin = fechaIni.AddMonths(1);
            int ultimoDia = fechaFin.Day;
            fechaFin = fechaFin.AddDays(-ultimoDia);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = mesActualtsMenuItem9.Text;
            filtrarFacturas();
        }

        private void tsDropDownFiltro_TextChanged(object sender, EventArgs e)
        {
            txtbxMensajes.Text = "";
        }

        private int filtrarFacturas()
        {
            bool cbFechaMarcada = checkBoxFecha.Checked;
            DateTime fini = dtPickerDesde.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime ffin = dtPickerHasta.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            if (!checkBoxPacientes_numero_pf.Checked && !checkBoxFecha.Checked && !checkBoxEstado.Checked && !checkBoxPacientes_nombre_cliente.Checked && !checkBoxPacientes_referencia.Checked && !checkBoxPacientes_sopnumbe.Checked)
            {
                cbFechaMarcada = true;
                fini = fechaIni;
                ffin = fechaFin;
            }

            listaDeFacturas = mainController.getFacturas(
                                                        checkBoxPacientes_numero_pf.Checked,
                                                        textBoxPacientes_numero_pf_desde.Text,
                                                        textBoxPacientes_numero_pf_hasta.Text,
                                                        cbFechaMarcada,
                                                        fini,
                                                        ffin,
                                                        checkBoxEstado.Checked,
                                                        cmbBEstado.SelectedItem.ToString(),
                                                        checkBoxPacientes_nombre_cliente.Checked,
                                                        textBoxPacientes_nombre_cliente.Text,
                                                        checkBoxPacientes_referencia.Checked,
                                                        textBoxPacientes_referencia.Text,
                                                        checkBoxPacientes_sopnumbe.Checked,
                                                        textBoxPacientes_sopnumbe_desde.Text,
                                                        textBoxPacientes_sopnumbe_hasta.Text
                                                    );
            bindingSource1.DataSource = listaDeFacturas;
            dgvFacturas.AutoGenerateColumns = false;
            dgvFacturas.DataSource = bindingSource1;
            dgvFacturas.AutoResizeColumns();
            //dgvFacturas.RowHeadersVisible = false;
            dgvFacturas.Refresh();

            //Restituir las filas marcadas usando la lista de docs no seleccionados
            InicializaCheckBoxDelGrid(dgvFacturas, idxChkBox, LDocsNoSeleccionados);

            return listaDeFacturas.Count;
        }

        // Valida los campos de filtrado y devuelve los errores
        private string validarFiltrosPreFacturas()
        {
            string errores = "";
            if (checkBoxFecha.Checked)
            {
                if (dtPickerDesde.Value > dtPickerHasta.Value)
                    errores += "El campo fecha inicial debe ser menor que la fecha final." + Environment.NewLine;
            }

            return errores;
        }

        private void limpiarFiltrosPreFacturas()
        {
            checkBoxPacientes_numero_pf.Checked = false;
            checkBoxPacientes_nombre_cliente.Checked = false;
            checkBoxFecha.Checked = false;
            checkBoxEstado.Checked = false;
            checkBoxPacientes_sopnumbe.Checked = false;
            checkBoxPacientes_referencia.Checked = false;

            textBoxPacientes_numero_pf_desde.Text = "";
            textBoxPacientes_numero_pf_hasta.Text = "";
            textBoxPacientes_nombre_cliente.Text = "";
            dtPickerDesde.ResetText();
            dtPickerHasta.ResetText();
            cmbBEstado.SelectedIndex = 0;
            textBoxPacientes_sopnumbe_desde.Text = "";
            textBoxPacientes_sopnumbe_hasta.Text = "";
            textBoxPacientes_referencia.Text = "";
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            marcarDesmarcarTodo(dgvFacturas);
        }

        // Marca/desmarca todos los checks del grid especificado
        private void marcarDesmarcarTodo(DataGridView dgv)
        {
            bool value = false;
            bool flag = false;

            foreach (DataGridViewRow item in dgv.Rows)
            {
                if (!flag)
                {
                    value = !(Boolean.Parse(dgv.Rows[0].Cells[0].Value.ToString()));
                    flag = true;
                }
                item.Cells[0].Value = value;

                // Console.WriteLine(((DataGridViewCheckBoxCell)item.Cells[0]));
            }
            dgv.Refresh();
        }


        private void dgvPacientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //var row = e.RowIndex;
            //// validar que el doble click no sea en la cabecera
            //if (row != -1)
            //{
            //    try
            //    {
            //        int numeroPF = Convert.ToInt32(((DataGridView)sender).Rows[row].Cells[1].Value);
            //        limpiarFiltrosDetallesPrefacturas();
            //        var prefactura = mainController.findPrefactura(numeroPF);
            //        if (prefactura != null)
            //        {
            //            this.idPrefacturaSeleccionada = prefactura.NUMERO_PF;
            //            mostrarDetallesPrefactura(prefactura);
            //        }
            //    }
            //    catch (Exception exc)
            //    {
            //        txtbxMensajes.Text = exc.Message + "\r\n";
            //    }
            //}
        }

        #endregion

        #region Integración de documentos

        private void tsBtnIntegraFactura_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "CSV files|*.csv"; // "Excel Files|*.xls|*.xlsx";
                openFileDialog1.Multiselect = true;
                DialogResult dr = openFileDialog1.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    string[] filenames = openFileDialog1.FileNames;
                    var nombreArchivos = filenames
                            .Select(y => new {  archivo = System.IO.Path.GetFileName(y),
                                                carpeta = System.IO.Path.GetDirectoryName(y) });
                    List<string> lNombreArchivos = nombreArchivos.Select(a => a.archivo).ToList();
                    string carpetaOrigen = nombreArchivos.Select(a => a.carpeta).FirstOrDefault();
                    IntegraVentasBandejaXL bandejaXL = new IntegraVentasBandejaXL(configuracion);

                    bandejaXL.ProgressHandler += reportaProgreso;

                    LectorCSV csv = new LectorCSV(configuracion.CodigosServicioDflt);
                    csv.ProgressHandler += reportaProgreso;

                    System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo(configuracion.CulturaParaMontos);
                    var archivosXl = csv.ConvierteCsvAExcel(carpetaOrigen, lNombreArchivos, culInfo);

                    bandejaXL.ProcesaCarpetaEnTrabajo(carpetaOrigen, archivosXl);

                    filtrarFacturas();
                }
            }
            catch (Exception ex)
            {
                reportaProgreso(0, ex.Message);
            }

        }

        #endregion

        #region Otros
        private void tabControlPrefactura_SelectedIndexChanged(object sender, EventArgs e)
        {
            //txtbxMensajes.Text = "";

            //if (mainController.probarConexion())
            //{
            //    try
            //    {
            //        var prefacturaSeleccionada = mainController.findPrefactura(this.idPrefacturaSeleccionada);
            //        if (prefacturaSeleccionada == null)
            //        {
            //            dgvDetallesPrefactura.Rows.Clear();
            //            limpiarCabecerasDetallesPrefacturas();

            //            // deshabilitar la creación de detalles cuando no hay prefactura seleccionada
            //            toolStripButton6.Enabled = false;
            //        }
            //        else
            //        {
            //            // habilitar la creación de detalles cuando hay prefactura seleccionada
            //            toolStripButton6.Enabled = true;
            //        }

            //        var detallePrefactura = mainController.findDetallePrefactura(this.idDetallePrefacturaSeleccionada);
            //        if (detallePrefactura == null)
            //        {
            //            limpiarCabecerasPrestaciones();
            //        }
            //    }
            //    catch (Exception exc)
            //    {
            //        txtbxMensajes.Text = exc.Message + "\r\n";
            //    }
            //}
            //else
            //{
            //    txtbxMensajes.Text = "No se pudo establecer la conexión con el servidor." + "\r\n";
            //}
        }

        private void tabControlPreFactura_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //e.Cancel = comprobarCambiosDetallesPrefactura() || comprobarCambiosPrestaciones();

            //if (comprobarCambiosDetallesPrefactura() || comprobarCambiosPrestaciones())
            //    txtbxMensajes.Text = "Para cambiar de pestaña debe guardar los cambios realizados." + "\r\n";
        }

        private void dgvPacientes_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvFacturas.IsCurrentCellDirty)
            {
                dgvFacturas.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // Devuelve las filas seleccionadas del grid especificado
        private List<DataGridViewRow> getFilasSeleccionadas(DataGridView dgv)
        {
            var listado = new List<DataGridViewRow>();
            foreach (DataGridViewRow item in dgv.Rows)
            {
                if (Boolean.Parse(item.Cells[0].Value.ToString()))
                    listado.Add(item);
            }

            return listado;
        }

        #endregion
        
        #region Actualizar/Cambiar status de documentos
        private void ActualizarStatus()
        {
            try
            {
                IntegraVentasBandejaDB bandejaDB = new IntegraVentasBandejaDB(configuracion, Environment.UserName);

                bandejaDB.eventoProgreso += reportaProgreso;
                //Si la factura fue contabilizada en GP cambia el status de INTEGRADO a CONTABILIZADO
                bandejaDB.ProcesaBandejaDBActualizaStatus("INTEGRADO", "CONTABILIZA_FACTURA_EN_GP");

            }
            catch (Exception ex)
            {
                reportaProgreso(0, ex.Message);
            }

        }

        private void tsbActualizarStatus_Click(object sender, EventArgs e)
        {
            ActualizarStatus();
            filtrarFacturas();
        }


        void ActualizarStatus(int idLog, string docStatus, string transicion)
        {
            configuracion.GetParametros(cmbBxCompannia.SelectedIndex);
            IntegraVentasBandejaDB IntegraSOP = new IntegraVentasBandejaDB(configuracion, Environment.UserName);
            IntegraSOP.eventoProgreso += new IntegraVentasBandejaDB.LogHandler(reportaProgreso);
            IntegraSOP.ProcesaBandejaDB(idLog, docStatus, transicion);

        }

        private void tsMenuItemCambiarAListo_Click(object sender, EventArgs e)
        {
            txtbxMensajes.Text = "";
            txtbxMensajes.Refresh();

            if (dgvFacturas.RowCount == 0)
            {
                txtbxMensajes.Text = "No hay documentos para procesar. Verifique los criterios de búsqueda.";
            }
            else
                try
                {
                    string idLog = dgvFacturas.SelectedRows[0].Cells[1].Value.ToString();
                    string docStatus = dgvFacturas.SelectedRows[0].Cells[8].Value.ToString();

                    this.ActualizarStatus(int.Parse(idLog), docStatus, "ELIMINA_FACTURA_EN_GP");
                    filtrarFacturas();
                    reportaProgreso(0, "Proceso finalizado.");
                }
                catch (Exception gr)
                {

                    reportaProgreso(0, gr.Message);
                }

        }

        private void anuleDespuesDeContabilizadaTsMenuItem_Click(object sender, EventArgs e)
        {
            txtbxMensajes.Text = "";
            txtbxMensajes.Refresh();

            if (dgvFacturas.RowCount == 0)
            {
                txtbxMensajes.Text = "No hay documentos para procesar. Verifique los criterios de búsqueda.";
            }
            else
                try
                {
                    string idLog = dgvFacturas.SelectedRows[0].Cells[1].Value.ToString();
                    string docStatus = dgvFacturas.SelectedRows[0].Cells[8].Value.ToString();

                    this.ActualizarStatus(int.Parse(idLog), docStatus, "ANULA_FACTURA_RM_EN_GP");
                    filtrarFacturas();
                    reportaProgreso(0, "Proceso finalizado.");
                }
                catch (Exception gr)
                {

                    reportaProgreso(0, gr.Message);
                }
        }

        #endregion

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-6);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos7tsMenuItem6.Text;
            filtrarFacturas();

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today;
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = hoytsMenuItem4.Text;
            filtrarFacturas();

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-1);
            fechaFin = DateTime.Today.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ayertsMenuItem5.Text;
            filtrarFacturas();

        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-29);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos30tsMenuItem7.Text;
            filtrarFacturas();

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            fechaIni = DateTime.Today.AddDays(-59);
            fechaFin = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = ultimos60tsMenuItem8.Text;
            filtrarFacturas();

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            fechaIni = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fechaFin = fechaIni.AddMonths(1);
            int ultimoDia = fechaFin.Day;
            fechaFin = fechaFin.AddDays(-ultimoDia);
            checkBoxFecha.Checked = false;
            tsDropDownFiltro.Text = mesActualtsMenuItem9.Text;
            filtrarFacturas();

        }

        /// <summary>
        /// Filtra las facturas marcadas en el grid y memoriza las filas no marcadas.
        /// </summary>
        /// <param name=""></param>
        /// <returns>bool: True indica que la lista ha sido filtrada exitosamente</returns>
        public IList<vwCfdiTransaccionesDeVenta> filtraListaSeleccionada(IList<vwCfdiTransaccionesDeVenta> lFacturas)
        {
            int i = 1;
            object[] llaveDocumento = new object[2];
            LDocsNoSeleccionados = new List<vwCfdiTransaccionesDeVenta>();
            dgvFacturas.EndEdit();
            tsProgressBar1.Value = 0;
            //cargar lista de no seleccionados
            foreach (DataGridViewRow dgvr in dgvFacturas.Rows)
            {
                if (!(dgvr.Cells[idxChkBox].Value != null && (dgvr.Cells[idxChkBox].Value.Equals(true) || dgvr.Cells[idxChkBox].Value.ToString().Equals("1"))))
                {
                    lFacturas.Where(x => x.soptype.ToString().Equals(dgvr.Cells[idxSoptype].Value.ToString()) && x.sopnumbe.Equals(dgvr.Cells[idxSopnumbe].Value.ToString()))
                            .First().marcado = false;
                }
                else
                {
                    lFacturas.Where(x => x.soptype.ToString().Equals(dgvr.Cells[idxSoptype].Value.ToString()) && x.sopnumbe.Equals(dgvr.Cells[idxSopnumbe].Value.ToString()))
                            .First().marcado = true;
                }

                tsProgressBar1.Value = Convert.ToInt32(i * 100 / dgvFacturas.RowCount);
                i++;
            }

            tsProgressBar1.Value = 0;
            LDocsNoSeleccionados = lFacturas.Where(x => x.marcado.Equals(false)).ToList();
            bool vacio = dgvFacturas.RowCount == LDocsNoSeleccionados.Count;
            if (vacio)
                throw new ArgumentNullException( "[filtraListaSeleccionada] No ha marcado ningún documento. Marque al menos una casilla en la primera columna para continuar con el proceso.\r\n");

            var marcadas = lFacturas.Where(x => x.marcado.Equals(true)).ToList();
            return (marcadas);
  
        }
        private bool ExistenTransaccionesAMedioContabilizar()
        {
            List<string> t = new List<string>();
            foreach (vwCfdiTransaccionesDeVenta item in listaDeFacturas)
            {
                t.Add(item.soptype.ToString() + "-" + item.sopnumbe);
            }
            var ragrupado = t.GroupBy(f => f)
                            .Where(repetido => repetido.Count() > 1)
                            .ToList();

            if (ragrupado.Count() > 0)
            {
                reportaProgreso(0, "Las siguientes facturas todavía no terminaron de contabilizar:");
                //ragrupado.ForEach(i => txtbxMensajes.AppendText(i.FirstOrDefault()));
                ragrupado.ForEach(i => reportaProgreso(0, i.FirstOrDefault()));
                reportaProgreso(0, "Espere a que finalice la contabilización y vuelva a intentar.");

            }
            return (ragrupado.Count() > 0);
        }

        private void HabilitarVentana(bool emite, bool anula, bool imprime, bool publica, bool envia, bool cambiaCia, bool integra)
        {
            cmbBxCompannia.Enabled = cambiaCia;
            tsBtnIntegraFactura.Enabled = integra;  
            tsButtonGenerarTxt.Enabled = emite;
            //tsButtonGeneraXml.Enabled = emite;

            //toolStripConsulta.Enabled = emite || anula || imprime || publica || envia;
            btnBuscar.Enabled = emite || anula || imprime || publica || envia || integra;
        }

        private void tsButtonGenerarTxt_Click(object sender, EventArgs e)
        {
            int errores = 0;
            txtbxMensajes.Text = "";

            if (listaDeFacturas.Count == 0)
            {
                txtbxMensajes.Text = "No hay documentos para generar. Verifique los criterios de búsqueda.";
                errores++;
            }

            try
            {
                var listaSeleccionadaPorUsuario = filtraListaSeleccionada(listaDeFacturas); //Filtra cfdiTransacciones sólo con docs marcados
                if (errores == 0 && !ExistenTransaccionesAMedioContabilizar())
                {
                    HabilitarVentana(false, false, false, false, false, false, false);
                    ProcesaCfdi proc = new ProcesaCfdi(lblUsuario.Text);
                    proc.Progreso += new ProcesaCfdi.LogHandler(reportaProgreso);
                    //pBarProcesoActivo.Visible = true;

                    if (this.tabNotaFiscal.SelectedTab.Name.Equals("gpFactura"))
                    {
                        var serviciosPrefeitura = new WebServicesNfe();
                        proc.GeneraDocumentoTxt(listaSeleccionadaPorUsuario, mainController, serviciosPrefeitura);
                    }
                }
                //Actualiza la pantalla
                HabilitarVentana(true, false, false, false, false, true, true);
                filtrarFacturas();
                tsProgressBar1.Value = 0;
                //pBarProcesoActivo.Visible = false;

            }
            catch (Exception ex)
            {
                reportaProgreso(0, ex.Message);
            }
        }

        void InicializaCheckBoxDelGrid(DataGridView dataGrid, short idxChkBox, bool marca)
        {
            for (int r = 0; r < dataGrid.RowCount; r++)
            {
                dataGrid[idxChkBox, r].Value = marca;
            }
            dataGrid.EndEdit();
        }
        void InicializaCheckBoxDelGrid(DataGridView dataGrid, short idxChkBox, List<vwCfdiTransaccionesDeVenta> LNoSeleccionados)
        {
            for (int r = 0; r < dataGrid.RowCount; r++)
            {
                dataGrid[idxChkBox, r].Value = !LNoSeleccionados.Exists(x => x.sopnumbe.Equals(dataGrid[idxSopnumbe, r].Value.ToString()) 
                                                                            && x.docid.ToString().Equals(dataGrid[idxIdDoc, r].Value.ToString()));
            }
            dataGrid.EndEdit();
            dataGrid.Refresh();
        }

        private void checkBoxMark_CheckedChanged(object sender, EventArgs e)
        {
            InicializaCheckBoxDelGrid(dgvFacturas, idxChkBox, checkBoxMark.Checked);
        }


    }
}
