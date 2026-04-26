-- ============================================================
-- CardDB Initial Migration
-- Run: dotnet ef migrations add InitialCreate --project CardService
--      dotnet ef database update --project CardService
-- OR apply this SQL manually
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CardDB')
    CREATE DATABASE CardDB;
GO

USE CardDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cards' AND xtype='U')
BEGIN
    CREATE TABLE Cards (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        CustomerId      UNIQUEIDENTIFIER NOT NULL,
        AccountId       UNIQUEIDENTIFIER NOT NULL,
        CardType        NVARCHAR(10)     NOT NULL,   -- Debit | Credit
        CardStatus      NVARCHAR(10)     NOT NULL DEFAULT 'Inactive',
        CardNumber      NVARCHAR(20)     NOT NULL,
        CardHolderName  NVARCHAR(100)    NOT NULL,
        Network         NVARCHAR(20)     NOT NULL DEFAULT 'Visa',
        IssuedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        ExpiresAt       DATETIME2        NOT NULL,
        CreditLimit     DECIMAL(18,2)    NULL,
        IsPinSet        BIT              NOT NULL DEFAULT 0,
        BlockReason     NVARCHAR(255)    NULL,
        BlockedAt       DATETIME2        NULL,
        CONSTRAINT UQ_Cards_CardNumber UNIQUE (CardNumber)
    );

    CREATE INDEX IX_Cards_CustomerId ON Cards(CustomerId);
END
GO
