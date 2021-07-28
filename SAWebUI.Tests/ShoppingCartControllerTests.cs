using System.Collections.Generic;
using Moq;
using SAWebUI.Controllers;
using SAModels;
using SABL;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using SAWebUI.Models;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;

namespace SAWebUI.Tests {
    public class ShoppingCartControllerTests {

        private static Mock<LineItemManager> InitMockManager() {
            Category c1 = new() { Name = "Category1" };
            Category c2 = new() { Name = "Category2" };
            Category c3 = new() { Name = "Category3" };
            var mockLineItemManager = new Mock<LineItemManager>(null, null, null);
            mockLineItemManager.Setup(
                manager => manager.QueryStoreInventory(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<LineItem>() {
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
                            Categories = new List<Category>() { c1, c3 }
                        }
                    }
                });
            return mockLineItemManager;
        }

        private static IRequestCookieCollection MockRequestCookieCollection(string key, string value) {
            var requestFeature = new HttpRequestFeature();
            var featureCollection = new FeatureCollection();

            requestFeature.Headers = new HeaderDictionary();
            requestFeature.Headers.Add(HeaderNames.Cookie, new StringValues(key + "=" + value));

            featureCollection.Set<IHttpRequestFeature>(requestFeature);

            var cookiesFeature = new RequestCookiesFeature(featureCollection);

            return cookiesFeature.Cookies;
        }

        [Fact]
        public void InventorySearchController() {
            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Loose);
            mockHttpContext.SetupGet(r => r.Request.Cookies).Returns(MockRequestCookieCollection("storefrontID", "1"));

            var mockLogger = new Mock<ILogger<InventoryController>>();

            InventoryController inventoryController = new(mockLogger.Object, InitMockManager().Object, null) {
                ControllerContext = new ControllerContext() {
                    HttpContext = mockHttpContext.Object
                }
            };

            var result = inventoryController.Search("a", "Category1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<InventoryViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Inventory.Count);
        }
    }
}
