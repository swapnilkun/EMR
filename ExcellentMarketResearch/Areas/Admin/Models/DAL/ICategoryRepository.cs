using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    interface ICategoryRepository
    {
        List<CategoryMaster> GetCategory();
        void InsertCategory(CategoryVM catvm);
        CategoryVM EditCategory(CategoryVM catvm);
        CategoryMaster GetCategoryById(int CategoryId);
        void DeleteCategory(int CategoryId);

    }
}
