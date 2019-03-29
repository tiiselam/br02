IF OBJECT_ID ('dbo.fCfdiUofM') IS NOT NULL
   DROP FUNCTION dbo.fCfdiUofM
GO

create function dbo.fCfdiUofM(@UOMSCHDL varchar(11), @UOFM varchar(9))
returns table
as
--Propósito. Obtiene la descripción larga de la unidad de medida 
--Requisitos. 
--02/08/12 jcf Creación 
--
return
( 
	select UOFMLONGDESC
	from iv40202	--unidades de medida [UOMSCHDL SEQNUMBR]
	WHERE UOMSCHDL = @UOMSCHDL
	and UOFM = @UOFM 
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiUofM()'
ELSE PRINT 'Error en la creación de la función: fCfdiUofM()'
GO

------------------------------------------------------------------------------------------
