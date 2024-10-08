/*
Run this script on:

        .\SQL2016.OL_DEV    -  This database will be modified

to synchronize it with:

        .\SQL2016.so

You are recommended to back up your database before running this script

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[COM_CUSTOMER]'
GO
IF OBJECT_ID(N'[dbo].[COM_CUSTOMER]', 'U') IS NULL
CREATE TABLE [dbo].[COM_CUSTOMER]
(
[COM_CUSTOMER_ID] [int] NOT NULL IDENTITY(1, 1),
[CUSTOMER_NAME] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_COM_CUSTOMER] on [dbo].[COM_CUSTOMER]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_COM_CUSTOMER]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[COM_CUSTOMER]', 'U'))
ALTER TABLE [dbo].[COM_CUSTOMER] ADD CONSTRAINT [PK_COM_CUSTOMER] PRIMARY KEY CLUSTERED  ([COM_CUSTOMER_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SO_ITEM]'
GO
IF OBJECT_ID(N'[dbo].[SO_ITEM]', 'U') IS NULL
CREATE TABLE [dbo].[SO_ITEM]
(
[SO_ITEM_ID] [bigint] NOT NULL IDENTITY(1, 1),
[SO_ORDER_ID] [bigint] NOT NULL CONSTRAINT [DF_Table_1_SALES_SO_ID] DEFAULT ((-99)),
[ITEM_NAME] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ITEM_ITEM_NAME] DEFAULT (''),
[QUANTITY] [int] NOT NULL CONSTRAINT [DF_SO_ITEM_QUANTITY] DEFAULT ((-99)),
[PRICE] [float] NOT NULL CONSTRAINT [DF_SO_ITEM_PRICE] DEFAULT ((0.0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_SO_ITEM] on [dbo].[SO_ITEM]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_SO_ITEM]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[SO_ITEM]', 'U'))
ALTER TABLE [dbo].[SO_ITEM] ADD CONSTRAINT [PK_SO_ITEM] PRIMARY KEY CLUSTERED  ([SO_ITEM_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SO_ORDER]'
GO
IF OBJECT_ID(N'[dbo].[SO_ORDER]', 'U') IS NULL
CREATE TABLE [dbo].[SO_ORDER]
(
[SO_ORDER_ID] [bigint] NOT NULL IDENTITY(1, 1),
[ORDER_NO] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ORDER_ORDER_NO] DEFAULT (''),
[ORDER_DATE] [datetime] NOT NULL CONSTRAINT [DF_SO_ORDER_ORDER_DATE] DEFAULT ('1900-01-01'),
[COM_CUSTOMER_ID] [int] NOT NULL CONSTRAINT [DF_SO_ORDER_COM_CUSTOMER_ID] DEFAULT ('-99'),
[ADDRESS] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_SO_ORDER_ADDRESS] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_SO_ORDER] on [dbo].[SO_ORDER]'
GO
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PK_SO_ORDER]', 'PK') AND parent_object_id = OBJECT_ID(N'[dbo].[SO_ORDER]', 'U'))
ALTER TABLE [dbo].[SO_ORDER] ADD CONSTRAINT [PK_SO_ORDER] PRIMARY KEY CLUSTERED  ([SO_ORDER_ID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO

/****** Object:  New Script for Store Procedure And add master customer ******/


/****** Object:  UserDefinedTableType [dbo].[UDT_OrderItem]    Script Date: 02/10/2024 08:37:29 ******/
CREATE TYPE [dbo].[UDT_OrderItem] AS TABLE(
	[SO_ITEM_ID] [bigint] NULL,
	[SO_ORDER_ID] [bigint] NULL,
	[ITEM_NAME] [varchar](100) NOT NULL,
	[QUANTITY] [int] NOT NULL,
	[PRICE] [float] NOT NULL
)
GO
CREATE PROCEDURE [dbo].[stp_GetCustomer]
AS
BEGIN
	SET NOCOUNT ON 
	
	SELECT 
		COM_CUSTOMER_ID Id,
		CUSTOMER_NAME Text
	FROM COM_CUSTOMER
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderDelete]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderDelete]
(
	@Id BIGINT
)
AS
BEGIN
	BEGIN TRY
	BEGIN TRANSACTION
	
	DELETE FROM SO_ORDER WHERE SO_ORDER_ID = @Id
	DELETE FROM SO_ITEM WHERE SO_ORDER_ID = @Id
	
	COMMIT TRANSACTION
	SELECT 1 AS 'Kode' , 'Delete Success' AS Message
	END TRY
	BEGIN CATCH
	ROLLBACK TRANSACTION
	SELECT 0 AS 'Kode' , ERROR_MESSAGE() AS Message
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderDeleteItem]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderDeleteItem]
(
	@Id BIGINT
)
AS
BEGIN
	BEGIN TRY
	BEGIN TRANSACTION
	
	DELETE FROM SO_ITEM WHERE SO_ITEM_ID = @Id
	
	COMMIT TRANSACTION
	SELECT 1 AS 'Kode' , 'Delete Success' AS Message
	END TRY
	BEGIN CATCH
	ROLLBACK TRANSACTION
	SELECT 0 AS 'Kode' , ERROR_MESSAGE() AS Message
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderGet]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderGet]
(
	@Date DATE = NULL,
	@Keyword VARCHAR(50) = NULL
)
AS
BEGIN
	SELECT SO_ORDER_ID, ORDER_NO, ORDER_DATE, CUSTOMER_NAME, ADDRESS FROM SO_ORDER SO
	JOIN COM_CUSTOMER CS ON CS.COM_CUSTOMER_ID = SO.COM_CUSTOMER_ID
	WHERE (@Keyword IS NULL OR ORDER_NO LIKE CONCAT('%', @Keyword, '%')
	OR CUSTOMER_NAME LIKE CONCAT('%', @Keyword, '%') OR ADDRESS LIKE CONCAT('%', @Keyword, '%'))
	AND (@Date IS null OR ORDER_DATE = @Date)
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderGetById]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderGetById]
(
	@Id BIGINT
)
AS
BEGIN
	SELECT SO_ORDER_ID, ORDER_NO, ORDER_DATE,CS.COM_CUSTOMER_ID, CUSTOMER_NAME, ADDRESS FROM SO_ORDER SO
	JOIN COM_CUSTOMER CS ON CS.COM_CUSTOMER_ID = SO.COM_CUSTOMER_ID
	WHERE SO_ORDER_ID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderItemByIdOrder]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderItemByIdOrder]
