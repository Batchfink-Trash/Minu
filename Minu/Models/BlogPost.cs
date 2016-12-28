using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Minu.Models
{
    public class BlogPost
    {
        /// <summary>
        /// The BlogPost model defines a blog post on the website.
        /// It requires a unique ID, an author, title and content.
        /// Tags will follow 
        /// </summary>
        [Required]
        public Int64 id { get; set; }

        //[Required]
        //public Author Author_ { get; set; }

        [Required]
        public Int64 AuthorID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string SubTitle { get; set; }

        [Required]
        public string Content { get; set; }

        public string Url = "/";

        //[Required]
        //public List<string> Tags { get; set; }

        public BlogPost()
        {
            Url += id;
            
            //Tags = new List<string>();
        }
    }
}

