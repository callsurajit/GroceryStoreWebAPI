using GroceryStoreAPI.Controllers;
using GroceryStoreData;
using GroceryStoreData.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreTests {
    public class GroceryStoreControllerTest {
        [Fact]
        public async Task GetCustomersTest_With2Customers() {
            var groceryService = new Mock<IDataService>();
            var controller = new ValuesController(groceryService.Object);

            groceryService
                .Setup(x => x.GetCustomerData())
                .ReturnsAsync(DefaultCustomerListJSON());

            var customerCollection = new List<Customer>
            {
                new Customer{ Id = 1, Name = "Bob" },
                new Customer{ Id = 2, Name = "Mary" }
            };
            var outputJSON = JsonConvert.SerializeObject(customerCollection);
            
            var result = await controller.GetCustomers();
            var resultJSON = JsonConvert.SerializeObject(result.Value);

            Assert.Equal(outputJSON, resultJSON);
        }

        [Fact]
        public async Task GetCustomerByIdTest()
        {
            var groceryService = new Mock<IDataService>();
            var controller = new ValuesController(groceryService.Object);

            groceryService
                .Setup(x => x.GetCustomerData())
                .ReturnsAsync(DefaultCustomerListJSON());
            var cust = new Customer { Id = 1, Name = "Bob" };           
            var outputJSON = JsonConvert.SerializeObject(cust);

            var result = await controller.GetCustomerById(1);
            var resultJSON = JsonConvert.SerializeObject(result.Value);

            Assert.Equal(outputJSON, resultJSON);
        }

        [Fact]
        public async Task GetCustomerByIdNotFoundTest()
        {
            var groceryService = new Mock<IDataService>();
            var controller = new ValuesController(groceryService.Object);

            groceryService
                .Setup(x => x.GetCustomerData())
                .ReturnsAsync(DefaultCustomerListJSON());

            var result = await controller.GetCustomerById(3);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateCustomerTest()
        {
            var groceryService = new Mock<IDataService>();
            var controller = new ValuesController(groceryService.Object);

            groceryService
                .Setup(x => x.GetCustomerData())
                .ReturnsAsync(DefaultCustomerListJSON());

            var cust = new Customer { Id = 3, Name = "Bryan" };
            var result = await controller.CreateCustomer(cust);

            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        private IEnumerable<Customer> DefaultCustomerListJSON() {

            List<Customer> customerList = new List<Customer>();
            using (var reader = new StreamReader("database.json")) {
                var json = reader.ReadToEnd();
                // creating a JSON object from the string
                var jsonToObj = JObject.Parse(json);
                // converting to a JSON array
                var array = (JArray)jsonToObj["customers"];

                if (array != null)
                {
                    customerList = array.ToObject<List<Customer>>();
                }

                return customerList;
            }
        }
    }
}
