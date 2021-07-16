using Microsoft.EntityFrameworkCore;
using SADL;
using SAModels;
using System;
using System.Linq;
using Xunit;

namespace SADL.Tests {
    public class CustomerTests {

        private readonly DbContextOptions<SADBContext> _options;

        public CustomerTests() {
            _options = new DbContextOptionsBuilder<SADBContext>()
                .UseSqlite("Filename = test.db").Options;
            Seed();
        }

        private void Seed() {
            using var context = new SADBContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Country _USA = new() {
                Alpha2 = "US",
                Name = "United States of America",
            };

            context.Countries.Add(_USA);

            context.Customers.AddRange(
                new Customer() {
                    Id = 1,
                    Name = "Sean Young",
                    Address = new Address() {
                        StreetAddress = "123 Street Way",
                        City = "Somewhereville",
                        State = new State() { Code = "CA", Name = "California" },
                        Country = _USA,
                        ZipCode = "12345"
                    },
                    Email = "syoung908@gmail.com",
                    Phone = "(123) 456-7890",
                },
                new Customer() {
                    Name = "Joe Shmoe",
                    Address = new Address() {
                        StreetAddress = "Nowhere Avenue",
                        City = "Nowhereville",
                        State = new State() { Code = "AZ", Name = "Arizona" },
                        Country = _USA,
                        ZipCode = "54321"
                    },
                    Email = "shmoethejoe@gmail.com",
                    Phone = "(555) 555-5555",
                }
            );

            context.SaveChanges();
        }

        [Fact]
        public void CreateCustomer() {
            using var context = new SADBContext(_options); ICRUD<Customer> db = new StoreModelDB<Customer>(context);

            Customer newCustomer = new() {
                Name = "Sum Gai",
                Address = new Address() {
                    StreetAddress = "12 Saloon Drive",
                    City = "San Francisco",
                    State = new State() { Code = "TX", Name = "Texas" },
                    Country = context.Countries.Find("US"),
                    ZipCode = "12345"
                },
                Email = "thatgai@live.com",
                Phone = "(888) 888-8888",
            };

            db.Create(newCustomer);

            Customer found = context.Customers.Find(newCustomer.Id);

            Assert.IsType<Customer>(found);
            Assert.Equal(newCustomer.Name, found.Name);
        }

        [Fact]
        public void DeleteCustomer() {
            using var context = new SADBContext(_options); ICRUD<Customer> db = new StoreModelDB<Customer>(context);

            db.Delete(new Customer() { Id = 1 });

            var results = context.Customers.Select(c => c).ToList();

            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Empty(results.Where(c => c.Name == "Sean Young"));
        }

        [Fact]
        public void UpdateCustomer() {
            using var context = new SADBContext(_options); ICRUD<Customer> db = new StoreModelDB<Customer>(context);

            db.Update(new Customer() { Id = 1, Name ="Bob Robertson" });

            Customer found = context.Customers
                .Include(c => c.Address)
                .Include(c => c.Address.Country)
                .Include(c => c.Address.State)
                .First(c => c.Id == 1);

            Assert.Equal("Bob Robertson", found.Name);
            Assert.Equal("123 Street Way", found.Address.StreetAddress);
            Assert.Equal("syoung908@gmail.com", found.Email);
        }
    }
}
