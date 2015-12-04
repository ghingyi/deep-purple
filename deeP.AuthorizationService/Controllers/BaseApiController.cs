using deeP.AuthorizationService.Infrastructure;
using deeP.AuthorizationService.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Http.Routing;

namespace deeP.AuthorizationService.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return this.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return this.Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        private ModelFactory _modelFactory;
        protected ModelFactory ModelFactory 
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.AppUserManager);
                }
                return _modelFactory;
            }
        }

        public BaseApiController()
        {
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}