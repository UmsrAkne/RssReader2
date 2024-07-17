using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    public class FeedRepository : IRepository<Feed>
    {
        private readonly DatabaseContext context;
        private readonly DbSet<Feed> dbSet;

        public FeedRepository(DatabaseContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Feed>();
        }

        public IEnumerable<Feed> GetAll()
        {
            return dbSet.ToList();
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
            dbSet.AddRange(entities);
            context.SaveChanges();
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
    }
}