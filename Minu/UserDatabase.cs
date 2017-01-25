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

        public UserDatabase(SQLiteHelper help)
        {
            DBHelper = help;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            string whereStr = "where id='" + identifier.ToString().ToUpper() + "'";

            var userRecord = new UserModel();
            try
            {
                userRecord = DBHelper.BindRecordToClass<UserModel>("users", whereStr)[0];
            }
            catch (Exception)
            {
                userRecord = null;
            }

            return userRecord == null
                       ? null
                       : userRecord;
        }

        public static Guid? ValidateUser(string username, string password, SQLiteHelper DBHelper)
        {
            //ENCRYPT PASSWORDS FOR THE LOVE OF GOD
            //Construct SQL statement sanitizing inputs  I SHOULD HAVE USED PERAMETERS BUT 300+ LINES LATER I DON'T FEEL LIKE CHANGING IT RIGHT NOW
            string whereStr = "where UserName='" + username.Replace("'", "''") + "' and password='" + password.Replace("'", "''") + "'";
            var userRecord = new UserModel();
            try
            {
                userRecord = DBHelper.BindRecordToClass<UserModel>("users", whereStr)[0];
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