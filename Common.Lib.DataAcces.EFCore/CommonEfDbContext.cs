﻿using Common.Lib.Core;
using Common.Lib.Core.Context;
using Microsoft.EntityFrameworkCore;

namespace Common.Lib.DataAccess.EFCore
{
    public class CommonEfDbContext : DbContext, IDbSetProvider
    {
        public Dictionary<string, Func<object>> DbSets { get; set; }

        public Dictionary<Type, CachedDbSetEFWrapper> DbSetsWrappers { get; set; }

        public CommonEfDbContext(DbContextOptions options)
               : base(options)
        {
            DbSets = new Dictionary<string, Func<object>>
            {
                //{ typeof(BabylonConcept).FullName, () => BabylonConcepts },
                //{ typeof(User).FullName, () => User },
            };

            DbSetsWrappers = new Dictionary<Type, CachedDbSetEFWrapper>
            {
                //{ typeof(BabylonConcept), new CachedDbSetEFWrapper<BabylonConcept>(BabylonConcepts) },
                //{ typeof(User), new CachedDbSetEFWrapper<User>(User) }
            };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // add your own confguration here            
            //modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        public DbSet<T> ResolveDbSet<T>() where T : Entity
        {
            var fname = typeof(T).FullName ?? throw new ArgumentNullException($"type T has not FullName");
            var dbset = ResolveDbSet(fname) ?? throw new ArgumentNullException($"{fname} DbSet is not registered");
            var output = dbset as DbSet<T>;

            return output ?? throw new ArgumentNullException($"{fname} is not a DbSet");
        }

        public object ResolveDbSet(string typeName)
        {
            var o = DbSets[typeName]();
            return o as object;
        }

        public Task<int> SaveChangesAsync()
        {
            var output = base.SaveChangesAsync();
            
            
            return output;
        }

        public new int SaveChanges()
        {
            return base.SaveChanges();
        }

        public virtual void AddEntity(string type, Entity entity)
        {
            switch (type)
            {                
                //case "Common.Lib.Authentication.User":
                //    UsersWrapper.Add(entity as User);
                //    break;
            }
        }

        public virtual void UpdateEntity(string type, Entity entity)
        {
            switch (type)
            {
                //case "Common.Lib.Authentication.User":
                //    UsersWrapper.Update(entity as User);
                //    break;
            }
        }

        public virtual Entity FindEntiy(string type, Guid id)
        {
            switch (type)
            {
                //case "Common.Lib.Authentication.User":
                //    return UsersWrapper.Find(id);
            }
            return null;            
        }

        public virtual Entity FindEntityFromDb(string type, Guid id)
        {
            return null;
        }

        public void InitWrappersCacheItems()
        {
            try
            {
                foreach (var wrapper in DbSetsWrappers)
                {
                    wrapper.Value.InitCacheItems();
                    Log.WriteLine("init cache for " + wrapper.Key.FullName);
                }

                foreach(var dbset in DbSetsWrappers.Values)
                {
                    foreach (var entity in dbset.GetReader())
                        entity.AssignToParents();
                }
            }
            catch (Exception e1)
            {
                var a = e1;
                Log.WriteLine("failed to init cache");
            }
        }

        public void ReinitWrappersCacheItems(IContextFactory ctx)
        {
            try
            {
                foreach (var wrapper in DbSetsWrappers)
                {
                    wrapper.Value.ReinitCacheItems();
                    Log.WriteLine("reinit cache for " + wrapper.Key.FullName);
                }

                foreach (var dbset in DbSetsWrappers.Values)
                {
                    foreach (var entity in dbset.GetReader())
                    {
                        entity.ContextFactory = ctx;
                        entity.AssignToParents();
                    }
                }
            }
            catch (Exception e1)
            {
                var a = e1;
                Log.WriteLine("failed to reinit cache");
            }
        }


        //public void UnloadToMemoryAfterTransactionFail()
        //{
        //    foreach(var item in DbSetsWrappers)
        //    {
        //        item.Value.UnloadFromMemoryPendingItems();
        //    }
        //}
    }
}
