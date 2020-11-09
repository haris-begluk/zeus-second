using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class Takmicenje 
    {
        public int Id { get; set; }
        public int SkolaId { get; set; }
        [ForeignKey(nameof(SkolaId))]
        public Skola Skola { get; set; }
        public int PredmetId { get; set; }
        [ForeignKey(nameof(PredmetId))]
        public Predmet Predmet { get; set; }
        //Ostali Atributi 
        public DateTime Datum { get; set; }
        public int Razred { get; set; }
        public bool Zakljucaj { get; set; }
    }
}
