using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Interfaces;
using WebAPI.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace WebAPI.Services
{
    public class AddressService
    {
        private readonly CustomerDatabaseEntities dbContext = new CustomerDatabaseEntities();
        private readonly HttpResponseMessage res = new HttpResponseMessage();

        private List<IAddress> SelectAddresses()
        {
            return dbContext.Addresses
                .Select(
                    (a) => new IAddress
                    {
                        address_id = a.address_id,
                        customer_id = a.customer_id,
                        street_number = a.street_number,
                        street_name = a.street_name,
                        city = a.city,
                        postcode = a.postcode
                    }
                )
                .ToList();
        }
        public HttpResponseMessage GetAddresses()
        {
            List<IAddress> addressList = SelectAddresses();

            if (addressList != null)
            {
                string customerListJson = JsonConvert.SerializeObject(addressList, Formatting.Indented, new JsonSerializerSettings
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
        public HttpResponseMessage GetAddress(int id)
        {
            List<IAddress> addressList = SelectAddresses();
            IAddress address = new IAddress { };

            addressList.ForEach(
                (x) =>
                {
                    if (id == x.address_id)
                    {
                        address = x;
                    }
                }
            );

            if (!String.IsNullOrEmpty(address.street_name))
            {
                string customerJson = JsonConvert.SerializeObject(address, Formatting.Indented);

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
        public async Task<HttpResponseMessage> AddAddress(Addresses addedAddress)
        {
            int lastId = dbContext.Addresses.Select(x => x).OrderByDescending(x => x.address_id).First().address_id;
            int customerId = dbContext.Customers.Single(c => addedAddress.customer.first_name == c.first_name && addedAddress.customer.last_name == c.last_name).customer_id;

            addedAddress.address_id = lastId + 1;
            addedAddress.customer_id = customerId;
            addedAddress.customer = null;

            if (addedAddress.customer_id == null)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Content = new StringContent("This customer does not exist.");

                return res;
            }

            try
            {
                using (CustomerDatabaseEntities context = new CustomerDatabaseEntities())
                {
                    context.Addresses.Add(addedAddress);
                    await context.SaveChangesAsync();

                    return new HttpResponseMessage(HttpStatusCode.Created);
                }
            }
            catch (Exception e)
            {
                res.StatusCode = HttpStatusCode.BadRequest;
                res.Content = new StringContent(e.Message);

                return res;
            }    
        }
        public async Task<HttpResponseMessage> UpdateAddress(int id, Addresses updatedAddress)
        {
            try
            {
                using (var context = new CustomerDatabaseEntities())
                {
                    Addresses address = context.Addresses.Single(x => x.address_id == id);

                    if (address != null)
                    {
                        address.street_number = updatedAddress.street_number;
                        address.street_name = updatedAddress.street_name;
                        address.city = updatedAddress.city;
                        address.postcode = updatedAddress.postcode;

                        await context.SaveChangesAsync();

                        return new HttpResponseMessage(HttpStatusCode.Accepted);
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
        public async Task<HttpResponseMessage> DeleteAddress(int id)
        {
            try
            {
                using (var context = new CustomerDatabaseEntities())
                {
                    Addresses address = context.Addresses.Single(x => x.address_id == id);
                    context.Addresses.Remove(address);
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