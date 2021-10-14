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
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer(Customer model)
        {
            if (model == null)
                return BadRequest("Please enter info");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

             _context.Customers.Add(model);

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"{model.FullName} was added to the database.");
            else
                return InternalServerError();

        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            return Ok(await _context.Customers.ToListAsync());
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomerById([FromUri]int id, [FromBody]Customer model)
        {
            if (model == null)
                return BadRequest("Please enter some info");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            if (await _context.SaveChangesAsync() == 1)
                return Ok();

            return InternalServerError();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomerById([FromUri]int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"{customer.FullName} was deleted.");

            return InternalServerError();
        }
    }
}
