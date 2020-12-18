using DMicroservices.DataAccess.DynamicQuery.Enum;
using DMicroservices.DataAccess.MongoRepository.Interfaces;
using DMicroservices.DataAccess.MongoRepository.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace DMicroservices.DataAccess.MongoRepository
{
    public class MongoRepository<T> : IDisposable, IMongoRepository<T> where T : class
    {
        //All mongodb databases and collections generated from mongoclient and mongoclient itself is threadsafe. because of that caching is logical choice.
        private static readonly ReaderWriterLockSlim _databaseLocker = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _clientLocker = new ReaderWriterLockSlim();


        private static Dictionary<string, IMongoDatabase> Databases { get; set; } = new Dictionary<string, IMongoDatabase>();
        private static Dictionary<string, MongoClient> MongoClients { get; set; } = new Dictionary<string, MongoClient>();

        public int CompanyNo { get; set; }

        public DatabaseSettings DatabaseSettings { get; set; } = new DatabaseSettings()
        {
            CollectionName = typeof(T).Name,
            ConnectionString = Environment.GetEnvironmentVariable("MONGO_URI"),
            DatabaseName = Environment.GetEnvironmentVariable("MONGO_DB_NAME"),
        };


        private IMongoCollection<T> GetCollection(IDatabaseSettings dbSettings)
        {
            return GetDatabase(dbSettings).GetCollection<T>(typeof(T).Name);
        }

        private IMongoDatabase GetDatabase(IDatabaseSettings dbSettings)
        {
            IMongoDatabase database = null;
            _databaseLocker.EnterReadLock();
            try
            {
                if (Databases.ContainsKey(dbSettings.DatabaseName))
                {
                    database = Databases[dbSettings.DatabaseName];
                }
                else
                {

                    _databaseLocker.ExitReadLock();
                    _databaseLocker.EnterWriteLock();
                    try
                    {
                        if (!MongoClients.ContainsKey(dbSettings.ConnectionString))
                        {
                            database = GetClient(dbSettings.ConnectionString).GetDatabase(dbSettings.DatabaseName);
                            Databases.Add(dbSettings.ConnectionString, database);
                        }
                        database = Databases[dbSettings.ConnectionString];
                    }
                    finally
                    {
                        _databaseLocker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                if (_databaseLocker.IsReadLockHeld)
                    _databaseLocker.ExitReadLock();
            }
            return database;
        }

        private MongoClient GetClient(string connectionString)
        {
            MongoClient client = null;
            _clientLocker.EnterReadLock();
            try
            {
                if (MongoClients.ContainsKey(connectionString))
                {
                    client = MongoClients[connectionString];
                }
                else
                {
                    _clientLocker.ExitReadLock();
                    _clientLocker.EnterWriteLock();
                    try
                    {
                        if (!MongoClients.ContainsKey(connectionString))
                        {
                            client = new MongoClient(connectionString);
                            MongoClients.Add(connectionString, client);
                        }
                        client = MongoClients[connectionString];
                    }
                    finally
                    {
                        _clientLocker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                if (_clientLocker.IsReadLockHeld)
                    _clientLocker.ExitReadLock();
            }
            return client;
        }

        public IMongoCollection<T> CurrentCollection { get; set; }

        private IMongoDatabase database;

        public IMongoDatabase Database
        {
            get
            {
                if (database == null)
                    database = GetDatabase(DatabaseSettings);
                return database;
            }
            set { database = value; }
        }

        public MongoRepository()
        {
            Database = GetDatabase(DatabaseSettings);
            if (Database.GetCollection<T>(typeof(T).Name) == null)
            {
                Database.CreateCollection(typeof(T).Name);
            }

            CurrentCollection = GetCollection(DatabaseSettings);
        }

        public MongoRepository(DatabaseSettings dbSettings)
        {
            DatabaseSettings = dbSettings;

            Database = GetDatabase(DatabaseSettings);
            if (Database.GetCollection<T>(typeof(T).Name) == null)
            {
                Database.CreateCollection(typeof(T).Name);
            }

            CurrentCollection = GetCollection(DatabaseSettings);
        }

        public MongoRepository(int companyNo, IDatabaseSettings dbSettings = null)
        {
            CompanyNo = companyNo;
            CurrentCollection = GetCollection(dbSettings);
        }

        public void Add(T entity)
        {
            CurrentCollection.InsertOne(entity);
        }

        public void Delete(Expression<Func<T, bool>> predicate, bool forceDelete = false)
        {
            CurrentCollection.DeleteOne(predicate);
        }

        public void Delete<TField>(FieldDefinition<T, TField> field, TField date)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Lte(field, date);
            CurrentCollection.DeleteMany(filter);
        }

        public void Update(Expression<Func<T, bool>> predicate, T entity)
        {
            CurrentCollection.ReplaceOneAsync(predicate, entity);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return (int)CurrentCollection.Find(predicate).CountDocuments();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return CurrentCollection.Find(predicate).ToEnumerable().AsQueryable();
        }

        /// <summary>
        /// Şarta göre tek veri getirir
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return CurrentCollection.Find(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Aynı kayıt eklememek için objeyi kontrol ederek true veya false dönderir.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return CurrentCollection.Find(predicate).FirstOrDefault() != null;
        }

        IQueryable<T> IMongoRepository<T>.GetAll()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public IQueryable<dynamic> SelectList(Expression<Func<T, bool>> where, Expression<Func<T, dynamic>> select)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetDataPart(Expression<Func<T, bool>> where, Expression<Func<T, dynamic>> sort, SortTypeEnum sortType, int skipCount, int takeCount)
        {
            throw new NotImplementedException();
        }

        public List<T> SendSql(string sqlQuery)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity, bool forceDelete = false)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<T> Query()
        {
            return CurrentCollection.Find(FilterDefinition<T>.Empty).ToList();
        }
    }
}
