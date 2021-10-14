using GeneralStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStore.Controllers
{
    public class ProductController : ApiController
    {
        //Repo Pattern in API
        //Where we store the data
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Create Post
        [HttpPost]
        public async Task<IHttpActionResult> PostProduct(Product model)
        {
            if (model == null)
                return BadRequest("Please enter info");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Products.Add(model);

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"{model.Name} was added to the database.");
            else
                return InternalServerError();
        }

        //Read get
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        //Get Product by SKU
        [HttpGet]
        [Route("api/Product/GetProductBySku/{sku}")]//updated the route in Postman and the website
        public async Task<IHttpActionResult> GetProductBySKU(string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        //update put
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProductBySKU([FromUri] string sku, [FromBody] Product model)
        {
            if (model == null)
                return BadRequest("Please enter info");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products.FindAsync(sku);

            if (product == null)
                return NotFound();

            //Do not update Keys
            product.Price = model.Price;
            product.Name = model.Name;
            product.Description = model.Description;
            product.NumberInStock = model.NumberInStock;

            if (await _context.SaveChangesAsync() == 1)
                return Ok();

            return InternalServerError();
        }

        //delete
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProductBySKU([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"{product.Name} was deleted.");

            return InternalServerError();
        }
    }
}
