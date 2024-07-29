using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    public class NgWordRepository : IRepository<NgWord>
    {
        private readonly DatabaseContext context;
        private readonly DbSet<NgWord> dbSet;

        public NgWordRepository(DatabaseContext context)
        {
            this.context = context;
            dbSet = this.context.Set<NgWord>();
        }

        public IQueryable<NgWord> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public NgWord GetById(int id)
        {
            return dbSet.Find(id);
        }

        public void Add(NgWord entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<NgWord> entities)
        {
            dbSet.AddRange(entities);
            context.SaveChanges();
        }

        public void Update(NgWord entity)
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

        public void UpdateRange(IEnumerable<NgWord> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}