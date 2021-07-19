﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class Utilizador
    {
        public Utilizador()
        {
                ListasDeFilmes = new HashSet<Filme>();
        }
        /// <summary>
        /// Identificador do utilizador
        /// </summary>
        [Key]
        public int Id { get;set; }

        /// <summary>
        /// Nome do utilizador
        /// </summary>
        [Required(ErrorMessage = "O Nome é de preenchimento obrigatório")]
        [StringLength(60, ErrorMessage = "O {0} não pode ter mais de {1} caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Email do utilizador
        /// </summary>
        
        [RegularExpression("((((aluno)|(es((tt)|(ta)|(gt))))[0-9]{4,5})|([a-z]+(.[a-z]+)*))",
                           ErrorMessage = "Só são aceites emails do IPT.")]
        public string Email { get; set; }

        /// <summary>
        /// Número de telemóvel do utilizador
        /// </summary>
        [StringLength(14, MinimumLength = 9, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres.")]
        [RegularExpression("(00)?([0-9]{2,3})?[1-9][0-9]{8}", 
                           ErrorMessage = "Escreva um nº Telemóvel com 9 algarismos. Pode acrescentar o indicativo.")]
        [Display(Name = "Telemóvel")]
        public string N_telemovel { get; set; }

        /// <summary>
        /// Código postal do utilizador
        /// </summary>
        [Display(Name = "Código Postal")]
        [RegularExpression("[0-9]{4}-[0-9]{3}", ErrorMessage = "Introduza um código postal válido")]
        public string CodPostal { get; set; }

        /// <summary>
        /// Morada do utilizador
        /// </summary>
        [Required]
        public string Morada { get; set; }

        /// <summary>
        /// Data de nascimento
        /// </summary>
        [Display(Name = "Data de Nascimento")]
        public DateTime dataNascimento { get; set; }

        /// <summary>
        /// Lista de filmes do utilizador
        /// </summary>
        public ICollection<Filme> ListasDeFilmes { get; set; }

        /// <summary>
        /// Chave de ligação entre a Autenticação e os Criadores 
        /// Consegue-se, por exemplo, filtrar os dados dos criadores quando se autenticam
        /// </summary>
        public string UserNameId { get; set; }

    }
}
