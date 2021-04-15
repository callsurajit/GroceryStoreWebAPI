using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Models
{
    public class Customer
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
