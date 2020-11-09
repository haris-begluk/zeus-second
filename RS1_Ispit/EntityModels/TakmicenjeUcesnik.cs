using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_Ispit_asp.net_core.EntityModels
{
    public class TakmicenjeUcesnik
    {
        public int Id { get; set; }
        public int TakmicenjeId { get; set; }
        [ForeignKey(nameof(TakmicenjeId))]
        public Takmicenje Takmicenje { get; set; }
        public int OdjeljenjeStavkaId { get; set; }
        [ForeignKey(nameof(OdjeljenjeStavkaId))]
        public OdjeljenjeStavka OdjeljenjeStavka { get; set; }
        //Dodatna polja 
        public int? Bodovi { get; set; }
        public bool Pristupio { get; set; }
    }
}
