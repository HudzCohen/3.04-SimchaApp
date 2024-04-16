using _3._04_HW.Data;
using _3._04_HW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _3._04_HW.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=SimchaApp; Integrated Security=true";

        public IActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }

            var db = new SimchaManager(_connectionString);
            SimchaViewModel vm = new();

            vm.Simchas = db.GetSimchas();
            vm.TotalContributorCount = db.GetContributorCount();
            foreach(Simcha s in vm.Simchas)
            {
                s.TotalContributors = db.GetContributorCountForSimcha(s.Id);
                s.TotalForSimcha = db.GetTotalForSimcha(s.Id);
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult NewSimcha(Simcha s)
        {
            var db = new SimchaManager(_connectionString);
            List<Contributor> automaticContrib = db.GetContributors().Where(c => c.AlwaysInclude).ToList();
            var simchaId = db.AddSimcha(s);
            db.AddAutoContributions(automaticContrib, simchaId);
            TempData["message"] = $"{s.SimchaName} added successfully!";
            return Redirect("/home/index");
        }

       
        public IActionResult Contributions(int id)
        {
            var db = new SimchaManager(_connectionString);
            ContributionsViewModel vm = new()
            {
                Simcha = db.GetSimchaById(id),
                Contributors = db.GetContributors(),
            };
            vm.Contributions = db.GetContributionsForSimcha(id, vm.Simcha.SimchaName);

            foreach (Contributor c in vm.Contributors)
            {
                c.Balance = db.GetBalance(db.GetDepositForContributor(c.Id), db.GetContributionTotalForContributor(c.Id));
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(List<Contributor> contributor, int simchaId)
        {
            var db = new SimchaManager(_connectionString);
            db.UpdateContributions(contributor, simchaId);
            TempData["message"] = $"Updated successfully!";
            return Redirect("/home/index");
        }


    }
}
