using deeP.AuthorizationService.Infrastructure;
using deeP.AuthorizationService.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace deeP.AuthorizationService.Controllers
{
    [Authorize]
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);
            if (user != null)
            {
                UserResultModel resultModel = await this.ModelFactory.Create(user);
                return Ok(resultModel);
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize]
        [Route("getclaims")]
        public IHttpActionResult GetClaims()
        {
            var identity = User.Identity as ClaimsIdentity;

            var claims = from c in identity.Claims
                         select new
                         {
                             subject = c.Subject.Name,
                             type = c.Type,
                             value = c.Value
                         };

            return Ok(claims);
        }

        [HttpGet]
        [Authorize]
        [Route("getuser")]
        public async Task<IHttpActionResult> GetUser()
        {
            var user = await this.AppUserManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                UserResultModel resultModel = await this.ModelFactory.Create(user);
                return Ok(resultModel);
            }

            return NotFound();
        }

        [AllowAnonymous]
        [Route("createseller")]
        public async Task<IHttpActionResult> CreateSeller(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = CreateApplicationUser(createUserModel);

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            IdentityResult addToRoleResult = await this.AppUserManager.AddToRolesAsync(user.Id, InternalConstants.RoleNames.Seller);
            if (!addToRoleResult.Succeeded)
            {
                return GetErrorResult(addToRoleResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
            UserResultModel resultModel = await this.ModelFactory.Create(user);
            return Created(locationHeader, resultModel);
        }

        [AllowAnonymous]
        [Route("createbuyer")]
        public async Task<IHttpActionResult> CreateBuyer(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = CreateApplicationUser(createUserModel);

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            IdentityResult addToRoleResult = await this.AppUserManager.AddToRolesAsync(user.Id, InternalConstants.RoleNames.Buyer);
            if (!addToRoleResult.Succeeded)
            {
                return GetErrorResult(addToRoleResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
            UserResultModel resultModel = await this.ModelFactory.Create(user);
            return Created(locationHeader, resultModel);
        }

        [Route("changepassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        private static ApplicationUser CreateApplicationUser(CreateUserBindingModel createUserModel)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                MiddleNames = createUserModel.MiddleNames,
                LastName = createUserModel.LastName,
                RegistrationDate = DateTime.UtcNow,
            };
            return user;
        }
    }
}
