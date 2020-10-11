using System;
using System.Collections.Generic;

namespace ESportRaise.BackEnd.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        T Get(int id);

        IEnumerable<T> Find(Predicate<T> predicate);

        void Create(T item);

        void Update(T item);

        void Delete(int id);
    }
}
