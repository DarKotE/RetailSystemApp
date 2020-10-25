CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id INT OUTPUT,
	@CashierId NVARCHAR(128),
	@SaleDate DATETIME2,
	@SubTotal MONEY,
	@Tax money,
	@Total money
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO [dbo].[Sale](CashierId,SubTotal,SaleDate,Tax,Total)
	VALUES (@CashierId,@SubTotal,@SaleDate,@Tax,@Total)

	SELECT @Id = SCOPE_IDENTITY();
END
