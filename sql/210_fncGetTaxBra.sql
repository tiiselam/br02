USE [GBRA]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetTaxBra]    Script Date: 3/29/2019 2:32:07 PM ******/
IF OBJECT_ID (N'dbo.fncGetTaxBra') IS NOT NULL
DROP FUNCTION [dbo].[fncGetTaxBra]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetTaxBra]    Script Date: 3/29/2019 2:32:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fncGetTaxBra] (@INSopType smallint
									,@INSopNumbe CHAR(21)
									,@INTaxdtlid CHAR(15)
									,@INLnItmSeq CHAR(15)
									)
RETURNS numeric(19,5)
AS
BEGIN
		begin
			DECLARE @TaxImport numeric(19,5)
			SELECT @TaxImport=SUM(A.STAXAMNT)	
			FROM SOP10105 A
			WHERE A.SOPTYPE = @INSopType
			AND A.SOPNUMBE = @INSopNumbe
			AND A.TAXDTLID LIKE '%'+ rtrim(@INTaxdtlid)+'%'
			AND A.LNITMSEQ = @INLnItmSeq

		end

	RETURN isnull((@TaxImport),0)
END
GO

IF (@@Error = 0) PRINT 'Creación exitosa de: fncGetTaxBra'
ELSE PRINT 'Error en la creación de: fncGetTaxBra'
GO


