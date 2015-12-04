using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace deeP.AuthorizationService.Models
{
    public class UserResultModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public string[] Roles { get; set; }
        public Claim[] Claims { get; set; }
    }
}