using ElecStoreAPI.DTOs.CategoriesDTO;
using ElecStoreBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace ElecStoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        // GET: api/Category
        [HttpGet]
        public IActionResult GetAll()
        {
            DataTable dt = CLSCategory.GetAllCategories();

            List<CategoryDTO> categories = new List<CategoryDTO>();

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new CategoryDTO
                {
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CategoryName = row["CategoryName"].ToString(),
                    CDescription = row["CDescription"].ToString()
                });
            }

            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            CLSCategory category = CLSCategory.Find(id);

            if (category == null)
                return NotFound($"لا يوجد فئة بالرقم {id}");

            CategoryDTO dto = new CategoryDTO
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                CDescription = category.CDescription
            };

            return Ok(dto);
        }

        // POST: api/Category
        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] CategoryDTO newCategory)
        {
            CLSCategory category = new CLSCategory
            {
                CategoryName = newCategory.CategoryName,
                CDescription = newCategory.CDescription
            };

            if (category.Save())
            {
                newCategory.CategoryID = category.CategoryID;
                return CreatedAtAction(nameof(GetById), new { id = category.CategoryID }, newCategory);
            }

            return BadRequest("فشل إضافة الفئة");
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] CategoryDTO updatedCategory)
        {
            CLSCategory category = CLSCategory.Find(id);

            if (category == null)
                return NotFound($"لا يوجد فئة بالرقم {id}");

            category.CategoryName = updatedCategory.CategoryName;
            category.CDescription = updatedCategory.CDescription;

            if (category.Save())
                return Ok(updatedCategory);

            return BadRequest("فشل تعديل الفئة");
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (!CLSCategory.IsCategoryExist(id))
                return NotFound($"لا يوجد فئة بالرقم {id}");

            if (CLSCategory.DeleteCategory(id))
                return Ok($"تم حذف الفئة رقم {id}");

            return BadRequest("فشل حذف الفئة");
        }
    
    }

}
