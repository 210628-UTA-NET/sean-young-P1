using SAModels;
using System;
using System.Reflection;
using Xunit;

namespace SAModels.Tests {
    public class CustomerTests {
        [Fact]
        public void TestTransfer() {
            Customer c1 = new() {
                Name = "Sean Young",
                Address = new Address() {
                    StreetAddress = "123 Street Way",
                    City = "Somewhereville",
                    State = new State() { Code = "CA", Name = "California" },
                    Country = new Country() { Alpha2 = "US", Name = "United States of America" },
                    ZipCode = "12345"
                },
                Email = "syoung908@gmail.com",
                Phone = "(123) 456-7890",
            };

            Customer c2 = new();

            c1.Transfer(c2);

            Assert.True(c1.Name == c2.Name);
            Assert.True(c1.Email == c2.Email);
            Assert.True(c1.Phone == c2.Phone);
            Assert.True(c1.Id == c2.Id);
            Assert.True(c1.Address == c2.Address);

        }
    }
}
