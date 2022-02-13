using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Data;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IAuthorizationService _authService;
        private readonly RecipeService _service;

        public EditModel(RecipeService service, IAuthorizationService authService)
        {
            _service = service;
            _authService = authService;
        }

        [BindProperty] public UpdateRecipeCommand Input { get; set; }

        [BindProperty] public Recipe Recipe { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Input = await _service.GetRecipeForUpdate(id);
            Recipe = await _service.GetRecipe(id);
            var authResult = await _authService.AuthorizeAsync(User, Recipe, "CanManageRecipe");
            if (Input is null)
                // If id is not for a valid Recipe, generate a 404 error page
                // TODO: Add status code pages middleware to show friendly 404 page
                return NotFound();

            if (!authResult.Succeeded) return new ForbidResult();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _service.UpdateRecipe(Input);
                    return RedirectToPage("View", new {id = Input.Id});
                }
            }
            catch (Exception)
            {
                // TODO: Log error
                // Add a model-level error by using an empty string key
                ModelState.AddModelError(
                    string.Empty,
                    "An error occured saving the recipe"
                );
            }

            //If we got to here, something went wrong
            return Page();
        }
    }
}