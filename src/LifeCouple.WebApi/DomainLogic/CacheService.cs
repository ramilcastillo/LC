using System;
using LifeCouple.DAL.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace LifeCouple.WebApi.DomainLogic
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        private readonly MemoryCacheEntryOptions staticMemoryCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60));
        private readonly MemoryCacheEntryOptions semiStaticMemoryCacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        private const string partnerUserIdKey = "PartnerUserId";
        private const string externalReferenceIdKey = "ExternalReferenceId";
        private const string relationshipIdKey = "RelationshipId";
        private const string firstnameKey = "FirstName";

        public CacheService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public string GetFirstname_byUserId(string userId) => _memoryCache.Get<string>($"{firstnameKey}-{userId}");

        private void SetFirstname_forUserId(string userId, string firstname)
        {
            if (false == string.IsNullOrEmpty(userId) && false == string.IsNullOrEmpty(firstname))
            {
                _memoryCache.Set($"{firstnameKey}-{userId}", firstname, semiStaticMemoryCacheEntryOptions);
            }
        }


        public string GetPartnerUserId_byCurrentUserId(string userId) => _memoryCache.Get<string>($"{partnerUserIdKey}-{userId}");

        public void SetPartnerUserId_forCurrentUserId(string userId, string partnerUserId)
        {
            if (false == string.IsNullOrEmpty(userId) && false == string.IsNullOrEmpty(partnerUserId))
            {
                _memoryCache.Set($"{partnerUserIdKey}-{userId}", partnerUserId, staticMemoryCacheEntryOptions);
            }
        }


        public string GetRelationshipId_byUserId(string userId) => _memoryCache.Get<string>($"{relationshipIdKey}-{userId}");

        private void SetRelationshipId_forUserId(string userId, string relationshipId)
        {
            if (false == string.IsNullOrEmpty(userId) && false == string.IsNullOrEmpty(relationshipId))
            {
                _memoryCache.Set($"{relationshipIdKey}-{userId}", relationshipId, staticMemoryCacheEntryOptions);
            }
        }


        public string GetUserId_byExternalReferenceId(string externalReferenceId) => _memoryCache.Get<string>($"{externalReferenceIdKey}-{externalReferenceId}");

        private void SetUserId_forExternalReferenceId(string externalReferenceId, string userId)
        {
            if (false == string.IsNullOrEmpty(externalReferenceId) && false == string.IsNullOrEmpty(userId))
            {
                _memoryCache.Set($"{externalReferenceIdKey}-{externalReferenceId}", userId, staticMemoryCacheEntryOptions);
            }
        }


        /// <summary>
        /// Used to set all UserProfile caches in 1 go...
        /// </summary>
        /// <param name="userProfile"></param>
        public void SetUserProfileCache(UserProfile userProfile)
        {
            SetFirstname_forUserId(userProfile.Id, userProfile.FirstName);
            SetRelationshipId_forUserId(userProfile.Id, userProfile.Relationship_Id);
            SetUserId_forExternalReferenceId(userProfile.ExternalRefId, userProfile.Id);
        }

    }
}
