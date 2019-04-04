IF OBJECT_ID ('dbo.fCfdiParametrosTipoLeyenda') IS NOT NULL
   DROP FUNCTION dbo.fCfdiParametrosTipoLeyenda
GO

create function dbo.fCfdiParametrosTipoLeyenda(@ADRSCODE char(15), @Master_Type varchar(3))
returns table
as
--Propósito. Devuelve todo el texto de notas de la dirección @ADRSCODE
--Requisitos. -
--02/01/18 jcf Creación 
--
return
(
	select ia.inetinfo, ia.INET7, ia.INET8
	from SY01200 ia								--coInetAddress Dirección de la compañía
	inner join dbo.synonymGPCompanyMaster ci	--sy_company_mstr 
		on ci.INTERID = DB_NAME()
		and ia.Master_ID = ci.INTERID
		and ia.ADRSCODE = case when @ADRSCODE = 'PREDETERMINADO' then ci.LOCATNID else @ADRSCODE end
	where ia.Master_Type = @Master_Type
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiParametrosTipoLeyenda()'
ELSE PRINT 'Error en la creación de la función: fCfdiParametrosTipoLeyenda()'
GO

-----------------------------------------------------------------------------------------------
--select *
--from dbo.fCfdiParametrosTipoLeyenda('LEYENDASFE', 'CMP')



--NOOOOOO