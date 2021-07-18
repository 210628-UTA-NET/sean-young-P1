using Microsoft.EntityFrameworkCore;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADL {
    public class SADBContext: DbContext {

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Storefront> Storefronts { get; set; }
        public SADBContext(): base() { }
        public SADBContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder p_modelBuilder) {
            p_modelBuilder.Entity<Address>();
            p_modelBuilder.Entity<Category>();
            p_modelBuilder.Entity<Country>();
            p_modelBuilder.Entity<Customer>();
            p_modelBuilder.Entity<LineItem>();
            p_modelBuilder.Entity<Order>();
            p_modelBuilder.Entity<Product>();
            p_modelBuilder.Entity<State>();
            p_modelBuilder.Entity<Storefront>();
        }
    }
}
