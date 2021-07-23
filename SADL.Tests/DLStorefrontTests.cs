using Microsoft.EntityFrameworkCore;
using SADL;
using SAModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SADL.Tests {
    public class DLStorefrontTests {
        private readonly DbContextOptions<SADBContext> _options;

        public DLStorefrontTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                //.UseLazyLoadingProxies()
                .UseSqlite("Filename = storefront_test.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Storefronts.AddRange(
               new Storefront() {
                   Id = 1,
                   Name = "Store1",
                   Address = new Address() {
                       StreetAddress = "123 Store1 Way",
                       City = "Store Town",
                       State = new State() { Code = "TX", Name = "Texas"},
                       ZipCode = "12345"
                   }
               },
               new Storefront() {
                   Name = "Store2",
                   Address = new Address() {
                       StreetAddress = "11 California Street",
                       City = "San Francisco",
                       State = new State() { Code = "CA", Name = "California" },
                       ZipCode = "54321"
                   }
               }
           );

            context.SaveChanges();
        }

        [Fact]
        public void CreateStorefront() {
            using var context = new SADBContext(_options);
            ICRUD<Storefront> db = new StoreModelDB<Storefront>(context);

            Storefront newStore = new() {
                Id = 20,
                Name = "Store3",
                Address = new Address() {
                    StreetAddress = "22 12th Street",
                    City = "New York City",
                    State = new State() { Code = "NY", Name = "New York" },
                    ZipCode = "11111"
                }
            };

            db.Create(newStore);

            Storefront found = context.Storefronts.Find(20);

            Assert.IsType<Storefront>(found);
            Assert.Equal(newStore.Name, found.Name);
            Assert.Equal(newStore.Id, found.Id);
        }


        [Fact]
        public void QueryStorefrontsByAddress() {
            using var context = new SADBContext(_options);
            ICRUD<Storefront> db = new StoreModelDB<Storefront>(context);

            IList<Func<Storefront, bool>> conditions = new List<Func<Storefront, bool>>();
            IList<string> includes = new List<string> {
                "Address",
                "Address.State"
            };

            conditions.Add(sf => sf.Address.ZipCode == "54321");

            IList<Storefront> results = db.Query(new(null) {
                Includes = includes,
                Conditions = conditions
            }); ; ;

            Assert.NotNull(results);
            Assert.Single(results);
            Assert.NotNull(results[0].Address);
            Assert.NotNull(results[0].Address.State);
        }
    }
}
