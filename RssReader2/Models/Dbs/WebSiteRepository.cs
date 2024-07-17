using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    public class WebSiteRepository : IRepository<WebSite>
    {
        private readonly DatabaseContext context;
        private readonly DbSet<WebSite> dbSet;

        public WebSiteRepository(DatabaseContext context)
        {
            this.context = context;
            dbSet = this.context.Set<WebSite>();
        }

        public IEnumerable<WebSite> GetAll()
        {
            return dbSet.ToList();
        }

        public WebSite GetById(int id)
        {
            return dbSet.Find(id);
        }

        public void Add(WebSite entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<WebSite> entities)
        {
            dbSet.AddRange(entities);
            context.SaveChanges();
        }

        public void Update(WebSite entity)
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