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
    //só os utilizadores autenticados 
    [Authorize]
    public class UtilizadoresController : Controller
    {
        /// <summary>
        /// este atributo representa a base de dados do projeto
        /// </summary>
        private readonly NotNetflixDataBase _context;
        /// <summary>
        /// esta variável recolhe os dados da pessoa q se autenticou
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;
        public UtilizadoresController(NotNetflixDataBase context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Gestor")]
        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }
        [Authorize(Roles = "Gestor,Utilizador")]
        // GET: Utilizadores/Details/5
        public async Task<IActionResult> Details(int? id, string? userName)
        {
            //var ola = _userManager.Users.FirstOrDefaultAsync(m => m.Id == _userManager.GetUserId(User));
            
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(f => f.Email == userName);
            if(userName != null)
            {
                if (utilizador != null)
                {
                    return View(utilizador);
                }
                else
                {
                    return NotFound();
                }
            }
            if (id == null)
            {
                return NotFound();
            }
            
            var user = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
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
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,N_telemovel,Morada,CodPostal,dataNascimento,UserNameId")] Utilizador newUtilizador)
        {
            //varável com o id do utilizador autenticado
            var meuUtilizador = _userManager.GetUserId(User);
            ApplicationUser identidade = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == newUtilizador.UserNameId);
            var eu = newUtilizador.UserNameId;
           
            if (id != newUtilizador.Id || identidade == null)
            {
                ModelState.AddModelError("", "Ocorreu um erro..");
                return View();
            }


            if (ModelState.IsValid)
            {
                if (newUtilizador.dataNascimento.CompareTo(DateTime.Now.AddYears(-18)) > 0)
                {
                    ModelState.AddModelError("", "Para entrar no site é necessário ser maior de 18 anos");
                    return View(newUtilizador);
                }
                identidade.Email = newUtilizador.Email;
                identidade.UserName = newUtilizador.Email;
                identidade.NormalizedEmail = identidade.Email.ToUpper();
                identidade.NormalizedUserName = identidade.UserName.ToUpper();
                try
                {
                    _context.Users.Update(identidade);
                    _context.Utilizador.Update(newUtilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(newUtilizador.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Utilizadores", new { id=newUtilizador.Id });
            }
            return View(newUtilizador);
        }



        [Authorize]
        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(m => m.Id == id);
            var identidade = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == utilizador.Email);
            
            if (utilizador == null || identidade == null)
            {
                ModelState.AddModelError("", "Utilizador Inválido");
                return View();
            }

            return View(utilizador);
        }
        [Authorize]
        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            var identidade = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == utilizador.Email);
            try { 
            _context.Utilizador.Remove(utilizador);
            _context.Users.Remove(identidade);
            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}
