
CREATE DATABASE ElecStore;

GO


USE ElecStore;

GO



CREATE TABLE Categories(

    CategoryID INT Identity(1,1) PRIMARY KEY,

	CategoryName NVARCHAR(100) NOT NULL,

	CDescription NVARCHAR(250) NULL

);


GO




CREATE TABLE Products(

	ProductID INT IDENTITY(1,1) PRIMARY KEY,

	ProductName NVARCHAR(150) NOT NULL,

	CategoryID INT NOT NULL,

	Price DECIMAL(18 , 2) NOT NULL CHECK(Price >= 0),

	QuantityInStock INT NOT NULL CHECK(QuantityInStock >= 0)

);
GO



ALTER TABLE Products 

ADD CONSTRAINT FK_Products_Categories

FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID);

GO




CREATE TABLE Sales(

	SaleID INT IDENTITY(1,1) PRIMARY KEY,

	SaleDate DATETIME NOT NULL DEFAULT GETDATE(),

	TotalAmount DECIMAL(18,2) NOT NULL DEFAULT(0) CHECK(TotalAmount >= 0)

);
GO



CREATE TABLE SaleDetails(

	SalesDetailsID INT IDENTITY(1,1) PRIMARY KEY,

	SaleID INT NOT NULL ,

	ProductID INT NOT NULL ,

	QuantitySold INT NOT NULL CHECK(QuantitySold >= 0),

	UnitPrice DECIMAL(18,2) NOT NULL CHECK(UnitPrice >= 0),

	TotalPrice AS (QuantitySold * UnitPrice),


	CONSTRAINT FK_SaleDetails_Sales FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),

	CONSTRAINT FK_SaleDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)

);
GO




DROP TABLE SaleDetails;
GO

CREATE TABLE SaleDetails(

    SalesDetailsID INT IDENTITY(1,1) PRIMARY KEY,

    SaleID INT NOT NULL,

    ProductID INT NOT NULL,

    QuantitySold INT NOT NULL CHECK(QuantitySold > 0), -- التعديل هنا

    UnitPrice DECIMAL(18,2) NOT NULL CHECK(UnitPrice >= 0),

    TotalPrice AS (QuantitySold * UnitPrice),

    CONSTRAINT FK_SaleDetails_Sales FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),

    CONSTRAINT FK_SaleDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)

);
GO








INSERT INTO Categories(CategoryName , CDescription)

VALUES

(N'Smartphones', N'All smartphone models and mobile accessories'),

(N'Laptops', N'Personal laptops and computers'),

(N'Accessories', N'Chargers, cables, and audio devices');

GO




INSERT INTO Products(ProductName , CategoryID , Price , QuantityInStock)

VALUES

(N'Samsung Galaxy A54', 1, 14500.00, 10),

(N'iPhone 15', 1, 45000.00, 5),

(N'Lenovo Laptop i5', 2, 28000.00, 3),

(N'20W Fast Charger', 3, 450.00, 25),

(N'Bluetooth Headset', 3, 850.00, 15);

GO




INSERT INTO Sales(SaleDate , TotalAmount)

VALUES 

(GETDATE() , 0);

GO




INSERT INTO SaleDetails (SaleID, ProductID, QuantitySold, UnitPrice)

VALUES 

(1, 4, 2, 450.00), -- 2 Chargers

(1, 5, 1, 850.00); -- 1 Headset

GO



UPDATE Sales

SET TotalAmount = (

    SELECT SUM(TotalPrice) 

    FROM SaleDetails 

    WHERE SaleID = 1

)

WHERE SaleID = 1;

GO




/*--------------------------------------------------------------------------------*/



SELECT 

    S.SaleID,

    S.SaleDate,

    P.ProductName,

    SD.QuantitySold,

    SD.UnitPrice,

    SD.TotalPrice AS SubTotal,

    S.TotalAmount AS GrandTotal

FROM Sales S

JOIN SaleDetails SD ON S.SaleID = SD.SaleID

JOIN Products P ON SD.ProductID = P.ProductID;

GO




















/*--------------------------------------------------------------------------------*/
/*--------------------------------------------------------------------------------*/






CREATE PROCEDURE sp_Categories_Insert

    @CategoryName NVARCHAR(100),

    @CDescription NVARCHAR(250) = NULL,

    @NewCategoryID INT OUTPUT -- متغير إرجاع للقيمة الجديدة

AS

BEGIN

    SET NOCOUNT ON;

    INSERT INTO Categories (CategoryName, CDescription)

    VALUES (@CategoryName, @CDescription);

    -- جلب أحدث رقم ID تم إنشاؤه في هذه الجلسة

    SET @NewCategoryID = SCOPE_IDENTITY();

END;

GO



/*--------------------------------------------------------------------------------*/



CREATE PROCEDURE sp_Products_Insert

    @ProductName NVARCHAR(150),

    @CategoryID INT,

    @Price DECIMAL(18,2),

    @QuantityInStock INT,

    @NewProductID INT OUTPUT

