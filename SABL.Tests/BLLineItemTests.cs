using Microsoft.EntityFrameworkCore;
using SAModels;
using SADL;
using System;
using System.Collections.Generic;
using Xunit;

namespace SABL.Tests {
    public class BLLineItemTests {
        private readonly DbContextOptions<SADBContext> _options;

        public BLLineItemTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                .UseSqlite("Filename = bl_lineitemtest.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Category c1 = new() { Name = "Category1" };
            Category c2 = new() { Name = "Category2" };
            Category c3 = new() { Name = "Category3" };

            Storefront sf = new() { Id = 1, Name = "SF" };
            context.Storefronts.Add(sf);


            context.LineItems.AddRange(
                new() {
                    Id = 1,
                    StorefrontId = 1,
                    Quantity = 10,
                    Product = new() {
                        Id = 1,
                        Name = "A Rock",
                        Price = 1.23M,
                        Description = "It's a rock...",
                        Categories = new List<Category>() { c1 }
                    },
                },
                new() {
                    Id = 2,
                    StorefrontId = 1,
                    Quantity = 200,
                    Product = new() {
                        Name = "Happiness",
                        Price = 9999.99M,
                        Description = "Money can actually buy it.",
                        Categories = new List<Category>() { c1, c2 }
                    },
                },
                new() {
                    Id = 3,
                    StorefrontId = 1,
                    Quantity = 3000,
                    Product = new() {
                        Name = "Potato Salad",
                        Price = 5.25M,
                        Description = "It took three days to make this potato salad..",
                        Categories = new List<Category>() { c2, c3 }
                    }
                }
            );
            context.SaveChanges();
        }

        [Theory]
        [InlineData(1, 2, 12)]
        [InlineData(2, 200, 400)]
        [InlineData(3, 1, 3001)]
        [InlineData(1, 0, 10)]
        [InlineData(1, -10, 0)]
        public void ReplenishSuccess(int p_id, int p_quantity, int p_expectedQuantity) {
            using var context = new SADBContext(_options);
            LineItemManager testManager = new(
                new StoreModelDB<LineItem>(context), 
                new StoreModelDB<Category>(context), 
                null
            );

            testManager.ReplenishItem(p_id, p_quantity);
            LineItem result = context.LineItems.Find(p_id);

            Assert.Equal(p_expectedQuantity, result.Quantity);
        }

        [Fact]
        public void ReplenishNotFound() {
            using var context = new SADBContext(_options);
            LineItemManager testManager = new(
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Category>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => {
                testManager.ReplenishItem(1234, 2);
            });
        }

        [Fact]
        public void ReplenishLessThanZero() {
            using var context = new SADBContext(_options);
            LineItemManager testManager = new(
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Category>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => {
                testManager.ReplenishItem(1, -11);
            });
        }


        [Theory]
        [InlineData(1, null, "Category1", 2)]
        [InlineData(2, null, "Category1", 0)]
        [InlineData(1, null, "Category2", 2)]
        [InlineData(1, null, "Category3", 1)]
        [InlineData(1, null, "Category4", 0)]
        [InlineData(1, "o", null, 2)]
        [InlineData(1, "happiness", null, 1)]
        [InlineData(2, "happiness", null, 0)]

        public void QueryStoreInventory(int p_storefrontId, string p_searchName, string p_category, int p_expectedCount) {
            using var context = new SADBContext(_options);
            LineItemManager testManager = new(
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Category>(context),
                null
            );

            IList<LineItem> results = testManager.QueryStoreInventory(p_storefrontId, p_searchName, p_category);

            Assert.Equal(p_expectedCount, results.Count);
        }
    }
}
