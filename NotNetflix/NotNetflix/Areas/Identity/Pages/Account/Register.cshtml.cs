﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using NotNetflix.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NotNetflix.Models;

namespace NotNetflix.Areas.Identity.Pages.Account {
   [AllowAnonymous]
   public class RegisterModel : PageModel {

      private readonly SignInManager<ApplicationUser> _signInManager;
      private readonly UserManager<ApplicationUser> _userManager;
      private readonly ILogger<RegisterModel> _logger;
      private readonly IEmailSender _emailSender;

      /// <summary>
      /// referência à BD do nosso sistema
      /// </summary>
      private readonly NotNetflixDataBase _context;

      /// <summary>
      /// construtor
      /// </summary>
      /// <param name="userManager"></param>
      /// <param name="signInManager"></param>
      /// <param name="logger"></param>
      /// <param name="emailSender"></param>
      public RegisterModel(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          ILogger<RegisterModel> logger,
          IEmailSender emailSender,
          NotNetflixDataBase context) {
         _userManager = userManager;
         _signInManager = signInManager;
         _logger = logger;
         _emailSender = emailSender;
         _context = context;
      }

      /// <summary>
      /// Model usado para 'transportar' os dados para a interface de'Registar'
      /// </summary>
      [BindProperty] // garante a existência de 'memória' entre o Model e a interface
      public InputModel Input { get; set; }

      /// <summary>
      /// serve para redirecionar o utilizador para o 'local' de origem
      /// </summary>
      public string ReturnUrl { get; set; }

      /*
       * Classe usada para 'transportar/recolher' os dados da Página para dentro do 'código'
       */
      public class InputModel {
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         [Required]
         [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         [DataType(DataType.Password)]
         [Display(Name = "Confirm password")]
         [Compare("Password", ErrorMessage = "A password e a sua confirmação não correspondem.")]
         public string ConfirmPassword { get; set; }

         /// <summary>
         /// Ao anexar um objeto deste tipo ao 'InpuModel' estamos a 
         /// permitir a recolha dos dados do Criador
         /// </summary>
         public Utilizador User { get; set; }
      }

      /// <summary>
      /// método a ser executado pela página, quando o HTTP é invocado em GET
      /// </summary>
      /// <param name="returnUrl">link para redirecionar o utilizador, se fornecido</param>
      /// <returns></returns>
      public void OnGet(string returnUrl = null) {
         ReturnUrl = returnUrl;
      }


      /// <summary>
      /// método a ser executado pela página, quando o HTTP é invocado em POST
      ///    - criar um novo Utilizador
      ///    - registar os dados pessoais do Utilizador
      /// </summary>
      /// <param name="returnUrl">link para redirecionar o utilizador, se fornecido</param>
      /// <returns></returns>
      public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
         
         // se o 'returnUrl' for null, é-lhe atribuído um URL
         // se não for Null, nada é feito
         returnUrl ??= Url.Content("~/");

         // validar se se pode criar um USER
         // Se os dados forem validados pela classe 'InputModel'
         if (ModelState.IsValid) {


            // criar um objeto do tipo 'ApplicationUser'
            var user = new ApplicationUser {
               UserName = Input.Email, // username
               Email = Input.Email,    // email do utilizador
               EmailConfirmed = true, // o email está formalmente confirmado
               LockoutEnabled = false,  // o utilizador não pode ser bloqueado
               DataRegisto = DateTime.Now // data do registo
            };

            // vou tentar criar, efetivamente, esse utilizador
            var result = await _userManager.CreateAsync(user, Input.Password);

            // se houver sucesso
            if (result.Succeeded) {
               _logger.LogInformation("User created a new account with password.");

                    
                    //verificar se o utilizador é maior de idade
                    if (Input.User.dataNascimento.CompareTo(DateTime.Now.AddYears(-18)) > 0)
                    {
                        ModelState.AddModelError("", "Para entrar no site é necessário ser maior de 18 anos");
                        return Page();
                    }
                    //Para a criação do gestor vai verificar se o email é o seguinte
                    if (Input.Email.EndsWith("@notnetflix.pt")) {   
                     await _userManager.AddToRoleAsync(user, "Gestor");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Utilizador");
                    }
                    Input.User.Email = Input.Email; // atribuir ao objeto 'criador' o email fornecido pelo utilizador,

                    Input.User.UserNameId = user.Id;  // adicionar o ID do utilizador,
                       
               // estamos em condições de guardar os dados na BD
               try {
                  _context.Add(Input.User);
                        
                  await _context.SaveChangesAsync(); // 'commit' da adição
                         
                        
                        //asp - area = "Identity" asp - page = "/Account/Manage/Index"
                        return RedirectToAction("Index", "Home");
                    }
               catch (Exception) {
                  // houve um erro na criação dos dados do Criador
                  // Mas, o USER já foi criado na BD
                  // vou efetuar o Roolback da ação
                  await _userManager.DeleteAsync(user);

                  // avisar que houve um erro
                  ModelState.AddModelError("", "Ocorreu um erro na criação de dados");
               }
            }
            foreach (var error in result.Errors) {
               ModelState.AddModelError(string.Empty, error.Description);
            }
         }

         // Se cheguei aqui algo falhou, volta a mostrar a página
         return Page();
      }
   }
}
