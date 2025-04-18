-- 1️⃣ إنشاء قاعدة البيانات
CREATE DATABASE BankLoyaltySystem1;
GO

-- استخدام قاعدة البيانات
USE BankLoyaltySystem1;
GO

-- 2️⃣ إنشاء جدول العملاء
CREATE TABLE Customers (
    customerID INT PRIMARY KEY IDENTITY(1,1),
    customerName NVARCHAR(100) NOT NULL,
    accountBalance DECIMAL(18,2) DEFAULT 0.00,
    depositCount INT DEFAULT 0,       -- عدد عمليات الإيداع
    bronzePoints INT DEFAULT 0,       -- النقاط البرونزية
    silverPoints INT DEFAULT 0,       -- النقاط الفضية
    goldPoints INT DEFAULT 0          -- النقاط الذهبية
);
GO

-- 3️⃣ إنشاء جدول الإيداع
CREATE TABLE Deposits (
    depositID INT PRIMARY KEY IDENTITY(1,1),
    customerID INT,                   -- معرّف العميل
    amount DECIMAL(18,2) NOT NULL,     -- المبلغ الذي تم إيداعه
    depositDate DATETIME DEFAULT GETDATE(), -- تاريخ الإيداع
    FOREIGN KEY (customerID) REFERENCES Customers(customerID)
);
GO

-- 4️⃣ إنشاء جدول السحوبات
CREATE TABLE Withdrawals (
    withdrawalID INT PRIMARY KEY IDENTITY(1,1),
    customerID INT,
    amount DECIMAL(18,2) NOT NULL,
    withdrawalDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (customerID) REFERENCES Customers(customerID)
);
GO

-- 5️⃣ إنشاء Trigger لحساب النقاط تلقائيًا بعد كل إيداع
CREATE or alter TRIGGER CalculateLoyaltyPoints
ON Deposits
AFTER INSERT
AS
BEGIN
    -- تحديث عدد عمليات الإيداع لكل عميل بناءً على عدد الإيداعات الجديدة
    UPDATE c
    SET c.depositCount = c.depositCount + newDeposits.newDepositCount
    FROM Customers c
    INNER JOIN (
        SELECT customerID, COUNT(*) AS newDepositCount
        FROM inserted
        GROUP BY customerID
    ) AS newDeposits
    ON c.customerID = newDeposits.customerID;
    ---- منح نقاط برونزية بناءً على عدد الإيداعات
    UPDATE c
    SET c.bronzePoints = c.bronzePoints + (c.depositCount / 5),  -- يحسب كل 5 إيداعات كنقطة برونزية
        c.depositCount = c.depositCount % 5  -- إبقاء ما تبقى من عمليات الإيداع
    FROM Customers c
    WHERE c.customerID IN (SELECT customerID FROM inserted)
    AND c.depositCount >= 5;

    ---- ترقية النقاط: تحويل 5 برونزية إلى 1 فضية
    UPDATE c
    SET c.silverPoints = c.silverPoints + (c.bronzePoints / 5),
        c.bronzePoints = c.bronzePoints % 5  -- إبقاء ما تبقى من البرونزية بعد التحويل
    FROM Customers c
    WHERE c.customerID IN (SELECT customerID FROM inserted)
    AND c.bronzePoints >= 5;

    ---- ترقية النقاط: تحويل 5 فضية إلى 1 ذهبية
    UPDATE c
    SET c.goldPoints = c.goldPoints + (c.silverPoints / 5),
        c.silverPoints = c.silverPoints % 5  -- إبقاء ما تبقى من الفضية بعد التحويل
    FROM Customers c
    WHERE c.customerID IN (SELECT customerID FROM inserted)
    AND c.silverPoints >= 5;
END;
GO





-- إعادة إنشاء Trigger لحساب النقاط والإيداعات بدقة
CREATE OR ALTER TRIGGER CalculateLoyaltyPoints
ON Deposits
AFTER INSERT
AS
BEGIN
    -- زيادة عدد الإيداعات لكل عميل حسب عدد الإيداعات الجديدة
    UPDATE c
    SET c.depositCount = c.depositCount + i.depositAddCount
    FROM Customers c
    INNER JOIN (
        SELECT customerID, COUNT(*) AS depositAddCount
        FROM inserted
        GROUP BY customerID
    ) i ON c.customerID = i.customerID;

    -- تحديث النقاط البرونزية
    UPDATE c
    SET c.bronzePoints = c.bronzePoints + (c.depositCount / 5),
        c.depositCount = c.depositCount % 5
    FROM Customers c
    WHERE c.customerID IN (SELECT DISTINCT customerID FROM inserted)
    AND c.depositCount >= 5;

    -- تحويل برونزي إلى فضي
    UPDATE c
    SET c.silverPoints = c.silverPoints + (c.bronzePoints / 5),
        c.bronzePoints = c.bronzePoints % 5
    FROM Customers c
    WHERE c.customerID IN (SELECT DISTINCT customerID FROM inserted)
    AND c.bronzePoints >= 5;

    -- تحويل فضي إلى ذهبي
    UPDATE c
    SET c.goldPoints = c.goldPoints + (c.silverPoints / 5),
        c.silverPoints = c.silverPoints % 5
    FROM Customers c
    WHERE c.customerID IN (SELECT DISTINCT customerID FROM inserted)
    AND c.silverPoints >= 5;
