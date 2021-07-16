using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, NotNetflixDataBase context, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            string[] headers= { "RECENTES","A ESCOLHA DOS CRÍTICOS", "POR DURAÇÃO" };
            ViewBag.Headers = headers;

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
        [Authorize(Roles = "Gestor,Utilizador")]
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
        public async Task<IActionResult> AllFilmes(int? a, string? s)
        {
            //fazer switch case
            //caso o id enviado seja 1 ordena os filmes por data de lançamento
            //enviar viewbag 
            var exemplo = _context.Filme.Include(l => l.ListasDeFotografias);
            switch (a)
            {
                case 1:
                var filmesRec = await _context.Filme.OrderByDescending(i => i.Data).Include(l => l.ListasDeFotografias).ToListAsync();
                ViewBag.Titulo = "FILMES MAIS RECENTES";
                return View(filmesRec);
                   

                case 2:
                var filmesRat = await _context.Filme.OrderByDescending(i => i.Rating).Include(l => l.ListasDeFotografias).ToListAsync();
                ViewBag.Titulo = "ESCOLHA DOS CRÍTICOS";
                return View(filmesRat);
                    

                case 3:
                var filmesDur = await _context.Filme.OrderBy(i => i.Duracao).Include(l => l.ListasDeFotografias).ToListAsync();
                ViewBag.Titulo = "FILMES MAIS LONGOS";
                return View(filmesDur);
                

                case 4:
                if (!String.IsNullOrEmpty(s))
                    {
                       
                        var pesquisa = await _context.Filme.Include(l => l.ListasDeFotografias).Where(n => n.Titulo.Contains(s)).ToListAsync();
                        if (pesquisa.Count()==0)
                        {
                            ViewBag.Titulo = "RESULTADO NÃO ENCONTRADO";
                            return View(exemplo);
                        }
                        ViewBag.Titulo = "RESULTADO DA PESQUISA";
                        return View(pesquisa);
                    }
                    else
                    {
                        return View();
                    }
                    

                default:
                var filmes = await _context.Filme.OrderBy(i => i.Id).Include(l => l.ListasDeFotografias).ToListAsync();
                ViewBag.Titulo = "FILMES DISPONÍVEIS";
                return View(filmes);
            }
            
      
            
            
        }
        
    }
}
