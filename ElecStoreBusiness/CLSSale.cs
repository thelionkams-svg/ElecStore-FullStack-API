using ElecStoreDataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace ElecStoreBusiness
{
    public class CLSSale
    {
        public int SaleID { get; private set; }
        public DateTime SaleDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public List<CLSSaleDetail> Items { get; private set; }

        private CLSSale(int saleID, DateTime saleDate, decimal totalAmount)
        {
            this.SaleID = saleID;
            this.SaleDate = saleDate;
            this.TotalAmount = totalAmount;
            this.Items = new List<CLSSaleDetail>();
        }

        public static CLSSale CreateNew()
        {
            int newSaleID = CLSSalesData.CreateHeader();

            if (newSaleID != -1)
                return CLSSale.Find(newSaleID);

            return null;
        }

        public static CLSSale Find(int saleID)
        {
            DateTime saleDate = DateTime.MinValue;
            decimal totalAmount = 0;

            if (!CLSSalesData.GetSaleByID(saleID, ref saleDate, ref totalAmount))
                return null;

            CLSSale sale = new CLSSale(saleID, saleDate, totalAmount);
            sale.Items = CLSSaleDetail.GetBySaleID(saleID);

            return sale;
        }

        public bool AddItem(int productID, int quantitySold, decimal unitPrice)
        {
            bool isAdded = CLSSaleDetailsData.AddItem(this.SaleID, productID, quantitySold, unitPrice);

            if (isAdded)
            {
                // إعادة تحميل الفاتورة عشان نجيب الإجمالي المحدث وقائمة الأصناف الجديدة
                CLSSale refreshed = CLSSale.Find(this.SaleID);

                if (refreshed != null)
                {
                    this.TotalAmount = refreshed.TotalAmount;
                    this.Items = refreshed.Items;
                }
            }

            return isAdded;
        }

        public static bool DeleteSale(int saleID)
        {
            return CLSSalesData.DeleteSale(saleID);
        }

        public static DataTable GetAllSales()
        {
            return CLSSalesData.GetAllSales();
        }
    }
}