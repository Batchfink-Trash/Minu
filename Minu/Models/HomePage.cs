using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minu.Models
{
    public class HomePage
    {
        /// <summary>
        /// A model for the homepage.  
        /// This defines the blog's title, subtitle and gives a list of recent articles.
        /// </summary>
        
        public string BlogName { get; set; }
        
        public string SubTitle { get; set; }
        
        public List<BlogPost> RecentArticles { get; set; }

        public HomePage()
        {
            RecentArticles = new List<BlogPost>();
        }
    }
}