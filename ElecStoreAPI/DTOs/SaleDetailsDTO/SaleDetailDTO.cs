namespace ElecStoreAPI.DTOs.SaleDetailsDTO
{
    public class SaleDetailDTO
    {
        public int SalesDetailsID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // شكل الطلب لإضافة صنف جديد لفاتورة (مش هنستقبل SalesDetailsID ولا TotalPrice من العميل)
    public class AddSaleItemRequest
    {
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
