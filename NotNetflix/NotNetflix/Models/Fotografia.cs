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
        /// Caminho para a fotografia do filme
        /// </summary>
        public string Path { get; set; }
        

        //criação da FK que referencia o filme a que pertence a foto
        [ForeignKey(nameof(Movie))]
        [Display(Name = "Filme")]
        public int FilmeFK { get; set; }

        public Filme Movie { get; set; }
    }
}
