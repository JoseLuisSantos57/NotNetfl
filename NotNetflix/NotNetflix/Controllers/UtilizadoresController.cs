using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotNetflix.Data;
using NotNetflix.Models;

namespace NotNetflix.Controllers
{
    [Authorize]
    public class UtilizadoresController : Controller
    {
        private readonly NotNetflixDataBase _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UtilizadoresController(NotNetflixDataBase context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }

        // GET: Utilizadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }



        /// <summary>
        /// Método para apresentar os dados dos Criadores a autorizar
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> ListaCriadoresPorAutorizar()
        {

            // quais os Criadores ainda não autorizados a aceder ao Sistema?
            // lista com os utilizadores bloqueados
            var listaDeUtilizadores = _userManager.Users.Where(u => u.LockoutEnd > DateTime.Now);
            // lista com os dados dos Criadores
            var listaUtilizadores = _context.Utilizador
                                         .Where(c => listaDeUtilizadores.Select(u => u.Id)
                                                                       .Contains(c.UserNameId));
            /* Em SQL seria algo deste género
             * SELECT c.*
             * FROM Criadores c, Users u
             * WHERE c.UserName = u. Id AND
             *       u.LockoutEnd > Data Atual          * 
             */

            // Enviar os dados para a View
            return View(await listaUtilizadores.ToListAsync());
        }

            //[Authorize(Roles = "Gestor")]   //  -->
         //[Authorize(Roles = "Cliente")]   // -->  Neste caso, a pessoa tem de pertencer aos dois roles
        /// <summary>
        /// método que recebe os dados dos utilizadores a desbloquear
        /// </summary>
        /// <param name="utilizadores">lista desses utilizadores</param>
        /// <returns></returns>
        [HttpPost]
         [Authorize(Roles = "Gestor,Utilizador")] // --> permite acesso a pessoas com uma das duas roles  
        public async Task<IActionResult> ListaUtilizadoresPorAutorizar(string[] utilizadores)
        {

            // será que algum utilizador foi selecionado?
            if (utilizadores.Count() != 0)
            {
                // há users selecionados
                // para cada um, vamos desbloqueá-los
                foreach (string u in utilizadores)
                {
                    try
                    {
                        // procurar o 'utilizador' na tabela dos Users
                        var user = await _userManager.FindByIdAsync(u);
                        // desbloquear o utilizador
                        await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddDays(-1));
                        // como não se pediu ao User para validar o seu email
                        // é preciso aqui validar esse email
                        string codigo = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _userManager.ConfirmEmailAsync(user, codigo);

                        // eventualmente, poderá ser enviado um email para o utilizador a avisar que 
                        // a sua conta foi desbloqueada
                    }
                    catch (Exception)
                    {
                        // deveria haver aqui uma mensagem de erro para o utilizador,
                        // se assim o entender
                    }
                }
            }

            return RedirectToAction("Index");
        }


        //#################################################################################
        // GET: Utilizadores/Create
        /*
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,N_telemovel")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilizador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizador);
        }*/
        //######################################################################################
        // GET: Utilizadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }

        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,N_telemovel")] Utilizador utilizador)
        {
            if (id != utilizador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.Id))
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
            return View(utilizador);
        }

        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            _context.Utilizador.Remove(utilizador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}
