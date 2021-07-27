using Microsoft.EntityFrameworkCore;
using SAModels;
using SADL;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace SABL.Tests {
    public class BLCartTests {
        private readonly DbContextOptions<SADBContext> _options;

        public BLCartTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                .UseSqlite("Filename = bl_carttest.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Category c1 = new() { Name = "Category1" };
            Category c2 = new() { Name = "Category2" };
            Category c3 = new() { Name = "Category3" };

            Product p1 = new() {
                Id = 1,
                Name = "A Rock",
                Price = 1.23M,
                Description = "It's a rock...",
                Categories = new List<Category>() { c1 }
            };

            Product p2 = new() {
                Name = "Happiness",
                Price = 9999.99M,
                Description = "Money can actually buy it.",
                Categories = new List<Category>() { c1, c2 }
            };

            Product p3 = new() {
                Name = "Potato Salad",
                Price = 5.25M,
                Description = "It took three days to make this potato salad..",
                Categories = new List<Category>() { c2, c3 }
            };

            Storefront sf1 = new() {
                Id = 1,
                Name = "Storefront1",
                Items = new List<LineItem>() {
                    new() {
                        Id = 100,
                        Product = p1,
                        Quantity = 100
                    },
                    new() {
                        Id = 101,
                        Product = p2,
                        Quantity = 1
                    },
                    new() {
                        Id = 102,
                        Product = p3,
                        Quantity = 100
                    }
                }
            };

            Storefront sf2 = new() {
                Id = 2,
                Name = "Storefront2"
            };

            CustomerUser customer1 = new() { Id = "user1" };

            context.ShoppingCarts.AddRange(
               new() {
                   Id = 1,
                   CustomerUser = customer1,
                   Storefront = sf1,
                   Items = new List<LineItem>() {
                       new() {
                           Id = 200,
                           Product = p1,
                           Quantity = 10
                       },
                       new() {
                           Id = 201,
                           Product = p3,
                           Quantity = 1
                       }
                   },
                   TotalAmount = 17.55M
               },
               new() {
                   Id = 2,
                   CustomerUser = customer1,
                   Storefront = sf2,
                   Items = new List<LineItem>(),
                   TotalAmount = 0.00M
               },
               new() {
                   Id = 3,
                   CustomerUser = new() { Id = "user2" },
                   Storefront = sf1,
                   Items = new List<LineItem>() {
                       new() {
                           Product = p2,
                           Quantity = 1
                       },
                       new() {
                           Product = p3,
                           Quantity = 100
                       }
                   },
                   TotalAmount = 10524.99M
               }
            );
            context.SaveChanges();
        }

        [InlineData("user1", 1, 1)]
        [InlineData("user1", 2, 2)]
        [InlineData("user2", 1, 3)]
        [Theory]
        public void GetCart(string p_userId, int p_storefrontId, int p_expectedCartId) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            ShoppingCart result = testManager.GetCart(p_userId, p_storefrontId);
            Assert.Equal(p_expectedCartId, result.Id);
        }

        [InlineData(102, 2, "Potato Salad", 3, 28.05)]
        [InlineData(101, 1, "Happiness", 1, 10017.54)]
        [Theory]
        public void AddItem(int p_itemId, int p_quantity, string p_productName, int p_expectedQuantity, decimal p_expectedTotal) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            testManager.AddItem(p_itemId, 1, "user1", p_quantity);

            ShoppingCart cart = context.ShoppingCarts.Find(1);
            LineItem addedItem = cart.Items.FirstOrDefault(i => i.Product.Name == p_productName);
            Assert.NotNull(addedItem);
            Assert.Equal(p_expectedQuantity, addedItem.Quantity);
            Assert.Equal(p_expectedTotal, cart.TotalAmount);
        }

        [InlineData(111, 1, "user1", 2)]
        [InlineData(101, 1, "user1", 2)]
        [InlineData(101, 1, "user1", -1000)]
        [InlineData(101, 1, "user1", 0)]
        [Theory]
        public void AddItemFailure(int p_itemId, int p_storefrontId, string p_userId, int p_quantity) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => testManager.AddItem(p_itemId, p_storefrontId, p_userId, p_quantity));
        }

        [InlineData(200, 5.25)]
        [InlineData(201, 12.3)]
        [Theory]
        public void RemoveItem(int p_itemId, decimal p_expectedTotal) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            testManager.RemoveItem(p_itemId, "user1", 1);
            ShoppingCart cart = context.ShoppingCarts.Find(1);

            Assert.DoesNotContain(cart.Items, i => i.Id == p_itemId);
            Assert.Equal(p_expectedTotal, cart.TotalAmount);
        }

        [InlineData(-1, "user1", 1)]
        [InlineData(200, "user3", 1)]
        [InlineData(201, "user1", -1)]
        [Theory]
        public void RemoveItemFailure(int p_itemId, string p_userId, int p_storefrontId) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => testManager.RemoveItem(p_itemId, p_userId, p_storefrontId));
        }
        
        [Fact]
        public void RemoveAll() {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            testManager.RemoveAll("user1", 1);
            ShoppingCart cart = context.ShoppingCarts.Find(1);
            Storefront store = context.Storefronts.Find(1);

            LineItem item1 = store.Items.FirstOrDefault(i => i.Id == 100);
            LineItem item2 = store.Items.FirstOrDefault(i => i.Id == 102);

            Assert.Equal(110, item1.Quantity);
            Assert.Equal(101, item2.Quantity);

            Assert.Empty(cart.Items);
            Assert.Equal(0.00M, cart.TotalAmount);
        }

        [InlineData("user4", 1)]
        [InlineData("user1", 3)]
        [InlineData("user1", -1)]
        [Theory]
        public void RemoveAllFailure(string p_userId, int p_storefrontId) {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => testManager.RemoveAll(p_userId, p_storefrontId));
        }

        [Fact]
        public void PlaceOrder() {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            testManager.PlaceOrder("user1", 1);

            Order newOrder = context.Orders.FirstOrDefault(o => o.CustomerUserId == "user1");
            Assert.NotNull(newOrder);
            Assert.Equal(17.55M, newOrder.TotalAmount);
        }

        [Fact]
        public void PlaceOrderFailure() {
            using var context = new SADBContext(_options);
            ShoppingCartManager testManager = new(
                new StoreModelDB<ShoppingCart>(context),
                new StoreModelDB<LineItem>(context),
                new StoreModelDB<Order>(context),
                null
            );

            Assert.Throws<ArgumentException>(() => testManager.PlaceOrder("user1", 2));
        }
    }
}
