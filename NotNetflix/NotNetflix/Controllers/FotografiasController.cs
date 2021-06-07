using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotNetflix.Data;
using NotNetflix.Models;

namespace NotNetflix.Controllers
{
    public class FotografiasController : Controller
    {
        /// <summary>
        /// Representa a base de dados do projeto
        /// </summary>
        private readonly NotNetflixDataBase _context;

        /// <summary>
        /// contém os atributos da app web no servidor
        /// </summary>
        private readonly IWebHostEnvironment _caminho;



        public FotografiasController(NotNetflixDataBase context, IWebHostEnvironment caminho)
        {
           
            _context = context;
            _caminho = caminho;
        }

        // GET: Fotografias
        public async Task<IActionResult> Index()
        {
            /*criação de uma variável que vai conter um conjunto de dados 
             f => f.Movie  <---- expressão 'lambda'
         *  ^ ^  ^
         *  | |  |
         *  | |  representa cada um dos registos individuais da tabela das Fotografias
         *  | |  e associa a cada fotografia o seu respetivo filme
         *  | |  equivalente à parte WHERE do comando SQL
         *  | |
         *  | um símbolo que separa os ramos da expressão
         *  |
         *  representa todos registos das fotografias
             
             
             */
            var notNetflixDataBase = _context.Fotografia.Include(f => f.Movie);
            //invoca a view, entragando-lhe a lista de registos
            return View(await notNetflixDataBase.ToListAsync());
        }

        // GET: Fotografias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var fotografia = await _context.Fotografia
                                    .Include(f => f.Movie)
                                     .FirstOrDefaultAsync(m => m.Id == id);
            if (fotografia == null)
            {
                return RedirectToAction("Index");
            }

            return View(fotografia);
        }

        // GET: Fotografias/Create
        public IActionResult Create()
        {
            ViewData["FilmeFK"] = new SelectList(_context.Filme.OrderBy(f => f.Titulo), "Id", "Titulo");
            return View();
        }






        // POST: Fotografias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmeFK")] Fotografias foto, IFormFile fotofilme)
        {
            //avaliar se o gestor escolheu um filme para associar à fotografia
            if (foto.FilmeFK < 0)
            {
                //não foi selecionado um filme válido
                ModelState.AddModelError("","Não foi selecionado um filme válido");

            }


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

            // var auxiliar
            string nomeImagem = "";
            if(fotofilme == null)
            {
                ModelState.AddModelError("", "Adicone por favor a fotografia do filme");
                ViewData["FilmeFK"] = new SelectList(_context.Filme, "Id", "Descricao", foto.FilmeFK);
                return View(foto);
            }
            else
            {
                if(fotofilme.ContentType == "image/jpeg" || fotofilme.ContentType == "image/jpg")
                {
                    Guid g;
                    g = Guid.NewGuid();
                    nomeImagem = foto.FilmeFK + "_" + g.ToString();
                    string extensao = Path.GetExtension(fotofilme.FileName).ToLower();
                    nomeImagem = nomeImagem + extensao;

                    foto.Path = nomeImagem;
                    string localizacao = _caminho.WebRootPath;
                    nomeImagem = Path.Combine(localizacao, "fotos", nomeImagem);
                }
                else
                {
                    ModelState.AddModelError("", "O ficheiro da fotografia não é válido");
                    ViewData["FilmeFK"] = new SelectList(_context.Filme, "Id", "Descricao", foto.FilmeFK);
                    return View(foto);
                }

            }

            if (ModelState.IsValid)
            {
               // try
               // {

                    //adiciona a fotografia à base de dados 
                    _context.Add(foto);
                    await _context.SaveChangesAsync();

                    using var stream = new FileStream(nomeImagem, FileMode.Create);
                    await fotofilme.CopyToAsync(stream);
                    return RedirectToAction(nameof(Index));
             // }
                /*catch(Exception ex)
                {
                    ModelState.AddModelError("", "Ocorreu um erro...");
                }*/
            }
            ViewData["FilmeFK"] = new SelectList(_context.Filme, "Id", "Descricao", foto.FilmeFK);
            return View(foto);
        }

        // GET: Fotografias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
           
            if (id == null)
            {
                return NotFound();
            }

            var fotografia = await _context.Fotografia.FindAsync(id);
            if (fotografia == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("NumFotoEmEdicao", fotografia.Id);
            return View(fotografia);
        }

        // POST: Fotografias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Path,FilmeFK")] Fotografias foto)
        {
            if (id != foto.Id)
            {
                return NotFound();
            }

            //recuperar o id fo objeto enviado para o browser
            var numIdFoto = HttpContext.Session.GetInt32("NumFotoEmEdicao");

            //e compará-lo com o id recebido
            //se forem iguais, continuamos
            // se forem diferentes não fazemos a alteração
            if(numIdFoto==null || numIdFoto!=foto.Id)
            {
                // se entrei nesta condiç~~ao houve problemas 

                //redirecionar para a página de início
                return RedirectToAction("Index");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FotografiaExists(foto.Id))
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
            ViewData["FilmeFK"] = new SelectList(_context.Filme, "Id", "Descricao", foto.FilmeFK);
            return View(foto);
        }

        // GET: Fotografias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fotografia = await _context.Fotografia
                .Include(f => f.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fotografia == null)
            {
                return NotFound();
            }

            return View(fotografia);
        }

        // POST: Fotografias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fotografia = await _context.Fotografia.FindAsync(id);
            _context.Fotografia.Remove(fotografia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FotografiaExists(int id)
        {
            return _context.Fotografia.Any(e => e.Id == id);
        }
    }
}
