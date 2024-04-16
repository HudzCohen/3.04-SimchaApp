using _3._04_HW.Data;
using _3._04_HW.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace _3._04_HW.Web.Controllers
{
    public class ContributorController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=SimchaApp; Integrated Security=true";

        public IActionResult Contributors()
        {

            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }

            var db = new SimchaManager(_connectionString);
            ContributorViewModel vm = new();
            vm.Contributors = db.GetContributors();
            vm.Total = db.GetBalance(db.GetDepositTotal(), db.GetContributionTotal());
            foreach(Contributor c in vm.Contributors)
            {
                c.Balance = db.GetBalance(db.GetDepositForContributor(c.Id), db.GetContributionTotalForContributor(c.Id));
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult EditContributor(Contributor c)
        {
            var db = new SimchaManager(_connectionString);
            db.EditContributor(c);
            TempData["message"] = $"{c.FirstName} {c.LastName} successfully edited";
            return Redirect("/contributor/contributors");
        }

        [HttpPost]
        public IActionResult New(Contributor c, Deposit d)
        {
            if (c == null)
            {
                return Redirect("/contributor/contributors");
            }
            else
            {
                var db = new SimchaManager(_connectionString);
                db.AddContributor(c, d);
                TempData["message"] = $"{c.FirstName} {c.LastName} was successfully added";
                return Redirect("/contributor/contributors");
            }
        }

        [HttpPost]
        public IActionResult Deposit(Deposit d)
        {
            if(d.Amount <= 0)
            {
                return Redirect("/contributor/contributors");
            }
            TempData["message"] = $"{d.Amount} deposited";
            var db = new SimchaManager(_connectionString);
            db.AddDeposit(d);
            return Redirect("/contributor/contributors");
        }

        public IActionResult History(int id)
        {
            var db = new SimchaManager(_connectionString);
            var trans = db.GetTransactionsForContributor(id);
            trans.AddRange(db.GetDepositsListForContrib(id));
            HistoryViewModel vm = new()
            {
                Contributor = db.GetContributorById(id),
                Balance = db.GetBalance(db.GetDepositForContributor(id), db.GetContributionTotalForContributor(id)),
                Transactions = trans
            };

            return View(vm);
        }

    }
}
