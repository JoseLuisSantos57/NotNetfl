using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class GeneroFilme
    {
        /// <summary>
        /// PK para a tabela do relacionamento entre género e filme
        /// </summary>
        [Key]
        public int Id { get; set; }

        //*******************************FilmeId e genero
        /// <summary>
        /// FK para o Filme
        /// </summary>
        [ForeignKey(nameof(Movie))]
        public int FilmeFK { get; set; }
        
        public Filme Movie { get; set; }

        /// <summary>
        /// Fk para o género do filme
        /// </summary>
        [ForeignKey(nameof(Genre))]
        public int GeneroFK { get; set; }

        public Genero Genre { get; set; }
    }
}
