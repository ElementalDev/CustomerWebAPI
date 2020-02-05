using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using WebAPI.Services;
using WebAPI.Interfaces;
using WebAPI.Models;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/addresses")]
    public class AddressesController : ApiController
    {
        private readonly AddressService addressService = new AddressService();

        [Route("")]
        [HttpGet]
        public HttpResponseMessage OnGetAddress()
        { 
            return addressService.GetAddresses();
        }

        [Route("{id}")]
        [HttpGet]
        public HttpResponseMessage OnGetAddress(int id)
        {
            return addressService.GetAddress(id);
        }

        [Route("")]
        [HttpPost]
        public Task<HttpResponseMessage> OnAddAddress([FromBody]Addresses json)
        {
            return addressService.AddAddress(json);
            
        }

        [Route("{id}")]
        [HttpPut]
        public Task<HttpResponseMessage> OnUpdateAddress(int id, [FromBody]Addresses json)
        {
            return addressService.UpdateAddress(id, json);
        }

        [Route("{id}")]
        [HttpDelete]
        public Task<HttpResponseMessage> OnDeleteAddress(int id)
        {
            return addressService.DeleteAddress(id);
        }
    }
}