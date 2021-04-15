using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GroceryStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {
        /// <summary>
        /// Get a list of all the customers of the store.
        /// </summary>
        /// <returns>Id and Name of all the customers if found otherwise sends 404 NotFound</returns>
        // GET: api/values
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Customer>>> GetCustomers() {
            using (var reader = new StreamReader("database.json")) {                
                var json = reader.ReadToEnd();
                // creating a JSON object from the string
                var jsonToObj = JObject.Parse(json);
                // converting to a JSON array
                var array = (JArray)jsonToObj["customers"];

                if (array != null) {
                    var customerList = array.ToObject<List<Customer>>();

                    return customerList.ToList();
                }

                return NotFound();
            }
        }

        /// <summary>
        /// Get the customer information based on the Id passed.
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>Id and Name of the customer if there is a match otherwise sends 404 NotFound</returns>
        // GET: api/values/5
        [HttpGet("{id}", Name = "CustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Customer>> GetCustomerById(int id) {            
            // calling the Get method to get the list of all the customers
            var customerList = GetCustomers().Result.Value;
            // the list should not be empty
            if(customerList != null) {
                // find the match if present
                var customer = customerList.FirstOrDefault(cust => cust.Id == id);
                // yayyy..record found...
                if (customer != null) {
                    return customer;                    
                }
            }

            return NotFound();
        }

        // POST: api/values
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer) {
            // the object cannot be null
            if (customer == null) {
                //log the error to any sink
                return BadRequest("Customer object is null");
            }

            // validate the model
            if (!ModelState.IsValid) {
                //log the error to any sink
                return BadRequest("Invalid model object");
            }

            // calling the Get method to get the list of all the customers
            var customerList = GetCustomers().Result.Value;
            
            // if it is empty create a new list
            if (customerList == null) {
                customerList = new List<Customer>();
            }

            using (StreamWriter dbFile = new StreamWriter("database.json")) {
                customerList.Add(customer);

                var collectionWrapper = new {
                    customers = customerList
                };
                // serialize the object to JSON
                var output = JsonConvert.SerializeObject(collectionWrapper);
                // save the JSON string to the file.
                dbFile.WriteLine(output);
            }

            return CreatedAtRoute("CustomerById", new { id = customer.Id }, customer);
        }

        // PUT: api/values/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ModifyCustomer(int id, [FromBody] Customer customer)
        {
            // the object cannot be null
            if (customer == null) {
                //log the error to any sink
                return BadRequest("Customer object is null");
            }

            // validate the model
            if (!ModelState.IsValid) {
                //log the error to any sink
                return BadRequest("Invalid model object");
            }

            // calling the Get method to get the list of all the customers
            var customerList = GetCustomers().Result.Value;

            // it can't be empty otherwise who am I trying to modify?
            if (customerList != null) {
                // look for the customer in the list
                var cust = customerList.FirstOrDefault(c => c.Id == id);
                if(cust == null) {
                    return NotFound();
                }

                using (StreamWriter dbFile = new StreamWriter("database.json")) {
                    // map the changes from the passing object
                    cust.Name = customer.Name;

                    var collectionWrapper = new
                    {
                        customers = customerList
                    };
                    // serialize the object to JSON
                    var output = JsonConvert.SerializeObject(collectionWrapper);
                    // save the JSON string to the file.
                    dbFile.WriteLine(output);

                    return NoContent();
                }
            }

            return NotFound();
        }
    }
}
