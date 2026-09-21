using ElecStoreAPI.DTOs.SaleDetailsDTO;
using ElecStoreAPI.DTOs.SalesDTO;
using ElecStoreBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ElecStoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {

        // GET: api/Sales  (قائمة الفواتير بدون تفاصيلها)
        [HttpGet]
        public IActionResult GetAll()
        {
            DataTable dt = CLSSale.GetAllSales();
            List<SaleDTO> sales = new List<SaleDTO>();

            foreach (DataRow row in dt.Rows)
            {
                sales.Add(new SaleDTO
                {
                    SaleID = Convert.ToInt32(row["SaleID"]),
                    SaleDate = Convert.ToDateTime(row["SaleDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Items = new List<SaleDetailDTO>()
                });
            }

            return Ok(sales);
        }

        // GET: api/Sales/5  (فاتورة واحدة مع كل أصنافها)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            CLSSale sale = CLSSale.Find(id);

            if (sale == null)
                return NotFound($"لا توجد فاتورة بالرقم {id}");

            SaleDTO dto = new SaleDTO
            {
                SaleID = sale.SaleID,
                SaleDate = sale.SaleDate,
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
            };

            return Ok(dto);
        }

        // POST: api/Sales  (إنشاء فاتورة جديدة فاضية - Header فقط)
        [HttpPost]
        [Authorize]
        public IActionResult CreateNew()
        {
            CLSSale sale = CLSSale.CreateNew();

            if (sale == null)
                return BadRequest("فشل إنشاء فاتورة جديدة");

            SaleDTO dto = new SaleDTO
            {
                SaleID = sale.SaleID,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                Items = new List<SaleDetailDTO>()
            };

            return CreatedAtAction(nameof(GetById), new { id = sale.SaleID }, dto);
        }

        // DELETE: api/Sales/5  (حذف الفاتورة كاملة مع أصنافها وإرجاع الكميات للمخزون)
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (CLSSale.Find(id) == null)
                return NotFound($"لا توجد فاتورة بالرقم {id}");

            if (CLSSale.DeleteSale(id))
                return Ok($"تم حذف الفاتورة رقم {id}");

            return BadRequest("فشل حذف الفاتورة");
        }
    
    }

}
