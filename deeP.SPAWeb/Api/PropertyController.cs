using deeP.SPAWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace deeP.SPAWeb.Api
{
    [Authorize]
    [RoutePrefix("api/properties")]
    public class PropertyController : ApiController
    {
        private ILoggingService LoggingService { get; set; }

        [AllowAnonymous]
        [HttpGet]
        [Route("hello")]
        public string Hello()
        {
            return "Hello!";
        }

        public PropertyController(ILoggingService loggingService)
        {
            this.LoggingService = loggingService;
        }
    }
}
