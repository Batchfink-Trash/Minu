using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minu
{
    /// <summary>
    /// Helper to deal with CRUD operations
    /// on a MongoDB database
    /// </summary>
    public class MongoHelper
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        /// <summary>
        /// Create instance of MongoHelper.
        /// </summary>
        /// <param name="database">The database to connect to</param>
        public MongoHelper(string database)
        {
            _client = new MongoClient();
            _database = _client.GetDatabase(database);
        }

        #region insertRecord overloads

        /// <summary>
        /// Insert a BsonDocument
        /// </summary>
        /// <param name="doc">The document to insert</param>
        /// <param name="collectionName">Name of collection to insert to</param>
        public async void insertRecord(BsonDocument doc, string collectionName)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertOneAsync(doc);
        }

        /// <summary>
        /// Insert a BsonDocument
        /// </summary>
        /// <param name="doc">The object to insert</param>
        /// <param name="collectionName">Name of collection to insert to</param>
        public async void insertRecord<T>(T obj, string collectionName)
        {
            BsonDocument Bdoc = toBsonDoc<T>(obj);
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertOneAsync(Bdoc);
        }

        #endregion

        #region findRecords overloads

        /// <summary>
        /// Return records from collection
        /// </summary>
        /// <param name="collectionName">Name of collection to return records from</param>
        /// <returns>List of records</returns>
        public async Task<List<BsonDocument>> findRecords(string collectionName)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var filter = new BsonDocument();
            var result = await collection.Find(filter).ToListAsync();
            return result;
        }

        /// <summary>
        /// Return records from collection based on a filter
        /// </summary>
        /// <param name="collectionName">Name of collection to return records from</param>
        /// <param name="filter">The filter object to apply to results</param>
        /// <returns>List of records</returns>
        public async Task<List<BsonDocument>> findRecords(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.Find(filter).ToListAsync();
            return result;
        }

        /// <summary>
        /// Return records from collection based on sort
        /// </summary>
        /// <param name="collectionName">Name of collection to return records from</param>
        /// <param name="sort">The sort object to apply to results</param>
        /// <returns>The list of records</returns>
        public async Task<List<BsonDocument>> findRecords(string collectionName, SortDefinition<BsonDocument> sort)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var filter = new BsonDocument();
            var result = await collection.Find(filter).Sort(sort).ToListAsync();
            return result;
        }

        /// <summary>
        /// Return records from collection based on a filter and sort
        /// </summary>
        /// <param name="collectionName">Name of collection to return records from</param>
        /// <param name="filter">The filter object to apply to results</param>
        /// <param name="sort">The sort object to apply to results</param>
        /// <returns>the list of records</returns>
        public async Task<List<BsonDocument>> findRecords(string collectionName, FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.Find(filter).Sort(sort).ToListAsync();
            return result;
        }

        public List<BsonDocument> findRecordsSync(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = collection.Find(filter).ToList();
            return result;
        }

        #endregion

        #region updateRecords overloads

        /// <summary>
        /// Update an entry
        /// </summary>
        /// <param name="collectionName">Name of collection to update records in</param>
        /// <param name="filter">The filter object to apply to update</param>
        /// <param name="update">The update object to apply to filtered results</param>
        public async void updateRecords(string collectionName, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.UpdateManyAsync(filter, update);
        }

        #endregion

        #region removeRecords 

        /// <summary>
        /// Remove records based on filter
        /// </summary>
        /// <param name="collectionName">Name of collection to remove records from</param>
        /// <param name="filter">Filter to identify records</param>
        /// <returns>Number of items deleted</returns>
        public async Task<long> removeRecords(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var result = await collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async void dropCollection(string collectionName)
        {
            await _database.DropCollectionAsync(collectionName);
        }

        #endregion

        #region serialization

        /// <summary>
        /// Create a BsonDocument based on an object passed into the method
        /// </summary>
        /// <typeparam name="T">The class type</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns></returns>
        public BsonDocument toBsonDoc<T>(T obj)
        {
            byte[] bson = BsonExtensionMethods.ToBson<T>(obj);
            return BsonSerializer.Deserialize<BsonDocument>(bson);
        }

        /// <summary>
        /// Deserilize from a Bson Document
        /// </summary>
        /// <typeparam name="T">Class to serialize to</typeparam>
        /// <param name="doc">Document to serialize</param>
        /// <returns>Deserialized object</returns>
        public T fromBsonDoc<T>(BsonDocument doc)
        {
            return BsonSerializer.Deserialize<T>(doc);
        }

        #endregion
    }
}
