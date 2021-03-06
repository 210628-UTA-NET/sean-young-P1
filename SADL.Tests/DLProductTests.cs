using Microsoft.EntityFrameworkCore;
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
                .UseSqlite("Filename = product_test.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Products.AddRange(
                new() {
                    Id = 1,
                    Name = "A Rock",
                    Price = 1.23M,
                    Description = "It's a rock..."
                },
                new() {
                    Name = "Happiness",
                    Price = 9999.99M,
                    Description = "Money can actually buy it."
                },
                new() {
                    Name = "Potato Salad",
                    Price = 5.25M,
                    Description = "It took three days to make this potato salad.."
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

        [Fact]
        public void DeleteProduct() {
            using var context = new SADBContext(_options);
            ICRUD<Product> db = new StoreModelDB<Product>(context);

            Product found = context.Products.Find(1);
            db.Delete(found);

            IList<Product> results = context.Products.Select(p => p).ToList();
            Assert.NotNull(results);
            Assert.DoesNotContain(results, p => p.Id == 1);
        }

        [Fact]
        public void QueryProduct() {
            using var context = new SADBContext(_options);
            ICRUD<Product> db = new StoreModelDB<Product>(context);

            IList<Product> results = db.Query(new(null) { 
                Conditions = new List<Func<Product, bool>>() {
                    p => p.Price < 10.00M
                }
            });

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(2, results.Count);
            Assert.DoesNotContain(results, p => p.Price >= 10.00M);
        }

        [Fact]
        public void QuerySingleProduct() {
            using var context = new SADBContext(_options);
            ICRUD<Product> db = new StoreModelDB<Product>(context);

            Product result = db.FindSingle(new(null) {
                Conditions = new List<Func<Product, bool>>() {
                    p => p.Id == 1
                }
            });

            Product doesNotExist = db.FindSingle(new(null) {
                Conditions = new List<Func<Product, bool>>() {
                    p => p.Id == 12345
                }
            });

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("A Rock", result.Name);
            Assert.Null(doesNotExist);
        }

        [Fact]
        public void UpdateProduct() {
            using var context = new SADBContext(_options);
            ICRUD<Product> db = new StoreModelDB<Product>(context);

            db.Update(new Product() {
                Id = 1,
                Name = "A bigger rock"
            });

            Product result = context.Products.Find(1);

            Assert.NotNull(result);
            Assert.Equal("A bigger rock", result.Name);
        }
    }
}
