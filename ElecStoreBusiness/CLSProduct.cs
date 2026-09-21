using ElecStoreDataAccess;
using System.Data;

namespace ElecStoreBusiness
{
    public class CLSProduct
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        public CLSProduct()
        {
            this.ProductID = -1;
            this.ProductName = "";
            this.CategoryID = -1;
            this.Price = 0;
            this.QuantityInStock = 0;
            Mode = enMode.AddNew;
        }

        private CLSProduct(int productID, string productName, int categoryID, decimal price, int quantityInStock)
        {
            this.ProductID = productID;
            this.ProductName = productName;
            this.CategoryID = categoryID;
            this.Price = price;
            this.QuantityInStock = quantityInStock;
            Mode = enMode.Update;
        }

        public static CLSProduct Find(int productID)
        {
            string productName = "";
            int categoryID = -1;
            decimal price = 0;
            int quantityInStock = 0;

            if (CLSProductData.GetProductByID(productID, ref productName, ref categoryID, ref price, ref quantityInStock))
                return new CLSProduct(productID, productName, categoryID, price, quantityInStock);

            return null;
        }

        public static bool IsProductExist(int productID)
        {
            return CLSProductData.IsProductExist(productID);
        }

        private bool _AddNewProduct()
        {
            this.ProductID = CLSProductData.AddNewProduct(this.ProductName, this.CategoryID, this.Price, this.QuantityInStock);
            return (this.ProductID != -1);
        }

        private bool _UpdateProduct()
        {
            return CLSProductData.UpdateProduct(this.ProductID, this.ProductName, this.CategoryID, this.Price, this.QuantityInStock);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewProduct())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateProduct();
            }

            return false;
        }

        public static bool DeleteProduct(int productID)
        {
            return CLSProductData.DeleteProduct(productID);
        }

        public static DataTable GetAllProducts()
        {
            return CLSProductData.GetAllProducts();
        }
    }
}
