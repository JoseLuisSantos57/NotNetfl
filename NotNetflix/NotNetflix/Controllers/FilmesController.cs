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
    [Authorize(Roles = "Gestor")] // esta 'anotação' garante que só as pessoas autenticadas têm acesso aos recursos
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,Data,Duracao,Rating")] Filme filme, IFormFile filmefile, int[] listaGenerosEditados, IFormFile[] fotografias, int[]listaFotosDelete)
        {
            /*Introduzir os dados novos do filme
             * Receber o ficheiro do filme --> se for igual não altera
             ***caso seja diferente cria o ficheiro novo,  cria o ficheiro, adiciona à base de dados,  
             * Receber o ficheiro da foto --> se for igual não altera
             * Receber os novos géneros --> se for igual não altera
             * Tratar dos outros dados 
             */

           




            //o problema está em associar o id recebido pelo método e o objeto filme recebido
            
            //filme = _context.Filme.Include(g => g.ListasDeGeneros).Include(f => f.ListasDeFotografias).FirstOrDefault(f => f.Id == id);


            //problemas aqui
            if (ModelState.IsValid)
            {
                //varíavel de controlo para tratar os erros do modelstate
                bool correto = true;

                //variável de controlo para a introdução de ficheiros
                bool intrFotos = false;

                //variável de controlo para a introdução de ficheiros
                bool intrFilme = false;


                //verificar a data
                if (filme.Data.CompareTo(DateTime.Now)>0)
                {
                    ModelState.AddModelError("", "Introduza por favor uma data válida");
                    correto = false;
                }

                if (filmefile != null)
                {
                    if(filmefile.ContentType != "video/webm")
                    {
                        ModelState.AddModelError("", "Adicione um ficheiro de vídeo válido");
                        correto = false;
                    }
                    else
                    {
                        intrFilme = true;
                    }
                }

                //verificar as fotografias inseridas e possibilidade de apagar fotos já existentes
                if(fotografias != null)
                {
                    foreach(var foto in fotografias)
                    {
                        if(foto.ContentType != "image/jpg" || foto.ContentType != "image/jpeg" || foto.ContentType != "image/png")
                        {
                            ModelState.AddModelError("", "Adicione um ficheiro de imagem válido por favor");
                            correto = false;
                        }
                        else
                        {
                            intrFotos = true; 
                        }
                    }
                }
                

                //se os dados introduzidos forem válidos altera os dados da base de dados
                if (correto)
                {
                    string nomeFoto = "";

                    string nomeFilme = ""; 
                    //tratar as fotos a apagar 
                    if(listaFotosDelete != null)
                    {
                        foreach(var foto in _context.Fotografia.Where(f => listaFotosDelete.Contains(f.Id)).ToList())
                        {
                            _context.Fotografia.Remove(foto);
                            System.IO.File.Delete(Path.Combine(_caminho.WebRootPath, "filmes", foto.Path));//realizar depois de guardar os dados na base de dados
                        }
                    }
                    
                    if (listaGenerosEditados != null)
                    {
                        //adicionar as géneros escolhidos à lista
                        // foram escolhidos géneros
                        var listaGeneros = _context.Genero.Where(g => listaGenerosEditados.Contains(g.Id)).ToList();
                        // adicionar esta lista de géneros ao filme
                        filme.ListasDeGeneros = (ICollection<Genero>)listaGeneros;
                    }

                    //se foram introduzidas novas fotografias
                    if (intrFotos) { 
                        //criar os ficheiros das fotografias
                        foreach (IFormFile photo in fotografias)
                        {
                            var modelo = new Fotografia();
                            //definir o nome do ficheiro
                            Guid a;
                            a = Guid.NewGuid();
                            nomeFoto = filme.Titulo + "_" + a.ToString();

                            //determinar a expressão do nome do filme
                            nomeFoto = nomeFoto + Path.GetExtension(photo.FileName).ToLower();

                            modelo.Path = nomeFoto;
                            nomeFoto = Path.Combine(_caminho.WebRootPath, "fotos", nomeFoto);
                            modelo.FilmeFK = filme.Id;
                            try
                            {
                                _context.Add(modelo);
                                await _context.SaveChangesAsync();
                                using var play = new FileStream(nomeFoto, FileMode.Create);
                                await photo.CopyToAsync(play);
                            }
                            catch(Exception o)
                            {

                            }
                        }
                    }
                    //caso o filme tenha sido alterado vamos criar o ficheiro e alterar na base de dados
                    if (intrFilme)
                    {
                        //definir o nome do ficheiro
                        Guid g;
                        g = Guid.NewGuid();

                        nomeFilme = filme.Titulo + "_" + g.ToString();

                        //determinar a expressão do nome do filme

                        nomeFilme = nomeFilme + Path.GetExtension(filmefile.FileName).ToLower();

                        filme.Path = nomeFilme;
                        nomeFilme = Path.Combine(_caminho.WebRootPath, "filmes", nomeFilme);
                    }
                


                    //alterar os dados na base de dados
                    try
                    {
                        //dá update ao filme
                        _context.Update(filme);
                        
                        //guarda as alterações 
                        await _context.SaveChangesAsync();
                        
                        //verifica se se alterou o ficheiro do filme
                        if (intrFilme) {
                            using var play = new FileStream(nomeFilme, FileMode.Create);
                            //cria o ficheiro
                            await filmefile.CopyToAsync(play);
                        }
                    }
                    catch(DbUpdateConcurrencyException)
                    {

                    }

                    //se cheguei até aqui envio de volta para o index
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Generos = await _context.Filme.Include(g => g.ListasDeGeneros).ToListAsync();
            return View(filme);
        }
        [Authorize(Roles = "Gestor")]
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
