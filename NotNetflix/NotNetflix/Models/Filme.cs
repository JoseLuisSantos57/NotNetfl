using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Models
{
    public class Filme
    {
        public Filme(){
            ListasDeUtilizadores = new HashSet<Utilizador>();
            ListasDeGeneros = new HashSet<Genero>();
            ListasDeFotografias = new HashSet<Fotografia>();
        }

    /// <summary>
    /// Identificador de filme
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// url do filme
    /// </summary>

    public string Path { get; set; }

    /// <summary>
    /// Titulo do filme
    /// </summary>
    [StringLength(50, ErrorMessage = "O {0} não pode ter mais de {1} carateres." )]

    public string Titulo { get; set; }

    /// <summary>
    /// Descrição do filme
    /// </summary>
    
    [StringLength(300, ErrorMessage = "O {0} não pode ter mais de {1} carateres.")]
    [Display(Name = "Descrição do filme")]
    public string Descricao { get; set; }

    /// <summary>
    /// Data de lançamento do filme
    /// </summary>

    public DateTime Data { get; set; }

        /// <summary>
        /// Duração do filme
        /// </summary>
        [Range(1, 300, ErrorMessage = "A duração do filme não pode ser menor que 1 minuto ou maior que 300 minutos")]
        [Display(Name = "Duração do filme em minutos")]
        public double Duracao { get; set; }


        ////tempo do filme
        //public TimeSpan Tempo { get; set; }

        /// <summary>
        /// Classificação do filme  de 0 a 10
        /// </summary>

        [Range(1,10,ErrorMessage = "O valor do rating tem que estar entre 1 e 10")]
        public double Rating { get; set;}

        /// <summary>
        /// lista de utilizadores do filme
        /// </summary>
    public ICollection<Utilizador> ListasDeUtilizadores { get; set; }

        /// <summary>
        /// lista de géneros do filme
        /// </summary>
    public ICollection<Genero> ListasDeGeneros { get; set; }

        /// <summary>
        /// lista de fotografias do filme
        /// </summary>
    public ICollection<Fotografia> ListasDeFotografias { get; set; }
    }
}
