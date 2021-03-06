﻿using deeP.AuthorizationService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;

namespace deeP.AuthorizationService.Models
{
    public sealed class ModelFactory
    {
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(ApplicationUserManager appUserManager)
        {
            _AppUserManager = appUserManager;
        }

        public async Task<UserResultModel> CreateModelForUser(ApplicationUser appUser)
        {
            return new UserResultModel
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                FirstName = appUser.FirstName,
                MiddleNames = appUser.MiddleNames,
                LastName = appUser.LastName,
                RegistrationDate = appUser.RegistrationDate,
                Email = appUser.Email,
                Roles = (await _AppUserManager.GetRolesAsync(appUser.Id)).ToArray(),
                Claims = (await _AppUserManager.GetClaimsAsync(appUser.Id)).ToArray()
            };
        }

        public ApplicationUser CreateUserForModel(CreateUserBindingModel createUserModel)
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