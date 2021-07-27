using Microsoft.EntityFrameworkCore;
using SAModels;
using SADL;
using System;
using System.Collections.Generic;
using Xunit;

namespace SABL.Tests {
    public class BLStorefrontTests {
        private readonly DbContextOptions<SADBContext> _options;

        public BLStorefrontTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                .UseSqlite("Filename = bl_storefronttest.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            State ca = new() { Code = "CA", Name = "California" };

            context.Storefronts.AddRange(
               new() {
                   Id = 1,
                   Name = "Store1",
                   Address = new() {
                       StreetAddress = "123 Store1 Way",
                       City = "Store Town",
                       State = new() { Code = "TX", Name = "Texas" },
                       ZipCode = "12345"
                   }
               },
               new() {
                   Id = 2,
                   Name = "Store2",
                   Address = new() {
                       StreetAddress = "11 California Street",
                       City = "San Francisco",
                       State = ca,
                       ZipCode = "54321"
                   }
               },
               new() {
                   Id = 3,
                   Name = "Store3",
                   Address = new() {
                       StreetAddress = "22 12th Street",
                       City = "Los Angeles",
                       State = ca,
                       ZipCode = "77777"
                   }
               }
           );
            context.SaveChanges();
        }

        [Theory]
        [InlineData(1, "Store1")]
        [InlineData(2, "Store2")]
        [InlineData(3, "Store3")]
        [InlineData(4, null)]
        public void GetStorefront(int p_id, string p_name) {
            using var context = new SADBContext(_options);
            ICRUD<Storefront> storefrontDb = new StoreModelDB<Storefront>(context);
            StorefrontManager testManager = new(storefrontDb, null);

            Storefront result = testManager.Get(p_id);

            if (p_name != null) {
                Assert.Equal(p_name, result.Name);
            } else {
                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData("12345", 1)]
        [InlineData("ca", 2)]
        [InlineData("S", 3)]
        [InlineData("Los angeles", 1)]
        [InlineData("NY", 0)]
        [InlineData("", 3)]
        public void QueryStorefrontAddress(string p_address, int p_expectedCount) {
            using var context = new SADBContext(_options);
            ICRUD<Storefront> storefrontDb = new StoreModelDB<Storefront>(context);
            StorefrontManager testManager = new(storefrontDb, null);

            IList<Storefront> results = testManager.QueryByAddress(p_address);

            Assert.Equal(p_expectedCount, results.Count);
        }
    }
}
