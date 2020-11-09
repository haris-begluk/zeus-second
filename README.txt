============================================================================================================================
1. Change connection string in appsetting.json 
2. Change database structure, Add table Takmicenje and TakmicenjeUcesnik: 

Jedno Takmicenje pripada jednoj skoli i jednom predmetu. 
Dodatna polja su Datum, Razred i polje Zakljucaj(pogledati zadatke u pdf file-u) 

Jedan TakmicenjeUcesnik pripada samo jednom takmicenju i samo jednoj odjeljenju stavka 
Dodatna Polja su Bodovi koji moze sadrzavati null vrijednost i polje Pristupio(bool)