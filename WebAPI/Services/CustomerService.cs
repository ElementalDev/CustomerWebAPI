using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Interfaces;
using WebAPI.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Globalization;

namespace WebAPI.Services
{
    public class CustomerService
    {
        private readonly CustomerDatabaseEntities dbContext = new CustomerDatabaseEntities();
        private readonly HttpResponseMessage res = new HttpResponseMessage();

        private List<ICustomer> SelectCustomers()
        {
            return dbContext.Customers.Select(
                (x) => new ICustomer
                {
                    customer_id = x.customer_id,
                    first_name = x.first_name,
                    last_name = x.last_name,
                    age = x.age,
                    dob = x.dob,
                    occupation = x.occupation
                }
            ).ToList();
        }
        private List<IAddress> SelectCustomerAddresses(ICustomer customer)
        {
            DateTime newDate = new DateTime();

            List<IAddress> addressList = new List<IAddress>();
            DateTime.TryParse(customer.dob.ToString(), out newDate);
            customer.dob = newDate;

            addressList = dbContext.Addresses.Where(a => a.customer_id == customer.customer_id).Select(
               (a) => new IAddress
               {
                   address_id = a.address_id,
                   customer_id = customer.customer_id,
                   street_number = a.street_number,
                   street_name = a.street_name,
                   city = a.city,
                   postcode = a.postcode
               }
               ).ToList();

            return addressList;
        }
        public HttpResponseMessage GetCustomers(bool? isDetailed)
        {
            List<ICustomer> customerList = SelectCustomers();
            bool customerIsDetailed = isDetailed.HasValue ? isDetailed.Value : false;

            if (customerList != null)
            {
                if (customerIsDetailed)
                {
                    foreach (ICustomer customer in customerList)
                    {
                        customer.addresses = SelectCustomerAddresses(customer);
                    }
                }

                string customerListJson = JsonConvert.SerializeObject(customerList, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                res.StatusCode = HttpStatusCode.OK;
                res.Content = new StringContent(customerListJson);
                res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return res;

            }
            else
            {
                res.StatusCode = HttpStatusCode.NotFound;
                res.Content = new StringContent("There are no customers. Please contact a database admin.");

                return res;
            }
        }
        public HttpResponseMessage GetCustomer(int id, bool? isDetailed)
        {
            List<ICustomer> customerList = SelectCustomers();
            ICustomer customer = new ICustomer { };
            
            bool customerIsDetailed = isDetailed.HasValue ? isDetailed.Value : false;

            customerList.ForEach(
                (x) =>
                {
                    if (id == x.customer_id)
                    {
                        customer = x;
                    }

                    if (customerIsDetailed)
                    {
                        customer.addresses = SelectCustomerAddresses(customer);
                    }
                }
            );

            if (!String.IsNullOrEmpty(customer.first_name))
            {
                string customerJson = JsonConvert.SerializeObject(customer, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                res.StatusCode = HttpStatusCode.OK;
                res.Content = new StringContent(customerJson);
                res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return res;
            }
            else
            {
                res.StatusCode = HttpStatusCode.NotFound;
                res.Content = new StringContent("This customer does not exist");

                return res;
            }
        }
        public async Task<HttpResponseMessage> AddCustomer(Customers addedCustomer)
        {
            int lastId = dbContext.Customers.Select(x => x).OrderByDescending(x => x.customer_id).First().customer_id;

            addedCustomer.customer_id = lastId + 1;

            try
            {
                using (var context = new CustomerDatabaseEntities())
                {
                    context.Customers.Add(addedCustomer);
                    await context.SaveChangesAsync();

                    res.StatusCode = HttpStatusCode.Created;
                    res.Content = new StringContent(JsonConvert.SerializeObject(addedCustomer));
                    res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return res;
                }
            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Content = new StringContent(e.ToString());

                return res;
            }    
        }
        public async Task<HttpResponseMessage> UpdateCustomer(int id, Customers updatedCustomer)
        {
            try
            {
                using (var context = new CustomerDatabaseEntities())
                {
                    Customers customer = context.Customers.Single(x => x.customer_id == id);

                    if (customer != null)
                    {
                        customer.first_name = updatedCustomer.first_name;
                        customer.last_name = updatedCustomer.last_name;
                        customer.age = updatedCustomer.age;
                        customer.dob = updatedCustomer.dob;
                        customer.occupation = updatedCustomer.occupation;

                        await context.SaveChangesAsync();

                        res.StatusCode = HttpStatusCode.Accepted;
                        res.Content = new StringContent(JsonConvert.SerializeObject(customer));
                        res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return res;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.BadRequest;
                        res.Content = new StringContent("There is no customer with that ID.");

                        return res;
                    }
                }

            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Content = new StringContent(e.ToString());

                return res;
            }

        }
        public async Task<HttpResponseMessage> DeleteCustomer(int id)
        {
            try
            {
                using (var context = new CustomerDatabaseEntities())
                {
                    Customers customer = context.Customers.Single(x => x.customer_id == id);
                    List<Addresses> addresses = context.Addresses.Where(a => a.customer_id == customer.customer_id).ToList();

                    context.Customers.Remove(customer);
                    context.Addresses.RemoveRange(addresses);

                    await context.SaveChangesAsync();
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Content = new StringContent(e.ToString());

                return res;
            }
        }
    }
}