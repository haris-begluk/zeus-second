using Microsoft.AspNetCore.Mvc;
using RS1_Ispit_asp.net_core.EF;
using RS1_Ispit_asp.net_core.EntityModels;
using RS1_Ispit_asp.net_core.ViewModels;
using System;
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

        [HttpGet]
        public ActionResult DodajTakmicenje(int skolaId)
        {
            var predmeti = _context.Predmet.ToList();
            var model  = new DodajTakmicenjeVM{ 
            Skole = _context.Skola.ToList(), 
            SkolaId = skolaId, 
            Predmeti = predmeti, 
            PredmetId = predmeti.FirstOrDefault().Id, 
            Datum = DateTime.Now
            };
            return PartialView("DodajTakmicenjePartial", model);
        }

        [HttpPost]
        public IActionResult DodajTakmicenje(DodajTakmicenjeVM obj)
        {
            var predmeti = _context.Predmet.ToList();
            var model = new Takmicenje
            {
                SkolaId = obj.SkolaId,
                PredmetId = obj.PredmetId,
                Datum = DateTime.Now,
                Zakljucaj = false, 
            };
            var result = _context.Takmicenje.Add(model);
            var relacije = _context.DodjeljenPredmet
                .Where(dp => dp.ZakljucnoKrajGodine.Equals(5) && dp.OdjeljenjeStavka.Odjeljenje.SkolaID.Equals(result.Entity.SkolaId) && dp.PredmetId.Equals(result.Entity.PredmetId))
                .Select(s => new { s.OdjeljenjeStavkaId, s.OdjeljenjeStavka.UcenikId });
            foreach (var item in relacije)
            { 
                if(UcenikProsjek(item.UcenikId))
                {
                    _context.TakmicenjeUcesnik.Add(new TakmicenjeUcesnik { 
                    TakmicenjeId = result.Entity.Id, 
                    OdjeljenjeStavkaId = item.OdjeljenjeStavkaId, 
                    Pristupio = true, 
                    Bodovi = 0
                    });
                }

            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), "Takmicenje");

        }
        private bool UcenikProsjek(int ucenikId)
        {
            var prosjek = _context.DodjeljenPredmet.Where(dp => dp.OdjeljenjeStavka.UcenikId.Equals(ucenikId)).Average(a => a.ZakljucnoKrajGodine);
            if (prosjek >= 4) return true; return false;
        }

    }
}