using Microsoft.AspNetCore.Mvc;
using RS1_Ispit_asp.net_core.EF;
using RS1_Ispit_asp.net_core.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RS1_Ispit_asp.net_core.Controllers
{
    public class TakmicenjeController : Controller
    {
        private readonly MojContext _context;

        public TakmicenjeController(MojContext mojContext)
        {
            _context = mojContext;
        }

        // GET: TakmicenjeController
        public ActionResult Index()
        {
            var skole = _context.Skola.ToList();
            var model = new IndexVM
            {
                Skole = skole,
                SkolaId = skole.FirstOrDefault().Id,
                Razredi = new List<int> { 1, 2, 3, 4 },
                RazredId = 1,
                Takmicenja = _context.Takmicenje.Where(t => t.SkolaId.Equals(skole.FirstOrDefault().Id) && t.PredmetId.Equals(1)).Select(s => new TakmicenjeRow
                {
                    TakmicenjeId = s.Id,
                    Datum = s.Datum,
                    Skola = s.Skola.Naziv,
                    Razred = s.Predmet.Razred,
                    Predmet = s.Predmet.Naziv,
                    NajboljiUcenik = _context.TakmicenjeUcesnik.Where(u => u.TakmicenjeId.Equals(s.Id)).OrderByDescending(o => o.Bodovi).Select(r => $"{r.Takmicenje.Skola.Naziv} | {r.OdjeljenjeStavka.Odjeljenje.Oznaka} | {r.OdjeljenjeStavka.Ucenik.ImePrezime}").FirstOrDefault()
                }).ToList()
            };

            return View(model);
        }

        public ActionResult Takmicenja(int skolaId, int razredId)
        {
            var skole = _context.Skola.ToList();
            var model = new IndexVM
            {
                Skole = skole,
                SkolaId = skolaId,
                Razredi = new List<int> { 1, 2, 3, 4 },
                RazredId = razredId,
                Takmicenja = _context.Takmicenje.Where(t => t.SkolaId.Equals(skolaId) && t.Predmet.Razred.Equals(razredId)).Select(s => new TakmicenjeRow
                {
                    TakmicenjeId = s.Id,
                    Datum = s.Datum,
                    Skola = s.Skola.Naziv,
                    Razred = s.Predmet.Razred,
                    Predmet = s.Predmet.Naziv,
                    NajboljiUcenik = _context.TakmicenjeUcesnik.Where(u => u.TakmicenjeId.Equals(s.Id)).OrderByDescending(o => o.Bodovi).Select(r => $"{r.Takmicenje.Skola.Naziv} | {r.OdjeljenjeStavka.Odjeljenje.Oznaka} | {r.OdjeljenjeStavka.Ucenik.ImePrezime}").FirstOrDefault()
                }).ToList()
            };

            return PartialView("TakmicenjaPartial", model);
        }
    }
}