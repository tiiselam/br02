USE [GBRA]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetConceptoBra]    Script Date: 3/29/2019 2:32:00 PM ******/
IF OBJECT_ID (N'dbo.fncGetConceptoBra') IS NOT NULL
DROP FUNCTION [dbo].[fncGetConceptoBra]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetConceptoBra]    Script Date: 3/29/2019 2:32:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[fncGetConceptoBra] (@INSopType smallint
										 ,@INSopNumbe CHAR(21)
																		)
RETURNS CHAR(1000)
AS

BEGIN
	DECLARE @Description AS CHAR(1000)
	DECLARE @Concepto AS CHAR(1000)
	DECLARE @TEMP AS CHAR(1000)
  	DECLARE 
		Curc CURSOR FOR SELECT 	 LTRIM(RTRIM(A.ITEMNMBR))+	' ' + LTRIM(RTRIM(A.ITEMDESC)) + ' ' 
									+ RTRIM(CONVERT(CHAR,A.UNITPRCE)) + ' |' 
									--+ RTRIM(CONVERT(CHAR,A.ReqShipDate,103)) + ' ' 
									--+ RTRIM(CONVERT(CHAR,A.ACTLSHIP,103)) + ' ' 
									--+ RTRIM(CONVERT(CHAR,B.CMMTTEXT))
							FROM SOP30300 A
							     LEFT OUTER JOIN SY04200 B ON B.COMMNTID = A.COMMNTID
							WHERE A.SOPTYPE = @INSopType
							AND A.SOPNUMBE = @INSopNumbe
							AND A.QUANTITY = 0
                          union 
						  SELECT 	 LTRIM(RTRIM(A.ITEMNMBR))+	' ' + LTRIM(RTRIM(A.ITEMDESC)) + ' ' 
									+ RTRIM(CONVERT(CHAR,A.UNITPRCE)) + ' |' 
									--+ RTRIM(CONVERT(CHAR,A.ReqShipDate,103)) + ' ' 
									--+ RTRIM(CONVERT(CHAR,A.ACTLSHIP,103)) + ' ' 
									--+ RTRIM(CONVERT(CHAR,B.CMMTTEXT))
							FROM SOP10200 A
							     LEFT OUTER JOIN SY04200 B ON B.COMMNTID = A.COMMNTID
							WHERE A.SOPTYPE = @INSopType
							AND A.SOPNUMBE = @INSopNumbe
							AND A.QUANTITY = 0        
	
	OPEN Curc
	FETCH NEXT FROM Curc INTO @Description
	WHILE @@fetch_status = 0
		BEGIN
			select @Concepto=CONCAT(RTRIM(@TEMP)+' ',@Description)	
			select @TEMP=RTRIM(@Concepto)
			FETCH NEXT FROM Curc INTO @Description
		END
	CLOSE Curc
	DEALLOCATE Curc
	
	Return @Concepto
	
END
GO

IF (@@Error = 0) PRINT 'Creación exitosa de: fncGetConceptoBra'
ELSE PRINT 'Error en la creación de: fncGetConceptoBra'
GO


