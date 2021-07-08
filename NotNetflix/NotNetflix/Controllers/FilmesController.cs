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
    [AllowAnonymous]
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
        private readonly UserManager<ApplicationUser> _userManager;
        //private ListarFotosViewModel<List>
        public FilmesController(NotNetflixDataBase context, IWebHostEnvironment caminho, UserManager<ApplicationUser> userManager)
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

            /*
             SELECT fotografia
            FROM fotografia, filme
            WHERE fotografia.FilmeFk = filme.listadefotografias
             
             */
            /* var lista_de_fotos = ;
             var fotografias = await _context.Fotografia.Where(f => );

             */

            var listaFilmes = await  _context.Filme.Include(f => f.ListasDeFotografias).Include(f => f.ListasDeGeneros).ToListAsync();
            //é possível utilizar uma viewbag(transporte do controller para a view)
            //ViewBag.Fotografias = lista_de_fotos;

            /* var listafotos = new ListarFotosViewModel
             {
                 ListaFotos = lista_de_fotos
             };*/

            return View(listaFilmes);
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
        public async Task<IActionResult> Create()
        {

            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
            //var listaFilmes = await _context.Filme.Include(f => f.ListasDeFotografias).Include(f => f.ListasDeGeneros).ToListAsync();

            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Path,Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile, int[] listaGenerosEscolhidos, List<IFormFile> fotografia)
        {
            //var auxiliar
            bool correto = true;
                
            if (ModelState.IsValid)
                {   

                if (DateTime.Compare(filme.Data, DateTime.Now) > 0)
                {
                    ModelState.AddModelError("", "A data introduzida não é válida, porque é posterior à data atual");
                    correto = false;
                }

                if (listaGenerosEscolhidos.Count() != 0)
                {
                    // foram escolhidos géneros
                    var listaGeneros = _context.Genero.Where(g => listaGenerosEscolhidos.Contains(g.Id)).ToList();
                    // adicionar esta lista de géneros ao filme
                    filme.ListasDeGeneros = (ICollection<Genero>)listaGeneros;
                }

                TimeSpan t = TimeSpan.FromMinutes(filme.Duracao);
                filme.Duracao = t.TotalHours;
                
                
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

                    correto = false;
                    
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
                        nomeFilme = Path.Combine(localizacaoFicheiro, "filmes", nomeFilme);
                    }
                    else
                    {
                        //ficheiro não é válido
                        //adicionar mensagem de erro
                        ModelState.AddModelError("", "O formato do ficheiro introduzido não é válido");
                        correto = false;

                    }
                }

                
                //adcionar fotografia
                string nomefoto = " ";
                foreach (IFormFile photo in fotografia) {
                    if (fotografia == null && (photo.ContentType != "image/jpg" || photo.ContentType != "image/jpeg" || photo.ContentType != "image/png"))
                    {
                        ModelState.AddModelError("", "Adicione um ficheiro válido de imagem");
                        correto = false;

                    } 
                }
                 if(correto)
                {
                    

                    try
                    {

                        _context.Add(filme);
                        await _context.SaveChangesAsync();

                        
                        
                        using var stream = new FileStream(nomeFilme, FileMode.Create);
                        await filmefile.CopyToAsync(stream);
                  
                        foreach (IFormFile photo in fotografia)
                        {

                            var modelo = new Fotografia();
                            //definir o nome do ficheiro
                            Guid a;
                        a = Guid.NewGuid();
                        nomefoto = filme.Titulo + "_" + a.ToString();

                        //determinar a expressão do nome do filme
                        string extensao = Path.GetExtension(photo.FileName).ToLower();

                        nomefoto = nomefoto + extensao;
                        
                        modelo.Path = nomefoto;
                        string localizacaodoficheiro = _caminho.WebRootPath;

                        nomefoto = Path.Combine(localizacaodoficheiro, "fotos", nomefoto);
                        modelo.FilmeFK = filme.Id;
                        _context.Add(modelo);
                        await _context.SaveChangesAsync();
                        using var play = new FileStream(nomefoto, FileMode.Create);
                            await photo.CopyToAsync(play);
                    }
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception o)
                    {
                        var erro = o.GetBaseException().ToString();
                        ModelState.AddModelError("", erro);
                        ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                        return View(filme);
                    }


                    
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


            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
            ViewBag.Fotos = await _context.Filme.Include(f => f.ListasDeFotografias).ToListAsync();
 
            return View(filme);


        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Path,Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile, int[] listaGenerosEditados, IFormFile fotografia)
        {
            /*Introduzir os dados novos do filme
             * Receber o ficheiro do filme --> se for igual não altera
             * Receber o ficheiro da foto --> se for igual não altera
             * Receber os novos géneros --> se for igual não altera
             * Tratar dos outros dados 
             */


            //se o filme não existir retorna not found
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
