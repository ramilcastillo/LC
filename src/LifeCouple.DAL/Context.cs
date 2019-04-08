using LifeCouple.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LifeCouple.DAL
{
    public class Context : DbContext
    {
        private static bool? _isInMemoryDb;

        public DbSet<Id> Ids { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserProfileRegistration> UserProfileRegistrations { get; set; }
        public DbSet<Relationship> Relationships { get; set; }


        public Context(DbContextOptions<Context> options) : base(options)
        {
            if (_isInMemoryDb == null)
            {
                _isInMemoryDb = (this.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory");
            }
        }

        private string GetNewId(object o)
        {
            return GetNewId(o.GetType());
        }

        public string GetNewId(Type t)
        {
            var entityTypeName = t.ToString().Split('.').Last();
            long lastIdIfNull = 0;

            if (_isInMemoryDb == true)
            {
                var idItem = this.Ids.SingleOrDefault(e => e.EntityType == entityTypeName);
                if (idItem == null)
                {
                    idItem = new Id { EntityType = entityTypeName, LastId = lastIdIfNull };
                    this.Add(idItem);
                }
                idItem.LastId++;
                this.SaveChanges();
                return idItem.LastId.ToString();
            }
            else
            {
                //Normal db, not in memory
                //https://dotnetthoughts.net/how-to-execute-a-stored-procedure-with-entity-framework-code-first/


                long id = -1;
                //TODO: Implement MOD 10 (instead of 1,2,3 it shoudl be 13,21,34 with last nr being a MOD 10 check digit)
                using (var txn = this.Database.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    //Need to disable change tracking of the entity will stay in the context with returning old values next time...
                    var defTracking = this.ChangeTracker.QueryTrackingBehavior;
                    this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                    var entityTypeIdValue = this.Ids.FromSql($"UPDATE Id SET LastId=LastId+1 OUTPUT INSERTED.LastId, INSERTED.EntityType WHERE EntityType={entityTypeName}").FirstOrDefault();
                    if (entityTypeIdValue != null)
                    {
                        id = entityTypeIdValue.LastId;
                    }
                    else
                    {
                        entityTypeIdValue = this.Ids.FromSql($"INSERT INTO Id(EntityType, LastId) OUTPUT INSERTED.LastId, INSERTED.EntityType VALUES ({entityTypeName}, {lastIdIfNull++})").FirstOrDefault();
                        id = entityTypeIdValue.LastId;
                    }
                    txn.Commit();
                    this.ChangeTracker.QueryTrackingBehavior = defTracking;
                }
                return id.ToString();
            }

        }
    }
}
