-- ============================================================
-- LoanDB Initial Migration
-- Run: dotnet ef migrations add InitialCreate --project LoanService
--      dotnet ef database update --project LoanService
-- OR apply this SQL manually in SQL Server Management Studio
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LoanDB')
    CREATE DATABASE LoanDB;
GO

USE LoanDB;
GO

-- Loans table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Loans' AND xtype='U')
BEGIN
    CREATE TABLE Loans (
        Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        CustomerId          UNIQUEIDENTIFIER NOT NULL,
        PrincipalAmount     DECIMAL(18,2)    NOT NULL,
        InterestRatePercent DECIMAL(5,2)     NOT NULL,
        TenureMonths        INT              NOT NULL,
        [Status]            NVARCHAR(20)     NOT NULL DEFAULT 'Pending',
        Purpose             NVARCHAR(500)    NULL,
        RejectionReason     NVARCHAR(500)    NULL,
        AppliedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        ApprovedAt          DATETIME2        NULL,
        DisbursedAt         DATETIME2        NULL,
        ApprovedBy          NVARCHAR(100)    NULL
    );

    CREATE INDEX IX_Loans_CustomerId ON Loans(CustomerId);
END
GO

-- EmiPlans table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmiPlans' AND xtype='U')
BEGIN
    CREATE TABLE EmiPlans (
        Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        LoanId              UNIQUEIDENTIFIER NOT NULL,
        InstallmentNumber   INT              NOT NULL,
        DueDate             DATETIME2        NOT NULL,
        EmiAmount           DECIMAL(18,2)    NOT NULL,
        PrincipalComponent  DECIMAL(18,2)    NOT NULL,
        InterestComponent   DECIMAL(18,2)    NOT NULL,
        OutstandingBalance  DECIMAL(18,2)    NOT NULL,
        [Status]            NVARCHAR(20)     NOT NULL DEFAULT 'Pending',
        PaidAt              DATETIME2        NULL,
        PaidAmount          DECIMAL(18,2)    NULL,
        CONSTRAINT FK_EmiPlans_Loans FOREIGN KEY (LoanId)
            REFERENCES Loans(Id) ON DELETE CASCADE
    );
END
GO
