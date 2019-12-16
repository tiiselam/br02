IF OBJECT_ID('dbo.spCfdiActualizaNumeroFiscalElectronico')IS NOT NULL
BEGIN
	DROP PROCEDURE dbo.spCfdiActualizaNumeroFiscalElectronico
END
GO

--Propósito. Actualiza el NFS-e de una nota fiscal en el campo cstponbr de la factura en GP
--06/12/19 p almada. Creación
--16/12/19 jcf Agrega actualización de tablas rm
--
create PROCEDURE dbo.spCfdiActualizaNumeroFiscalElectronico(@SOPTYPE SMALLINT,@NUMFAC VARCHAR(21),@SERNUM VARCHAR(21),@MENS VARCHAR(200) OUTPUT)
AS
	BEGIN TRY
		declare @custnmbr varchar(15);
		set @custnmbr = null;
		IF EXISTS(SELECT * FROM sop10100 WHERE SOPTYPE = @SOPTYPE AND SOPNUMBE= @SERNUM)
		BEGIN
			UPDATE sop10100 SET cstponbr = @NUMFAC WHERE SOPTYPE = @SOPTYPE AND SOPNUMBE= @SERNUM			
			SET @MENS = 'La factura fue actualizada con el NFS-e: ' +@NUMFAC+ ' (no contabilizada)'
		END
		ELSE			
			SELECT @custnmbr = custnmbr FROM SOP30200 WHERE SOPTYPE = @SOPTYPE AND SOPNUMBE= @SERNUM
			IF isnull(@custnmbr, 'no-existe') != 'no-existe'
			BEGIN
				UPDATE SOP30200 SET cstponbr = @NUMFAC 
				WHERE SOPTYPE = @SOPTYPE 
				AND SOPNUMBE = @SERNUM

				update rm20101 set cspornbr = @NUMFAC 
				where DOCNUMBR = @SERNUM
				and CUSTNMBR = @custnmbr
				and RMDTYPAL = @SOPTYPE + case when @SOPTYPE=3 then -2 else +4 end

				update rm30101 set cspornbr = @NUMFAC 
				where DOCNUMBR = @SERNUM
				and CUSTNMBR = @custnmbr
				and RMDTYPAL = @SOPTYPE + case when @SOPTYPE=3 then -2 else +4 end

				SET @MENS = 'La factura fue actualizada con el NFS-e: ' +@NUMFAC+ ' (contabilizada)'
			END
			ELSE
				SET @MENS = 'La factura no existe en GP'
	END TRY
	BEGIN CATCH
		SET @MENS = substring(ERROR_MESSAGE(), 1, 200);
	END CATCH
GO

IF (@@Error = 0) PRINT 'Procedure Creation: spCfdiActualizaNumeroFiscalElectronico Succeeded'
ELSE PRINT 'Procedure Creation: spCfdiActualizaNumeroFiscalElectronico Error on Creation'
GO

