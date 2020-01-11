using LibraryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices.BaseService
{
    public abstract class BaseService<T> where T : class, IEntityWithId
    {
        #region Properties

        internal readonly LibraryContext libraryContext;

        #endregion

        #region Constructor
        public BaseService(LibraryContext context)
        {
            libraryContext = context;
        }

        #endregion

        #region Methods
        public virtual void Add(T newEntity)
        {
            libraryContext.Add(newEntity);
            libraryContext.SaveChanges();
        }
        public abstract IEnumerable<T> GetAll();
        public virtual T GetById(int id)
        {
            return GetAll().FirstOrDefault(e => e.Id == id);
        }
        #endregion
    }
}
