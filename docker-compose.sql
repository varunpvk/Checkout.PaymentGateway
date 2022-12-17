CREATE TABLE [dbo].[MerchantDetails] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [MerchantId]       NVARCHAR (MAX) NOT NULL,
    [MerchantDetails]  NVARCHAR (MAX) NOT NULL,
    [AccessToken]      NVARCHAR (MAX) NULL,
	[ExpiresIn]        BIGINT NULL, 
    [CreatedDate]      DATETIME2 (7)  NOT NULL,
    [LastModifiedDate] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_MerchantDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
);