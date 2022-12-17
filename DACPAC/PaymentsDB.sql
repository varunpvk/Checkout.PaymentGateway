Create database PaymentsDB
Go

Use [PaymentsDB]
CREATE TABLE [dbo].[PaymentDetails] (
    [PaymentId]           NVARCHAR (MAX) NOT NULL,
    [ConsentId]           NVARCHAR (MAX) NOT NULL,
    [AccessToken]         NVARCHAR (MAX) NULL,
    [PaymentStatus]       NVARCHAR (MAX) NOT NULL,
    [PaymentDetails]      NVARCHAR (MAX) NOT NULL,
    [CreatedDateTime] DATETIME2 NOT NULL,
	[LastUpdatedDateTime] DATETIME2     NOT NULL, 
);