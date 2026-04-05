-- ============================================================
--  Finance Dashboard – MySQL Schema
--  Run this if you prefer manual DB setup over EF migrations
-- ============================================================

CREATE DATABASE IF NOT EXISTS FinanceDashboard CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE FinanceDashboard;

-- ── Roles ─────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Roles (
    Id          INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Name        VARCHAR(50)  NOT NULL,
    Description VARCHAR(255) NOT NULL DEFAULT '',
    CreatedAt   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt   DATETIME     NULL,
    IsDeleted   TINYINT(1)   NOT NULL DEFAULT 0,
    DeletedAt   DATETIME     NULL,
    UNIQUE KEY UQ_Roles_Name (Name)
);

-- ── Users ─────────────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Users (
    Id                  INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
    FirstName           VARCHAR(100) NOT NULL,
    LastName            VARCHAR(100) NOT NULL,
    Email               VARCHAR(255) NOT NULL,
    PasswordHash        TEXT         NOT NULL,
    IsActive            TINYINT(1)   NOT NULL DEFAULT 1,
    LastLoginAt         DATETIME     NULL,
    RefreshToken        TEXT         NULL,
    RefreshTokenExpiry  DATETIME     NULL,
    RoleId              INT          NOT NULL,
    CreatedAt           DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt           DATETIME     NULL,
    IsDeleted           TINYINT(1)   NOT NULL DEFAULT 0,
    DeletedAt           DATETIME     NULL,
    UNIQUE KEY UQ_Users_Email (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- ── Transactions ──────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Transactions (
    Id                INT              NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Amount            DECIMAL(18, 2)   NOT NULL,
    Type              VARCHAR(20)      NOT NULL,       -- 'Income' | 'Expense'
    Category          VARCHAR(100)     NOT NULL,
    Date              DATETIME         NOT NULL,
    Description       VARCHAR(500)     NOT NULL DEFAULT '',
    Notes             VARCHAR(1000)    NULL,
    CreatedByUserId   INT              NOT NULL,
    CreatedAt         DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt         DATETIME         NULL,
    IsDeleted         TINYINT(1)       NOT NULL DEFAULT 0,
    DeletedAt         DATETIME         NULL,
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

-- ── Seed data ─────────────────────────────────────────────────────────────────
INSERT IGNORE INTO Roles (Id, Name, Description, CreatedAt) VALUES
    (1, 'Admin',   'Full system access',          '2024-01-01 00:00:00'),
    (2, 'Analyst', 'View records and analytics',  '2024-01-01 00:00:00'),
    (3, 'Viewer',  'View dashboard data only',    '2024-01-01 00:00:00');

-- Admin user  (password = Admin@123)
INSERT IGNORE INTO Users (Id, FirstName, LastName, Email, PasswordHash, IsActive, RoleId, CreatedAt)
VALUES (1, 'System', 'Admin', 'admin@finance.com',
        '$2a$11$rJNMhMBXHoW.M4Lp1UWI3.K1q7TBXBxsOk.YQy8ZYAQR.A3WXFMbi',
        1, 1, '2024-01-01 00:00:00');

-- ── Sample transactions (optional) ────────────────────────────────────────────
INSERT IGNORE INTO Transactions (Amount, Type, Category, Date, Description, CreatedByUserId, CreatedAt) VALUES
    (5000.00, 'Income',  'Salary',      '2024-11-01', 'Monthly salary November',  1, NOW()),
    (1200.00, 'Expense', 'Rent',        '2024-11-05', 'Monthly rent payment',     1, NOW()),
    (350.00,  'Expense', 'Food',        '2024-11-10', 'Grocery shopping',         1, NOW()),
    (5000.00, 'Income',  'Salary',      '2024-12-01', 'Monthly salary December',  1, NOW()),
    (1200.00, 'Expense', 'Rent',        '2024-12-05', 'Monthly rent payment',     1, NOW()),
    (200.00,  'Expense', 'Utilities',   '2024-12-12', 'Electricity bill',         1, NOW()),
    (800.00,  'Income',  'Freelance',   '2024-12-20', 'Freelance project payment',1, NOW()),
    (5000.00, 'Income',  'Salary',      '2025-01-01', 'Monthly salary January',   1, NOW()),
    (450.00,  'Expense', 'Food',        '2025-01-08', 'Grocery and dining',       1, NOW()),
    (180.00,  'Expense', 'Transport',   '2025-01-15', 'Monthly transit pass',     1, NOW());