END;
GO

-- 6️⃣ إنشاء إجراءات مخزنة للعمليات المصرفية

-- إجراء الإيداع
CREATE or alter  PROCEDURE DepositMoney
    @customerID INT,
    @amount DECIMAL(18,2)
AS
BEGIN
	SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- التحقق من صحة المبلغ
        IF @amount <= 0
        BEGIN
            ROLLBACK;
            SELECT 'المبلغ يجب أن يكون أكبر من الصفر' AS Result;
            RETURN;
        END;
        
        -- تحديث رصيد العميل
        UPDATE Customers
        SET accountBalance = accountBalance + @amount
        WHERE customerID = @customerID;
        
        -- تسجيل عملية الإيداع
        INSERT INTO Deposits (customerID, amount)
        VALUES (@customerID, @amount);
        
        COMMIT;
        --SELECT 'تم الإيداع بنجاح' AS Result;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SELECT ERROR_MESSAGE() AS Error;
    END CATCH;
END;
GO

-- إجراء السحب
CREATE or alter PROCEDURE WithdrawMoney
    @customerID INT,
    @amount DECIMAL(18,2)
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @currentBalance DECIMAL(18,2);
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- الحصول على الرصيد الحالي
        SELECT @currentBalance = accountBalance
        FROM Customers
        WHERE customerID = @customerID;
        
        -- التحقق من صحة المبلغ
        IF @amount <= 0
        BEGIN
            ROLLBACK;
            SELECT 'المبلغ يجب أن يكون أكبر من الصفر' AS Result;
            RETURN;
        END;
        
        -- التحقق من وجود رصيد كافي
        IF @currentBalance < @amount
        BEGIN
            ROLLBACK;
            SELECT 'الرصيد غير كافي لإتمام عملية السحب' AS Result;
            RETURN;
        END;
        
        -- تحديث رصيد العميل
        UPDATE Customers
        SET accountBalance = accountBalance - @amount
        WHERE customerID = @customerID;
        
        -- تسجيل عملية السحب
        INSERT INTO Withdrawals (customerID, amount)
        VALUES (@customerID, @amount);
        
        COMMIT;
        --SELECT 'تم السحب بنجاح' AS Result;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SELECT ERROR_MESSAGE() AS Error;
    END CATCH;
END;
GO

-- 7️⃣ إنشاء عرض لبيانات العميل الكاملة
CREATE or alter VIEW CustomerFullDetails AS
SELECT 
    c.customerID,
    c.customerName,
    c.accountBalance,
    c.bronzePoints,
    c.silverPoints,
    c.goldPoints,
    (SELECT COUNT(*) FROM Deposits d WHERE d.customerID = c.customerID) AS totalDeposits,
    (SELECT COUNT(*) FROM Withdrawals w WHERE w.customerID = c.customerID) AS totalWithdrawals,
    (SELECT SUM(amount) FROM Deposits d WHERE d.customerID = c.customerID) AS totalDepositedAmount,
    (SELECT SUM(amount) FROM Withdrawals w WHERE w.customerID = c.customerID) AS totalWithdrawnAmount
FROM Customers c;
GO

-- 8️⃣ إدخال بيانات العملاء الأولية
INSERT INTO Customers (customerName) VALUES 
('أحمد العتيبي'),
('سارة محمد'),
('خالد عبدالعزيز'),
('نورة الدوسري'),
('فيصل المطيري');
GO

-- 9️⃣ أمثلة على استخدام النظام

-- إيداع مبلغ لعميل
EXEC DepositMoney @customerID = 1, @amount = 1000.00;
EXEC DepositMoney @customerID = 1, @amount = 500.00;
EXEC DepositMoney @customerID = 2, @amount = 1500.00;

-- سحب مبلغ من عميل
EXEC WithdrawMoney @customerID = 1, @amount = 200.00;
EXEC WithdrawMoney @customerID = 2, @amount = 200.00;

-- إيداعات متعددة لزيادة النقاط (5 إيداعات = 1 نقطة برونزية)
EXEC DepositMoney @customerID = 1, @amount = 100.00;
EXEC DepositMoney @customerID = 1, @amount = 100.00;
EXEC DepositMoney @customerID = 1, @amount = 100.00;
EXEC DepositMoney @customerID = 1, @amount = 100.00;
EXEC DepositMoney @customerID = 1, @amount = 100.00;

-- 🔟 استعلامات للتحقق من البيانات

-- عرض بيانات جميع العملاء
SELECT * FROM CustomerFullDetails;

-- عرض سجل المعاملات لعميل معين
SELECT 
    'Deposit' AS TransactionType,
    amount,
    depositDate AS TransactionDate
FROM Deposits
WHERE customerID = 1

UNION ALL

SELECT 
    'Withdrawal' AS TransactionType,
    amount,
    withdrawalDate AS TransactionDate
FROM Withdrawals
WHERE customerID = 1

ORDER BY TransactionDate DESC;


--not important
select * from Deposits