using ElecStoreAPI.DTOs.SaleDetailsDTO;
using ElecStoreBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElecStoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SaleDetailsController : ControllerBase
    {

        // GET: api/SaleDetails/sale/5  (كل أصناف فاتورة معينة)
        [HttpGet("sale/{saleId}")]
        public IActionResult GetBySaleID(int saleId)
        {
            List<CLSSaleDetail> details = CLSSaleDetail.GetBySaleID(saleId);

            List<SaleDetailDTO> result = details.Select(d => new SaleDetailDTO
            {
                SalesDetailsID = d.SalesDetailsID,
                SaleID = d.SaleID,
                ProductID = d.ProductID,
                QuantitySold = d.QuantitySold,
                UnitPrice = d.UnitPrice,
                TotalPrice = d.TotalPrice
            }).ToList();

            return Ok(result);
        }

        // POST: api/SaleDetails  (إضافة صنف لفاتورة موجودة)
        [HttpPost]
        [Authorize]
        public IActionResult AddItem([FromBody] AddSaleItemRequest request)
        {
            CLSSale sale = CLSSale.Find(request.SaleID);

            if (sale == null)
                return NotFound($"لا توجد فاتورة بالرقم {request.SaleID}");

            bool isAdded = sale.AddItem(request.ProductID, request.QuantitySold, request.UnitPrice);

            if (!isAdded)
                return BadRequest("فشل إضافة الصنف (تأكد من رقم المنتج والكمية المتوفرة في المخزون)");

            return Ok(new
            {
                SaleID = sale.SaleID,
                TotalAmount = sale.TotalAmount,
                Items = sale.Items.Select(i => new SaleDetailDTO
                {
                    SalesDetailsID = i.SalesDetailsID,
                    SaleID = i.SaleID,
                    ProductID = i.ProductID,
                    QuantitySold = i.QuantitySold,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            });
        }

        // DELETE: api/SaleDetails/5  (حذف صنف من فاتورة)
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteItem(int id)
        {
            if (CLSSaleDetail.DeleteItem(id))
                return Ok($"تم حذف الصنف رقم {id} من الفاتورة");

            return BadRequest("فشل حذف الصنف (تأكد إن الرقم صحيح)");
        }
    
    }

}