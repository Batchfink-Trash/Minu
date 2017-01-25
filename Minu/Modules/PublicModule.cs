using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;
using Minu.Models;

namespace Minu.Modules
{
    public class PublicModule : NancyModule
    {
        /// <summary>
        /// This module returns the public views.  These are pages that visitors can see, such as blog posts and the homepage.
        /// </summary>
        public PublicModule(SQLiteHelper DBHelper)
        {
            //Return the homepage on navigation to root
            Get["/"] = perameters =>
            {
                //Prepare Homepage model with info from database
                //TODO

                //List<BlogPost> posts = DBHelper.BindToClass<BlogPost>("posts");

                /*BlogPost newPost = new BlogPost();
                newPost.id = 3;
                newPost.AuthorID = 1;
                newPost.Date = DateTime.Today;
                newPost.Title = "New and shiny";
                newPost.Content = "WOw lots and lots of shiny new content to be pushed to the database";

                DBHelper.InsertRecord<BlogPost>(newPost, "posts");

                DBHelper.CreateTable<Author2>("author2");

                Author2 testAuth = new Author2();
                testAuth.id = 1;
                testAuth.FirstName = "wow";
                testAuth.Surname = "wee";
                testAuth.Bio = "owee look at me lol";
                testAuth.testBool = false;

                DBHelper.InsertRecord<Author2>(testAuth, "author2");*/

                List<BlogPost> posts = DBHelper.BindRecordToClass<BlogPost>("posts");

                //bind Author to IDs

                HomePage homePage = new HomePage();

                homePage.BlogName = "Super cool blog";
                homePage.SubTitle = "real cool";
                homePage.RecentArticles = posts;

                return View["HomePage", homePage];
            };

            //Return blog posts by id
            Get["/{id}"] = perameters =>
            {
                //Get blog post with ID of 'id' and return it in a View.
                //TODO
                BlogPost returnPost = DBHelper.BindRecordToClass<BlogPost>("posts", "where id like '" + perameters.id[0] + "'");

                return View["BlogPost", returnPost];
            };
        }
    }
}
 