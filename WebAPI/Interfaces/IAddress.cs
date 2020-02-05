using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models;

namespace WebAPI.Interfaces
{
    public class IAddress
    {
        public int address_id;
        public int? customer_id;
        public int? street_number;
        public string street_name;
        public string city;
        public string postcode;
        public ICustomer customer;
    }
}