using System.Collections.Generic;
using System.Linq;

namespace RssReader2.Models.Dbs
{
    public interface IRepository<T>
        where T : class
    {
        IQueryable<T> GetAll();

        T GetById(int id);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(int id);
    }
}