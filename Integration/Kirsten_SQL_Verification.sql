/* =====================================================================
   INKED MP2 - Kirsten's tables: SQL Server verification queries
   Run BEFORE and AFTER each Swagger call and screenshot both results.
   ===================================================================== */
USE INKED_CoconutPrinting;
GO

-- Prerequisite rows (create them through Ileana's / Jethro's APIs first)
SELECT TOP 5 CustomerID, FirstName, LastName FROM dbo.Customers ORDER BY CustomerID;
SELECT TOP 5 UserID, Username, UserRole        FROM dbo.UserAccounts ORDER BY UserID;
SELECT TOP 5 ProductID, ProductName, BasePrice FROM dbo.Products ORDER BY ProductID;
SELECT TeamID, TeamName                        FROM dbo.ProductionTeams ORDER BY TeamID;
GO

-- 1. Inquiries
SELECT * FROM dbo.Inquiries ORDER BY InquiryID;
-- 2. FeasibilityChecks
SELECT * FROM dbo.FeasibilityChecks ORDER BY FeasibilityID;
-- 3. Quotations
SELECT * FROM dbo.Quotations ORDER BY QuotationID;
-- 4. QuotationItems (LineTotal is computed by SQL Server)
SELECT QuotationItemID, QuotationID, ProductID, Quantity, UnitPrice, LineTotal, DesignNotes
FROM dbo.QuotationItems ORDER BY QuotationItemID;
GO

-- Relationship check: full chain for one inquiry (change @InquiryID)
DECLARE @InquiryID INT = 1;
SELECT i.InquiryID, c.FirstName + ' ' + c.LastName AS Customer, i.Outcome,
       q.QuotationID, q.Status, q.TotalAmount,
       qi.QuotationItemID, p.ProductName, qi.Quantity, qi.UnitPrice, qi.LineTotal,
       f.FeasibilityID, t.TeamName, f.Result
FROM dbo.Inquiries i
JOIN dbo.Customers c               ON c.CustomerID = i.CustomerID
LEFT JOIN dbo.Quotations q         ON q.InquiryID = i.InquiryID
LEFT JOIN dbo.QuotationItems qi    ON qi.QuotationID = q.QuotationID
LEFT JOIN dbo.Products p           ON p.ProductID = qi.ProductID
LEFT JOIN dbo.FeasibilityChecks f  ON f.InquiryID = i.InquiryID
LEFT JOIN dbo.ProductionTeams t    ON t.TeamID = f.TeamID
WHERE i.InquiryID = @InquiryID;
GO

-- Constraint evidence for section 6.4
SELECT OBJECT_NAME(parent_object_id) AS TableName, name AS CheckName, definition
FROM sys.check_constraints
WHERE OBJECT_NAME(parent_object_id) IN ('Inquiries','Quotations','QuotationItems','FeasibilityChecks');

SELECT fk.name AS ForeignKey, OBJECT_NAME(fk.parent_object_id) AS ChildTable,
       OBJECT_NAME(fk.referenced_object_id) AS ParentTable
FROM sys.foreign_keys fk
WHERE OBJECT_NAME(fk.parent_object_id) IN ('Inquiries','Quotations','QuotationItems','FeasibilityChecks');
GO
