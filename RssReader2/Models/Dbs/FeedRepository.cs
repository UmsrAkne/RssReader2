using System.Collections.Generic;
using System.Linq;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedRepository : IRepository<Feed>
    {
        private readonly DatabaseContext context;
        private readonly DbSet<Feed> dbSet;

        public FeedRepository(DatabaseContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Feed>();
        }

        public IQueryable<Feed> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public Feed GetById(int id)
        {
            return dbSet.Find(id);
        }

        public void Add(Feed entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<Feed> entities)
        {
            using var transaction = context.Database.BeginTransaction();
            context.BulkInsert(entities.ToList());
            transaction.Commit();
        }

        public void Update(Feed entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = dbSet.Find(id);

            if (entity != null)
            {
                dbSet.Remove(entity);
            }

            context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<Feed> entities)
        {
            dbSet.UpdateRange(entities);
            context.SaveChanges();
        }
    }
}