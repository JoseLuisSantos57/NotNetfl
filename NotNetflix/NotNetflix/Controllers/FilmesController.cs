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
using System.Net;

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
        public async Task<IActionResult> Create([Bind("Path,Titulo,Data,Duracao,Rating,Descricao")] Filme filme, int[] listaGenerosEscolhidos, List<IFormFile> fotografia)
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
         
         

                //verificar se existe ficheiro
                if (filme.Path == null)
                {
                    //apresentar mensagem a pedir o ficheiro
                    ModelState.AddModelError("", "Adicione por favor o ficheiro do filme");

                    correto = false;
                    
                }
                else //existe ficheiro
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(filme.Path);
                    request.Method = "HEAD";
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (!response.ResponseUri.ToString().Contains("youtube.com/embed/"))
                        {
                            ModelState.AddModelError("", "Insira um link válido por favor");
                            correto = false;
                        }
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
                if (correto)
                {
                    //criar array com as fotografias a criar 
                    List<FileStream> criadorFiles = new List<FileStream>();
                    try
                    {
                        //problemas aqui
                        _context.Add(filme);
                        await _context.SaveChangesAsync(); 
                        
                  
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
                        return RedirectToAction("Index", "Home", new { area = "" });
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
        public async Task<IActionResult> Edit(int id,[ Bind("Id,Titulo,Descricao,Data,Duracao,Rating,Path")] Filme newFilme, List<IFormFile> fotografias, int[] listaGenerosEditados,  int[]listaFotosDelete)
        {
            
            if (id != newFilme.Id)
            {
                return NotFound();
            }

            /*Introduzir os dados novos do filme
             * Receber o ficheiro do filme --> se for igual não altera
             ***caso seja diferente cria o ficheiro novo,  cria o ficheiro, adiciona à base de dados,  
             * Receber o ficheiro da foto --> se for igual não altera
             * Receber os novos géneros --> se for igual não altera
             * Tratar dos outros dados 
             */

            //dados anteriormente guardados no filme
            var filme = await _context.Filme
                                      .Include(l => l.ListasDeFotografias)
                                      .Include(g => g.ListasDeGeneros)
                                      .Where(f => f.Id == id)
                                      .FirstOrDefaultAsync();

            //variável com os géneros antes de serem editados
            var oldGeneros = filme.ListasDeGeneros.Select(c => c.Id).ToList();

            // avaliar se o utilizador alterou algum género associado ao Filme
            // adicionados -> lista de géneros adicionados
            // retirados   -> lista de géneros retirados
            var gensAdicionados = listaGenerosEditados.Except(oldGeneros);
            var gensRetirados = oldGeneros.Except(listaGenerosEditados.ToList());

            //variável com as fotografias antes de serem editadas
            var oldFotos = filme.ListasDeFotografias.Select(f => f.Id).ToList();

            var fotosRetiradas = listaFotosDelete.ToList();


            if (ModelState.IsValid)
            {
                filme.Titulo = newFilme.Titulo;

                filme.Descricao = newFilme.Descricao;

                filme.Duracao = newFilme.Duracao;

                filme.Rating = newFilme.Rating;

                //flag para verificar se não houve problemas com os dados introduzidos
                bool correto = true;

                //contador de fotografias para impedir que o filme fique sem fotografias

                //flag para apagar as fotografias selecionadas
                bool eraseFoto = false;
                
                //falg para introduzir as fotografias novas
                bool newFoto = false;
                //verificar se foram introduzidas fotografias novas
                if (fotografias.Count()>0)
                {
                    //verifica a validade dos ficheiros das fotos
                    foreach (IFormFile foto in fotografias)
                        if (foto.ContentType != "image/jpg" || foto.ContentType != "image/jpeg" || foto.ContentType != "image/png")
                        {
                            //ativa uma flag para a alteração de fotografias
                            newFoto = true;
                        }
                        else
                        {
                            //os ficheiros não são válidos
                            ModelState.AddModelError("", "Adicione um ficheiro válido de imagem por favor");
                            correto = false;
                        }
                }

                ////verificar a existência de novos géneros
                //if (listaGenerosEditados != null)
                //{
                //    var listaGeneros = _context.Genero.Where(g => listaGenerosEditados.Contains(g.Id)).ToList();
                //    // adicionar esta lista de géneros ao filme
                //    newFilme.ListasDeGeneros = (ICollection<Genero>)listaGeneros;
                //}
                
                
                //se algum género foi adicionado ou retirado
                //é necessário alterar a lista de géneros
                //associados ao filme

                if(gensAdicionados.Any()|| gensRetirados.Any())
                {
                    if (gensRetirados.Any())
                    {
                        foreach (int generoApagar in gensRetirados) { 
                            var generoRemove = filme.ListasDeGeneros.FirstOrDefault(g => g.Id == generoApagar);
                        filme.ListasDeGeneros.Remove(generoRemove);
                        }
                    }
                    if (gensAdicionados.Any())
                    {
                        foreach (int generoAdd in gensAdicionados)
                        {
                            var generoAdicionar = await _context.Genero.FirstOrDefaultAsync(g => g.Id == generoAdd);
                            filme.ListasDeGeneros.Add(generoAdicionar);
                        }
                    }
                }




                //verificar a validade da data introduzida
                if (newFilme.Data.CompareTo(DateTime.Now) <= 0)
                {
                    filme.Data = newFilme.Data;
                }
                else
                {
                    correto = false;
                    ModelState.AddModelError("","Adicione uma data válida");
                }



                if (newFilme.Path.Contains("youtube.com/embed/"))
                {
                    filme.Path = newFilme.Path;
                }
                else
                {
                        ModelState.AddModelError("", "Insira um link válido por favor");
                        correto = false;
                }

                if(listaFotosDelete != null)
                {
                    eraseFoto = true;
                }

                //caso não haja erros com os dados introduzidos
                if (correto)
                {
                    //verificar se o número de fotografias != 0

                    
                    
                    //nº de fotografias adicionadas
                    if ((filme.ListasDeFotografias.Count() + fotografias.Count()-listaFotosDelete.Count()) <= 0)
                    {
                        //caso o filme deixe de possuir fotografias devolve o controlo à view
                        ModelState.AddModelError("","O Filme tem que possuir pelo menos uma fotografia associada");
                        ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                        return View(newFilme);
                    }


                    //atualizar o filme
                    //guardar alterações
                    try
                    {
                        _context.Filme.Update(filme);
                        await _context.SaveChangesAsync();
                    }
                    catch(Exception o)
                    {
                        ModelState.AddModelError("",o.ToString());
                        ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                        return View(newFilme);
                    }

                    //se chegámos aqui o update foi bem sucedido

                    //criar os fichieros das fotografias
                    
                    if (newFoto) 
                    { 
                        string nomeFoto = "";
                        foreach (IFormFile photo in fotografias)
                        {
                            var modelo = new Fotografia();
                            //definir o nome do ficheiro
                            Guid a;
                            a = Guid.NewGuid();
                            nomeFoto = newFilme.Titulo + "_" + a.ToString();

                            //determinar a expressão do nome do filme
                            nomeFoto = nomeFoto + Path.GetExtension(photo.FileName).ToLower();

                            modelo.Path = nomeFoto;
                            nomeFoto = Path.Combine(_caminho.WebRootPath, "fotos", nomeFoto);
                            modelo.FilmeFK = newFilme.Id;
                            try
                            {
                                //cria o ficheiro da foto
                                using var play = new FileStream(nomeFoto, FileMode.Create);
                                await photo.CopyToAsync(play);//pode ocorrer erros aqui
                                _context.Add(modelo);
                                await _context.SaveChangesAsync();//pode ocorrer erros aqui
                            }
                            catch (Exception o)
                            {
                                ModelState.AddModelError("", o.ToString());
                                ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                                return View(newFilme);
                            }
                        }
                    }


                    if (fotosRetiradas.Any())
                    {
                       
                        foreach (int foto in fotosRetiradas)
                        {
                            try
                            { 
                                //apagar a fotografia da base de dados
                                var fotoRemove = await _context.Fotografia.FirstOrDefaultAsync(f => f.Id == foto);
                                _context.Fotografia.Remove(fotoRemove);
                                await _context.SaveChangesAsync();
                                //apagar a fotografia do disco rígido
                                var link = Path.Combine(_caminho.WebRootPath, "fotos", fotoRemove.Path);
                                System.IO.File.Delete(link);//realizar depois de guardar os dados na base de dados
                            }
                            catch (Exception o) {
                                ModelState.AddModelError("","Houve um erro na remoção das fotografias por favor tente outravaz");
                                ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
                                return View(newFilme);
                            }

                        }

                    }
                    //caso não ocorram problemas com a criação dos ficheiros prossegue | caso contrário devolve o controlo à view

                    //caso as alterações tenham sido guardadas sem problemas cria as fotografias na base de dados
                }
                return RedirectToAction("Index", "Home", new { area = "" });
            }





            //o problema está em associar o id recebido pelo método e o objeto filme recebido

            //filme = _context.Filme.Include(g => g.ListasDeGeneros).Include(f => f.ListasDeFotografias).FirstOrDefault(f => f.Id == id);

            /*
       
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
            */
            ViewBag.Generos = await _context.Genero.OrderBy(g => g.Genre).ToListAsync();
            return View(newFilme);
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
