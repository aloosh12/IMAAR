using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Imaar.MobileResponses;
using Microsoft.Win32;
using Volo.Abp.BlobStoring;

namespace Imaar.UserProfiles
{
    public abstract class UserProfileManagerBase : DomainService
    {
        protected IUserProfileRepository _userProfileRepository;
        protected IdentityUserManager _identityUserManager;
        protected IIdentityUserRepository _identityUserRepository;

        protected IDataFilter _dataFilter;
        protected IBlobContainer<UserProfileContainer> _userProfileContainer;
        public UserProfileManagerBase(IUserProfileRepository userProfileRepository, IdentityUserManager identityUserManager, IIdentityUserRepository identityUserRepository, IDataFilter  dataFilter, IBlobContainer<UserProfileContainer> userProfileContainer)
        {
            _userProfileRepository = userProfileRepository;
            _identityUserManager = identityUserManager;
            _identityUserRepository = identityUserRepository;
            _dataFilter = dataFilter;
            _userProfileContainer = userProfileContainer;
        }

        public virtual async Task<UserProfile> CreateAsync(
        string securityNumber, string firstName, string lastName, string phoneNumber, string email, BiologicalSex? biologicalSex = null, DateOnly? dateOfBirth = null, string? latitude = null, string? longitude = null, string? profilePhoto = null)
        {
            Check.NotNullOrWhiteSpace(securityNumber, nameof(securityNumber));
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
            Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            Check.NotNullOrWhiteSpace(email, nameof(email));

            var userProfile = new UserProfile(
             GuidGenerator.Create(),
             securityNumber, firstName, lastName, phoneNumber, email, biologicalSex, dateOfBirth, latitude, longitude, profilePhoto
             );

            return await _userProfileRepository.InsertAsync(userProfile);
        }

        //public virtual async Task<MobileResponse> CreatWithDetialsAsync(string firstName, string lastName, string phoneNumber, string email,  BiologicalSex? biologicalSex = null, DateOnly? dateOfBirth = null, string? latitude = null, string? longitude = null, string? profilePhoto = null)
        //{
        //    MobileResponse mobileResponse = new MobileResponse();
        //    Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
        //    Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
        //    Check.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        //    Check.NotNullOrWhiteSpace(email, nameof(email));

        //    using (_dataFilter.Disable<ISoftDelete>())
        //    {
        //        try
        //        {
        //            var user = _identityUserManager.Users.Where(u => u.PhoneNumber == phoneNumber || u.Email == email).FirstOrDefault();
        //            if (user != null)
        //            {
        //                if (user.IsDeleted == true)
        //                {
        //                    //var identityUser = new Volo.Abp.Identity.IdentityUser(user.Id, email.Split("@")[0], user.Email);
        //                    //identityUser.Name = firstName;
        //                    //identityUser.Surname = lastName;
        //                    //identityUser.SetPhoneNumber(phoneNumber, false);
        //                    //identityUser.IsDeleted = false;
        //                    //Up
        //                    //user = email;
        //                    //user.Surname = userName;
        //                    //var test = await _userManager.UpdateUserAsync(user);
        //                }
        //                return null;
        //            }
        //            else
        //            {
        //                var identityUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), email.Split("@")[0], email);
        //                identityUser.Name = firstName;
        //                identityUser.Surname = lastName;
        //                identityUser.SetPhoneNumber(phoneNumber, false);
        //                var result = await _identityUserManager.CreateAsync(identityUser);

        //                if (result.Succeeded)
        //                {
        //                    Random random = new Random();
        //                    int securityNum = random.Next(1000, 10000);
        //                    var userProfile = new UserProfile(
        //   identityUser.Id,
        //   securityNum.ToString(), firstName, lastName, phoneNumber, email, biologicalSex, dateOfBirth, latitude, longitude, profilePhoto
        //   );
        //                    //var userProfile = new UserProfile( GuidGenerator.Create(), securityNum.ToString(), biologicalSex, dateOfBirth, latitude, longitude, profilePhoto);

        //                    var userProf = await _userProfileRepository.InsertAsync(userProfile);
        //                    if (userProf != null)
        //                    {
        //                        var register = new RegisterResponse()
        //                        {
        //                            FirstName = firstName,
        //                            LastName = lastName,
        //                            DateOfBirth = dateOfBirth,
        //                            BiologicalSex = biologicalSex,
        //                            Latitude = latitude,
        //                            Longitude = longitude,
        //                            PhoneNumber = phoneNumber,
        //                            Email = email,
        //                            SecurityCode = securityNum.ToString(),
        //                            ProfilePhoto = $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/adas/{profilePhoto}"
        //                        };
        //                        mobileResponse.Code = 200;
        //                        mobileResponse.Message = "SUCCESS";
        //                        mobileResponse.Data = register;
        //                    }
        //                    return mobileResponse;
        //                }
        //                else
        //                {
        //                    mobileResponse.Code = 501;
        //                    mobileResponse.Message = result.ToString();
        //                    mobileResponse.Data = null;

        //                    return mobileResponse;
        //                    //throw new Volo.Abp.BusinessException(message: result.ToString());
        //                }
        //            }
        //        }
        //        catch(Exception e)
        //        {
        //            mobileResponse.Code = 501;
        //            mobileResponse.Message = "Internal server error";
        //            mobileResponse.Data = null;
        //            return mobileResponse;
        //        }
        //        }
            

           
        //}

        public virtual async Task<UserProfile> UpdateAsync(
            Guid id,
            string securityNumber, string firstName, string lastName, string phoneNumber, string email, BiologicalSex? biologicalSex = null, DateOnly? dateOfBirth = null, string? latitude = null, string? longitude = null, string? profilePhoto = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(securityNumber, nameof(securityNumber));

            var userProfile = await _userProfileRepository.GetAsync(id);

            userProfile.SecurityNumber = securityNumber;
            userProfile.FirstName = firstName;
            userProfile.LastName = lastName;
            userProfile.PhoneNumber = phoneNumber;
            userProfile.Email = email;
            userProfile.BiologicalSex = biologicalSex;
            userProfile.DateOfBirth = dateOfBirth;
            userProfile.Latitude = latitude;
            userProfile.Longitude = longitude;
            userProfile.ProfilePhoto = profilePhoto;

            userProfile.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userProfileRepository.UpdateAsync(userProfile);
        }

    }
}