using ElecStoreAPI.DTOs.SaleDetailsDTO;

namespace ElecStoreAPI.DTOs.SalesDTO
{
    public class SaleDTO
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleDetailDTO> Items { get; set; }
    }
}
