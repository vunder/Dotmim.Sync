If(db_id('SyncTest') IS NULL)
    BEGIN
        CREATE DATABASE [SyncTest]
    END;
GO

use SyncTest;

CREATE TABLE [dbo].[Account](
    [AccountID] [int] NOT NULL,
    [Name] [nvarchar](MAX) NULL,
    [Type] [int] NOT NULL,
    [Description] [nvarchar](MAX) NULL,
    [Image] [varbinary](MAX) NULL,
    [UserId] [nvarchar](MAX) NULL,
    [SyncId] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_Account_SyncId] PRIMARY KEY CLUSTERED 
(
    [SyncId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Category](
    [ParentCategoryCategoryID] [int] NULL,
    [CategoryID] [int] NOT NULL,
    [Name] [nvarchar](MAX) NULL,
    [FullName] [nvarchar](MAX) NULL,
    [Description] [nvarchar](MAX) NULL,
    [UserId] [nvarchar](MAX) NULL,
    [SyncId] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_Category_SyncId] PRIMARY KEY CLUSTERED 
(
    [SyncId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Currency](
    [CurrencyID] [int] NOT NULL,
    [Name] [nvarchar](MAX) NULL,
    [ISOCode] [nvarchar](MAX) NULL,
    [Suffix] [nvarchar](MAX) NULL,
    [IsPrefix] [bit] NOT NULL,
    [Digits] [int] NOT NULL,
    [UserId] [nvarchar](MAX) NULL,
    [SyncId] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_Currency_SyncId] PRIMARY KEY CLUSTERED 
(
    [SyncId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Option](
    [OptionID] [int] NOT NULL,
    [Name] [nvarchar](MAX) NULL,
    [Value] [nvarchar](MAX) NULL,
    [UserId] [nvarchar](MAX) NULL,
    [SyncId] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_Option_SyncId] PRIMARY KEY CLUSTERED 
(
    [SyncId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


DECLARE @cnt INT = 0;

WHILE @cnt < 50
BEGIN
   INSERT INTO Account VALUES(@cnt+1, concat('Account', cast(@cnt as varchar(10))), 0, NULL, CRYPT_GEN_RANDOM(300), NULL, newid())
   SET @cnt = @cnt + 1;
END;

SET @cnt = 0;
WHILE @cnt < 10000
BEGIN
   INSERT INTO Category VALUES(NULL, @cnt+1, concat('Category', cast(@cnt as varchar(10))), NULL, NULL, NULL, newid())
   SET @cnt = @cnt + 1;
END;

SET @cnt = 0;
WHILE @cnt < 500
BEGIN
   INSERT INTO Currency VALUES(@cnt+1, concat('Currency', cast(@cnt as varchar(10))), 'ICO', 'c', 0, 2, NULL, newid())
   SET @cnt = @cnt + 1
END;

SET @cnt = 0;
WHILE @cnt < 4000
BEGIN
   INSERT INTO [Option] VALUES(@cnt+1, concat('Option', cast(@cnt as varchar(10))), concat('Value ', cast((@cnt * 2) as varchar(10))), NULL, newid())
   SET @cnt = @cnt + 1
END;

