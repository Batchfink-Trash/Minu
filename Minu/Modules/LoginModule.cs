using Minu.Models;
using MongoDB.Bson;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using System;
using System.Dynamic;

namespace Minu.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule(MongoHelper DBHelper)
        {
            Get["/login"] = args =>
            {
                dynamic model = new ExpandoObject();
                model.Errored = this.Request.Query.error.HasValue;

                /*UserModel newUsr = new UserModel();
                newUsr.id = new Guid();
                newUsr.UserName = "admin";
                newUsr.Password = "1234";

                DBHelper.insertRecord<UserModel>(newUsr, "users");*/

                return View["login", model];
            };

            Post["/login"] = args => {
                var userGuid = UserDatabase.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password, DBHelper);

                if (userGuid == null)
                {
                    return this.Context.GetRedirect("~/login?error=true&username=" + (string)this.Request.Form.Username);
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };

            Get["/logout"] = args =>
            {
                return this.LogoutAndRedirect("~/");
            };
        }
    }
}