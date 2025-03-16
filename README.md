Envanter Yonetim Paneli<br /><br />
Nedir?:<br />
-Bu web uygulamasi sirketler, okullar vs. gibi kuruluslarin envanterinde tuttuklari bilgisayarlarin kayitlarini tutmak ve yonetmek icin gelistirilen bir ASP.NET uygulamasidir.<br />
<br />
Neden?:<br />
-Su anda calistigim sirkette gordugum envanter karmasasi ve yonetiminin zorlugundan kaynakli aklimi kurcalayan bu durumu nasil cozebilecegimi dusunurken boyle bir projeyi evde kendimi gelistirebilmek icin kullanabilecegimi dusundum.<br />
<br />
Nicin?:<br />
Uygulamayi gelistirirmekteki amacim ASP.NET Core Web API, MSSQL, ASP.NET MVC teknolojilerini ogrenmek ve gercek hayat uygulamalarina gecirebilmek.<br />
<br />
Nasil calisir?:<br />
-Envanter kaydi tutulmasi istenen tum bilgisayarlarda EnvanterServis adindaki Windows servisi kurulur.<br />
-Bu servis 5 dakikada bir Envanter Yonetim Paneli'ne calistigi bilgisayara ait seri no, bilgisayar adi, islemci bilgisi gibi bilgileri json veri olarak gonderir.<br />
-EYP'deki EnvanterApi adindaki API bu gelen bilgileri isler ve EnvanterTablosu ve varsa ilgili bilgisayarin seri numarasiyla ayni addaki(yoksa API ayni adda bir tane olusturur) SQL tablolarina yazar.<br />
-EYP'ye erisebilmek icin yetkili kullanici bilgisi ile giris yapilir. Bu bolumu ileride Active Directory ile entegre etmeyi planliyorum. Eger kullanici bir sekilde oturumdan atilmissa(yani session bittiyse) kullanici giris ekranina geri yonlendirilir<br />
-EYP ana sayfasina erisildiginde kullaniciyi bir tablo karsilar. Bu tablo data gondermis olan tum bilgisayarlarin bulundugu EnvanterTablosu'dur. Istenirse bu kisimda secili sutunlardan siralama yapilabilir.<br />
-Her satirda bulunan bilgisayar icin satir sonlarinda Detay isimli tus bulunur ve bu tus ile secilen bilgisayarin gecmiste gonderdigi verilere ulasilabilir.<br />
-EYP ana sayfasinda bulunan bir buton ile Seri No ile Asset Eslestime ekranina erisilir. Bu kisimda kurulus kendisine ait olan envanter tutma birimi olan assetleri bilgisayarlarla eslestirebilir ve bu da envanter yonetiminde kolaylik saglar.<br />
-Devam edecek...
