using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Inventoryweb.Models
{

    public class Products
    {
        public int Id { get; set; }
        public string? Name { get; set; }         // Make nullable
        public string? Description { get; set; }
        public string? Category { get; set; }     // Make nullable
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime? CreatedDate { get; set; } // Make nullable
        public bool IsDiscontinued { get; set; }
    }


}
