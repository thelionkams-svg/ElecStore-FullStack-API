using Microsoft.AspNetCore.Mvc;
using ElecStoreBusiness;
using ElecStoreAPI.DTOs.ProductsDTO;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ElecStoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAll()
        {
            DataTable dt = CLSProduct.GetAllProducts();
            List<ProductDTO> products = new List<ProductDTO>();

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new ProductDTO
                {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    ProductName = row["ProductName"].ToString(),
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    QuantityInStock = Convert.ToInt32(row["QuantityInStock"])
                });
            }

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            CLSProduct product = CLSProduct.Find(id);

            if (product == null)
                return NotFound($"لا يوجد منتج بالرقم {id}");

            return Ok(new ProductDTO
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] ProductDTO newProduct)
        {
            CLSProduct product = new CLSProduct
            {
                ProductName = newProduct.ProductName,
                CategoryID = newProduct.CategoryID,
                Price = newProduct.Price,
                QuantityInStock = newProduct.QuantityInStock
            };

            if (product.Save())
            {
                newProduct.ProductID = product.ProductID;
                return CreatedAtAction(nameof(GetById), new { id = product.ProductID }, newProduct);
            }

            return BadRequest("فشل إضافة المنتج (تأكد إن رقم القسم CategoryID موجود)");
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] ProductDTO updatedProduct)
        {
            CLSProduct product = CLSProduct.Find(id);

            if (product == null)
                return NotFound($"لا يوجد منتج بالرقم {id}");

            product.ProductName = updatedProduct.ProductName;
            product.CategoryID = updatedProduct.CategoryID;
            product.Price = updatedProduct.Price;
            product.QuantityInStock = updatedProduct.QuantityInStock;

            if (product.Save())
                return Ok(updatedProduct);

            return BadRequest("فشل تعديل المنتج");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (!CLSProduct.IsProductExist(id))
                return NotFound($"لا يوجد منتج بالرقم {id}");

            if (CLSProduct.DeleteProduct(id))
                return Ok($"تم حذف المنتج رقم {id}");

            return BadRequest("فشل حذف المنتج");
        }
    
    }

}
