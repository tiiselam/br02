USE [GBRA]
GO

/****** Object:  UserDefinedFunction [dbo].[fncGetConceptoBra]    Script Date: 23/07/2019 11:10:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO










ALTER FUNCTION [dbo].[fncGetConceptoBra] (@INSopType smallint
										 ,@INSopNumbe CHAR(21)
										 ,@INFileType CHAR(30)
										 ,@INLeyenda  CHAR(1650)
																		)
RETURNS CHAR(1650)
AS

BEGIN
	DECLARE @Description AS CHAR(1900)
	DECLARE @Concepto AS VARCHAR(1650)
	DECLARE @TEMP AS VARCHAR(1650)
  	
	DECLARE	Curc CURSOR FOR 
				SELECT 
					CASE 
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in( 'RM') THEN			    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 12),12) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 30),30) + ' ' -- Uso
								+ RIGHT(REPLICATE(' ',22)+ isnull(Substring(C.COMMENT_1,charindex('-',C.COMMENT_1,charindex('-',C.COMMENT_1,1)+1)+1,22),' '),22) + ' '-- Industria
								+ RIGHT(isnull(Substring(RTRIM(CONVERT(CHAR,C.COMMENT_1)),1,1),'  '),2) + ' '-- Prot
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ReqShipDate,3)),8) + ' ' --Inicio
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ACTLSHIP,3)),8) + ' ' --Fin
								+ ISNULL(Substring(RTRIM(C.COMMENT_1),charindex('-',C.COMMENT_1,1)+1,2),'  ') + ' '-- Territ
							    + RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in( 'PREMIUM') THEN			    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 20),20) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 30),30) + ' ' -- Uso
								+ REPLICATE(' ',17+2+8+8+2) + ' '
							    + RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RF','ISTOCK','PAXP') THEN
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 20),20) + ' ' -- Imagem
								+ LEFT(RTRIM(A.ITEMDESC)+REPLICATE(' ', 67) ,67) + ' '-- Descipsao Tamabnho
								+ RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RR') THEN			    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 12),12) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 50),50) + ' ' -- Utilizasao
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ReqShipDate,3)),8) + ' ' --Inicio
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ACTLSHIP,3)),8) + ' ' --Fin
								+ RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						END
				FROM SOP30300 A
				     LEFT OUTER JOIN SY04200 B ON B.COMMNTID = A.COMMNTID
					 LEFT OUTER JOIN SOP10202 C ON C.SOPTYPE = A.SOPTYPE and C.SOPNUMBE = A.SOPNUMBE AND C.LNITMSEQ = A.LNITMSEQ
				WHERE A.SOPTYPE = @INSopType
				AND A.SOPNUMBE = @INSopNumbe
				AND A.QUANTITY = 0
				union 
				SELECT 
					CASE
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in( 'RM') THEN			    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 12),12) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 30),30) + ' ' -- Uso
								+ RIGHT(REPLICATE(' ',22)+ isnull(Substring(C.COMMENT_1,charindex('-',C.COMMENT_1,charindex('-',C.COMMENT_1,1)+1)+1,22),' '),22) + ' '-- Industria
								+ RIGHT(isnull(Substring(RTRIM(CONVERT(CHAR,C.COMMENT_1)),1,1),'  '),2) + ' '-- Prot
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ReqShipDate,3)),8) + ' ' --Inicio
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ACTLSHIP,3)),8) + ' ' --Fin
								+ ISNULL(Substring(RTRIM(C.COMMENT_1),charindex('-',C.COMMENT_1,1)+1,2),'  ') + ' '-- Territ
							    + RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in( 'PREMIUM') THEN			    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 20),20) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 30),30) + ' ' -- Uso
								+ REPLICATE(' ',17+2+8+8+2) + ' '
							    + RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RF','ISTOCK','PAXP') THEN
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 20),20) + ' ' -- Imagem
								+ LEFT(RTRIM(A.ITEMDESC)+REPLICATE(' ', 67) ,67) + ' '-- Descipsao Tamabnho
								+ RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RR') THEN			    	    
								LEFT(LTRIM(RTRIM(A.ITEMNMBR))+REPLICATE(' ', 12),12) + ' ' -- Imagem
								+ LEFT(LTRIM(RTRIM(A.ITEMDESC))+ REPLICATE(' ', 50),50) + ' ' -- Utilizasao
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ReqShipDate,3)),8) + ' ' --Inicio
								+ RIGHT(RTRIM(CONVERT(CHAR,A.ACTLSHIP,3)),8) + ' ' --Fin
								+ RIGHT(REPLICATE(' ',12) + RTRIM(cast(format(A.UNITPRCE, 'N', 'de-de') as char)),12) + '|' --VAlor
						END
				FROM SOP10200 A
				     LEFT OUTER JOIN SY04200 B ON B.COMMNTID = A.COMMNTID
					 LEFT OUTER JOIN SOP10202 C ON C.SOPTYPE = A.SOPTYPE and C.SOPNUMBE = A.SOPNUMBE AND C.LNITMSEQ = A.LNITMSEQ
				WHERE A.SOPTYPE = @INSopType
				AND A.SOPNUMBE = @INSopNumbe
				AND A.QUANTITY = 0        
	
	SELECT @TEMP=CASE 
					WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in( 'RM','PREMIUM') THEN
							-- 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
							--'Imagem	            Uso	              Industria       Prot Inicio   Termino  Territ   Valor|'
							'Cessão de dereitos'  +' |'
							+ LEFT(LTRIM(RTRIM('Imagem'))+REPLICATE(' ', 12),12) + ' ' -- Imagem
							+ LEFT(LTRIM(RTRIM('Uso'	))+ REPLICATE(' ', 30),30) + ' ' -- Uso
							+ Substring('Industria               ',1,22) + ' '-- Industria
							+ RIGHT(Substring(RTRIM(CONVERT(CHAR,'Prot')),1,1),2) + ' '-- Prot
							+ RIGHT(REPLICATE(' ',8)+'Início',8) + ' ' --Inicio
							+ RIGHT(REPLICATE(' ',8)+'Término',8) + ' ' --Fin
							+ Substring(RTRIM('Territ'),1,2) + ' '-- Territ
							+ RIGHT(REPLICATE(' ',12) + RTRIM('Valor'),12) + '|' --VAlor
					WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RF','ISTOCK','PAXP') THEN 
							-- 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
							'Cessão de dereitos'  +' |'
							+ LEFT(LTRIM(RTRIM('Imagem'))+REPLICATE(' ', 20),20) + ' ' -- Imagem
							+ LEFT(RTRIM('Descripção Tamanho')+REPLICATE(' ', 67) ,67) + ' '-- Descipsao Tamabnho
							+ RIGHT(REPLICATE(' ',12) + RTRIM('Valor'),12) + '|' --VAlor
					WHEN UPPER(Substring(RTRIM(@INFileType),1,charindex('-',@INFileType,1)-2)) in('RR') THEN 
							'Cessão de dereitos'  +' |'
							+ LEFT(LTRIM(RTRIM('Imagem'))+REPLICATE(' ', 12),12) + ' ' -- Imagem
							+ LEFT(LTRIM(RTRIM('Utilização'))+ REPLICATE(' ', 50),50) + ' ' -- Utilizasao
							+ RIGHT(REPLICATE(' ',8)+'Início',8) + ' ' --Inicio
							+ RIGHT(REPLICATE(' ',8)+'Término',8) + ' ' --Fin
							+ RIGHT(REPLICATE(' ',12) + RTRIM('Valor'),12) + '|' --VAlor
					END 

	OPEN Curc
	SELECT @Concepto=''
	FETCH NEXT FROM Curc INTO @Description
	WHILE @@fetch_status = 0
		BEGIN
			IF (LEN(@TEMP) + LEN(@Description)) > 1650
				BEGIN
					SELECT @Concepto=RTRIM(@INLeyenda)+'|'
					BREAK
				END
			ELSE
				SELECT @Concepto=CONCAT(RTRIM(@TEMP),@Description)	
			
			select @TEMP=RTRIM(@Concepto)
			
			FETCH NEXT FROM Curc INTO @Description
		END
	CLOSE Curc
	DEALLOCATE Curc
	
	Return @Concepto
	
END








GO


