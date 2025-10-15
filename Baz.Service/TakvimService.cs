using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.Service.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Baz.Model.Entity.ViewModel.KaynakTanimlariRezerveVM;

namespace Baz.Service
{
    public interface ITakvimService
    {
        // Define methods for the TakvimService here
        public Result<bool> EventSil(int id); //Rezervasyon Silme
        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet( KaynakRezervasyonCariDegerlerVM model); //Rezervasyon Kayıt
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele(); // rezervasyon Listeleme
        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle(KaynakRezervasyonCariDegerlerVM model);
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id); // Rezervasyon Veri getirme
        public Result<List<KaynakTanimlariRezerveVM>> KaynakSelectVeriGetir([FromBody] List<int> id);//Kaynak rezerve verilerini id olarak alıp selecte doldurur.
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir(); //Kaynak rezerve takvim verilerini getirir.
    }
    public class TakvimService : ITakvimService
    {
        private readonly ILoginUser _loginUser;
        private readonly IKaynakTanimlariService _kaynakTanimlariService;
        private readonly IKaynakTanimlariMedyalarService _kaynakTanimlariMedyalarService;
        private readonly IKaynakRezerveTanimlariService _kaynakRezerveTanimlariService;
        private readonly IKaynakRezervasyonCariDegerlerService _kaynakRezervasyonCariDegerlerService;
        private readonly IParamRezervasyonOnayStatuService _paramRezervasyonOnayStatuService;
        private readonly IKaynakGunIciIstisnaTanimlariService _kaynakGunIciIstisnaTanimlariService;
        private readonly IKaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService _kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService;
        private bool isDevelopment;

        public TakvimService(
            IKaynakTanimlariService kaynakTanimlariService,
            IKaynakTanimlariMedyalarService kaynakTanimlariMedyalarService,
            IKaynakRezerveTanimlariService kaynakRezerveTanimlariService,
            IKaynakRezervasyonCariDegerlerService kaynakRezervasyonCariDegerlerService,
            IParamRezervasyonOnayStatuService paramRezervasyonOnayStatuService,
            IKaynakGunIciIstisnaTanimlariService kaynakGunIciIstisnaTanimlariService,
            ILoginUser loginUser,
            IKaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService)
        {
            _kaynakTanimlariService = kaynakTanimlariService;
            _kaynakTanimlariMedyalarService = kaynakTanimlariMedyalarService;
            _kaynakRezerveTanimlariService = kaynakRezerveTanimlariService;
            // Assuming isDevelopment is set based on some configuration or environment variable
            isDevelopment = true; // This should be set based on your actual logic
            _kaynakRezervasyonCariDegerlerService = kaynakRezervasyonCariDegerlerService;
            _paramRezervasyonOnayStatuService = paramRezervasyonOnayStatuService;
            _kaynakGunIciIstisnaTanimlariService = kaynakGunIciIstisnaTanimlariService;
            _loginUser = loginUser;
            _kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService = kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService;
        }

        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir()
        {
            var KaynakDbo = _kaynakTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            var rezerve = _kaynakRezerveTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            var istisnaKaynak = _kaynakGunIciIstisnaTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            var kapasite = _kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();

            var KaynakRezerveBilgileri = KaynakDbo
                .Select(item => new KaynakTanimlariRezerveVM
                {
                    TabloID = item.TabloID,
                    KaynakTanimVM = new KaynakTanim
                    {
                        
                        KaynakAdi = item.KaynakAdi,
                        KaynakAciklama = item.KaynakAciklama,
                        ParamKaynakTipiID = item.ParamKaynakTipiID,
                        KaynakIkonID = 0
                    },

                    KaynakRezerveTanimVM = rezerve
                    .Where(r => r.KaynakTanimID == item.TabloID)
                    .Select(rez => new KaynakRezerveTanim
                    {
                        KaynakTanimID = rez.KaynakTanimID,
                        RezerveSaatBaslangicDegeri = rez.RezerveSaatBaslangicDegeri.ToString(),
                        RezerveSaatBitisDegeri = rez.RezerveSaatBitisDegeri.ToString(),
                        UygunGunTipleri = rez.UygunGunTipleri
                    }).FirstOrDefault(),

                    KapsiteVM = kapasite
                    .Where(x => x.KaynakRezerveTanimID == item.TabloID)
                    .Select(kap => new Kapsite
                    {
                        KaynakRezerveTanimID = kap.KaynakRezerveTanimID,
                        Kapasaite = kap.Kapasaite,
                        KapasiteBirimID = kap.KapasiteBirimID,
                    }).ToList(),

                    KaynakIstisnalariVM = istisnaKaynak
                    .Where(i => i.KaynakTanimID == item.TabloID)
                    .Select(i => new KaynakIstisnalari
                    {
                        IstisnaSaatBaslangicDegeri = i.IstisnaSaatBaslangicDegeri.ToString(),
                        IstisnaSaatBitisDegeri = i.IstisnaSaatBitisDegeri.ToString(),
                        IstisnaTarihBaslangicDegeri = i.IstisnaTarihBaslangicDegeri,
                        IstisnaTarihtBitisDegeri = i.IstisnaTarihtBitisDegeri
                    }).ToList()
                }).ToList();
            return KaynakRezerveBilgileri.ToResult();
        }
        
        public Result<List<KaynakTanimlariRezerveVM>> KaynakSelectVeriGetir( List<int> id)
        {
            var Rezerve = _kaynakRezerveTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            var kapasite = _kaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            var tumKaynakRezerveBilgileri = id
                .SelectMany(itemId => _kaynakTanimlariService.List(x => x.TabloID == itemId && x.AktifMi == 1 && x.SilindiMi == 0).Value)
                .Select(item =>
                {
                    var kaynakVm = new KaynakTanim
                    {
                        KaynakAdi = item.KaynakAdi,
                        KaynakAciklama = item.KaynakAciklama,
                        ParamKaynakTipiID = item.ParamKaynakTipiID,
                        KaynakIkonID = 0
                    };
                    var rezerveVm = Rezerve
                        .Where(r => r.KaynakTanimID == item.TabloID)
                        .Select(rez => new KaynakRezerveTanim
                        {
                            KaynakTanimID = rez.KaynakTanimID,
                            RezerveSaatBaslangicDegeri = rez.RezerveSaatBaslangicDegeri.ToString(),
                            RezerveSaatBitisDegeri = rez.RezerveSaatBitisDegeri.ToString(),
                            UygunGunTipleri = rez.UygunGunTipleri
                        })
                        .LastOrDefault();
                    var KapasiteVM = kapasite
                    .Where(x => x.KaynakRezerveTanimID == item.TabloID)
                        .Select(kap => new Kapsite
                        {
                            KaynakRezerveTanimID = kap.KaynakRezerveTanimID,
                            Kapasaite = kap.Kapasaite,
                            KapasiteBirimID = kap.KapasiteBirimID,
                        }).ToList();

                    var IstisnaKaynak = _kaynakGunIciIstisnaTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.KaynakTanimID == item.TabloID).Value;

                    var Istisna = IstisnaKaynak
                        .Select(i => new KaynakIstisnalari
                        {
                            IstisnaSaatBaslangicDegeri = i.IstisnaSaatBaslangicDegeri.ToString(),
                            IstisnaSaatBitisDegeri = i.IstisnaSaatBitisDegeri.ToString(),
                            IstisnaTarihBaslangicDegeri = i.IstisnaTarihBaslangicDegeri,
                            IstisnaTarihtBitisDegeri = i.IstisnaTarihtBitisDegeri
                        }).ToList();

                    return new KaynakTanimlariRezerveVM
                    {
                        TabloID = item.TabloID,
                        KaynakTanimVM = kaynakVm,
                        KaynakRezerveTanimVM = rezerveVm,
                        KaynakIstisnalariVM = Istisna,
                        KapsiteVM = KapasiteVM
                    };
                }).ToList();   
            //foreach (var ids in id)
            //{
            //    var KaynakDbo = _kaynakTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.TabloID == ids).Value.ToList();
            //    var Rezerve = _kaynakRezerveTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value.ToList();
            //    var IstisnaKaynak = _kaynakGunIciIstisnaTanimlariService.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.KaynakTanimID == ids).Value;
            //    var Istisna = new List<KaynakIstisnalari>();

            //    foreach (var item in KaynakDbo)
            //    {
            //        var kaynakVm = new KaynakTanim()
            //        {
            //            KaynakAdi = item.KaynakAdi,
            //            KaynakAciklama = item.KaynakAciklama,
            //            ParamKaynakTipiID = item.ParamKaynakTipiID,
            //            KaynakIkonID = 0
            //        };

            //        var rezerveVm = Rezerve
            //            .Where(r => r.KaynakTanimID == item.TabloID)
            //            .Select(rez => new KaynakRezerveTanim()
            //            {
            //                KaynakTanimID = rez.KaynakTanimID,
            //                RezerveSaatBaslangicDegeri = rez.RezerveSaatBaslangicDegeri.ToString(),
            //                RezerveSaatBitisDegeri = rez.RezerveSaatBitisDegeri.ToString(),
            //                UygunGunTipleri = rez.UygunGunTipleri,
            //            })
            //            .LastOrDefault();
            //        foreach (var i in IstisnaKaynak)
            //        {
            //            var istisna = new KaynakIstisnalari()
            //            {
            //                IstisnaSaatBaslangicDegeri = i.IstisnaSaatBaslangicDegeri.ToString(),
            //                IstisnaSaatBitisDegeri = i.IstisnaSaatBitisDegeri.ToString(),
            //                IstisnaTarihBaslangicDegeri = i.IstisnaTarihBaslangicDegeri,
            //                IstisnaTarihtBitisDegeri = i.IstisnaTarihtBitisDegeri,
            //            }; Istisna.Add(istisna);
            //        }
            //        var kaynakAna = new KaynakTanimlariRezerveVM()
            //        {
            //            TabloID = item.TabloID,
            //            KaynakTanimVM = kaynakVm,
            //            KaynakRezerveTanimVM = rezerveVm,
            //            KaynakIstisnalariVM = Istisna,
            //        }; tumKaynakRezerveBilgileri.Add(kaynakAna);
            //    }
            //}
            return tumKaynakRezerveBilgileri.ToResult();
        }
        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet( KaynakRezervasyonCariDegerlerVM model)
        {
            if (model.RezerveBaslangicTarihi < new DateTime(1753, 1, 1) || model.RezerveBitisTarihi < new DateTime(1753, 1, 1))
                throw new ArgumentException("Invalid date range.");

            var cagri = new KaynakRezervasyonCariDegerler
            {
                AktifMi = 1,
                SilindiMi = 0,
                KayitEdenID = _loginUser.KisiID,
                KurumID = _loginUser.KurumID,
                KisiID = _loginUser.KisiID,
                GuncellenmeTarihi = DateTime.Now,
                RezerveEdenKisiID = _loginUser.KisiID,
                RezerveEdenKurumID = _loginUser.KurumID,
                KayitTarihi = DateTime.Now,
                KaynakTanimID= model.KaynakTanimID,
                RezervasyonBaslik = model.RezervasyonBaslik,
                RezervasyonAciklama = model.RezervasyonAciklama,
                RezerveBaslangicZamani = TimeSpan.Parse(model.RezerveBaslangicZamani),
                RezerveBaslangicTarihi = model.RezerveBaslangicTarihi > SqlDateTime.MinValue.Value
                ? model.RezerveBaslangicTarihi
                : (DateTime?)null,

                RezerveBitisTarihi = DateTime.Now,
                RezerveBitisZamani = TimeSpan.Parse(model.RezerveBitisZamani),
                ParamRezervasyonOnyaStatuTipID = 1 //Deafult Beklemede

            };
            //foreach (var prop in typeof(KaynakRezervasyonCariDegerler).GetProperties())
            //{
            //    var val = prop.GetValue(cagriNew);
            //    Console.WriteLine($"{prop.Name}: {val}");
            //}
            _kaynakRezervasyonCariDegerlerService.Add(cagri);

            return model.ToResult();
        }

        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele()
        {
            var kaynak = _kaynakRezervasyonCariDegerlerService
                .List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.KaynakTanimID > 0)
                .Value;

            var kaynakVMList = kaynak.Select(k => new KaynakRezervasyonCariDegerlerVM
            {
                TabloID =k.TabloID,
                KaynakTanimID = k.KaynakTanimID,
                RezerveEdenKisiID = k.RezerveEdenKisiID,
                RezerveEdenKurumID = k.RezerveEdenKurumID,
                RezerveBaslangicTarihi = k.RezerveBaslangicTarihi,
                RezerveBaslangicZamani = k.RezerveBaslangicZamani?.ToString(),
                RezerveBitisTarihi = k.RezerveBitisTarihi,
                RezerveBitisZamani = k.RezerveBitisZamani?.ToString(),
                RezervasyonBaslik = k.RezervasyonBaslik,
                RezervasyonAciklama = k.RezervasyonAciklama,
                ParamRezervasyonOnyaStatuTipID = k.ParamRezervasyonOnyaStatuTipID
            }).ToList();
            return kaynakVMList.ToResult();
        }

        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id)
        {
            var kaynak = _kaynakRezervasyonCariDegerlerService
                .List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.TabloID == id)
                .Value;

            var kaynakVMList = kaynak.Select(k => new KaynakRezervasyonCariDegerlerVM
            {
                TabloID = k.TabloID,
                KaynakTanimID = k.KaynakTanimID,
                RezerveEdenKisiID = k.RezerveEdenKisiID,
                RezerveEdenKurumID = k.RezerveEdenKurumID,
                RezerveBaslangicTarihi = k.RezerveBaslangicTarihi,
                RezerveBaslangicZamani = k.RezerveBaslangicZamani?.ToString(),
                RezerveBitisTarihi = k.RezerveBitisTarihi,
                RezerveBitisZamani = k.RezerveBitisZamani?.ToString(),
                RezervasyonBaslik = k.RezervasyonBaslik,
                RezervasyonAciklama = k.RezervasyonAciklama,
                ParamRezervasyonOnyaStatuTipID = k.ParamRezervasyonOnyaStatuTipID
            }).ToList();
            return kaynakVMList.ToResult();
        }

        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle(KaynakRezervasyonCariDegerlerVM model)
        {

            var mevcutKaynakResult = _kaynakRezervasyonCariDegerlerService.List(
                x => x.AktifMi == 1 && x.SilindiMi == 0 && x.TabloID == model.TabloID
            ).Value;

            var guncellenecek = mevcutKaynakResult.FirstOrDefault();
            if (guncellenecek != null)
            {
                guncellenecek.GuncelleyenKisiID = _loginUser.KisiID;
                guncellenecek.GuncellenmeTarihi = DateTime.Now;
                guncellenecek.RezerveEdenKisiID = _loginUser.KisiID;
                guncellenecek.RezerveEdenKurumID = _loginUser.KurumID;
                guncellenecek.RezervasyonBaslik = model.RezervasyonBaslik;
                guncellenecek.RezervasyonAciklama = model.RezervasyonAciklama;
                guncellenecek.RezerveBaslangicZamani = TimeSpan.Parse(model.RezerveBaslangicZamani);
                guncellenecek.RezerveBaslangicTarihi = model.RezerveBaslangicTarihi > SqlDateTime.MinValue.Value
                    ? model.RezerveBaslangicTarihi
                    : (DateTime?)null;
                guncellenecek.RezerveBitisZamani = TimeSpan.Parse(model.RezerveBitisZamani);

                _kaynakRezervasyonCariDegerlerService.Update(guncellenecek);
            }
            else
            {
                var yeniKayit = new KaynakRezervasyonCariDegerler
                {
                    AktifMi = 1,
                    SilindiMi = 0,
                    KayitEdenID = _loginUser.KisiID,
                    KurumID = _loginUser.KurumID,
                    KisiID = _loginUser.KisiID,
                    GuncellenmeTarihi = DateTime.Now,
                    RezerveEdenKisiID = _loginUser.KisiID,
                    RezerveEdenKurumID = _loginUser.KurumID,
                    KayitTarihi = DateTime.Now,
                    KaynakTanimID = model.KaynakTanimID,
                    RezervasyonBaslik = model.RezervasyonBaslik,
                    RezervasyonAciklama = model.RezervasyonAciklama,
                    RezerveBaslangicZamani = TimeSpan.Parse(model.RezerveBaslangicZamani),
                    RezerveBaslangicTarihi = model.RezerveBaslangicTarihi > SqlDateTime.MinValue.Value
                        ? model.RezerveBaslangicTarihi
                        : (DateTime?)null,
                    RezerveBitisZamani = TimeSpan.Parse(model.RezerveBitisZamani),
                    RezerveBitisTarihi = model.RezerveBitisTarihi > SqlDateTime.MinValue.Value
                        ? model.RezerveBitisTarihi
                        : (DateTime?)null,
                    ParamRezervasyonOnyaStatuTipID = 1 // default
                };

                var eklenen = _kaynakRezervasyonCariDegerlerService.Add(yeniKayit).Value;
            }
            //foreach(var items in kaynak)
            //{
            //    if(kaynak != null)
            //    {
            //        items.GuncelleyenKisiID = _loginUser.KisiID;
            //        items.GuncellenmeTarihi = DateTime.Now;
            //        items.RezerveEdenKisiID = _loginUser.KisiID;
            //        items.RezerveEdenKurumID = _loginUser.KurumID;
            //        items.RezervasyonBaslik = model.RezervasyonBaslik;
            //        items.RezervasyonAciklama = model.RezervasyonAciklama;
            //        items.RezerveBaslangicZamani = TimeSpan.Parse(model.RezerveBaslangicZamani);
            //        items.RezerveBaslangicTarihi = model.RezerveBaslangicTarihi > SqlDateTime.MinValue.Value
            //        ? model.RezerveBaslangicTarihi
            //        : (DateTime?)null;
            //        items.RezerveBitisZamani = TimeSpan.Parse(model.RezerveBitisZamani);
            //        items.ParamRezervasyonOnyaStatuTipID = items.ParamRezervasyonOnyaStatuTipID; //Deafult Beklemede

            //        foreach (var prop in typeof(KaynakRezervasyonCariDegerler).GetProperties())
            //        {
            //            var val = prop.GetValue(items);
            //            Console.WriteLine($"{prop.Name}: {val}");
            //        }
            //        _kaynakRezervasyonCariDegerlerService.Update(items);
            //    }
            //    else
            //    {
            //        var cagriNew = new KaynakRezervasyonCariDegerler
            //        {
            //            AktifMi = 1,
            //            SilindiMi = 0,
            //            KayitEdenID = _loginUser.KisiID,
            //            KurumID = _loginUser.KurumID,
            //            KisiID = _loginUser.KisiID,
            //            GuncellenmeTarihi = DateTime.Now,
            //            RezerveEdenKisiID = _loginUser.KisiID,
            //            RezerveEdenKurumID = _loginUser.KurumID,
            //            KayitTarihi = DateTime.Now,
            //            KaynakTanimID = model.KaynakTanimID,
            //            RezervasyonBaslik = model.RezervasyonBaslik,
            //            RezervasyonAciklama = model.RezervasyonAciklama,
            //            RezerveBaslangicZamani = TimeSpan.Parse(model.RezerveBaslangicZamani),
            //            RezerveBaslangicTarihi = model.RezerveBaslangicTarihi > SqlDateTime.MinValue.Value
            //            ? model.RezerveBaslangicTarihi
            //            : (DateTime?)null,
            //            RezerveBitisTarihi = model.RezerveBitisTarihi > SqlDateTime.MinValue.Value
            //            ? model.RezerveBitisTarihi
            //            : (DateTime?)null,
            //            RezerveBitisZamani = TimeSpan.Parse(model.RezerveBitisZamani),
            //            ParamRezervasyonOnyaStatuTipID = 1 //Deafult Beklemede
            //        };
            //        //foreach (var prop in typeof(KaynakRezervasyonCariDegerler).GetProperties())
            //        //{
            //        //    var val = prop.GetValue(cagriNew);
            //        //    Console.WriteLine($"{prop.Name}: {val}");
            //        //}
            //        _kaynakRezervasyonCariDegerlerService.Add(cagriNew);
            //    }
            //}
            return model.ToResult();
        }

        public Result<bool> EventSil(int id)
        { 
            var kaynak  = _kaynakRezervasyonCariDegerlerService.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.TabloID == id).Value;
            foreach(var items in kaynak)
            {

                items.AktifMi = 0;
                items.SilindiMi = 1;
                items.SilenKisiID = _loginUser.KisiID;
                items.SilinmeTarihi = DateTime.Now;
                items.PasifEdenKisiID = _loginUser.KisiID;
                items.PasiflikTarihi = DateTime.Now;

                //foreach (var prop in typeof(KaynakRezervasyonCariDegerler).GetProperties())
                //{
                //    var val = prop.GetValue(items);
                //    Console.WriteLine($"{prop.Name}: {val}");
                //}
                _kaynakRezervasyonCariDegerlerService.Update(items);

            }
            return true.ToResult();
        }
    }

}
