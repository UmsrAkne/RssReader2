using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    public class WebSiteGroupRepository : IRepository<WebSiteGroup>
    {
        private readonly DatabaseContext context;
        private readonly DbSet<WebSiteGroup> dbSet;

        public WebSiteGroupRepository(DatabaseContext context)
        {
            this.context = context;
            dbSet = this.context.Set<WebSiteGroup>();
        }

        public IQueryable<WebSiteGroup> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public WebSiteGroup GetById(int id)
        {
            return dbSet.Find(id);
        }

        public void Add(WebSiteGroup entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<WebSiteGroup> entities)
        {
            dbSet.AddRange(entities);
            context.SaveChanges();
        }

        public void Update(WebSiteGroup entity)
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