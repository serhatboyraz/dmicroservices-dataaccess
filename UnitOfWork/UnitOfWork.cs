using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using DMicroservices.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace DMicroservices.DataAccess.UnitOfWork
{
    public class UnitOfWork<T> : IDisposable, IUnitOfWork
        where T : DbContext
    {
        #region Members

        private DbContext dbContext;
        private bool disposed = false;

        /// <summary>
        /// İşlemlerde hata oluşusa bu liste doldurulur.
        /// </summary>
        public readonly List<string> ErrorMessageList = new List<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Açılan veri bağlantısı.
        /// </summary>
        private DbContext DbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = (DbContext)Activator.CreateInstance(typeof(T));
                }
                return dbContext;
            }
            set { dbContext = value; }
        }

        #endregion

        #region Constructre

        /// <summary>
        /// UnitOfWork başlangıcı 
        /// </summary>
        public UnitOfWork()
        {

        }

        #endregion

        #region IUnitOfWork Members

        /// <summary>
        /// Repository instance'ı başlatmak için kullanılır.
        /// </summary>
        /// <typeparam name="T">Veri Tabanı Tür Nesnesi</typeparam>
        /// <returns>Tür nesnesi ile ilgili Repository</returns>
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(DbContext);
        }

        /// <summary>
        /// Değişiklikleri kaydet.
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            try
            {
                int result = 0;
                using (TransactionScope tScope = new TransactionScope())
                {
                    result = DbContext.SaveChanges();
                    tScope.Complete();
                }
                return result;
            }
            catch (ValidationException ex)
            {
                string errorString = ex.Message;
                return -1;
            }
            catch (DbUpdateException ex)
            {
                string errorString = ex.Message;
                if (ex.InnerException != null)
                {
                    errorString += ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorString += ex.InnerException.InnerException.Message;
                        ErrorMessageList.Add(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        ErrorMessageList.Add(ex.InnerException.Message);
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                ErrorMessageList.Add(ex.Message);
                return -1;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }
        #endregion
    }
}