(
	@Id BIGINT
)
AS
BEGIN
	SELECT SO_ORDER_ID, SO_ITEM_ID, ITEM_NAME, QUANTITY, PRICE FROM SO_ITEM WHERE SO_ORDER_ID = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderSubmit]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderSubmit]
(
	@OrderDate DATETIME,
	@Address VARCHAR(100),
	@OrderNo VARCHAR(20),
	@CutomerID INT,
	@Detail dbo.UDT_OrderItem READONLY
)
AS
BEGIN
	DECLARE @OrderID bigint, @IsDuplicate INT, @Message VARCHAR(50);
	BEGIN TRY
		BEGIN TRANSACTION
		
		SELECT TOP 1 @IsDuplicate = 1 FROM SO_ORDER WHERE ORDER_NO = @OrderNo
		
		IF @IsDuplicate = 1
		BEGIN
			SET @Message = 'There is a duplicate order number ' + @OrderNo ;
			THROW 51000, @Message, 1;
		END
		
		INSERT INTO SO_ORDER (ORDER_NO, ORDER_DATE, COM_CUSTOMER_ID, ADDRESS) VALUES (@OrderNo, @OrderDate, @CutomerID, @Address)
		
		SELECT @OrderID = SCOPE_IDENTITY()
		
		INSERT INTO SO_ITEM (SO_ORDER_ID, ITEM_NAME, QUANTITY, PRICE)
		SELECT @OrderID, ITEM_NAME, QUANTITY, PRICE FROM @Detail
		
		COMMIT TRANSACTION
		SELECT 1 AS 'Kode' , 'Submit Success' AS Message
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT 0 AS 'Kode' , ERROR_MESSAGE() AS Message
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[stp_OrderUpdate]    Script Date: 02/10/2024 08:37:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[stp_OrderUpdate]
(
	@SoOrderId BIGINT,
	@OrderDate DATETIME,
	@Address VARCHAR(100),
	@OrderNo VARCHAR(20),
	@CutomerID INT,
	@Detail dbo.UDT_OrderItem READONLY
)
AS
BEGIN 
	DECLARE @OrderID bigint, @IsDuplicate INT, @Message VARCHAR(50);
	BEGIN TRY
	BEGIN TRANSACTION
		SELECT TOP 1 @IsDuplicate = 1 FROM SO_ORDER WHERE ORDER_NO = @OrderNo AND SO_ORDER_ID <> @SoOrderId
		
		IF @IsDuplicate = 1
		BEGIN
			SET @Message = 'There is a duplicate order number ' + @OrderNo ;
			THROW 51000, @Message, 1;
		END
	
	
	UPDATE SO_ORDER 
	SET ORDER_NO = @OrderNo, ORDER_DATE = @OrderDate, COM_CUSTOMER_ID = @CutomerID, ADDRESS = @Address
	WHERE SO_ORDER_ID = @SoOrderId
	
	MERGE SO_ITEM AS Target
  USING @Detail	AS Source
  ON Source.SO_ORDER_ID = Target.SO_ORDER_ID
	AND Source.SO_ITEM_ID = Target.SO_ITEM_ID
    
  WHEN NOT MATCHED BY Target THEN
      INSERT (SO_ORDER_ID, ITEM_NAME, QUANTITY, PRICE)
      VALUES (Source.SO_ORDER_ID,Source.ITEM_NAME, Source.QUANTITY, Source.PRICE)
			
  WHEN MATCHED THEN UPDATE SET
      Target.ITEM_NAME	= Source.ITEM_NAME,
      Target.QUANTITY		= Source.QUANTITY,
			Target.PRICE		= Source.PRICE;
	
	COMMIT TRANSACTION
	SELECT 1 AS 'Kode' , 'Update Success' AS Message, @SoOrderId AS Id
	END TRY
	BEGIN CATCH
	ROLLBACK TRANSACTION
	SELECT 0 AS 'Kode' , ERROR_MESSAGE() AS Message, @SoOrderId AS Id
	END CATCH
END
GO

SET IDENTITY_INSERT [dbo].[COM_CUSTOMER] ON
GO

INSERT INTO [dbo].[COM_CUSTOMER] ([COM_CUSTOMER_ID], [CUSTOMER_NAME]) VALUES (N'1', N'PROFES')
GO

INSERT INTO [dbo].[COM_CUSTOMER] ([COM_CUSTOMER_ID], [CUSTOMER_NAME]) VALUES (N'2', N'TITAN')
GO

INSERT INTO [dbo].[COM_CUSTOMER] ([COM_CUSTOMER_ID], [CUSTOMER_NAME]) VALUES (N'3', N'DIPS')
GO

SET IDENTITY_INSERT [dbo].[COM_CUSTOMER] OFF
GO