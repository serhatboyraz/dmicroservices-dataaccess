using DMicroservices.DataAccess.MongoRepository.Interfaces;
using DMicroservices.DataAccess.MongoRepository.Settings;

namespace DMicroservices.DataAccess.MongoRepository
{
    public static class MongoRepositoryFactory
    {
        public static MongoRepository<T> CreateMongoRepository<T>(int companyNo) where T : class, IMongoRepositoryCollection
        {
            return new MongoRepository<T>(companyNo);
        }

        public static MongoRepository<T> CreateMongoRepository<T>(DatabaseSettings databaseSettings) where T : class, IMongoRepositoryCollection
        {
            return new MongoRepository<T>(databaseSettings.CompanyNo, databaseSettings);
        }

        public static MongoRepository<T> CreateMongoRepository<T>(int companyNo, string collectionName) where T : class, IMongoRepositoryCollection
        {
            return new MongoRepository<T>(companyNo, new DatabaseSettings() { CollectionName = collectionName });
        }
    }
}
