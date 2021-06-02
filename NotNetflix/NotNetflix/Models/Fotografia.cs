using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class Fotografia
    {
        /// <summary>
        /// Identificão do poster do filme
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Caminho para o poster
        /// </summary>
        [Required]
        public string Path { get; set; }
        
        [ForeignKey(nameof(Movie))]
        public int FilmeFK { get; set; }

        public Filme Movie { get; set; }
    }
}
