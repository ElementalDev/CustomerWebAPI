using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models;

namespace WebAPI.Interfaces
{
    public class ICustomer
    {
        public int customer_id;
        public string first_name;
        public string last_name;
        public int? age;
        public DateTime? dob;
        public string occupation;
        public List<IAddress> addresses;

    }
}