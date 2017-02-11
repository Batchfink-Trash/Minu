using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using Minu.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Minu.Modules
{
    public class PublicModule : NancyModule
    {
        /// <summary>
        /// This module returns the public views.  These are pages that visitors can see, such as blog posts and the homepage.
        /// </summary>
        public PublicModule(MongoHelper DBHelper)
        {
            //Return the homepage on navigation to root
            Get["/", true] = async (perameters, ct) =>
            {
                //Prepare Homepage model with info from database
                //TODO

                List<BsonDocument> posts = await DBHelper.findRecords("posts");
                List<BlogPost> serializedPosts = new List<BlogPost>();

                foreach(var i in posts)
                {
                    serializedPosts.Add(DBHelper.fromBsonDoc<BlogPost>(i));
                }

                // Bind Author to IDs

                HomePage homePage = new HomePage();

                homePage.BlogName = "Super cool blog";
                homePage.SubTitle = "real cool";
                homePage.RecentArticles = serializedPosts;

                return View["HomePage", homePage];
            };

            //Return blog posts by id
            Get["/{id}", true] = async (perameters, ct) =>
            {
                // Create filter that matches the perameter id.  Query database
                var filter = Builders<BsonDocument>.Filter.Eq("id", perameters.id[0]);
                List<BsonDocument> queryResult = await DBHelper.findRecords("posts");

                // Select the first result (there should only be one anyway)
                BlogPost returnPost = DBHelper.fromBsonDoc<BlogPost>(queryResult[0]);

                return View["BlogPost", returnPost];
            };
        }
    }
}
 