
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Inventoryweb.Models
{
    public class InventoryDBContext : DbContext
    {
        public InventoryDBContext(DbContextOptions<InventoryDBContext> options)
            : base(options) { }

        public virtual DbSet<Products> Products { get; set; }
    }
}
