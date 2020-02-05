using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Interfaces;

namespace WebAPI.Classes
{
    public class Customer : ICustomer
    {
        public int? _customer_id { get; set; }
        public string _first_name { get; set; }
        public string _last_name { get; set; }
        public int? _age { get; set; }
        public DateTime? _dob { get; set; }
        public string _occupation { get; set; }

        public Customer(int? customer_id, string first_name, string last_name, int? age, DateTime? dob, string occupation)
        {
            _customer_id = customer_id;
            _first_name = first_name;
            _last_name = last_name;
            _age = age;
            _dob = dob;
            _occupation = occupation;
            
        }
    }
}