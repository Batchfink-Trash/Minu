using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Minu.Models;
using Nancy.ModelBinding;
using System.Data.SQLite;
using Nancy.Security;

namespace Minu.Modules
{
    public class AdminModule : NancyModule
    {
        /// <summary>
        /// This returns the view for the admin section
        /// </summary>
        public AdminModule(SQLiteHelper DBHelper) : base("/admin")
        {
            this.RequiresAuthentication();
            //Return the admin dashboard
            Get["/"] = perameters =>
            {
                return View["Admin", new BlogPost()];
            };

            //Take post requestes from the new post form
            Post["/new"] = perameters =>
            {
                //Take info, bind to model and save to database.
                var newPost = this.Bind<BlogPost>("id", "AuthorID", "Date");

                //Set id by adding 1 to the number of posts already in the table
                newPost.id = Convert.ToInt64(DBHelper.SendScalarQuery("select count(*) from posts")) + 1;

                //Set AuthorID from session cookies or something.
                newPost.AuthorID = 1;
                
                //Set Date
                newPost.Date = DateTime.Today;

                //Insert new post into the table
                DBHelper.InsertRecord<BlogPost>(newPost, "posts");
                return Response.AsRedirect("/");
            };
        }
    }
}