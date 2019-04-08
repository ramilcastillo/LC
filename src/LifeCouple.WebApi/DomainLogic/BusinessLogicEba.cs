using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeCouple.DAL;
using LifeCouple.DAL.Entities;
using LifeCouple.Server.Instrumentation;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.DomainLogic.BL_DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace LifeCouple.WebApi.DomainLogic
{
    public class BusinessLogicEba : BusinessLogic
    {
        private readonly BusinessLogicAppCenter _blAppCenter;

        public BusinessLogicEba(IRepository repository, PhoneService phoneService, IMemoryCache memoryCache, BusinessLogicAppCenter blAppCenter) : base(repository, phoneService, memoryCache)
        {
            _blAppCenter = blAppCenter;
        }

        public async Task<string> SetEbaTransactionAsync(BL_EbaTransactionRequest bl_dto)
        {
            var utcNow = DateTimeOffset.UtcNow;

            //Read Existing one
            var existingItem = await _repo.GetEbaPointsSent_ByUserIdAsync(bl_dto.FromUserprofileId);
            if (existingItem == null)
            {
                var existingUser = await _repo.GetUserProfile_byIdAsync(bl_dto.FromUserprofileId);
                existingItem = new EbaPointsSent { DTCreated = utcNow };
                existingItem.RelationshipId = existingUser.Relationship_Id;
                existingItem.UserprofileId = existingUser.Id;
            }

            //Map and Update Balances
            //Map incoming data to existing entity (need to be mapped this way to not overwrite new properties not part of incoming BL_UserProfile)
            existingItem.DTLastUpdated = utcNow;
            if (existingItem.Transactions == null)
            {
                existingItem.Transactions = new List<EbaPointsSent.EbaTransaction>();
            }
            existingItem.Transactions.Insert(0, new EbaPointsSent.EbaTransaction
            {
                Comment = bl_dto.Comment,
                Key = Guid.NewGuid().ToString("D").ToUpper(),
                Points = bl_dto.PointsToDeposit,
                PostedDT = utcNow
            });
            existingItem.TotalPoints = existingItem.TotalPoints + bl_dto.PointsToDeposit;

            //Validate - done after mapping to ensure the mapping logic not broken
            var settings = await GetBusinessLogicSettings();
            if (validator.CanSetEbaPoints(existingItem, settings, out var validationResult))
            {
                var idUpdatedOrCreated = await _repo.SetEbaPointsSentAsync(existingItem);
                await publishPushNotification(existingItem.UserprofileId, existingItem.Transactions.First().Key);
                return idUpdatedOrCreated;
            }
            else
            {
                throw new BusinessLogicException(validationResult);
            }

            //local method
            async Task publishPushNotification(string currentUserId, string entityId)
            {
                var partnerUserId = await base.GetCachedPartnerUserId(currentUserId);

                if (string.IsNullOrEmpty(partnerUserId))
                {
                    LCLog.Information($"PushNotifications - for UserId:null - Unable to find Partner Id for Current User Id:{currentUserId}. Cannot proceed publising push notificaiton for '{BL_PushNotificationMessageTypeEnum.Type_EBA_Points_Sent}'");
                }
                else
                {
                    await _blAppCenter.PublishNotificationMessage(BL_PushNotificationMessageTypeEnum.Type_EBA_Points_Sent, partnerUserId, entityId);
                }
            }
        }

        /// <summary>
        /// Gets the EBA balance and deposited points + the last 3 transactations + details required for the UI for Sending/Depositing new EBA points
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BL_EbaResponse> GetEbaTransactions_ByUserIdAsync(string userId)
        {
            var relationshipId = await GetCachedRelationshipId_byUserIdAsync(userId);

            //Get EBA for both users belonging to same relationshipId
            var ebaPointsSents = await _repo.FindEbaPointsSent_ByRelationshipIdAsync(relationshipId);

            var results = new BL_EbaResponse
            {
                EbaPointOptions = await getEbaPointOptions(),
                EbaPointsBalance = getPointsBalance(),
                EbaPointsDeposited = getPointsDeposited(),
                RecentTransactions = await getMostRecentEbaTransactions()
            };

            return results;

            //local methods

            async Task<List<BL_EbaPointOption>> getEbaPointOptions()
            {
                var r = new List<BL_EbaPointOption>();
                var settings = await GetBusinessLogicSettings();

                var languageLocale = "";
                foreach (var item in settings.EmotionalBankAccontPointOptions)
                {
                    r.Add(new BL_EbaPointOption { Text = item.Value.GetValueOrDefault(languageLocale), PointValue = item.Key });
                }

                return r;
            }

            int getPointsBalance()
            {
                var otherUserPoints = ebaPointsSents.SingleOrDefault(e => e.UserprofileId != userId);
                return otherUserPoints?.TotalPoints ?? 0;
            }

            int getPointsDeposited()
            {
                var thisUserPoints = ebaPointsSents.SingleOrDefault(e => e.UserprofileId == userId);
                return thisUserPoints?.TotalPoints ?? 0;
            }

            async Task<List<BL_EbaTransactionDetail>> getMostRecentEbaTransactions()
            {
                var r = new List<BL_EbaTransactionDetail>();

                //Get details for the other user
                var otherUsersFirstName = string.Empty;
                var otherUserId = ebaPointsSents.SingleOrDefault(e => e.UserprofileId != userId)?.UserprofileId;
                if (false == string.IsNullOrEmpty(otherUserId))
                {
                    otherUsersFirstName = await GetCachedFirstname_byUserIdAsync(otherUserId);

                    //Get the most recent for otherUser
                    if (null != ebaPointsSents.SingleOrDefault(e => e.UserprofileId != userId)?.Transactions)
                    {
                        foreach (var txn in ebaPointsSents.SingleOrDefault(e => e.UserprofileId != userId)?.Transactions?.Take(3))
                        {
                            r.Add(new BL_EbaTransactionDetail { Comment = txn.Comment, FirstName = otherUsersFirstName, Id = txn.Key, Point = txn.Points, Posted = txn.PostedDT, TypeOfTransaction = BL_EbaTransactionTypeEnum.Received });
                        }
                    }
                }

                //Get the most recent for thisUser
                if (null != ebaPointsSents.SingleOrDefault(e => e.UserprofileId == userId)?.Transactions)
                {
                    var thisUsersFirstName = await GetCachedFirstname_byUserIdAsync(userId);
                    foreach (var txn in ebaPointsSents.SingleOrDefault(e => e.UserprofileId == userId).Transactions.Take(3))
                    {
                        r.Add(new BL_EbaTransactionDetail { Comment = txn.Comment, FirstName = thisUsersFirstName, Id = txn.Key, Point = txn.Points, Posted = txn.PostedDT, TypeOfTransaction = BL_EbaTransactionTypeEnum.Sent });
                    }
                }

                //Sort and keep most recent 3
                return r.OrderByDescending(e => e.Posted).Take(3).ToList();
            }
        }
    }
}
