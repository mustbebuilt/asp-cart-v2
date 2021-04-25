using System;
using System.ComponentModel.DataAnnotations;

namespace MyFilmMVCV1.Models
{
    public class CartLine
    {
        [Key]
        public int CartLineID { get; set; }

        public string UserID { get; set; }

        public int FilmID { get; set; }

        public int OrderQuantity { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
