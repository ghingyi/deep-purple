using deeP.SPAWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deeP.SPAWeb.Filters
{
    public class DefaultsFilter : ActionFilterAttribute
    {
        private readonly IConfigurationService ConfigurationService;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            dynamic bag = filterContext.Controller.ViewBag;

            bag.Configuration = this.ConfigurationService;
            bag.BaseUri = "/";
        }

        #region Construction logic

        public DefaultsFilter(IConfigurationService configurationService)
        {
            if (configurationService == null)
            {
                throw new ArgumentNullException("configurationService");
            }

            this.ConfigurationService = configurationService;
        }

        #endregion
    }
}