using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class UtilizadorFilme
    {
        
        /// <summary>
        /// PK para a tabela do relacionamento entre Utilizador e filme
        /// </summary>
        [Key]
        public int Id { get; set; }

        //*******************************FilmeId e Utilizador
        /// <summary>
        /// FK para a tabela dos filmes
        /// </summary>
        [ForeignKey(nameof(Movie))]
        public int FilmeFK { get; set; }
        public Filme Movie { get; set; }

        /// <summary>
        /// FK para a tabela Utilizador
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UtilizadorFK { get; set; }
        public Utilizador User { get; set; }

    }
}
