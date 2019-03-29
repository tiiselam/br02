--Factura electrónica 
--Propósito. Crea vista de los id de documentos que se incluyen en factura electrónica.
--
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[vwCfdIdDocumentos]') AND OBJECTPROPERTY(id,N'IsView') = 1)
    DROP view dbo.[vwCfdIdDocumentos];
GO
create view dbo.vwCfdIdDocumentos as
--Propósito. Obtiene id de documentos de venta gp
--Utilizado por. Factura electrónica
--28/03/19 jcf Creación
--
select ds.soptype, ds.docid, ds.SOPNUMBE
from sop40200 ds			--sop_id_setp
where soptype in (3, 4)
--where exists (
--	select invdocid 
--	from SOP40100			--sop_setp
--	where (INVDOCID = ds.DOCID
--		or RETDOCID = ds.DOCID)
--	)
go
IF (@@Error = 0) PRINT 'Creación exitosa de la vista: vwCfdIdDocumentos'
ELSE PRINT 'Error en la creación de la vista: vwCfdIdDocumentos'
GO

----------------------------------------------------------------------------------------------------------------------------------------
