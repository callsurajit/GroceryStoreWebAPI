using GroceryStoreData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreData {
    public interface IDataService {
        Task<IEnumerable<Customer>> GetCustomerData();

        Task SaveCustomerData(List<Customer> customerList);
    }
}
