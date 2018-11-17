using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    interface IActionRepository
    {
        List<ActionMaster> GetAction();
        void InsertAction(ActionVM actionvm);
        ActionVM GetActionById(int ActionId);
        void UpdateAction(ActionVM actionvm);
        void DeleteAction(int ActionId);
    }
}
