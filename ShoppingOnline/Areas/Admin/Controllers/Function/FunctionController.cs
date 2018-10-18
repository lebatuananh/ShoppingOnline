using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Application.Systems.Functions;
using ShoppingOnline.Application.Systems.Functions.Dtos;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Function
{
    public class FunctionController : BaseController
    {
        private readonly IFunctionService _functionService;

        public FunctionController(IFunctionService functionService)
        {
            this._functionService = functionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var model = await _functionService.GetAll(string.Empty);
            var rootFunctions = model.Where(x => x.ParentId == null);
            var items = new List<FunctionViewModel>();
            foreach (var function in rootFunctions)
            {
                //add the parent category to the item list
                items.Add(function);
                //now get all its children (separate Category in case you need recursion)
                GetByParentId(model.ToList(), function, items);
            }

            return new ObjectResult(items);
        }

        #region Private Functions

        private void GetByParentId(IEnumerable<FunctionViewModel> allFunctions,
            FunctionViewModel parent, IList<FunctionViewModel> items)
        {
            var functionsEntities = allFunctions as FunctionViewModel[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.Id);
            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, cat, items);
            }
        }

        #endregion
    }
}