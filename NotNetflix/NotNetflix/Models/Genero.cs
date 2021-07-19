using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class Genero
    {
        public Genero()
        {
            ListaDeFilmes = new HashSet<Filme>();
        }
        /// <summary>
        /// Identificação do genero
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Género do filme
        /// </summary>
        [Required]
        [Display (Name ="Género")]
        public string Genre { get; set; }

        /// <summary>
        /// lista de filmes associados aos géneros
        /// </summary>
        public ICollection<Filme> ListaDeFilmes { get; set; }
    }
}
