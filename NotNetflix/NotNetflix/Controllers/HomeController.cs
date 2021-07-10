using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotNetflix.Data;
using NotNetflix.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NotNetflix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NotNetflixDataBase _context;
        public HomeController(ILogger<HomeController> logger, NotNetflixDataBase context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var lista = await _context.Filme.Include(f => f.ListasDeFotografias).ToListAsync();
            return View(lista);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> FilmePag(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filme.Include(f => f.ListasDeFotografias).Include(f => f.ListasDeGeneros)
                .FirstOrDefaultAsync(m => m.Id == id);



            if (filme == null)
            {
                return NotFound();
            }
            return View(filme);
        }
        public async Task<IActionResult> AllFilmes(int a)
        {
            //fazer switch case
            //caso o id enviado seja 1 ordena os filmes por data de lançamento
            if (a == 1)
            {
                var filmes = await _context.Filme.OrderByDescending(i => i.Data).Include(l => l.ListasDeFotografias).ToListAsync();
                return View(filmes);
            }
            else if (a == 2 )//caso o id enviado seja 2 ordena os filmes por rating
            {
                var filmes = await _context.Filme.OrderByDescending(i => i.Rating).Include(l => l.ListasDeFotografias).ToListAsync();
                return View(filmes);
            }
            else if ( a == 3 )//caso o id enviado seja 3 ordena os filmes por duração
            {
                var filmes = await _context.Filme.OrderBy(i => i.Duracao).Include(l => l.ListasDeFotografias).ToListAsync();
                return View(filmes);
            }
            else //caso o id enviado seja diferente ordena os filmes por ordem de adição ao site
            {
                var filmes = await _context.Filme.OrderBy(i => i.Id).Include(l => l.ListasDeFotografias).ToListAsync();
                return View(filmes);
            }

            
            
        }
        public async Task<IActionResult> SearchBar(string s)
        {


            if (!String.IsNullOrEmpty(s))
            {
                var pesquisa = await _context.Filme.Include(l => l.ListasDeFotografias).Where(n => n.Titulo.Contains(s) || n.Titulo.StartsWith(s) || n.Titulo.EndsWith(s)).ToListAsync();
                return View(pesquisa);
            }
            else
            {
                return View();
            }
        }
    }
}
