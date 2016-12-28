using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Minu.Models
{
    public class Author
    {
        /// <summary>
        /// The Author Model defines data associated with an author of an article.
        /// It needs a unique ID, a name and bio for the author.
        /// The name and bio will be displayed on the author page and on articles written by the author
        /// </summary>
        [Required]
        public int id { get; set; }

        [Required, StringLength(30)]
        public string FirstName { get; set; }

        [Required, StringLength(30)]
        public string Surname { get; set; }

        [Required, StringLength(500)]
        public string Bio { get; set; }
    }
}