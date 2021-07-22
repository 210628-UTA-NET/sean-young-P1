using Microsoft.EntityFrameworkCore;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SADL.Tests {
    public class DLProductTests {

        private readonly DbContextOptions<SADBContext> _options;

        public DLProductTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                //.UseLazyLoadingProxies()
                .UseSqlite("Filename = product_test.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Products.AddRange(
                new Product() {
                    Id = 1,
                    Name = "A Rock",
                    Price = 1.23M,
                    Description = "It's a rock..."
                },
                new Product() {
                    Name = "Happiness",
                    Price = 9999.99M,
                    Description = "Money can actually buy it."
                }
            ); 

            context.SaveChanges();
        }

        [Fact]
        public void CreateProduct() {
            using var context = new SADBContext(_options);
            ICRUD<Product> db = new StoreModelDB<Product>(context);

            Product newProd = new() {
                Id= 20,
                Name = "Tears of the Poor",
                Price = 1000.10M,
                Description = "Gluten Free. Processed in a facility that processes nuts."
            };

            db.Create(newProd);

            Product found = context.Products.Find(20);

            Assert.IsType<Product>(found);
            Assert.Equal(newProd.Name, found.Name);
        }
    }
}
