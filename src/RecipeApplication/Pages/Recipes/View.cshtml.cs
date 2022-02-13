using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeApplication.Data;
using RecipeApplication.Models;

namespace RecipeApplication.Pages.Recipes
{
    public class ViewModel : PageModel
    {
        private readonly IAuthorizationService _authService;
        private readonly RecipeService _service;

        public ViewModel(RecipeService service, IAuthorizationService authService)
        {
            _service = service;
            _authService = authService;
        }

        public RecipeDetailViewModel RecipeDetail { get; set; }
        public Recipe Recipe { get; set; }
        public bool CanEditRecipe { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            RecipeDetail = await _service.GetRecipeDetail(id);
            Recipe = await _service.GetRecipe(id);
            if (RecipeDetail is null)
                // If id is not for a valid Recipe, generate a 404 error page
                // TODO: Add status code pages middleware to show friendly 404 page
                return NotFound();
            var isAuthorized = await _authService.AuthorizeAsync(User, Recipe, "CanManageRecipe");
            CanEditRecipe = isAuthorized.Succeeded;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteRecipe(id);

            return RedirectToPage("/Index");
        }
    }
}