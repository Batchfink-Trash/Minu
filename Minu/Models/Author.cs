using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minu.Models
{
    public class Author
    {
        /// <summary>
        /// The Author Model defines data associated with an author of an article.
        /// It needs a unique ID, a name and bio for the author.
        /// The name and bio will be displayed on the author page and on articles written by the author
        /// </summary>
        public int id { get; set; }
        
        public string FirstName { get; set; }
        
        public string Surname { get; set; }
        
        public string Bio { get; set; }
    }
}