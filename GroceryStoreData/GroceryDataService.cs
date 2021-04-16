using GroceryStoreData.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreData {
    public class GroceryDataService : IDataService {
        /// <summary>
        /// Method to get the collection of Customers from the db file.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> GetCustomerData() {
            List<Customer> customerList = new List<Customer>();
            using (var reader = new StreamReader("database.json")) {
                var json = reader.ReadToEnd();
                // creating a JSON object from the string
                var jsonToObj = JObject.Parse(json);
                // converting to a JSON array
                var array = (JArray)jsonToObj["customers"];

                if (array != null) {
                    customerList = array.ToObject<List<Customer>>();
                }

                return customerList;
            }
        }

        /// <summary>
        /// Method to write the changes to the db file.
        /// </summary>
        /// <param name="customerList">Changes in the Customer Collection</param>
        /// <returns></returns>
        public async Task SaveCustomerData(List<Customer> customerList) {
            using (StreamWriter dbFile = new StreamWriter("database.json")) {
                var collectionWrapper = new {
                    customers = customerList
                };
                // serialize the object to JSON
                var output = JsonConvert.SerializeObject(collectionWrapper);
                // save the JSON string to the file.
                dbFile.WriteLine(output);
            }
        }
    }
}
