using ElecStoreDataAccess;
using System;
using System.Data;
using System.Collections.Generic;

namespace ElecStoreBusiness
{
    public class CLSSaleDetail
    {
        public int SalesDetailsID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public CLSSaleDetail() { }

        public CLSSaleDetail(int salesDetailsID, int saleID, int productID, int quantitySold, decimal unitPrice, decimal totalPrice)
        {
            this.SalesDetailsID = salesDetailsID;
            this.SaleID = saleID;
            this.ProductID = productID;
            this.QuantitySold = quantitySold;
            this.UnitPrice = unitPrice;
            this.TotalPrice = totalPrice;
        }

        public static List<CLSSaleDetail> GetBySaleID(int saleID)
        {
            List<CLSSaleDetail> details = new List<CLSSaleDetail>();
            DataTable dt = CLSSaleDetailsData.GetBySaleID(saleID);

            foreach (DataRow row in dt.Rows)
            {
                details.Add(new CLSSaleDetail(
                    Convert.ToInt32(row["SalesDetailsID"]),
                    Convert.ToInt32(row["SaleID"]),
                    Convert.ToInt32(row["ProductID"]),
                    Convert.ToInt32(row["QuantitySold"]),
                    Convert.ToDecimal(row["UnitPrice"]),
                    Convert.ToDecimal(row["TotalPrice"])
                ));
            }

            return details;
        }

        public static bool DeleteItem(int salesDetailsID)
        {
            return CLSSaleDetailsData.DeleteItem(salesDetailsID);
        }
    }
}