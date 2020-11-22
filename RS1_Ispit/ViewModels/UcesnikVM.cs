using RS1_Ispit_asp.net_core.EntityModels;
using System.Collections.Generic;

namespace RS1_Ispit_asp.net_core.ViewModels
{
    public class UcesnikVM
    {
        public List<UcesnikSelect> Ucesnici { get; set; }
        public int UcesnikId { get; set; }
        public int? Bodovi { get; set; }
        public int? TakmicenjeId { get; set; }
    }
    public class UcesnikSelect
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

    }
}
