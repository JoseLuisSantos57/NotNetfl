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

namespace NotNetflix.Controllers
{
    public class FilmesController : Controller
    {
        
        /// <summary>
        /// representa a base de dados do projeto
        /// </summary>
        private readonly NotNetflixDataBase _context;

        private readonly IWebHostEnvironment _caminho;

        public FilmesController(NotNetflixDataBase context, IWebHostEnvironment caminho)
        {
            _context = context;
            _caminho = caminho;

        }

        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filme.ToListAsync());
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
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Path,Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile)
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
            if (filmefile==null)
            {
                //apresentar mensagem a pedir o ficheiro
                ModelState.AddModelError("", "Adicione por favor o ficheiro do filme");

                //devolve o controlo à view
                ViewData["FilmeFK?"] = new SelectList(_context.Filme.OrderBy(c => c.Titulo), "Id", "Titulo");
                return View(filme);
            }
            else //existe ficheiro
            {
                //verificar se ele é válido
                if(filmefile.ContentType == "video/webm")
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









            //26/5/2021-modelstate não é válido sabe-se lá porquê
            //02/06/2021- modelstate já é válido

            if (ModelState.IsValid)
            {
                //try
                //{
                    _context.Add(filme);
                    await _context.SaveChangesAsync();
                    using var stream = new FileStream(nomeFilme, FileMode.Create);
                    await filmefile.CopyToAsync(stream);
                    return RedirectToAction(nameof(Index));
               // }
               /* catch(Exception o)
                {

                }*/
            }
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
