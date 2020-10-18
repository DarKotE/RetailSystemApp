CREATE PROCEDURE [dbo].[spUserLookup]
	@Id NVARCHAR(128)	
AS
BEGIN
	SET NOCOUNT ON;
		
	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate
	FROM [dbo].[User]
	where Id = @Id;
END
