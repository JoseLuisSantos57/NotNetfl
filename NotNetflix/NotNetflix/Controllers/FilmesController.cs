using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotNetflix.Data;
using NotNetflix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace NotNetflix.Controllers
{
    [Authorize]
    public class FilmesController : Controller
    {

        /// <summary>
        /// representa a base de dados do projeto
        /// </summary>
        private readonly NotNetflixDataBase _context;

        private readonly IWebHostEnvironment _caminho;
        /// <summary>
        /// Atributo que irá receber todos os dados referentes à pessoa que se autenticou no sistema
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        public FilmesController(NotNetflixDataBase context, IWebHostEnvironment caminho, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _caminho = caminho;
            _userManager = userManager;

        }
        [AllowAnonymous]
        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            //pretendemos vizualizar os dados todos de cada  filme o que é semelhante a executar a subconsulta
            //Select * from filmes, fotografias where filmes.id=fotografias.filmeFK

            var lista_de_fotos = await _context.Filme.Include(f => f.ListasDeFotografias)
                                                                    .OrderByDescending(f => f.Data)
                                                                    .ToListAsync();

            //é possível utilizar uma viewbag(transporte do controller para a view)
            //ViewBag.Fotografias = lista_de_fotos;

            /* var listafotos = new ListarFotosViewModel
             {
                 ListaFotos = lista_de_fotos
             };*/

            return View(lista_de_fotos);
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filme
                .FirstOrDefaultAsync(m => m.Id == id);

            /*var teste = await _context.GeneroFilme
                .FirstOrDefault(m.)
            */

            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public async Task<IActionResult> Create()
        {

            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();

            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Path,Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile, int[] listaGenerosEscolhidos)
        {       DateTime data = filme.Data;
                
                if (DateTime.Compare(data, DateTime.Now) > 0)
                {
                    ModelState.AddModelError("", "A data introduzida não é válida");
                    
                }


            //o model state não era válido porque o path recebido pelo model era nula pois esta é criada no controller
            if (ModelState.IsValid)
            {   
                /* processar o ficheiro
                 *   - existe ficheiro?
                 *     - se não existe, o q fazer?  => gerar uma msg erro, e devolver controlo à View
                 *     - se continuo, é pq ficheiro existe
                 *       - mas, será q é do tipo correto?
                 *         - avaliar se é imagem,
                 *           - se sim: - especificar o seu novo nome
                 *                     - associar ao objeto 'foto', o nome deste ficheiro
                 *                     - especificar a localização                     
                 *                     - guardar ficheiro no disco rígido do servidor
                 *           - se não  => gerar uma msg erro, e devolver controlo à View
                */
                //variável auxiliar
                string nomeFilme = "";

                //verificar se existe ficheiro
                if (filmefile == null)
                {
                    //apresentar mensagem a pedir o ficheiro
                    ModelState.AddModelError("", "Adicione por favor o ficheiro do filme");

                    //devolve o controlo à view
                    ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                    return View(filme);
                }
                else //existe ficheiro
                {
                    //verificar se ele é válido
                    if (filmefile.ContentType == "video/webm")
                    {
                        //definir o nome do ficheiro
                        Guid g;
                        g = Guid.NewGuid();

                        nomeFilme = filme.Titulo + "_" + g.ToString();

                        //determinar a expressão do nome do filme
                        string extensao = Path.GetExtension(filmefile.FileName).ToLower();

                        nomeFilme = nomeFilme + extensao;

                        filme.Path = nomeFilme;

                        string localizacaoFicheiro = _caminho.WebRootPath;
                        nomeFilme = Path.Combine(localizacaoFicheiro, "filme", nomeFilme);
                    }
                    else
                    {
                        //ficheiro não é válido
                        //adicionar mensagem de erro
                        ModelState.AddModelError("", "O formato do ficheiro introduzido não é válido");


                    }
                }


                // processar os géneros do Filme
                if (listaGenerosEscolhidos.Count() != 0)
                {
                    // foram escolhidos géneros
                    var listaGeneros = _context.Genero.Where(g => listaGenerosEscolhidos.Contains(g.Id)).ToList();
                    // adicionar esta lista de géneros ao filme
                    filme.ListasDeGeneros = (ICollection<Genero>)listaGeneros;
                }

                

                try
                {
                    _context.Add(filme);
                    await _context.SaveChangesAsync();

                    //_context.Add(GeneroFilme)
                    using var stream = new FileStream(nomeFilme, FileMode.Create);
                    await filmefile.CopyToAsync(stream);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception o)
                {
                    ModelState.AddModelError("", "Ocorreu um erro com a introdução dos dados do Filme.");
                }
            }

            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filme.FindAsync(id);
            if (filme == null)
            {
                return NotFound();
            }
            return View(filme);


        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Path,Titulo,Descricao,Data,Duracao,Rating")] Filme filme)
        {
            if (id != filme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filme
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filme.FindAsync(id);
            _context.Filme.Remove(filme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filme.Any(e => e.Id == id);
        }
    }
}
