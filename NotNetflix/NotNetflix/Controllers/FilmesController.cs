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
                //horas:fração dos minutos da hora 
                //pretendido----> horas:minutos
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
                        nomefoto = Path.Combine(_caminho.WebRootPath, "fotos", nomefoto);
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
                        ModelState.AddModelError("", o.GetBaseException().ToString());
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

            var dados = await _context.Filme.Include(g => g.ListasDeGeneros).Include(l => l.ListasDeFotografias).FirstOrDefaultAsync(m => m.Id == id);
            if (dados == null)
            {
                return NotFound();
            }
            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
            return View(dados);


        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile, int[] listaGenerosEditados, List<IFormFile> fotografia, string originalFile, int[]listaFotosDelete)
        {
            /*Introduzir os dados novos do filme
             * Receber o ficheiro do filme --> se for igual não altera
             * Receber o ficheiro da foto --> se for igual não altera
             * Receber os novos géneros --> se for igual não altera
             * Tratar dos outros dados 
             */

            
            //??
            var movie= await _context.Filme
                .FirstOrDefaultAsync(m => m.Id == id);

            movie.Path = originalFile;//??

            //se o filme não existir retorna not found
            if (id != movie.Id)
            {
                return NotFound();
            }
            

            //Modelstate não passa válido, será necessário verificar a sua validade?
            if (ModelState.IsValid)
            {
                string nomeFilme="";
                
                //flag indicadora de alterações nos ficheiros das fotografias
                bool flagfotos = false;
                
                //flag indicadora de alterações no ficheiro do filme
                bool flagfilmes = false;

                string localizacaoFicheiro = _caminho.WebRootPath;
                
                //flag indicadora da existência de erros 
                bool valido = true;
                
                
                //verificar se o ficheiro do filme foi introduzido
                if (filmefile != null) {
                    //Introdução de dados do filme
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

                        nomeFilme = Path.Combine(localizacaoFicheiro, "filmes", nomeFilme);
                        
                        //o ficheiro existe e é válido
                        flagfilmes = true;
                    }
                    else
                    {
                        ModelState.AddModelError("", "O ficheiro do filme introduzido não é válido");
                        valido = false;
                    
                    }
                }
                    //se o array com os ids das fotos a apagar possuir elementos
                    if (listaFotosDelete != null)
                    {
                        try
                        {
                        //verificar todas os ids das fotografias do filme para verificar se estão no array de fotografias a apagar
                            foreach (var foto in filme.ListasDeFotografias)
                            {
                                for (int i = 0; i <= listaFotosDelete.Length; i++)
                                {
                                    if (foto.Id == listaFotosDelete[i])
                                    {
                                        //vai buscar a localização do ficheiro no servidor
                                        var fileFotoDelete = Path.Combine(localizacaoFicheiro, "filmes", foto.Path);
                                        //remove a fotografia da tabela
                                        _context.Fotografia.Remove(foto);
                                        //apaga o ficheiro do servidor
                                        System.IO.File.Delete(fileFotoDelete);
                                    }

                                }

                            }

                            await _context.SaveChangesAsync();
                        }
                        catch(Exception e)
                        {

                        }
                    }
                    
                    //adicionar fotografias

                string nomefoto = " ";
                //caso a lista de fotografias possua elementos
                if (fotografia != null)
                {
                    foreach (IFormFile photo in fotografia)
                    {
                        //verifica se os elementos da lista de fotografias são válidos
                        if (photo.ContentType != "image/jpg" || photo.ContentType != "image/jpeg" || photo.ContentType != "image/png")
                        {
                            ModelState.AddModelError("", "Adicione um ficheiro válido de imagem");
                            flagfotos = false;
                            valido = false;
                        }
                    }
                
                
                
                }
                //caso não tenham ocorrido erros
                if (valido) { 
                try
                {
                    //se foram introduzidas novas fotografias
                    if (flagfotos)
                    {
                        
                    
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
                            nomefoto = Path.Combine(_caminho.WebRootPath, "fotos", nomefoto);
                            modelo.FilmeFK = filme.Id;
                            _context.Add(modelo);
                            await _context.SaveChangesAsync();
                            using var play = new FileStream(nomefoto, FileMode.Create);
                            await photo.CopyToAsync(play);
                        }
                       }
                        //se foi introduzido um novo ficheiro de filme
                        if (flagfilmes) { 
                            //apaga o ficheiro antigo do servidor
                            System.IO.File.Delete(originalFile);//originalFile?
                            //adiciona o filme à base de dados
                            _context.Update(filme);
                            await _context.SaveChangesAsync();
                            using var stream = new FileStream(nomeFilme, FileMode.Create);
                            await filmefile.CopyToAsync(stream);
                        }
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
                  
            }
            ViewBag.Generos = await _context.Filme.Include(g => g.ListasDeGeneros).ToListAsync();
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
                .Include(g => g.ListasDeGeneros)
                .Include(l => l.ListasDeFotografias)
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
            var fotos = await _context.Fotografia.Where(p => p.FilmeFK.Equals(id)).ToListAsync();
            var generos = await _context.Filme.Include(g => g.ListasDeGeneros).Where(f => f.Id.Equals(id)).ToListAsync();
            
            
            string localizacaoFicheiro = _caminho.WebRootPath;
             var fileFilmeDelete = Path.Combine(localizacaoFicheiro, "filmes", filme.Path);
            _context.Filme.Remove(filme);
            System.IO.File.Delete(fileFilmeDelete);
            foreach (var gen in generos)
            {
                _context.Filme.Remove(gen);
            }
            foreach (var foto in fotos)
            {
                
                var fileFotoDelete = Path.Combine(localizacaoFicheiro, "filmes", foto.Path);
                _context.Fotografia.Remove(foto);
                System.IO.File.Delete(fileFotoDelete);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filme.Any(e => e.Id == id);
        }
    }
}
