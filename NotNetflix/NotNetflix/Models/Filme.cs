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
    /// Caminho da localização do filme
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Titulo do filme
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "O {0} não pode ter mais de {1} carateres." )]
    public string Titulo { get; set; }

    /// <summary>
    /// Resumo do filme
    /// </summary>
    [Required]
    [StringLength(300, ErrorMessage = "O {0} não pode ter mais de {1} carateres.")]
    [Display(Name = "Descrição do filme")]
    public string Descricao { get; set; }

    /// <summary>
    /// Data de lançamento do filme
    /// </summary>
    [Required]
    public DateTime Data { get; set; }

        /// <summary>
        /// Duração do filme
        /// </summary>
        //[Required(ErrorMessage = "A duração do filme é de preenchimento obrigatório")]
        //[RegularExpression("^(1[0-2]|0?[1-9]):([0-5]?[0-9])(●?[AP]M)?$")]
        [Display(Name = "Duração do filme em minutos")]
        public double Duracao { get; set; }


        ////tempo do filme
        //public TimeSpan Tempo { get; set; }

        /// <summary>
        /// Classificação do filme (conforme o imbd) de 0 a 10
        /// </summary>
        [Required]
    [RegularExpression("^[1-9]$|^(10)$")]
    public double Rating { get; set;}


    public ICollection<Utilizador> ListasDeUtilizadores { get; set; }

    public ICollection<Genero> ListasDeGeneros { get; set; }

    public ICollection<Fotografia> ListasDeFotografias { get; set; }
    }
}
