--FACTURA ELECTRONICA GP - BRASIL
--Proyectos:		GETTY BRASIL
--Propósito:		Genera funciones y vistas de FACTURAS para la facturación electrónica en GP - BRASIL
--Referencia:		
--Utilizado por:	Aplicación C# de generación de factura electrónica BRASIL
--
--------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fCfdiCertificadoVigente') IS NOT NULL
   DROP FUNCTION dbo.fCfdiCertificadoVigente
GO

create function dbo.fCfdiCertificadoVigente(@fecha datetime)
returns table
as
--Propósito. Verifica que la fecha corresponde a un certificado vigente y activo
--			Si existe más de uno o ninguno, devuelve el estado: inconsistente
--			También devuelve datos del folio y certificado asociado.
--Requisitos. Los estados posibles para generar o no archivos xml son: no emitido, inconsistente
--28/03/19 jcf Creación
--
return
(  
	--declare @fecha datetime
	--select @fecha = '1/4/12'
	select top 1 --fyc.noAprobacion, fyc.anoAprobacion, 
			fyc.ID_Certificado, fyc.ruta_certificado, fyc.ruta_clave, fyc.contrasenia_clave, fyc.fila, 
			case when fyc.fila > 1 then 'inconsistente' else 'no emitido' end estado
	from (
		SELECT top 2 rtrim(B.ID_Certificado) ID_Certificado, rtrim(B.ruta_certificado) ruta_certificado, rtrim(B.ruta_clave) ruta_clave, 
				rtrim(B.contrasenia_clave) contrasenia_clave, row_number() over (order by B.ID_Certificado) fila
		FROM cfd_CER00100 B
		WHERE B.estado = '1'
			and B.id_certificado <> 'PAC'	--El id PAC está reservado para el PAC
			and datediff(day, B.fecha_vig_desde, @fecha) >= 0
			and datediff(day, B.fecha_vig_hasta, @fecha) <= 0
		) fyc
	order by fyc.fila desc
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiCertificadoVigente()'
ELSE PRINT 'Error en la creación de la función: fCfdiCertificadoVigente()'
GO

--------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fCfdiCertificadoPAC') IS NOT NULL
   DROP FUNCTION dbo.fCfdiCertificadoPAC
GO

create function dbo.fCfdiCertificadoPAC(@fecha datetime)
returns table
as
--Propósito. Obtiene el certificado del PAC. 
--			Verifica que la fecha corresponde a un certificado vigente y activo
--Requisitos. El id PAC está reservado para registrar el certificado del PAC. 
--28/03/19 jcf Creación 
--
return
(  
	--declare @fecha datetime
	--select @fecha = '5/4/12'
	SELECT rtrim(B.ID_Certificado) ID_Certificado, rtrim(B.ruta_certificado) ruta_certificado, rtrim(B.ruta_clave) ruta_clave, 
			rtrim(B.contrasenia_clave) contrasenia_clave
	FROM cfd_CER00100 B
	WHERE B.estado = '1'
		and B.id_certificado = 'PAC'	--El id PAC está reservado para el PAC
		and datediff(day, B.fecha_vig_desde, @fecha) >= 0
		and datediff(day, B.fecha_vig_hasta, @fecha) <= 0
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiCertificadoPAC()'
ELSE PRINT 'Error en la creación de la función: fCfdiCertificadoPAC()'
GO

-----------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiTransaccionesDeVenta', 'V') IS NULL)
   exec('create view dbo.vwCfdiTransaccionesDeVenta as SELECT 1 as t');
go

alter view dbo.vwCfdiTransaccionesDeVenta as
--Propósito. Todos los documentos de venta: facturas y notas de crédito. 
--Usado por. App Factura digital
--Requisitos. El estado "no emitido" indica que no se ha emitido el archivo xml pero que está listo para ser generado.
--			El estado "inconsistente" indica que existe un problema en el folio o certificado, por tanto no puede ser generado.
--			El estado "emitido" indica que el archivo xml ha sido generado y sellado por el PAC y está listo para ser impreso.
--28/03/19 jcf Creación
--

select tv.estadoContabilizado, tv.soptype, tv.docid, tv.sopnumbe, tv.fechahora, 
	tv.CUSTNMBR, tv.nombreCliente, tv.idImpuestoCliente, cast(tv.total as numeric(19,2)) total, tv.montoActualOriginal, tv.voidstts, 

	isnull(lf.estado, isnull(fv.estado, 'inconsistente')) estado,
	case when isnull(lf.estado, isnull(fv.estado, 'inconsistente')) = 'inconsistente' 
		then 'folio o certificado inconsistente'
		else ISNULL(lf.mensaje, tv.estadoContabilizado)
	end mensaje,
	case when isnull(lf.estado, isnull(fv.estado, 'inconsistente')) = 'no emitido' 
		then null	--dbo.fCfdiGeneraDocumentoDeVentaXML (tv.soptype, tv.sopnumbe) 
		else cast('' as xml) 
	end comprobanteXml,
	
	fv.ID_Certificado, fv.ruta_certificado, fv.ruta_clave, fv.contrasenia_clave, 
	isnull(pa.ruta_certificado, '_noexiste') ruta_certificadoPac, isnull(pa.ruta_clave, '_noexiste') ruta_clavePac, isnull(pa.contrasenia_clave, '') contrasenia_clavePac, 
	emi.TAXREGTN rfc, 
	isnull(lf.noAprobacion, '3') regimen, 
	emi.INET7 rutaXml, 
	emi.ZIPCODE codigoPostal,
	isnull(lf.estadoActual, '000100000000') estadoActual, 
	isnull(lf.mensajeEA, tv.estadoContabilizado) mensajeEA,
	tv.curncyid isocurrc,
	null addenda
from dbo.vwCfdiSopTransaccionesVenta tv
	cross join dbo.fCfdiEmisor() emi
	outer apply dbo.fCfdiCertificadoVigente(tv.fechahora) fv
	outer apply dbo.fCfdiCertificadoPAC(tv.fechahora) pa
	left join cfdlogfacturaxml lf
		on lf.soptype = tv.SOPTYPE
		and lf.sopnumbe = tv.sopnumbe
		and lf.estado = 'emitido'

go

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: vwCfdiTransaccionesDeVenta'
ELSE PRINT 'Error en la creación de la vista: vwCfdiTransaccionesDeVenta'
GO

-----------------------------------------------------------------------------------------
