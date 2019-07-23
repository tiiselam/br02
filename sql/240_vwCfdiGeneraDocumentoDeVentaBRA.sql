USE [GBRA]
GO

/****** Object:  View [dbo].[vwCfdiGeneraDocumentoDeVentaBRA]    Script Date: 23/07/2019 11:10:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








ALTER view [dbo].[vwCfdiGeneraDocumentoDeVentaBRA]
as
--Propósito. Elabora un comprobante xml para factura electrónica cfdi Perú
--Requisitos.  
--27/11/17 jcf Creación cfdi Perú
--27/04/18 jcf Ajusta montos exonerado, inafecto, gratuito
--06/06/18 jcf Agrega montos funcionales (pen)
--13/08/18 jcf Agrega emailTo y formaPago
--08/11/18 jcf Agrega ajustes para ubl 2.1
--16/01/19 jcf Agrega dirección
--21/02/19 jcf Agrega leyenda por factura
--18/07/19 msal incorpora datos adicionales para identificar el archivo
	select	
		convert(varchar(20),1)													correlativo, 
		CMP.TAXREGTN															CPFCNPJRemetente,
		0																		transacao,
		1																		QtdRPS,
		FAC.soptype,
		FAC.SOPNUMBE,
		SUBSTRING(FAC.SOPNUMBE,1,1)												serie, --?????
		RTRIM(SUBSTRING(FAC.SOPNUMBE,2,100))									numero,
		'3'											emisorTipoDoc, 
		''											emisorNroDoc,
		CMP.CMPNYNAM															emisorNombre,
		Substring(INFO.INETINFO,charindex('NRO_INSCRIP=',INFO.INETINFO,1)+12,8)	InscricaoPrestador,
		'1'																		TipoRPS,
		FAC.DOCDATE																fechaEmision,
		convert(varchar,FAC.DOCDATE, 8)											horaEmision,
		'N'																		StatusRPS, --Ver la tabla porque falta
		RTRIM(UDF.USRTAB01)														TributacaoRPS,
		FAC.DOCAMNT																montoTotalVenta,
		FAC.TRDISAMT															descuentoGlobalMonto,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-PIS',DET.LNITMSEQ)			ValorPIS,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-COFIN',DET.LNITMSEQ)		ValorCOFINS,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-INSS',DET.LNITMSEQ)		ValorINSS	,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-IR',DET.LNITMSEQ)			ValorIR,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-CSLL',DET.LNITMSEQ)		ValorCSLL,
		RTRIM(ITE.ITMSHNAM)														CodigoServico,
		CONVERT(CHAR,dbo.fncGetTaxRateBra(DET.SOPTYPE,DET.SOPNUMBE,'S-ISS',DET.LNITMSEQ))	AliquotaServicos ,
		RTRIM(UDF.USRTAB09)														ISSRetido ,
		RTRiM(CST.TXRGNNUM)														CPFCNPJTomador ,
		'3'																		indicadorTomador,
		RTRiM(CST.TAXEXMT1)														InscricaoMunicipalTomador ,
		RTRiM(CST.TAXEXMT2)														InscricaoEstadualTomador ,
		RTRiM(CST.CUSTNAME)	 													RazaoSocialTomador ,
		'3'																		indicadorIntermediario,
		'00000000000000'														CPFCNPJIntermediario,
		'N'																		ISSRetidoIntermediario,
		RTRiM(SUBSTRING(CST.ADDRESS1,1,3))										Emisor_TipoLogradouro,
		RTRiM(SUBSTRING(CST.ADDRESS1,4,100))									Emisor_Logradouro ,
		RTRiM(CST.ADDRESS2)														Emisor_NumeroEndereco ,
		RTRiM(CST.ADDRESS3)														Emisor_Bairro ,
		RTRIM(CST.CITY)															Emisor_Cidade ,
		RTRIM(CST.STATE)														Emisor_UF ,
		RTRIM(CST.ZIP)															Emisor_CEP ,
		CASE WHEN em.emailTo is not null then em.emailTo  END 
		--+ CASE WHEN em.emailCC is not null then em.emailCC END 
		--+CASE WHEN em.emailCCO is not null then em.emailCCO END					
																				EmailTomador ,
		RTRIM(dbo.fncGetConceptoBra(DET.SOPTYPE,DET.SOPNUMBE,FAC.REFRENCE,DET.ITEMDESC))
		+ 'Venc: ' +  + RIGHT(RTRIM(CONVERT(CHAR,FAC.DUEDATE,3)),8)  + '|' --Inicio
		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('FIX_MSJ=',INFO.INETINFO,1)+8
								 ,charindex('TRIB=',INFO.INETINFO,1)-8 - charindex('FIX_MSJ=',INFO.INETINFO,1))
					),CHAR(9),''),CHAR(10),''),CHAR(13),'')  +' |'
		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('TRIB=',INFO.INETINFO,1)+5
								 ,charindex('FONTE=',INFO.INETINFO,1)-5 - charindex('TRIB=',INFO.INETINFO,1))
		
				),CHAR(9),''),CHAR(10),''),CHAR(13),'') +' '
 		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('FONTE=',INFO.INETINFO,1)+6
								 , 100)),CHAR(9),''),CHAR(10),''),CHAR(13),'')
											Concepto ,
		0											ValorCargaTributaria ,
		0											PercentualCargaTributaria ,
		'IBPT'										FonteCargaTributaria ,
		FAC.DOCAMNT									ValorTotalRecebido 

	from  SOP30200                AS FAC
		  LEFT OUTER JOIN SOP10106	  AS UDF ON FAC.SOPTYPE = UDF.SOPTYPE AND FAC.SOPNUMBE = UDF.SOPNUMBE
		  InNER JOIN SOP30300		AS DET ON FAC.SOPTYPE = DET.SOPTYPE AND FAC.SOPNUMBE = DET.SOPNUMBE AND DET.QUANTITY != 0
		  INNER JOIN IV00101		AS ITE ON DET.ITEMNMBR = ITE.ITEMNMBR
		  INNER JOIN RM00101		AS CST ON FAC.CUSTNMBR = CST.CUSTNMBR
		  LEFT OUTER JOIN SY04200 B ON B.COMMNTID = FAC.COMMNTID
		  LEFT OUTER JOIN vwCfdClienteDireccionesCorreo em ON FAC.CUSTNMBR = em.CUSTNMBR,
		  DYNAMICS.dbo.SY01500    AS CMP
		  LEFT OUTER JOIN SY01200 AS INFO ON CMP.INTERID = INFO.MASTER_ID
	where CMP.INTERID = DB_NAME()
     AND  INFO.ADRSCODE = 'NOTA_FISCAL' 
	 AND  INFO.MASTER_TYPE = 'CMP'
union all
select	
		convert(varchar(20),1)													correlativo, 
		CMP.TAXREGTN															CPFCNPJRemetente,
		0																		transacao,
		1																		QtdRPS,
		FAC.soptype,
		FAC.SOPNUMBE,
		SUBSTRING(FAC.SOPNUMBE,1,1)												serie, --?????
		RTRIM(SUBSTRING(FAC.SOPNUMBE,2,100))									numero,
		'3'																		emisorTipoDoc, 
		''																		emisorNroDoc,
		CMP.CMPNYNAM															emisorNombre,
		Substring(INFO.INETINFO,charindex('NRO_INSCRIP=',INFO.INETINFO,1)+12,8)	InscricaoPrestador,
		'1'																		TipoRPS,
		FAC.DOCDATE																fechaEmision,
		convert(varchar,FAC.DOCDATE, 8)											horaEmision,
		'N'																		StatusRPS,
		RTRIM(UDF.USRTAB01)														TributacaoRPS,
		FAC.DOCAMNT																montoTotalVenta,
		FAC.TRDISAMT															descuentoGlobalMonto,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-PIS',DET.LNITMSEQ)			ValorPIS,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-COFIN',DET.LNITMSEQ)		ValorCOFINS,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-INSS',DET.LNITMSEQ)		ValorINSS	,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-IR',DET.LNITMSEQ)			ValorIR,
		dbo.fncGetTaxBra(DET.SOPTYPE,DET.SOPNUMBE,'S-CSLL',DET.LNITMSEQ)		ValorCSLL,
		RTRIM(ITE.ITMSHNAM)														CodigoServico,
		CONVERT(CHAR,dbo.fncGetTaxRateBra(DET.SOPTYPE,DET.SOPNUMBE,'S-ISS',DET.LNITMSEQ))	AliquotaServicos ,
		RTRIM(UDF.USRTAB09)														ISSRetido ,
		RTRiM(CST.TXRGNNUM)														CPFCNPJTomador ,
		'3'																		indicadorTomador,
		RTRiM(CST.TAXEXMT1)														InscricaoMunicipalTomador ,
		RTRiM(CST.TAXEXMT2)														InscricaoEstadualTomador ,
		RTRiM(CST.CUSTNAME)	 													RazaoSocialTomador ,
		'3'																		indicadorIntermediario,
		'00000000000000'														CPFCNPJIntermediario,
		'N'																		ISSRetidoIntermediario,
		RTRiM(SUBSTRING(CST.ADDRESS1,1,3))										Emisor_TipoLogradouro,
		RTRiM(SUBSTRING(CST.ADDRESS1,4,100))									Emisor_Logradouro ,
		RTRiM(CST.ADDRESS2)														Emisor_NumeroEndereco ,
		RTRiM(CST.ADDRESS3)														Emisor_Bairro ,
		RTRIM(CST.CITY)															Emisor_Cidade ,
		RTRIM(CST.STATE)														Emisor_UF ,
		RTRIM(CST.ZIP)															Emisor_CEP ,
		CASE WHEN em.emailTo is not null then em.emailTo  END 
		--+ CASE WHEN em.emailCC is not null then em.emailCC END 
		--+CASE WHEN em.emailCCO is not null then em.emailCCO END					
																				EmailTomador ,
		RTRIM(dbo.fncGetConceptoBra(DET.SOPTYPE,DET.SOPNUMBE,FAC.REFRENCE,DET.ITEMDESC))
		+ 'Venc: ' +  + RIGHT(RTRIM(CONVERT(CHAR,FAC.DUEDATE,3)),8) + '|' --Inicio
		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('FIX_MSJ=',INFO.INETINFO,1)+8
								 ,charindex('TRIB=',INFO.INETINFO,1)-8 - charindex('FIX_MSJ=',INFO.INETINFO,1))
					),CHAR(9),''),CHAR(10),''),CHAR(13),'')  +'|'
		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('TRIB=',INFO.INETINFO,1)+5
								 ,charindex('FONTE=',INFO.INETINFO,1)-5 - charindex('TRIB=',INFO.INETINFO,1))
		
				),CHAR(9),''),CHAR(10),''),CHAR(13),'') +' '
 		+ REPLACE(REPLACE(REPLACE(RTRIM(Substring(INFO.INETINFO,charindex('FONTE=',INFO.INETINFO,1)+6
								 , 100)),CHAR(9),''),CHAR(10),''),CHAR(13),'')
											Concepto ,
		0											ValorCargaTributaria ,
		0											PercentualCargaTributaria ,
		'IBPT'										FonteCargaTributaria ,
		FAC.DOCAMNT									ValorTotalRecebido 
from  SOP10100                AS FAC
		  LEFT OUTER JOIN SOP10106	  AS UDF ON FAC.SOPTYPE = UDF.SOPTYPE AND FAC.SOPNUMBE = UDF.SOPNUMBE
		  InNER JOIN SOP10200		AS DET ON FAC.SOPTYPE = DET.SOPTYPE AND FAC.SOPNUMBE = DET.SOPNUMBE AND DET.QUANTITY != 0
		  INNER JOIN IV00101		AS ITE ON DET.ITEMNMBR = ITE.ITEMNMBR
		  INNER JOIN RM00101		AS CST ON FAC.CUSTNMBR = CST.CUSTNMBR
		  LEFT OUTER JOIN SY04200 B ON B.COMMNTID = FAC.COMMNTID
		  LEFT OUTER JOIN vwCfdClienteDireccionesCorreo em ON FAC.CUSTNMBR = em.CUSTNMBR,
		  DYNAMICS.dbo.SY01500    AS CMP
		  LEFT OUTER JOIN SY01200 AS INFO ON CMP.INTERID = INFO.MASTER_ID
	where CMP.INTERID = DB_NAME()
     AND  INFO.ADRSCODE = 'NOTA_FISCAL' 
	 AND  INFO.MASTER_TYPE = 'CMP'












GO


