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
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Post- Create
        [HttpPost]
        public async Task<IHttpActionResult> PostTransaction(Transaction model)
        {
            if (model == null)
                return BadRequest("Please enter info");
            if (!ModelState.IsValid)
                return BadRequest("Make sure to enter valid info");

            model.DateOfTransaction = DateTime.Now;//sets the date of the transaction

            Product product = await _context.Products.FindAsync(model.ProductSKU);//finding the instance of product
            //checking for null
            if (product == null)
                return NotFound();

            //Verify in stock
            if (!product.IsInStock)
                return BadRequest("Product is out of stock.");

            //Enough product for the transaction
            if (product.NumberInStock < model.ItemCount)
                return BadRequest("Not enough in stock");

            //If transaction is made deduct from stock
            product.NumberInStock -= model.ItemCount;

            _context.Transactions.Add(model);
            int example = await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
            //or return Ok(await _context.Transactions.ToListAsync());
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionById(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpGet]
        //[Route("api/Transaction/GetTransactionByCustomerId/{id}")]
        //you would use (int id)
        public async Task<IHttpActionResult> GetTransactionByCustomerId(int customerId)
        {
            //check customerId actually exists
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound();

            List<Transaction> transactionsToReturn = new List<Transaction>();
            List<Transaction> transactionsInDatabase = await _context.Transactions.ToListAsync();

            /* foreach(var transaction in transactionsInDatabase)
             {
                 if(customerId == transaction.CustomerId)
                 {
                     transactionsToReturn.Add(transaction);
                 }
             }*/

            //Linq query
            transactionsToReturn = transactionsInDatabase.Where(t => t.CustomerId == customerId).ToList();

            return Ok(transactionsToReturn);
        }

        //Update
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransactionById([FromUri]int id, [FromBody]Transaction model)
        {
            if (model == null)
                return BadRequest("Please enter some info");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            //these would stay the same (wouldn't let them update those)
            transaction.ProductSKU = model.ProductSKU;
            transaction.CustomerId = model.CustomerId;

            Product product = await _context.Products.FindAsync(transaction.ProductSKU);

            int newTransactionNum;
            if(model.ItemCount > transaction.ItemCount)
            {
                newTransactionNum = model.ItemCount - transaction.ItemCount;
                if (product.NumberInStock < newTransactionNum)
                    return BadRequest("Not enough in stock");
                product.NumberInStock -= newTransactionNum;
            }
            else
            {
                newTransactionNum = transaction.ItemCount - model.ItemCount;
                product.NumberInStock += newTransactionNum;
            }

            transaction.ItemCount = model.ItemCount;
            return Ok(await _context.SaveChangesAsync());
        }

        //Delete
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransactionById(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            Product product = await _context.Products.FindAsync(transaction.ProductSKU);
            product.NumberInStock += transaction.ItemCount;

            _context.Transactions.Remove(transaction);

            return Ok(await _context.SaveChangesAsync());
        }
    }
}
