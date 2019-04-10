USE [GBRA]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetTaxRateBra]    Script Date: 3/29/2019 2:32:17 PM ******/
IF OBJECT_ID (N'dbo.fncGetTaxRateBra') IS NOT NULL
DROP FUNCTION [dbo].[fncGetTaxRateBra]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetTaxRateBra]    Script Date: 3/29/2019 2:32:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fncGetTaxRateBra] (@INSopType smallint
									,@INSopNumbe CHAR(21)
									,@INTaxdtlid CHAR(15)
									,@INLnItmSeq CHAR(15)
									)
RETURNS CHAR(19)
AS
BEGIN
	
		begin
			DECLARE @TaxRate CHAR(19)
			SELECT @TaxRate=SUM(B.TXDTLPCT)
			FROM SOP10105 A
			    ,TX00201 B  
			WHERE A.SOPTYPE = @INSopType
			AND A.SOPNUMBE = @INSopNumbe
			AND A.TAXDTLID LIKE '%'+ rtrim(@INTaxdtlid)+'%'
			AND A.LNITMSEQ = @INLnItmSeq
			AND A.TAXDTLID = B.TAXDTLID
			AND A.STAXAMNT >0
			
		end
	return isnull((@TaxRate),0)
END
GO

IF (@@Error = 0) PRINT 'Creación exitosa de: fncGetTaxRateBra'
ELSE PRINT 'Error en la creación de: fncGetTaxRateBra'
GO


