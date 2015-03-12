
Alter PROCEDURE SaveSource
	-- Add the parameters for the stored procedure here
	@tags varchar(max),
	@UserID bigint,
	@title varchar(500),
	@link nvarchar(1000),
	@summary nvarchar(2000),
	@readLater bit,
	@saveOffline bit,
	@privacy bit,
	@rating int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	return 0
   
END
GO
