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

        //############################################
        //Adicionar expressões regulares
        //############################################

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
    [StringLength(50)]
    public string Titulo { get; set; }

    /// <summary>
    /// Resumo do filme
    /// </summary>
    
    public string Descricao { get; set; }

    /// <summary>
    /// Data de lançamento do filme
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Duração do filme
    /// </summary>
    
    public int Duracao { get; set; }

    /// <summary>
    /// Classificação do filme (conforme o imbd) de 0 a 10
    /// </summary>
    
    public Double Rating { get; set; }


    //public ICollection<Filme> ListasDeFilmes { get; set; }

    public ICollection<Utilizador> ListasDeUtilizadores { get; set; }

    public ICollection<Genero> ListasDeGeneros { get; set; }

    public ICollection<Fotografia> ListasDeFotografias { get; set; }
    }
}
