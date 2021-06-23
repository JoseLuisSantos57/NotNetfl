using System;
using System.Collections;
using System.Collections.Generic;

namespace NotNetflix.Models
{
    /// <summary>
    /// classe usada para transportar os dados necess�rios
    /// � correta utiliza��o das fotos dos filmes � respetiva interface
    /// </summary>
    public class ListarFotosViewModel
    {
        /// <summary>
        /// lista de fotografias dos filmes
        /// </summary>
        public ICollection<Fotografia> ListaFotos { get; set; }

        //public ICollection<int> MyProperty { get; set; }
    }
    
    /// <summary>
    /// classe utilizada para transportar os dados necess�rios 
    /// � correta utiliza��o dos g�neros dos filmes � respetiva interface
    /// </summary>
    public class ListarGenerosModel
    {
        /// <summary>
        /// lsita de generos do filme
        /// </summary>
        public ICollection<Genero> ListaGeneros { get; set; }
    }
    
    
    
    
    
    
    
    
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
