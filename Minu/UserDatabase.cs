namespace Minu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using Minu.Models;

    public class UserDatabase : IUserMapper
    {

        private List<UserModel> users = new List<UserModel>();

        public SQLiteHelper DBHelper;

        public UserDatabase()
        {
            
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            string whereStr = "where id='" + identifier.ToString() + "'";
            var userRecord = DBHelper.BindRecordToClass<UserModel>("users", whereStr)[0];

            return userRecord == null
                       ? null
                       : userRecord;
        }

        public static Guid? ValidateUser(string username, string password, SQLiteHelper DBHelper)
        {
            //ENCRYPT PASSWORDS FOR THE LOVE OF GOD
            string whereStr = "where UserName='" + username + "' and password='" + password + "'";
            var userRecord = DBHelper.BindRecordToClass<UserModel>("users", whereStr)[0];

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.id;
        }

    }
}