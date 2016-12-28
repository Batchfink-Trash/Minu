using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Minu.Models;
using System.Data.SQLite;

namespace Minu.Modules
{
    public class AdminModule : NancyModule
    {
        /// <summary>
        /// This returns the view for the admin section
        /// </summary>
        public AdminModule(SQLiteHelper DBHelper) : base("/admin")
        {
            //Return the admin dashboard
            Get["/"] = perameters =>
            {
                return View["Admin"];
            };

            //Take post requestes from the new post form
            Post["/new"] = perameters =>
            {
                //Take info, bind to model and save to database.
                return View["HomePage"];
            };
        }
    }
}