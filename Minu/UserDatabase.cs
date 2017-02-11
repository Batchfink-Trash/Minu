namespace Minu
{
    using Minu.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public class UserDatabase : IUserMapper
    {

        private List<UserModel> users = new List<UserModel>();

        public MongoHelper DBHelper;

        public UserDatabase(MongoHelper help)
        {
            DBHelper = help;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            string whereStr = "where id='" + identifier.ToString().ToUpper() + "'";

            var userRecord = new UserModel();
            try
            {
                // Get user record from database
                var filter = Builders<BsonDocument>.Filter.Eq("id", identifier.ToString().ToUpper());
                List<BsonDocument> queryResult = DBHelper.findRecordsSync("users", filter);
                // Get first user returned
                userRecord = DBHelper.fromBsonDoc<UserModel>(queryResult[0]);
            }
            catch (Exception)
            {
                userRecord = null;
            }

            return userRecord == null
                       ? null
                       : userRecord;
        }

        public static Guid? ValidateUser(string username, string password, MongoHelper DBHelper)
        {
            // ENCRYPT PASSWORDS FOR THE LOVE OF GOD
            // Construct SQL statement sanitizing inputs  I SHOULD HAVE USED PERAMETERS BUT 300+ LINES LATER I DON'T FEEL LIKE CHANGING IT RIGHT NOW
            // HEY I CHANGED DATABASE PROVIDER LOOK AT ME HOW CLEVER AM I!  SECURITY INCOMING
            var userRecord = new UserModel();
            try
            {
                // Create filter
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("UserName", username) & builder.Eq("Password", password);
                List<BsonDocument> queryResult = DBHelper.findRecordsSync("users", filter);
                // Get first user returned
                DBHelper.fromBsonDoc<UserModel>(queryResult[0]);
            }
            catch (Exception)
            {
                userRecord = null;
            }

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.id;
        }

    }
}