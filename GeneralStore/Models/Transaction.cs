using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStore.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        //Navigation Property
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(Product))]
        public string ProductSKU { get; set; }
        public virtual Product Product { get; set; }

        public int ItemCount
        {
            get 
            {
                int count = 0;
                foreach (var item in Product.SKU)
                {
                    count = count + item;
                }
                return count;
            }
        }

        public DateTime DateOfTransaction { get; set; }
    }
}