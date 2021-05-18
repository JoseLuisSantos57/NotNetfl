using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class Genero
    {
        /// <summary>
        /// Identificação do genero
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Género do filme
        /// </summary>
        [Required]
        public string Genre { get; set; }
    }
}
