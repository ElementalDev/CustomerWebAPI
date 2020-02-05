using System.Net.Http;
using System.Web.Http;
using WebAPI.Services;
using WebAPI.Models;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomerController : ApiController
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [Route("")]
        [HttpGet]
        public HttpResponseMessage OnGetCustomers()
        {
            return _customerService.GetCustomers(null);
        }

        [Route("")]
        [HttpGet]
        public HttpResponseMessage OnGetCustomers(bool? isDetailed)
        { 
            return _customerService.GetCustomers(isDetailed);
        }

        [Route("{id}")]
        [HttpGet]
        public HttpResponseMessage OnGetCustomer(int id)
        {
            return _customerService.GetCustomer(id, null);
        }

        [Route("{id}")]
        [HttpGet]
        public HttpResponseMessage OnGetCustomer(int id, bool? isDetailed)
        {
            return _customerService.GetCustomer(id, isDetailed);
        }

        [Route("")]
        [HttpPost]
        public Task<HttpResponseMessage> OnAddCustomer([FromBody]Customers json)
        {
            return _customerService.AddCustomer(json);
            
        }

        [Route("{id}")]
        [HttpPut]
        public Task<HttpResponseMessage> OnUpdateCustomer(int id, [FromBody]Customers json)
        {
            return _customerService.UpdateCustomer(id, json);
        }

        [Route("{id}")]
        [HttpDelete]
        public Task<HttpResponseMessage> OnDeleteCustomer(int id)
        {
            return _customerService.DeleteCustomer(id);
        }
    }
}