AS

BEGIN

    SET NOCOUNT ON;

    -- التحقق من وجود رقم القسم في جدول Categories أولاً

	    IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryID = @CategoryID)

    BEGIN

        RAISERROR('Category ID does not exist!', 16, 1);

        RETURN;

    END

    INSERT INTO Products (ProductName, CategoryID, Price, QuantityInStock)

    VALUES (@ProductName, @CategoryID, @Price, @QuantityInStock);

    SET @NewProductID = SCOPE_IDENTITY();

END;

GO



-- 1. إجراء تعديل بيانات قسم
CREATE PROCEDURE sp_Categories_Update
    @CategoryID INT,
    @CategoryName NVARCHAR(100),
    @CDescription NVARCHAR(MAX)
AS
BEGIN
    UPDATE Categories
    SET CategoryName = @CategoryName,
        CDescription = @CDescription
    WHERE CategoryID = @CategoryID;
END
GO

-- 2. إجراء حذف قسم
CREATE PROCEDURE sp_Categories_Delete
    @CategoryID INT
AS
BEGIN
    DELETE FROM Categories
    WHERE CategoryID = @CategoryID;
END
GO

-- 3. إجراء جلب كافة الأقسام
CREATE PROCEDURE sp_Categories_GetAll
AS
BEGIN
    SELECT CategoryID, CategoryName, CDescription
    FROM Categories;
END
GO


CREATE PROCEDURE sp_Categories_GetByID
    @CategoryID INT
AS
BEGIN
    SELECT CategoryID, CategoryName, CDescription
    FROM Categories
    WHERE CategoryID = @CategoryID;
END
GO


/*--------------------------------------------------------------------------------*/


CREATE PROCEDURE sp_Sales_CreateHeader

    @NewSaleID INT OUTPUT

AS

BEGIN

    SET NOCOUNT ON;

    INSERT INTO Sales (SaleDate, TotalAmount)

    VALUES (GETDATE(), 0);

    SET @NewSaleID = SCOPE_IDENTITY();

END;

GO



/*------------------------------------------------------------------------------*/



CREATE PROCEDURE sp_Sales_AddItem

    @SaleID INT,

    @ProductID INT,

    @QuantitySold INT,

    @UnitPrice DECIMAL(18,2)

AS

BEGIN

    SET NOCOUNT ON;

    -- 1. التأكد من توفر كمية كافية في المخزن

    DECLARE @AvailableStock INT;

    SELECT @AvailableStock = QuantityInStock FROM Products WHERE ProductID = @ProductID;

    IF @AvailableStock IS NULL

    BEGIN

        RAISERROR('Product does not exist!', 16, 1);

        RETURN;

    END

    IF @AvailableStock < @QuantitySold

    BEGIN

        RAISERROR('Insufficient stock quantity for this product!', 16, 1);

        RETURN;
    END

    -- 2. إدخال السطر في جدول التفاصيل

    INSERT INTO SaleDetails (SaleID, ProductID, QuantitySold, UnitPrice)

    VALUES (@SaleID, @ProductID, @QuantitySold, @UnitPrice);


    -- 3. خصم الكمية من المخزن

    UPDATE Products

    SET QuantityInStock = QuantityInStock - @QuantitySold

    WHERE ProductID = @ProductID;


    -- 4. إعادة حساب وتحديث إجمالي الفاتورة كلياً

    UPDATE Sales

    SET TotalAmount = (

        SELECT ISNULL(SUM(TotalPrice), 0)

        FROM SaleDetails

        WHERE SaleID = @SaleID

    )

    WHERE SaleID = @SaleID;

END;

GO




/*------------------------------------------------------------------------------*/


CREATE PROCEDURE sp_Sales_GetInvoiceDetails

    @SaleID INT

AS

BEGIN

    SET NOCOUNT ON;

    SELECT 

        S.SaleID,

        S.SaleDate,

        P.ProductName,

        SD.QuantitySold,

        SD.UnitPrice,

        SD.TotalPrice AS SubTotal,

        S.TotalAmount AS GrandTotal

    FROM Sales S

    JOIN SaleDetails SD ON S.SaleID = SD.SaleID

    JOIN Products P ON SD.ProductID = P.ProductID

    WHERE S.SaleID = @SaleID;

END;

GO




/*----------------------------------------------------------------------------*/


-- أ) إنشاء فاتورة جديدة

DECLARE @CurrentSaleID INT;

EXEC sp_Sales_CreateHeader @NewSaleID = @CurrentSaleID OUTPUT;

-- ب) إضافة منتج للفاتورة (المنتج رقم 1 - خصم 2 قطعة)

EXEC sp_Sales_AddItem 

    @SaleID = @CurrentSaleID, 

    @ProductID = 1, 

    @QuantitySold = 2, 

    @UnitPrice = 14500.00;

-- ج) عرض الفاتورة الناتجة

EXEC sp_Sales_GetInvoiceDetails @SaleID = @CurrentSaleID;







































