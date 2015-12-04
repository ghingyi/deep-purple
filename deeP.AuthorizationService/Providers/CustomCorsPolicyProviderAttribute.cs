using Microsoft.Owin;
using Microsoft.Owin.Cors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Cors;

namespace deeP.AuthorizationService.Providers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CustomCorsPolicyProviderAttribute : Attribute, ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        public CustomCorsPolicyProviderAttribute(IEnumerable<string> origins = null, bool allowAnyMethod = true, bool allowAnyHeader = true)
        {
            // Create a CORS policy
            _policy = new CorsPolicy
            {
                AllowAnyMethod = allowAnyMethod,
                AllowAnyHeader = allowAnyHeader
            };

            // Add allowed origins
            if (origins != null)
            {
                foreach (string origin in origins)
                {
                    _policy.Origins.Add(origin);
                }
            }
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(IOwinRequest request)
        {
            return Task.FromResult(_policy);
        }
    }
}