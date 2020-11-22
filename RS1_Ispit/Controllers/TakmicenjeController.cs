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
            var model = new DodajTakmicenjeVM
            {
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
                if (UcenikProsjek(item.UcenikId))
                {
                    _context.TakmicenjeUcesnik.Add(new TakmicenjeUcesnik
                    {
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

        public IActionResult Rezultati(int id)
        {
            var model = _context.Takmicenje.Select(s => new RezultatiTakmicenjaVM
            {
                TakmicenjeId = s.Id,
                Skola = s.Skola.Naziv,
                Predmet = s.Predmet.Naziv,
                Razred = s.Predmet.Razred,
                Datum = s.Datum,
                Zakljucaj = s.Zakljucaj
            }).FirstOrDefault(t => t.TakmicenjeId.Equals(id));
            if (!model.Zakljucaj)
            {
                model.Ucesnici = _context.TakmicenjeUcesnik.Where(t => t.TakmicenjeId.Equals(id)).Select(su => new UcesniciRow
                {
                    UcesnikId = su.Id,
                    Odjeljenje = su.OdjeljenjeStavka.Odjeljenje.Oznaka,
                    BrojUDnevniku = su.OdjeljenjeStavka.BrojUDnevniku,
                    MaxBodovi = su.Bodovi,
                    Pristupio = su.Pristupio
                }).ToList();
            }
            return View(model);
        }

        public void Pristupio(int id)
        {
            var obj = _context.TakmicenjeUcesnik.FirstOrDefault(u => u.Id.Equals(id));
            obj.Pristupio = !obj.Pristupio;
            _context.SaveChanges();
        }

        public void Zakljucaj(int id)
        {
            var model = _context.Takmicenje.FirstOrDefault(t => t.Id.Equals(id));
            model.Zakljucaj = !model.Zakljucaj;
            _context.SaveChanges();
        }

        public IActionResult DodajUcesnika(int id)
        {
            var ucesnici = _context.TakmicenjeUcesnik;
            var ucesnik = ucesnici.FirstOrDefault();
            var model = new UcesnikVM
            {
                TakmicenjeId = id,
                UcesnikId = ucesnik.Id,
                Bodovi = ucesnik.Bodovi,
                Ucesnici = ucesnici.Select(s => new UcesnikSelect
                {
                    Id = s.Id,
                    Naziv = $"{s.OdjeljenjeStavka.Odjeljenje.Oznaka} - {s.OdjeljenjeStavka.Ucenik.ImePrezime} - {s.OdjeljenjeStavka.BrojUDnevniku}"
                }).ToList()
            };

            return PartialView("EditUcesnikaPartial", model);
        }

        public IActionResult EditUcesnika(int id, int bodovi)
        {
            var model = new UcesnikVM
            {
                UcesnikId = id,
                Bodovi = bodovi,
                Ucesnici = _context.TakmicenjeUcesnik.Select(s => new UcesnikSelect
                {
                    Id = s.Id,
                    Naziv = $"{s.OdjeljenjeStavka.Odjeljenje.Oznaka} - {s.OdjeljenjeStavka.Ucenik.ImePrezime} - {s.OdjeljenjeStavka.BrojUDnevniku}"
                }).ToList()
            };

            return PartialView("EditUcesnikaPartial", model);
        }

        [HttpPost]
        public void EditUcesnika(UcesnikVM obj)
        {
            var model = _context.TakmicenjeUcesnik.FirstOrDefault(a => a.Id.Equals(obj.UcesnikId));
            if (obj.TakmicenjeId == null)
            {
                model.Bodovi = obj.Bodovi;
            }
            else
            {
                _context.TakmicenjeUcesnik.Add(new TakmicenjeUcesnik
                {
                    TakmicenjeId = (int)obj.TakmicenjeId,
                    OdjeljenjeStavkaId = model.OdjeljenjeStavkaId,
                    Pristupio = true,
                    Bodovi = obj.Bodovi
                });
            }
            _context.SaveChanges();
        }

        //HELPER METODA
        private bool UcenikProsjek(int ucenikId)
        {
            var prosjek = _context.DodjeljenPredmet.Where(dp => dp.OdjeljenjeStavka.UcenikId.Equals(ucenikId)).Average(a => a.ZakljucnoKrajGodine);
            if (prosjek >= 4) return true; return false;
        }
    }
}