using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baz.Service
{
    /// <summary>
    /// Malzeme Talep Genel Bilgileri servis arayüzü
    /// </summary>
    public interface IMalzemeTalepGenelBilgilerService : IService<MalzemeTalepGenelBilgiler>
    {
        /// <summary>
        /// Malzeme taleplerini filtreli olarak getiren metod
        /// </summary>
        /// <param name="request">Filtre parametreleri</param>
        /// <returns>Filtrelenmiş malzeme talep listesi (detaylı miktar bilgileri ile)</returns>
        Result<List<MalzemeTalepDetayResponse>> MalzemeTalepleriniGetir(MalzemeTalepFiltreleRequest request);

        /// <summary>
        /// Malzeme talebinde bulunma metodu
        /// </summary>
        /// <param name="request">Talep parametreleri</param>
        /// <returns>Talep işlemi sonucu</returns>
        Task<Result<string>> MalzemeTalepEt(MalzemeTalepEtRequest request);

        /// <summary>
        /// Toplu malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="request">Toplu hazırlama parametreleri</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        Result<string> TopluMalzemeleriHazirla(TopluMalzemeleriHazirlaRequest request);

        /// <summary>
        /// Malzeme talebini iade etme metodu
        /// </summary>
        /// <param name="request">İade parametreleri</param>
        /// <returns>İade işlemi sonucu</returns>
        Result<bool> MalzemeIadeEt(MalzemeIadeEtRequest request);

        /// <summary>
        /// Malzeme kabul etme metodu
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        Result<bool> MalKabulEt(int malzemeTalepSurecTakipID);

        /// <summary>
        /// Toplu malzeme kabul etme metodu
        /// </summary>
        /// <param name="request">Toplu kabul parametreleri</param>
        /// <returns>Kabul işlemi sonucu</returns>
        Result<string> TopluMalKabulEt(TopluMalKabulEtRequest request);

        /// <summary>
        /// Malzemeyi hasarlı olarak işaretleme metodu
        /// </summary>
        /// <param name="request">Hasarlı işaretleme parametreleri</param>
        /// <returns>İşaretleme işlemi sonucu</returns>
        Result<bool> HasarliOlarakIsaretle(HasarliOlarakIşaretleRequest request);

        /// <summary>
        /// Toplu SAT bilgisi güncelleme metodu
        /// </summary>
        /// <param name="request">Güncelleme parametreleri</param>
        /// <returns>Güncelleme işlemi sonucu</returns>
        Result<string> TopluSATBilgisiGuncelle(TopluSATBilgisiGuncellemeRequest request);
    }

    /// <summary>
    /// MalzemeTalepGenelBilgiler ile ilgili işlemleri yöneten servis sınıfı
    /// </summary>
    public class MalzemeTalepGenelBilgilerService : Service<MalzemeTalepGenelBilgiler>,
        IMalzemeTalepGenelBilgilerService
    {
        private readonly IRepository<MalzemeTalepSurecTakip> _surecTakipRepository;
        private readonly IRepository<MalzemeTalepMiktarTarihcesi> _miktarTarihcesiRepository;
        private readonly IRepository<MalzemeTalepSurecTakipNotlari> _surecTakipNotlariRepository;
        private readonly IRepository<SurecStatuleriBildirimTipleri> _surecStatuleriBildirimTipleriRepository;
        private readonly ILoginUser _loginUser;

        /// <summary>
        /// MalzemeTalepGenelBilgiler ile ilgili işlemleri yöneten servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="surecTakipRepository"></param>
        /// <param name="miktarTarihcesiRepository"></param>
        /// <param name="surecTakipNotlariRepository"></param>
        /// <param name="surecStatuleriBildirimTipleriRepository"></param>
        /// <param name="loginUser"></param>
        public MalzemeTalepGenelBilgilerService(
            IRepository<MalzemeTalepGenelBilgiler> repository,
            IDataMapper dataMapper,
            IServiceProvider serviceProvider,
            ILogger<MalzemeTalepGenelBilgilerService> logger,
            IRepository<MalzemeTalepSurecTakip> surecTakipRepository,
            IRepository<MalzemeTalepMiktarTarihcesi> miktarTarihcesiRepository,
            IRepository<MalzemeTalepSurecTakipNotlari> surecTakipNotlariRepository,
            IRepository<SurecStatuleriBildirimTipleri> surecStatuleriBildirimTipleriRepository,
            ILoginUser loginUser) : base(repository, dataMapper, serviceProvider, logger)
        {
            _surecTakipRepository = surecTakipRepository;
            _miktarTarihcesiRepository = miktarTarihcesiRepository;
            _surecTakipNotlariRepository = surecTakipNotlariRepository;
            _surecStatuleriBildirimTipleriRepository = surecStatuleriBildirimTipleriRepository;
            _loginUser = loginUser;
        }

        public Result<List<MalzemeTalepDetayResponse>> MalzemeTalepleriniGetir(MalzemeTalepFiltreleRequest request)
        {
            try
            {
                IQueryable<MalzemeTalepSurecTakip> surecQuery = _surecTakipRepository.List(x => x.AktifMi == 1);

                // 1. ADIM: Ek talep filtresi - Malzeme ID'lerini belirle
                List<int> filtreliMalzemeIDs = null;

                if (request.SadeceEkTalepleriGetir)
                {
                    filtreliMalzemeIDs = _repository.List(x =>
                            x.BaglantiliMalzemeTalebiEssizID > 0 && x.AktifMi == 1)
                        .Select(x => x.MalzemeTalebiEssizID)
                        .ToList();
                }
                else
                {
                    filtreliMalzemeIDs = _repository.List(x =>
                            x.BaglantiliMalzemeTalebiEssizID == 0 && x.AktifMi == 1)
                        .Select(x => x.MalzemeTalebiEssizID)
                        .ToList();
                }

                // Süreç query'sini filtrele
                surecQuery = surecQuery.Where(x => filtreliMalzemeIDs.Contains(x.MalzemeTalebiEssizID));

                // Statü filtresi
                if (request.MalzemeTalepEtGetir)
                {
                    var statusOneOrTwoIds = surecQuery.Where(x =>
                            x.ParamTalepSurecStatuID == 1 ||
                            x.ParamTalepSurecStatuID == 2)
                        .Select(x => x.TabloID)
                        .ToList();

                    var allMalzemeler = _repository.List(x =>
                            filtreliMalzemeIDs.Contains(x.MalzemeTalebiEssizID) && x.AktifMi == 1)
                        .ToList();

                    var kalanMiktarVarIds = new List<int>();

                    foreach (var malzeme in allMalzemeler)
                    {
                        // SevkID bazında MAX alarak hesapla (split sonrası çift saymayı önle)
                        var tumMiktarKayitlari = _miktarTarihcesiRepository.List(x =>
                                x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .ToList();

                        var sevkEdilenToplam = tumMiktarKayitlari
                            .GroupBy(x => x.SevkID ?? "NOSEVK")
                            .Sum(g => g.Max(x => x.SevkEdilenMiktar));

                        if (malzeme.MalzemeOrijinalTalepEdilenMiktar > sevkEdilenToplam)
                        {
                            kalanMiktarVarIds.Add(malzeme.MalzemeTalebiEssizID);
                        }
                    }

                    var kalanMiktarSureclerIds = _surecTakipRepository.List(x =>
                            kalanMiktarVarIds.Contains(x.MalzemeTalebiEssizID) &&
                            filtreliMalzemeIDs.Contains(x.MalzemeTalebiEssizID) &&
                            x.AktifMi == 1)
                        .AsEnumerable()
                        .GroupBy(x => x.MalzemeTalebiEssizID)
                        .Select(g => g.OrderByDescending(x => x.TabloID).First().TabloID)
                        .ToList();

                    var combinedIds = statusOneOrTwoIds.Union(kalanMiktarSureclerIds).ToList();

                    surecQuery = _surecTakipRepository.List(x =>
                        combinedIds.Contains(x.TabloID) &&
                        filtreliMalzemeIDs.Contains(x.MalzemeTalebiEssizID) &&
                        x.AktifMi == 1);

                    if (request.TalepSurecStatuIDs != null && request.TalepSurecStatuIDs.Any())
                    {
                        surecQuery =
                            surecQuery.Where(x => request.TalepSurecStatuIDs.Contains(x.ParamTalepSurecStatuID));
                    }
                }
                else if (request.TalepSurecStatuIDs != null && request.TalepSurecStatuIDs.Any())
                {
                    surecQuery = surecQuery.Where(x => request.TalepSurecStatuIDs.Contains(x.ParamTalepSurecStatuID));
                }

                var surecKayitlari = surecQuery.ToList();

                // ÖZEL DURUM: Statü 4 veya 6 için gruplama YAPMA
                bool gruplamayiAtla = request.TalepSurecStatuIDs != null &&
                                      request.TalepSurecStatuIDs.Any(s => s == 4 || s == 6);

                if (gruplamayiAtla)
                {
                    // GRUPLAMA YOK - Her kayıt ayrı satır
                    var sonuclar = new List<MalzemeTalepDetayResponse>();

                    foreach (var surecTakip in surecKayitlari)
                    {
                        var malzeme = _repository.List(x =>
                            x.MalzemeTalebiEssizID == surecTakip.MalzemeTalebiEssizID &&
                            x.AktifMi == 1).FirstOrDefault();

                        if (malzeme == null) continue;

                        // Search text filtresi
                        if (!string.IsNullOrEmpty(request.SearchText))
                        {
                            var searchTerm = request.SearchText.ToLower();
                            if (!malzeme.MalzemeKodu.ToLower().Contains(searchTerm) &&
                                !malzeme.MalzemeIsmi.ToLower().Contains(searchTerm) &&
                                !malzeme.SATSiraNo.ToLower().Contains(searchTerm))
                            {
                                continue;
                            }
                        }

                        var miktarTarihcesi = _miktarTarihcesiRepository.List(x =>
                                x.MalzemeTalepSurecTakipID == surecTakip.TabloID &&
                                x.AktifMi == 1)
                            .FirstOrDefault();

                        if (miktarTarihcesi == null) continue;

                        // Bu kaydın miktarları
                        var sevkEdilenMiktar = miktarTarihcesi.SevkEdilenMiktar;
                        var islenenMiktar = (int)miktarTarihcesi.IslenenMiktar;
                        var buKaydinKalanMiktari = miktarTarihcesi.KalanMiktar;

                        var sevkID = miktarTarihcesi.SevkID;
                        var hazirId = miktarTarihcesi.HazirId;
                        var onayId = miktarTarihcesi.OnayId;

                        // *** YENİ: Departman bilgisi ***
                        var departmanSevkiyatlari = new List<DepartmanSevkiyatDetay>
                {
                    new DepartmanSevkiyatDetay
                    {
                        DepartmanID = miktarTarihcesi.MalzemeSevkTalebiYapanDepartmanID,
                        TalepEdilenMiktar = sevkEdilenMiktar,
                        SevkZamani = miktarTarihcesi.SevkZamani
                    }
                };

                        // Tüm miktar kayıtlarını al
                        var tumMiktarKayitlari = _miktarTarihcesiRepository.List(x =>
                                x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID &&
                                x.AktifMi == 1)
                            .ToList();

                        // Bu SevkID için toplam işlenen miktar (toplam kalan hesabı için)
                        var sevkIDBazindaToplamIslenen = !string.IsNullOrEmpty(sevkID)
                            ? tumMiktarKayitlari
                                .Where(x => x.SevkID == sevkID)
                                .Sum(x => (int)x.IslenenMiktar)
                            : islenenMiktar;

                        // Bu SevkID için hazırlanabilecek miktar = Orijinal talep - Toplam işlenen
                        var sevkIDBazindaHazirlanabilecek = sevkEdilenMiktar - sevkIDBazindaToplamIslenen;

                        // Toplam sevk edilen = SevkID bazında MAX alarak hesapla (çift saymayı önle)
                        var toplamSevkEdilen = tumMiktarKayitlari
                            .GroupBy(x => x.SevkID ?? "NOSEVK")
                            .Sum(g => g.Max(x => x.SevkEdilenMiktar));

                        // Kalan miktar = Orijinal talep - Toplam sevk edilen (talep edilebilir miktar)
                        var kalanMiktar = Math.Max(0, malzeme.MalzemeOrijinalTalepEdilenMiktar - toplamSevkEdilen);

                        var enSonNot = _surecTakipNotlariRepository.List(n =>
                                n.MalzemeTalepSurecTakipID == surecTakip.TabloID && n.AktifMi == 1)
                            .OrderByDescending(n => n.TabloID)
                            .FirstOrDefault();

                        string bildirimTipiTanimlama = null;
                        if (enSonNot?.SurecStatuBildirimTipiID != null && enSonNot.SurecStatuBildirimTipiID > 0)
                        {
                            bildirimTipiTanimlama = _surecStatuleriBildirimTipleriRepository.List(bt =>
                                    bt.TabloID == enSonNot.SurecStatuBildirimTipiID && bt.AktifMi == 1)
                                .FirstOrDefault()?.BildirimTipiTanimlama;
                        }

                        sonuclar.Add(new MalzemeTalepDetayResponse
                        {
                            MalzemeTalepSurecTakipID = surecTakip.TabloID,
                            GrupIcindekiSurecTakipIDler = new List<int> { surecTakip.TabloID },
                            MalzemeTalebiEssizID = malzeme.MalzemeTalebiEssizID,
                            MalzemeTalep = malzeme,
                            TalepEdilenMiktar = sevkEdilenMiktar,
                            BuKaydinMiktari = islenenMiktar,
                            IslenenMiktar = islenenMiktar,
                            HazirlanabilecekMiktar = sevkIDBazindaHazirlanabilecek,
                            ToplamSevkEdilenMiktar = toplamSevkEdilen,
                            KalanMiktar = kalanMiktar,
                            ParamTalepSurecStatuID = surecTakip.ParamTalepSurecStatuID,
                            SurecOlusturmaTarihi = surecTakip.SurecTetiklenmeZamani,
                            SurecStatuGirilenNot = enSonNot?.SurecStatuGirilenNot,
                            SurecStatuBildirimTipiID = enSonNot?.SurecStatuBildirimTipiID,
                            BildirimTipiTanimlama = bildirimTipiTanimlama,
                            SevkID = sevkID,
                            HazirId = hazirId,
                            OnayId = onayId,
                            SevkIDBazindaKayitSayisi = 1,
                            DepartmanSevkiyatlari = departmanSevkiyatlari // ← YENİ
                        });
                    }

                    return sonuclar.ToResult();
                }
                else
                {
                    // MEVCUT GRUPLAMA LOJİĞİ (Statü 1,2,3 için)
                    var tumSonuclar = new List<(
                        MalzemeTalepSurecTakip surecTakip,
                        MalzemeTalepGenelBilgiler malzeme,
                        int talepEdilenMiktar,
                        decimal islenenMiktar,
                        int toplamSevkEdilen,
                        int kalanMiktar,
                        string sevkID,
                        string hazirId,
                        string onayId,
                        string surecStatuGirilenNot,
                        int? surecStatuBildirimTipiID,
                        string bildirimTipiTanimlama,
                        int departmanID, // ← YENİ
                        DateTime? sevkZamani // ← YENİ
                        )>();

                    foreach (var surecTakip in surecKayitlari)
                    {
                        var malzeme = _repository.List(x =>
                            x.MalzemeTalebiEssizID == surecTakip.MalzemeTalebiEssizID &&
                            x.AktifMi == 1).FirstOrDefault();

                        if (malzeme == null) continue;

                        // Search text filtresi
                        if (!string.IsNullOrEmpty(request.SearchText))
                        {
                            var searchTerm = request.SearchText.ToLower();
                            if (!malzeme.MalzemeKodu.ToLower().Contains(searchTerm) &&
                                !malzeme.MalzemeIsmi.ToLower().Contains(searchTerm) &&
                                !malzeme.SATSiraNo.ToLower().Contains(searchTerm))
                            {
                                continue;
                            }
                        }

                        var miktarTarihcesi = _miktarTarihcesiRepository.List(x =>
                                x.MalzemeTalepSurecTakipID == surecTakip.TabloID &&
                                x.AktifMi == 1)
                            .FirstOrDefault();

                        var sevkID = miktarTarihcesi?.SevkID;
                        var hazirId = miktarTarihcesi?.HazirId;
                        var onayId = miktarTarihcesi?.OnayId;
                        var departmanID = miktarTarihcesi?.MalzemeSevkTalebiYapanDepartmanID ?? 0; // ← YENİ
                        var sevkZamani = miktarTarihcesi?.SevkZamani; // ← YENİ

                        // Tüm miktar kayıtlarını al
                        var tumMiktarKayitlari = _miktarTarihcesiRepository.List(x =>
                                x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID &&
                                x.AktifMi == 1)
                            .ToList();

                        // Bu SevkID için orijinal talep miktarı (MAX - hepsi aynı değeri taşır)
                        var talepEdilenMiktar = !string.IsNullOrEmpty(sevkID)
                            ? tumMiktarKayitlari
                                .Where(x => x.SevkID == sevkID)
                                .Max(x => x.SevkEdilenMiktar)
                            : miktarTarihcesi?.SevkEdilenMiktar ?? 0;

                        // Bu SevkID için toplam işlenen miktar (tüm kayıtların IslenenMiktar toplamı)
                        var islenenMiktar = !string.IsNullOrEmpty(sevkID)
                            ? tumMiktarKayitlari
                                .Where(x => x.SevkID == sevkID)
                                .Sum(x => x.IslenenMiktar)
                            : miktarTarihcesi?.IslenenMiktar ?? 0;

                        // Toplam sevk edilen = SevkID bazında MAX alarak hesapla (çift saymayı önle)
                        var toplamSevkEdilen = tumMiktarKayitlari
                            .GroupBy(x => x.SevkID ?? "NOSEVK")
                            .Sum(g => g.Max(x => x.SevkEdilenMiktar));

                        // Kalan miktar = Orijinal talep - Toplam sevk edilen (talep edilebilir miktar)
                        var kalanMiktar = Math.Max(0, malzeme.MalzemeOrijinalTalepEdilenMiktar - toplamSevkEdilen);

                        var enSonNot = _surecTakipNotlariRepository.List(n =>
                                n.MalzemeTalepSurecTakipID == surecTakip.TabloID && n.AktifMi == 1)
                            .OrderByDescending(n => n.TabloID)
                            .FirstOrDefault();

                        string bildirimTipiTanimlama = null;
                        if (enSonNot?.SurecStatuBildirimTipiID != null && enSonNot.SurecStatuBildirimTipiID > 0)
                        {
                            bildirimTipiTanimlama = _surecStatuleriBildirimTipleriRepository.List(bt =>
                                    bt.TabloID == enSonNot.SurecStatuBildirimTipiID && bt.AktifMi == 1)
                                .FirstOrDefault()?.BildirimTipiTanimlama;
                        }

                        tumSonuclar.Add((
                            surecTakip,
                            malzeme,
                            talepEdilenMiktar,
                            islenenMiktar,
                            toplamSevkEdilen,
                            kalanMiktar,
                            sevkID,
                            hazirId,
                            onayId,
                            enSonNot?.SurecStatuGirilenNot,
                            enSonNot?.SurecStatuBildirimTipiID,
                            bildirimTipiTanimlama,
                            departmanID, // ← YENİ
                            sevkZamani // ← YENİ
                        ));
                    }

                    // ADIM 2: SevkID + Statü bazında grupla
                    var gruplanmisSonuclar = tumSonuclar
                        .GroupBy(x => new
                        {
                            x.malzeme.MalzemeTalebiEssizID,
                            SevkID = x.sevkID ?? "NOSEVK",
                            x.surecTakip.ParamTalepSurecStatuID
                        })
                        .Select(g =>
                        {
                            var seciliKayit = g.OrderByDescending(x => x.surecTakip.TabloID).First();
                            var sevkIDBazindaTalepMiktari = g.Max(x => x.talepEdilenMiktar);
                            var sevkIDBazindaToplamIslenen = g.Max(x => x.islenenMiktar);
                            var sevkIDBazindaHazirlanabilecek = sevkIDBazindaTalepMiktari - (int)sevkIDBazindaToplamIslenen;
                            var sevkIDBazindaKayitSayisi = g.Count();
                            var grupIcindekiIDler = g.Select(x => x.surecTakip.TabloID).ToList();

                            // *** YENİ: Departman bazında grupla ***
                            var departmanSevkiyatlari = g
                                .GroupBy(x => x.departmanID)
                                .Select(deptGroup => new DepartmanSevkiyatDetay
                                {
                                    DepartmanID = deptGroup.Key,
                                    TalepEdilenMiktar = deptGroup.Sum(x => x.talepEdilenMiktar), // Bu departmanın talep ettiği miktar
                                    SevkZamani = deptGroup.Min(x => x.sevkZamani) // İlk sevkiyat tarihi
                                })
                                .ToList();

                            return new MalzemeTalepDetayResponse
                            {
                                MalzemeTalepSurecTakipID = seciliKayit.surecTakip.TabloID,
                                GrupIcindekiSurecTakipIDler = grupIcindekiIDler,
                                MalzemeTalebiEssizID = seciliKayit.malzeme.MalzemeTalebiEssizID,
                                MalzemeTalep = seciliKayit.malzeme,
                                TalepEdilenMiktar = sevkIDBazindaTalepMiktari,
                                BuKaydinMiktari = sevkIDBazindaHazirlanabilecek,
                                IslenenMiktar = sevkIDBazindaToplamIslenen,
                                HazirlanabilecekMiktar = sevkIDBazindaHazirlanabilecek,
                                ToplamSevkEdilenMiktar = seciliKayit.toplamSevkEdilen,
                                KalanMiktar = seciliKayit.kalanMiktar,
                                ParamTalepSurecStatuID = seciliKayit.surecTakip.ParamTalepSurecStatuID,
                                SurecOlusturmaTarihi = g.Min(x => x.surecTakip.SurecTetiklenmeZamani),
                                SurecStatuGirilenNot = seciliKayit.surecStatuGirilenNot,
                                SurecStatuBildirimTipiID = seciliKayit.surecStatuBildirimTipiID,
                                BildirimTipiTanimlama = seciliKayit.bildirimTipiTanimlama,
                                SevkID = string.IsNullOrEmpty(seciliKayit.sevkID) ? null : seciliKayit.sevkID,
                                HazirId = seciliKayit.hazirId,
                                OnayId = seciliKayit.onayId,
                                SevkIDBazindaKayitSayisi = sevkIDBazindaKayitSayisi,
                                DepartmanSevkiyatlari = departmanSevkiyatlari // ← YENİ
                            };
                        })
                        .Where(x =>
                        {
                            if (x.ParamTalepSurecStatuID == 3)
                            {
                                if (request.MalzemeTalepEtGetir)
                                {
                                    return x.KalanMiktar > 0;
                                }
                                else
                                {
                                    return x.HazirlanabilecekMiktar > 0;
                                }
                            }
                            return true;
                        })
                        .ToList();

                    return gruplanmisSonuclar.ToResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MalzemeTalepleriniGetir hatası: {Message}", ex.Message);
                throw new Exception("Malzeme talepleri getirilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzeme talebinde bulunma metodu
        /// </summary>
        /// <param name="request">Talep parametreleri</param>
        /// <returns>Talep işlemi sonucu</returns>
        public async Task<Result<string>> MalzemeTalepEt(MalzemeTalepEtRequest request)
        {
            try
            {
                if (request == null || request.TalepItems == null || !request.TalepItems.Any())
                {
                    throw new Exception("Lütfen en az bir malzeme seçiniz.");
                }

                // Tüm işlem için tek bir SevkID üret
                var sevkID = YeniSevkIDUret();

                var projeBazindaSayim = new Dictionary<int, int>();
                var basariliIslemSayisi = 0;
                var hataliIslemler = new List<string>();
                var normalTalepSayisi = 0;
                var ekTalepSayisi = 0;

                foreach (var item in request.TalepItems)
                {
                    try
                    {
                        if (item.SevkEdilenMiktar <= 0)
                        {
                            hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: Geçersiz miktar");
                            continue;
                        }

                        // 1. Ana malzeme kaydı
                        var malzemeTalep = List(x =>
                                x.MalzemeTalebiEssizID == item.MalzemeTalebiEssizID && x.AktifMi == 1).Value
                            .FirstOrDefault();

                        if (malzemeTalep == null)
                        {
                            hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: Malzeme talebi bulunamadı");
                            continue;
                        }

                        // 2. Kalan miktar hesaplama
                        var oncekiSevkler = _miktarTarihcesiRepository.List(x =>
                            x.MalzemeTalebiEssizID == item.MalzemeTalebiEssizID && x.AktifMi == 1).ToList();

                        var toplamSevkEdilen = oncekiSevkler.Sum(x => x.SevkEdilenMiktar);
                        var mevcutKalanMiktar = malzemeTalep.MalzemeOrijinalTalepEdilenMiktar - toplamSevkEdilen;

                        // 3. Normal ve ek talep miktarlarını ayır
                        int normalMiktar = 0;
                        int ekTalepMiktar = 0;

                        if (item.SevkEdilenMiktar <= mevcutKalanMiktar)
                        {
                            // Tamamen normal talep
                            normalMiktar = item.SevkEdilenMiktar;
                        }
                        else if (mevcutKalanMiktar > 0)
                        {
                            // Kısmen normal, kısmen ek talep
                            normalMiktar = mevcutKalanMiktar;
                            ekTalepMiktar = item.SevkEdilenMiktar - mevcutKalanMiktar;
                        }
                        else
                        {
                            // Tamamen ek talep
                            ekTalepMiktar = item.SevkEdilenMiktar;
                        }

                        // 4. NORMAL MİKTAR İÇİN İŞLEM
                        if (normalMiktar > 0)
                        {
                            var enSonSurecTakip = _surecTakipRepository.List(x =>
                                    x.MalzemeTalebiEssizID == item.MalzemeTalebiEssizID && x.AktifMi == 1)
                                .OrderByDescending(x => x.TabloID)
                                .FirstOrDefault();

                            MalzemeTalepSurecTakip aktifSurecTakip = null;

                            if (enSonSurecTakip != null &&
                                (enSonSurecTakip.ParamTalepSurecStatuID == 1 ||
                                 enSonSurecTakip.ParamTalepSurecStatuID == 2))
                            {
                                // İlk kez talep ediliyor → Mevcut kaydı güncelle
                                enSonSurecTakip.ParamTalepSurecStatuID = 3;
                                enSonSurecTakip.SurecTetiklenmeZamani = DateTime.Now;
                                enSonSurecTakip.SurecTetikleyenKisiID = _loginUser?.KisiID ?? 0;
                                enSonSurecTakip.GuncellenmeTarihi = DateTime.Now;
                                enSonSurecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 0;

                                _surecTakipRepository.Update(enSonSurecTakip);
                                _surecTakipRepository.SaveChanges();

                                aktifSurecTakip = enSonSurecTakip;
                            }
                            else
                            {
                                // Paralel süreç → Yeni kayıt oluştur
                                var yeniSurecTakip = new MalzemeTalepSurecTakip
                                {
                                    MalzemeTalebiEssizID = item.MalzemeTalebiEssizID,
                                    ParamTalepSurecStatuID = 3,
                                    SurecTetiklenmeZamani = DateTime.Now,
                                    SurecTetikleyenKisiID = _loginUser?.KisiID ?? 0,
                                    AktifMi = 1,
                                    SilindiMi = 0,
                                    DilID = 1,
                                    KisiID = _loginUser?.KisiID ?? 0,
                                    KurumID = _loginUser?.KurumID ?? 1,
                                    KayitTarihi = DateTime.Now,
                                    KayitEdenID = _loginUser?.KisiID ?? 0,
                                    GuncellenmeTarihi = DateTime.Now,
                                    GuncelleyenKisiID = _loginUser?.KisiID ?? 0
                                };

                                _surecTakipRepository.Add(yeniSurecTakip);
                                _surecTakipRepository.SaveChanges();

                                aktifSurecTakip = yeniSurecTakip;
                            }

                            // Miktar tarihçesi
                            var kalanMiktar = Math.Max(0, mevcutKalanMiktar - normalMiktar);

                            var miktarTarihcesi = new MalzemeTalepMiktarTarihcesi
                            {
                                MalzemeTalebiEssizID = item.MalzemeTalebiEssizID,
                                MalzemeTalepSurecTakipID = aktifSurecTakip.TabloID,
                                SevkEdilenMiktar = normalMiktar,
                                KalanMiktar = kalanMiktar,
                                SevkZamani = DateTime.Now,
                                SevkTalepEdenKisiID = _loginUser?.KisiID ?? 0,
                                MalzemeSevkTalebiYapanDepartmanID = item.MalzemeSevkTalebiYapanDepartmanID,
                                MalzemeSevkTalebiYapanKisiID = _loginUser?.KisiID ?? 0,
                                SevkID = sevkID,
                                AktifMi = 1,
                                SilindiMi = 0,
                                DilID = 1,
                                KisiID = _loginUser?.KisiID ?? 0,
                                KurumID = _loginUser?.KurumID ?? 1,
                                KayitTarihi = DateTime.Now,
                                KayitEdenID = _loginUser?.KisiID ?? 0,
                                GuncellenmeTarihi = DateTime.Now,
                                GuncelleyenKisiID = _loginUser?.KisiID ?? 0
                            };

                            _miktarTarihcesiRepository.Add(miktarTarihcesi);
                            _miktarTarihcesiRepository.SaveChanges();

                            normalTalepSayisi++;
                        }

                        // 5. EK TALEP MİKTARI İÇİN İŞLEM
                        if (ekTalepMiktar > 0)
                        {
                            var yeniMalzemeTalebiEssizID = YeniMalzemeTalebiEssizIDUret();

                            // Yeni MalzemeTalepGenelBilgiler kaydı oluştur
                            var yeniMalzemeTalep = new MalzemeTalepGenelBilgiler
                            {
                                ProjeKodu = malzemeTalep.ProjeKodu,
                                MalzemeTalebiEssizID = yeniMalzemeTalebiEssizID,
                                TalepGirenKisiKod = malzemeTalep.TalepGirenKisiKod,
                                ParamDepoID = malzemeTalep.ParamDepoID,
                                MalzemeKodu = malzemeTalep.MalzemeKodu,
                                MalzemeIsmi = malzemeTalep.MalzemeIsmi,
                                SATOlusturmaTarihi = malzemeTalep.SATOlusturmaTarihi,
                                SATSeriNo = malzemeTalep.SATSeriNo,
                                SATSiraNo = malzemeTalep.SATSiraNo,
                                MalzemeOrijinalTalepEdilenMiktar = ekTalepMiktar,
                                BaglantiliMalzemeTalebiEssizID = malzemeTalep.MalzemeTalebiEssizID, //BAĞLANTI
                                BuTalebiKarsilayanSATSeriNo = null,
                                BuTalebiKarsilayanSATSiraNo = null,
                                AktifMi = 1,
                                SilindiMi = 0,
                                DilID = 1,
                                KisiID = _loginUser?.KisiID ?? 0,
                                KurumID = _loginUser?.KurumID ?? 1,
                                KayitTarihi = DateTime.Now,
                                KayitEdenID = _loginUser?.KisiID ?? 0,
                                GuncellenmeTarihi = DateTime.Now,
                                GuncelleyenKisiID = _loginUser?.KisiID ?? 0,
                                Aciklama = malzemeTalep.Aciklama,
                                Kod = malzemeTalep.Kod,
                                SATCariHesap = malzemeTalep.SATCariHesap
                            };

                            _repository.Add(yeniMalzemeTalep);
                            _repository.SaveChanges();

                            // Yeni talep için MalzemeTalepSurecTakip kaydı (Statü 1 - Gelmedi)
                            var yeniSurecTakip = new MalzemeTalepSurecTakip
                            {
                                MalzemeTalebiEssizID = yeniMalzemeTalebiEssizID,
                                ParamTalepSurecStatuID = 1, //Gelmedi
                                SurecTetiklenmeZamani = DateTime.Now,
                                SurecTetikleyenKisiID = _loginUser?.KisiID ?? 0,
                                AktifMi = 1,
                                SilindiMi = 0,
                                DilID = 1,
                                KisiID = _loginUser?.KisiID ?? 0,
                                KurumID = _loginUser?.KurumID ?? 1,
                                KayitTarihi = DateTime.Now,
                                KayitEdenID = _loginUser?.KisiID ?? 0,
                                GuncellenmeTarihi = DateTime.Now,
                                GuncelleyenKisiID = _loginUser?.KisiID ?? 0,
                            };

                            _surecTakipRepository.Add(yeniSurecTakip);
                            _surecTakipRepository.SaveChanges();

                            ekTalepSayisi++;

                            _logger.LogInformation(
                                $"Ek talep oluşturuldu: Orijinal ID={malzemeTalep.MalzemeTalebiEssizID}, Yeni ID={yeniMalzemeTalebiEssizID}, Miktar={ekTalepMiktar}");
                        }

                        // Proje bazında sayım (sadece normal talep için)
                        if (normalMiktar > 0)
                        {
                            if (projeBazindaSayim.ContainsKey(malzemeTalep.ProjeKodu))
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu]++;
                            }
                            else
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu] = 1;
                            }
                        }

                        basariliIslemSayisi++;
                    }
                    catch (Exception ex)
                    {
                        hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: {ex.Message}");
                        _logger.LogError(ex, "TopluMalzemeTalepEt - ID {MalzemeTalebiEssizID} için hata",
                            item.MalzemeTalebiEssizID);
                    }
                }

                // Response mesajı oluştur
                if (basariliIslemSayisi == 0)
                {
                    throw new Exception("Hiçbir talep işlemi başarılı olmadı. Hatalar: " +
                                        string.Join("; ", hataliIslemler));
                }

                // Proje bazında mesaj oluştur
                var projeMesajlari = new List<string>();
                foreach (var kvp in projeBazindaSayim.OrderBy(x => x.Key))
                {
                    projeMesajlari.Add($"{kvp.Value} kalem {kvp.Key}");
                }

                string sonucMesaji;
                if (projeBazindaSayim.Count == 1)
                {
                    var ilkProje = projeBazindaSayim.First();
                    sonucMesaji = $"{ilkProje.Value} kalem {ilkProje.Key} projesine talep edildi. (Sevk No: {sevkID})";
                }
                else
                {
                    sonucMesaji = string.Join(", ", projeMesajlari) + $" projesine talep edildi. (Sevk No: {sevkID})";
                }

                //Ek talep detayını ekle
                if (ekTalepSayisi > 0)
                {
                    sonucMesaji += $" [{normalTalepSayisi} normal, {ekTalepSayisi} ek talep oluşturuldu]";
                }

                // Hatalı işlemler varsa uyarı ekle
                if (hataliIslemler.Any())
                {
                    sonucMesaji += $" (Uyarı: {hataliIslemler.Count} işlem başarısız oldu)";
                }

                return sonucMesaji.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TopluMalzemeTalepEt hatası: {Message}", ex.Message);
                throw new Exception("Toplu malzeme talep edilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Toplu malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="request">Toplu hazırlama parametreleri</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        /// <summary>
        /// Toplu malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="request">Toplu hazırlama parametreleri</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        /// <summary>
        /// Toplu malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="request">Toplu hazırlama parametreleri</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        public Result<string> TopluMalzemeleriHazirla(TopluMalzemeleriHazirlaRequest request)
        {
            try
            {
                if (request == null || request.Items == null || !request.Items.Any())
                {
                    throw new Exception("Lütfen en az bir malzeme seçiniz.");
                }

                var projeBazindaSayim = new Dictionary<int, int>();
                var basariliIslemSayisi = 0;
                var hataliIslemler = new List<string>();

                foreach (var item in request.Items)
                {
                    try
                    {
                        // Validasyon
                        if (item.MalzemeTalepSurecTakipID <= 0)
                        {
                            hataliIslemler.Add($"SurecTakipID {item.MalzemeTalepSurecTakipID}: Geçersiz ID");
                            continue;
                        }

                        if (item.HazirlananMiktar <= 0)
                        {
                            hataliIslemler.Add($"SurecTakipID {item.MalzemeTalepSurecTakipID}: Geçersiz miktar");
                            continue;
                        }

                        // 1. Temsili kaydı bul
                        var temsiliSurecTakip = _surecTakipRepository
                            .List(x => x.TabloID == item.MalzemeTalepSurecTakipID && x.AktifMi == 1)
                            .FirstOrDefault();

                        if (temsiliSurecTakip == null)
                        {
                            hataliIslemler.Add(
                                $"SurecTakipID {item.MalzemeTalepSurecTakipID}: Süreç takip kaydı bulunamadı");
                            continue;
                        }

                        // 2. Bu kaydın SevkID'sini bul
                        var temsiliMiktarTarihcesi = _miktarTarihcesiRepository
                            .List(x => x.MalzemeTalepSurecTakipID == item.MalzemeTalepSurecTakipID && x.AktifMi == 1)
                            .FirstOrDefault();

                        if (temsiliMiktarTarihcesi == null || string.IsNullOrEmpty(temsiliMiktarTarihcesi.SevkID))
                        {
                            hataliIslemler.Add($"SurecTakipID {item.MalzemeTalepSurecTakipID}: SevkID bulunamadı");
                            continue;
                        }

                        var sevkID = temsiliMiktarTarihcesi.SevkID;

                        // 3. AYNI SevkID + AYNI Statü olan TÜM kayıtları bul
                        var grupIcindekiSurecTakipler = _surecTakipRepository
                            .List(st => st.MalzemeTalebiEssizID == temsiliSurecTakip.MalzemeTalebiEssizID &&
                                        st.ParamTalepSurecStatuID == temsiliSurecTakip.ParamTalepSurecStatuID &&
                                        st.AktifMi == 1)
                            .ToList();

                        // Bu kayıtların SevkID'lerini kontrol et
                        var grupIcindekiIDler = new List<int>();
                        foreach (var st in grupIcindekiSurecTakipler)
                        {
                            var mt = _miktarTarihcesiRepository
                                .List(x => x.MalzemeTalepSurecTakipID == st.TabloID &&
                                           x.SevkID == sevkID &&
                                           x.AktifMi == 1)
                                .FirstOrDefault();

                            if (mt != null)
                            {
                                grupIcindekiIDler.Add(st.TabloID);
                            }
                        }

                        if (!grupIcindekiIDler.Any())
                        {
                            hataliIslemler.Add(
                                $"SurecTakipID {item.MalzemeTalepSurecTakipID}: Grup içinde kayıt bulunamadı");
                            continue;
                        }

                        // 4. Toplam mevcut miktarı hesapla (İslenenMiktar'a göre)
                        var toplamMevcutMiktar = 0;
                        foreach (var id in grupIcindekiIDler)
                        {
                            var mt = _miktarTarihcesiRepository
                                .List(x => x.MalzemeTalepSurecTakipID == id && x.AktifMi == 1)
                                .FirstOrDefault();

                            if (mt != null)
                            {
                                // Kalan miktar = SevkEdilenMiktar - IslenenMiktar
                                var kalanMiktar = mt.SevkEdilenMiktar - (int)mt.IslenenMiktar;
                                toplamMevcutMiktar += kalanMiktar;
                            }
                        }

                        // 5. Hazırlanan miktar kontrolü
                        if (item.HazirlananMiktar > toplamMevcutMiktar)
                        {
                            hataliIslemler.Add(
                                $"SurecTakipID {item.MalzemeTalepSurecTakipID}: Hazırlanan miktar ({item.HazirlananMiktar}), toplam mevcut miktardan ({toplamMevcutMiktar}) fazla olamaz");
                            continue;
                        }

                        // Her hazırlama işlemi (parti) için YENİ HazirId üret
                        // Aynı veri birden fazla partide hazırlanabilir, her parti farklı HazirId alır
                        var hazirId = GenerateUniqueHazirId();

                        // 6. FIFO mantığıyla hazırlama işlemini yap
                        var kalanHazirlanacak = item.HazirlananMiktar;
                        var siraliKayitlar = grupIcindekiIDler
                            .Select(id =>
                                _surecTakipRepository.List(x => x.TabloID == id && x.AktifMi == 1).FirstOrDefault())
                            .Where(x => x != null)
                            .OrderBy(x => x.TabloID) // En eski önce (FIFO)
                            .ToList();

                        foreach (var surecTakip in siraliKayitlar)
                        {
                            if (kalanHazirlanacak <= 0) break;

                            var miktarTarihcesi = _miktarTarihcesiRepository
                                .List(x => x.MalzemeTalepSurecTakipID == surecTakip.TabloID && x.AktifMi == 1)
                                .FirstOrDefault();

                            if (miktarTarihcesi == null) continue;

                            // KALAN MİKTAR = SevkEdilenMiktar - IslenenMiktar
                            var kalanMiktarBuKayit =
                                miktarTarihcesi.SevkEdilenMiktar - (int)miktarTarihcesi.IslenenMiktar;

                            if (kalanMiktarBuKayit <= 0) continue; // Bu kayıtta hazırlanacak bir şey yok

                            var buKayittanAlinacak = Math.Min(kalanHazirlanacak, kalanMiktarBuKayit);

                            if (buKayittanAlinacak < kalanMiktarBuKayit)
                            {
                                // SPLIT: Yeni kayıt oluştur (Kalan kısım için)
                                var kalanMiktar = kalanMiktarBuKayit - buKayittanAlinacak;

                                // Yeni MalzemeTalepSurecTakip (Statü değişmeden kalıyor)
                                var yeniSurecTakip = new MalzemeTalepSurecTakip
                                {
                                    MalzemeTalebiEssizID = surecTakip.MalzemeTalebiEssizID,
                                    ParamTalepSurecStatuID = surecTakip.ParamTalepSurecStatuID, // Aynı statü
                                    SurecTetiklenmeZamani = surecTakip.SurecTetiklenmeZamani,
                                    SurecTetikleyenKisiID = surecTakip.SurecTetikleyenKisiID,
                                    AktifMi = 1,
                                    SilindiMi = 0,
                                    DilID = 1,
                                    KisiID = _loginUser?.KisiID ?? 0,
                                    KurumID = _loginUser?.KurumID ?? 1,
                                    KayitTarihi = DateTime.Now,
                                    KayitEdenID = _loginUser?.KisiID ?? 0,
                                    GuncellenmeTarihi = DateTime.Now,
                                    GuncelleyenKisiID = _loginUser?.KisiID ?? 0
                                };

                                _surecTakipRepository.Add(yeniSurecTakip);
                                _surecTakipRepository.SaveChanges();

                                // Orijinal SevkEdilenMiktar'ı sakla (hiç değişmeyecek)
                                var orijinalSevkEdilenMiktar = miktarTarihcesi.SevkEdilenMiktar;

                                // Yeni MalzemeTalepMiktarTarihcesi (Kalan kısım - split kaydı)
                                var yeniMiktarTarihcesi = new MalzemeTalepMiktarTarihcesi
                                {
                                    MalzemeTalebiEssizID = miktarTarihcesi.MalzemeTalebiEssizID,
                                    MalzemeTalepSurecTakipID = yeniSurecTakip.TabloID,
                                    SevkEdilenMiktar = orijinalSevkEdilenMiktar, // ORİJİNAL TALEP MİKTARI (değişmez!)
                                    KalanMiktar = kalanMiktar, // Kalan işlenecek miktar
                                    SevkZamani = miktarTarihcesi.SevkZamani,
                                    SevkTalepEdenKisiID = miktarTarihcesi.SevkTalepEdenKisiID,
                                    MalzemeSevkTalebiYapanDepartmanID =
                                        miktarTarihcesi.MalzemeSevkTalebiYapanDepartmanID,
                                    MalzemeSevkTalebiYapanKisiID = miktarTarihcesi.MalzemeSevkTalebiYapanKisiID,
                                    SevkID = miktarTarihcesi.SevkID,

                                    // Split edilen kısım için - henüz işlenmedi
                                    IslenenMiktar = 0,
                                    HazirId = null,
                                    OnayId = null,

                                    AktifMi = 1,
                                    SilindiMi = 0,
                                    DilID = 1,
                                    KisiID = _loginUser?.KisiID ?? 0,
                                    KurumID = _loginUser?.KurumID ?? 1,
                                    KayitTarihi = DateTime.Now,
                                    KayitEdenID = _loginUser?.KisiID ?? 0,
                                    GuncellenmeTarihi = DateTime.Now,
                                    GuncelleyenKisiID = _loginUser?.KisiID ?? 0
                                };

                                _miktarTarihcesiRepository.Add(yeniMiktarTarihcesi);
                                _miktarTarihcesiRepository.SaveChanges();

                                // Mevcut kaydı güncelle - SevkEdilenMiktar DEĞİŞMEZ!
                                // Sadece IslenenMiktar güncellenir
                                miktarTarihcesi.IslenenMiktar += buKayittanAlinacak;
                                miktarTarihcesi.KalanMiktar = 0; // Bu kayıttaki kalan işlendi
                                miktarTarihcesi.HazirId = hazirId;
                                miktarTarihcesi.GuncellenmeTarihi = DateTime.Now;
                                miktarTarihcesi.GuncelleyenKisiID = _loginUser?.KisiID ?? 0;

                                _miktarTarihcesiRepository.Update(miktarTarihcesi);
                                _miktarTarihcesiRepository.SaveChanges();
                            }
                            else
                            {
                                // TAM HAZIRLANMA - Tüm miktar işlendi
                                miktarTarihcesi.IslenenMiktar = miktarTarihcesi.SevkEdilenMiktar;
                                miktarTarihcesi.KalanMiktar = 0;
                                miktarTarihcesi.HazirId = hazirId;
                                miktarTarihcesi.GuncellenmeTarihi = DateTime.Now;
                                miktarTarihcesi.GuncelleyenKisiID = _loginUser?.KisiID ?? 0;

                                _miktarTarihcesiRepository.Update(miktarTarihcesi);
                                _miktarTarihcesiRepository.SaveChanges();
                            }

                            // Statü 4'e güncelle (Üretim Mal Kabul)
                            surecTakip.ParamTalepSurecStatuID = 4;
                            surecTakip.SurecTetiklenmeZamani = DateTime.Now;
                            surecTakip.GuncellenmeTarihi = DateTime.Now;
                            surecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 0;

                            _surecTakipRepository.Update(surecTakip);
                            _surecTakipRepository.SaveChanges();

                            kalanHazirlanacak -= buKayittanAlinacak;
                        }

                        // 7. Proje bazında sayım
                        var malzemeTalep = _repository
                            .List(x => x.MalzemeTalebiEssizID == temsiliSurecTakip.MalzemeTalebiEssizID &&
                                       x.AktifMi == 1)
                            .FirstOrDefault();

                        if (malzemeTalep != null)
                        {
                            if (projeBazindaSayim.ContainsKey(malzemeTalep.ProjeKodu))
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu]++;
                            }
                            else
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu] = 1;
                            }
                        }

                        basariliIslemSayisi++;
                    }
                    catch (Exception ex)
                    {
                        hataliIslemler.Add($"SurecTakipID {item.MalzemeTalepSurecTakipID}: {ex.Message}");
                        _logger.LogError(ex,
                            "TopluMalzemeleriHazirla - SurecTakipID {MalzemeTalepSurecTakipID} için hata",
                            item.MalzemeTalepSurecTakipID);
                    }
                }

                // 8. Response mesajı oluştur
                if (basariliIslemSayisi == 0)
                {
                    throw new Exception("Hiçbir hazırlama işlemi başarılı olmadı. Hatalar: " +
                                        string.Join("; ", hataliIslemler));
                }

                var projeMesajlari = new List<string>();
                foreach (var kvp in projeBazindaSayim.OrderBy(x => x.Key))
                {
                    projeMesajlari.Add($"{kvp.Value} kalem {kvp.Key}");
                }

                string sonucMesaji;
                if (projeBazindaSayim.Count == 1)
                {
                    var ilkProje = projeBazindaSayim.First();
                    sonucMesaji = $"{ilkProje.Value} kalem {ilkProje.Key} projesine hazırlandı.";
                }
                else
                {
                    sonucMesaji = string.Join(", ", projeMesajlari) + " projesine hazırlandı.";
                }

                if (hataliIslemler.Any())
                {
                    sonucMesaji += $" (Uyarı: {hataliIslemler.Count} işlem başarısız oldu)";
                }

                return sonucMesaji.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TopluMalzemeleriHazirla hatası: {Message}", ex.Message);
                throw new Exception("Toplu malzeme hazırlama işlemi sırasında hata oluştu: " + ex.Message);
            }
        }


        /// <summary>
        /// Malzeme talebini iade etme metodu
        /// </summary>
        /// <param name="request">İade parametreleri</param>
        /// <returns>İade işlemi sonucu</returns>
        public Result<bool> MalzemeIadeEt(MalzemeIadeEtRequest request)
        {
            try
            {
                if (request == null || request.MalzemeTalepSurecTakipID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // 1. MalzemeTalepSurecTakip kaydını bul ve statüsünü 5 olarak güncelle
                var surecTakip = _surecTakipRepository
                    .List(x => x.TabloID == request.MalzemeTalepSurecTakipID && x.AktifMi == 1)
                    .FirstOrDefault();

                if (surecTakip == null)
                {
                    throw new Exception("Belirtilen malzeme talebine ait kayıt bulunamadı.");
                }

                // Statüsünü 5 olarak güncelle (İade)
                surecTakip.ParamTalepSurecStatuID = 5;
                surecTakip.GuncellenmeTarihi = DateTime.Now;
                surecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                var updateResult = _surecTakipRepository.Update(surecTakip);
                var saveResult = _surecTakipRepository.SaveChanges();

                if (saveResult <= 0)
                {
                    throw new Exception("Statü güncellemesi kaydedilemedi.");
                }

                // 2. MalzemeTalepSurecTakipNotlari tablosuna yeni not kaydı ekle
                try
                {
                    var surecNot = new MalzemeTalepSurecTakipNotlari
                    {
                        MalzemeTalepSurecTakipID = surecTakip.TabloID,
                        SurecStatuGirilenNot = request.SurecStatuGirilenNot,
                        SurecStatuBildirimTipiID = request.SurecStatuBildirimTipiID,
                        // BaseModel zorunlu alanları
                        AktifMi = 1,
                        SilindiMi = 0,
                        DilID = 1,
                        KisiID = _loginUser?.KisiID ?? 1,
                        KurumID = _loginUser?.KurumID ?? 1,
                        KayitTarihi = DateTime.Now,
                        KayitEdenID = _loginUser?.KisiID ?? 1,
                        GuncellenmeTarihi = DateTime.Now,
                        GuncelleyenKisiID = _loginUser?.KisiID ?? 1
                    };

                    var notResult = _surecTakipNotlariRepository.Add(surecNot);
                    var notSaveResult = _surecTakipNotlariRepository.SaveChanges();

                    if (notSaveResult <= 0)
                    {
                        throw new Exception("Süreç takip notu kaydedilemedi.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message +
                                        (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                throw new Exception("Malzeme iade işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzeme kabul etme metodu
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Kabul edilecek malzeme süreç takip ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID)
        {
            try
            {
                if (malzemeTalepSurecTakipID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // İlgili MalzemeTalepSurecTakip kaydını bul (Statü 4 olmalı - Üretim Mal Kabul)
                var surecTakip = _surecTakipRepository
                    .List(x => x.TabloID == malzemeTalepSurecTakipID && x.AktifMi == 1)
                    .FirstOrDefault();

                if (surecTakip == null)
                {
                    throw new Exception("Belirtilen malzeme talebine ait kayıt bulunamadı.");
                }

                // Bu süreç takip kaydının tarihçesini getir (HazirId'yi bulmak için)
                var miktarTarihcesi = _miktarTarihcesiRepository
                    .List(x => x.MalzemeTalepSurecTakipID == malzemeTalepSurecTakipID && x.AktifMi == 1)
                    .FirstOrDefault();

                if (miktarTarihcesi == null)
                {
                    throw new Exception("Malzeme tarihçe kaydı bulunamadı.");
                }

                // HazirId'yi al
                string hazirId = miktarTarihcesi.HazirId;

                if (string.IsNullOrEmpty(hazirId))
                {
                    throw new Exception("Bu kayıt için HazirId bulunamadı. Önce malzeme hazırlanmalı.");
                }

                // Bu HazirId için daha önce OnayId oluşturulmuş mu kontrol et
                var mevcutOnayKaydi = _miktarTarihcesiRepository
                    .List(x => x.HazirId == hazirId
                               && !string.IsNullOrEmpty(x.OnayId)
                               && x.AktifMi == 1)
                    .FirstOrDefault();

                string onayId;

                if (mevcutOnayKaydi != null)
                {
                    // Mevcut OnayId'yi kullan (3. madde gereği - güncelleme)
                    onayId = mevcutOnayKaydi.OnayId;
                }
                else
                {
                    // Yeni OnayId üret
                    onayId = GenerateUniqueOnayId();
                }

                // Tarihçe kaydını güncelle - OnayId ekle
                miktarTarihcesi.OnayId = onayId;
                miktarTarihcesi.GuncellenmeTarihi = DateTime.Now;
                miktarTarihcesi.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                _miktarTarihcesiRepository.Update(miktarTarihcesi);
                _miktarTarihcesiRepository.SaveChanges();

                // Kaydın statüsünü 6 olarak güncelle (Depo Kabul)
                surecTakip.ParamTalepSurecStatuID = 6;
                surecTakip.SurecTetiklenmeZamani = DateTime.Now;
                surecTakip.GuncellenmeTarihi = DateTime.Now;
                surecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                var updateResult = _surecTakipRepository.Update(surecTakip);
                var saveResult = _surecTakipRepository.SaveChanges();

                if (saveResult <= 0)
                {
                    throw new Exception("Statü güncellemesi kaydedilemedi.");
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MalKabulEt hatası: {Message}", ex.Message);
                throw new Exception("Mal kabul işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Toplu malzeme kabul etme metodu
        /// </summary>
        /// <param name="request">Toplu kabul parametreleri</param>
        /// <returns>Kabul işlemi sonucu</returns>
        public Result<string> TopluMalKabulEt(TopluMalKabulEtRequest request)
        {
            try
            {
                if (request == null || request.MalzemeTalepSurecTakipIDler == null || !request.MalzemeTalepSurecTakipIDler.Any())
                {
                    throw new Exception("Lütfen en az bir malzeme seçiniz.");
                }

                var basariliIslemSayisi = 0;
                var hataliIslemler = new List<string>();
                var projeBazindaSayim = new Dictionary<int, int>();

                foreach (var surecTakipID in request.MalzemeTalepSurecTakipIDler)
                {
                    try
                    {
                        if (surecTakipID <= 0)
                        {
                            hataliIslemler.Add($"ID {surecTakipID}: Geçersiz ID");
                            continue;
                        }

                        // İlgili MalzemeTalepSurecTakip kaydını bul (Statü 4 olmalı - Üretim Mal Kabul)
                        var surecTakip = _surecTakipRepository
                            .List(x => x.TabloID == surecTakipID && x.AktifMi == 1)
                            .FirstOrDefault();

                        if (surecTakip == null)
                        {
                            hataliIslemler.Add($"ID {surecTakipID}: Kayıt bulunamadı");
                            continue;
                        }

                        // Bu süreç takip kaydının tarihçesini getir (HazirId'yi bulmak için)
                        var miktarTarihcesi = _miktarTarihcesiRepository
                            .List(x => x.MalzemeTalepSurecTakipID == surecTakipID && x.AktifMi == 1)
                            .FirstOrDefault();

                        if (miktarTarihcesi == null)
                        {
                            hataliIslemler.Add($"ID {surecTakipID}: Tarihçe kaydı bulunamadı");
                            continue;
                        }

                        // HazirId'yi al
                        string hazirId = miktarTarihcesi.HazirId;

                        if (string.IsNullOrEmpty(hazirId))
                        {
                            hataliIslemler.Add($"ID {surecTakipID}: HazirId bulunamadı, önce hazırlanmalı");
                            continue;
                        }

                        // Bu HazirId için daha önce OnayId oluşturulmuş mu kontrol et
                        var mevcutOnayKaydi = _miktarTarihcesiRepository
                            .List(x => x.HazirId == hazirId
                                       && !string.IsNullOrEmpty(x.OnayId)
                                       && x.AktifMi == 1)
                            .FirstOrDefault();

                        string onayId;

                        if (mevcutOnayKaydi != null)
                        {
                            // Mevcut OnayId'yi kullan (3. madde gereği - güncelleme)
                            onayId = mevcutOnayKaydi.OnayId;
                        }
                        else
                        {
                            // Yeni OnayId üret
                            onayId = GenerateUniqueOnayId();
                        }

                        // Tarihçe kaydını güncelle - OnayId ekle
                        miktarTarihcesi.OnayId = onayId;
                        miktarTarihcesi.GuncellenmeTarihi = DateTime.Now;
                        miktarTarihcesi.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                        _miktarTarihcesiRepository.Update(miktarTarihcesi);
                        _miktarTarihcesiRepository.SaveChanges();

                        // Kaydın statüsünü 6 olarak güncelle (Depo Kabul)
                        surecTakip.ParamTalepSurecStatuID = 6;
                        surecTakip.SurecTetiklenmeZamani = DateTime.Now;
                        surecTakip.GuncellenmeTarihi = DateTime.Now;
                        surecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                        _surecTakipRepository.Update(surecTakip);
                        var saveResult = _surecTakipRepository.SaveChanges();

                        if (saveResult <= 0)
                        {
                            hataliIslemler.Add($"ID {surecTakipID}: Statü güncellemesi kaydedilemedi");
                            continue;
                        }

                        // Proje bazında sayım
                        var malzemeTalep = _repository
                            .List(x => x.MalzemeTalebiEssizID == surecTakip.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .FirstOrDefault();

                        if (malzemeTalep != null)
                        {
                            if (projeBazindaSayim.ContainsKey(malzemeTalep.ProjeKodu))
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu]++;
                            }
                            else
                            {
                                projeBazindaSayim[malzemeTalep.ProjeKodu] = 1;
                            }
                        }

                        basariliIslemSayisi++;
                    }
                    catch (Exception ex)
                    {
                        hataliIslemler.Add($"ID {surecTakipID}: {ex.Message}");
                        _logger.LogError(ex, "TopluMalKabulEt - ID {SurecTakipID} için hata", surecTakipID);
                    }
                }

                // Response mesajı oluştur
                if (basariliIslemSayisi == 0)
                {
                    throw new Exception("Hiçbir kabul işlemi başarılı olmadı. Hatalar: " + string.Join("; ", hataliIslemler));
                }

                // Proje bazında mesaj oluştur
                var projeMesajlari = new List<string>();
                foreach (var kvp in projeBazindaSayim.OrderBy(x => x.Key))
                {
                    projeMesajlari.Add($"{kvp.Value} kalem {kvp.Key}");
                }

                string sonucMesaji;
                if (projeBazindaSayim.Count == 1)
                {
                    var ilkProje = projeBazindaSayim.First();
                    sonucMesaji = $"{ilkProje.Value} kalem {ilkProje.Key} projesine mal kabul edildi.";
                }
                else
                {
                    sonucMesaji = string.Join(", ", projeMesajlari) + " projesine mal kabul edildi.";
                }

                if (hataliIslemler.Any())
                {
                    sonucMesaji += $" (Uyarı: {hataliIslemler.Count} işlem başarısız oldu)";
                }

                return sonucMesaji.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TopluMalKabulEt hatası: {Message}", ex.Message);
                throw new Exception("Toplu mal kabul işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzemeyi hasarlı olarak işaretleme metodu
        /// </summary>
        /// <param name="request">Hasarlı işaretleme parametreleri</param>
        /// <returns>İşaretleme işlemi sonucu</returns>
        public Result<bool> HasarliOlarakIsaretle(HasarliOlarakIşaretleRequest request)
        {
            try
            {
                if (request == null || request.MalzemeTalepSurecTakipID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // 1. MalzemeTalepSurecTakip kaydını bul ve statüsünü 7 olarak güncelle
                var surecTakip = _surecTakipRepository
                    .List(x => x.TabloID == request.MalzemeTalepSurecTakipID && x.AktifMi == 1)
                    .FirstOrDefault();

                if (surecTakip == null)
                {
                    throw new Exception("Belirtilen malzeme talebine ait kayıt bulunamadı.");
                }

                // Statüsünü 7 olarak güncelle (Hasarlı)
                surecTakip.ParamTalepSurecStatuID = 7;
                surecTakip.GuncellenmeTarihi = DateTime.Now;
                surecTakip.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                var updateResult = _surecTakipRepository.Update(surecTakip);
                var saveResult = _surecTakipRepository.SaveChanges();

                if (saveResult <= 0)
                {
                    throw new Exception("Statü güncellemesi kaydedilemedi.");
                }

                // 2. MalzemeTalepSurecTakipNotlari tablosuna yeni not kaydı ekle
                try
                {
                    var surecNot = new MalzemeTalepSurecTakipNotlari
                    {
                        MalzemeTalepSurecTakipID = surecTakip.TabloID,
                        SurecStatuGirilenNot = request.SurecStatuGirilenNot,
                        SurecStatuBildirimTipiID = request.SurecStatuBildirimTipiID,
                        // BaseModel zorunlu alanları
                        AktifMi = 1,
                        SilindiMi = 0,
                        DilID = 1,
                        KisiID = _loginUser?.KisiID ?? 1,
                        KurumID = _loginUser?.KurumID ?? 1,
                        KayitTarihi = DateTime.Now,
                        KayitEdenID = _loginUser?.KisiID ?? 1,
                        GuncellenmeTarihi = DateTime.Now,
                        GuncelleyenKisiID = _loginUser?.KisiID ?? 1
                    };

                    var notResult = _surecTakipNotlariRepository.Add(surecNot);
                    var notSaveResult = _surecTakipNotlariRepository.SaveChanges();

                    if (notSaveResult <= 0)
                    {
                        throw new Exception("Süreç takip notu kaydedilemedi.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message +
                                        (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                throw new Exception("Malzemeyi hasarlı olarak işaretleme işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Toplu SAT bilgisi güncelleme metodu
        /// </summary>
        /// <param name="request">Güncelleme parametreleri</param>
        /// <returns>Güncelleme işlemi sonucu</returns>
        public Result<string> TopluSATBilgisiGuncelle(TopluSATBilgisiGuncellemeRequest request)
        {
            try
            {
                if (request == null || request.Items == null || !request.Items.Any())
                {
                    throw new Exception("Lütfen en az bir kayıt giriniz.");
                }

                var basariliGuncellenenSayisi = 0;
                var hataliIslemler = new List<string>();

                foreach (var item in request.Items)
                {
                    try
                    {
                        // Validasyon
                        if (item.MalzemeTalebiEssizID <= 0)
                        {
                            hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: Geçersiz ID");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(item.BuTalebiKarsilayanSATSeriNo) ||
                            string.IsNullOrWhiteSpace(item.BuTalebiKarsilayanSATSiraNo))
                        {
                            hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: SAT Seri No veya Sıra No boş olamaz");
                            continue;
                        }

                        // Malzeme talebini bul
                        var malzemeTalep = List(x =>
                                x.MalzemeTalebiEssizID == item.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .Value
                            .FirstOrDefault();

                        if (malzemeTalep == null)
                        {
                            hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: Malzeme talebi bulunamadı");
                            continue;
                        }

                        // SAT bilgilerini güncelle
                        malzemeTalep.BuTalebiKarsilayanSATSeriNo = item.BuTalebiKarsilayanSATSeriNo;
                        malzemeTalep.BuTalebiKarsilayanSATSiraNo = item.BuTalebiKarsilayanSATSiraNo;
                        malzemeTalep.GuncellenmeTarihi = DateTime.Now;
                        malzemeTalep.GuncelleyenKisiID = _loginUser?.KisiID ?? 1;

                        _repository.Update(malzemeTalep);
                        _repository.SaveChanges();

                        basariliGuncellenenSayisi++;

                        _logger.LogInformation(
                            $"SAT bilgisi güncellendi: ID={item.MalzemeTalebiEssizID}, SAT={item.BuTalebiKarsilayanSATSeriNo}/{item.BuTalebiKarsilayanSATSiraNo}");
                    }
                    catch (Exception ex)
                    {
                        hataliIslemler.Add($"ID {item.MalzemeTalebiEssizID}: {ex.Message}");
                        _logger.LogError(ex, "TopluSATBilgisiGuncelle - ID {MalzemeTalebiEssizID} için hata",
                            item.MalzemeTalebiEssizID);
                    }
                }

                // Response mesajı oluştur
                if (basariliGuncellenenSayisi == 0)
                {
                    throw new Exception("Hiçbir kayıt güncellenemedi. Hatalar: " + string.Join("; ", hataliIslemler));
                }

                string sonucMesaji = $"{basariliGuncellenenSayisi} kayıt başarıyla güncellendi.";

                if (hataliIslemler.Any())
                {
                    sonucMesaji += $" (Uyarı: {hataliIslemler.Count} kayıt güncellenemedi)";
                }

                return sonucMesaji.ToResult();
            }
            catch (Exception ex)
            {
                throw new Exception("SAT bilgisi güncellenirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Yeni SevkID üretir - Format: TF2025000000001
        /// </summary>
        /// <returns>Yeni SevkID</returns>
        private string YeniSevkIDUret()
        {
            try
            {
                var yil = DateTime.Now.Year;

                // Bu yıl için en son kullanılan SevkID'yi bul
                var sonSevkID = _miktarTarihcesiRepository.List(x =>
                        x.SevkID != null &&
                        x.SevkID.StartsWith($"TF{yil}") &&
                        x.AktifMi == 1)
                    .OrderByDescending(x => x.SevkID)
                    .Select(x => x.SevkID)
                    .FirstOrDefault();

                int yeniSiraNo = 1;

                if (!string.IsNullOrEmpty(sonSevkID))
                {
                    // Son SevkID'den sıra numarasını çıkar
                    // TF2025000000001 -> 000000001 -> 1
                    var siraNoStr = sonSevkID.Substring(6); // "TF2025" = 6 karakter
                    if (int.TryParse(siraNoStr, out int sonSiraNo))
                    {
                        yeniSiraNo = sonSiraNo + 1;
                    }
                }

                // Format: TF{YIL}{SIRANO:9 haneli}
                var yeniSevkID = $"TF{yil}{yeniSiraNo:D9}";
                return yeniSevkID;
            }
            catch (Exception ex)
            {
                throw new Exception("SevkID üretilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Yeni Malzeme Talebi Essiz ID üretir - Format: TF2025000000001
        /// </summary>
        /// <returns>Yeni Malzeme Talebi Essiz ID</returns>
        private int YeniMalzemeTalebiEssizIDUret()
        {
            try
            {
                var sonMalzemeTalebiEssizID = _repository.List(x =>
                        x.MalzemeTalebiEssizID != 0 &&
                        x.AktifMi == 1)
                    .OrderByDescending(x => x.MalzemeTalebiEssizID)
                    .Select(x => x.MalzemeTalebiEssizID)
                    .FirstOrDefault();

                return sonMalzemeTalebiEssizID + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Malzeme Talebi Essiz ID üretilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Benzersiz HazirId üretir (Format: HZ2025000000001)
        /// </summary>
        /// <returns>Üretilen HazirId</returns>
        private string GenerateUniqueHazirId()
        {
            var yil = DateTime.Now.Year;
            var prefix = $"HZ{yil}";

            // Son HazirId'yi bul
            var sonHazirId = _miktarTarihcesiRepository
                .List(x => x.HazirId != null && x.HazirId.StartsWith(prefix) && x.AktifMi == 1)
                .OrderByDescending(x => x.HazirId)
                .Select(x => x.HazirId)
                .FirstOrDefault();

            int siradakiNo = 1;
            if (!string.IsNullOrEmpty(sonHazirId))
            {
                // HZ2025000000001 -> 000000001 kısmını al
                var noKismi = sonHazirId.Substring(prefix.Length);
                if (int.TryParse(noKismi, out int mevcutNo))
                {
                    siradakiNo = mevcutNo + 1;
                }
            }

            return $"{prefix}{siradakiNo:D9}"; // HZ2025000000001 formatında (9 haneli)
        }

        /// <summary>
        /// Benzersiz OnayId üretir (Format: ON2025000000001)
        /// </summary>
        /// <returns>Üretilen OnayId</returns>
        private string GenerateUniqueOnayId()
        {
            var yil = DateTime.Now.Year;
            var prefix = $"ON{yil}";

            var sonOnayId = _miktarTarihcesiRepository
                .List(x => x.OnayId != null && x.OnayId.StartsWith(prefix) && x.AktifMi == 1)
                .OrderByDescending(x => x.OnayId)
                .Select(x => x.OnayId)
                .FirstOrDefault();

            int siradakiNo = 1;
            if (!string.IsNullOrEmpty(sonOnayId))
            {
                var noKismi = sonOnayId.Substring(prefix.Length);
                if (int.TryParse(noKismi, out int mevcutNo))
                {
                    siradakiNo = mevcutNo + 1;
                }
            }

            return $"{prefix}{siradakiNo:D9}"; // ON2025000000001 formatında
        }
    }

    /// <summary>
    /// Malzeme talep filtreleme request modeli
    /// </summary>
    public class MalzemeTalepFiltreleRequest
    {
        /// <summary>
        /// Proje kodu filtresi
        /// </summary>
        public int? ProjeKodu { get; set; }

        /// <summary>
        /// Talep süreç statü ID'leri listesi
        /// </summary>
        public List<int> TalepSurecStatuIDs { get; set; }

        /// <summary>
        /// Arama metni (MalzemeKodu, MalzemeIsmi, SATSeriNo, SATSiraNo için)
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// True ise: [1,2] statüleri veya kalan miktar > 0 olanları getirir (TalepSurecStatuIDs önemsiz)
        /// False ise: Sadece TalepSurecStatuIDs'e bakılır
        /// </summary>
        public bool MalzemeTalepEtGetir { get; set; }

        /// <summary>
        /// True ise: Sadece ek talepleri (BaglantiliMalzemeTalebiEssizID > 0) getirir
        /// </summary>
        public bool SadeceEkTalepleriGetir { get; set; }
    }

    /// <summary>
    /// Departman bazında sevkiyat detayı
    /// </summary>
    public class DepartmanSevkiyatDetay
    {
        /// <summary>
        /// Departman ID
        /// </summary>
        public int DepartmanID { get; set; }

        /// <summary>
        /// Bu departmanın talep ettiği toplam miktar
        /// </summary>
        public int TalepEdilenMiktar { get; set; }

        /// <summary>
        /// Sevkiyat tarihi (ilk sevkiyat)
        /// </summary>
        public DateTime? SevkZamani { get; set; }
    }

    /// <summary>
    /// Malzeme talep detay response modeli
    /// </summary>
    public class MalzemeTalepDetayResponse
    {
        /// <summary>
        /// Malzeme talep süreç takip ID'si (Temsili - Grup içindeki en yeni)
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }

        /// <summary>
        /// Grup içindeki tüm süreç takip ID'leri
        /// </summary>
        public List<int> GrupIcindekiSurecTakipIDler { get; set; }

        /// <summary>
        /// Malzeme talebi essiz ID'si
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }

        /// <summary>
        /// Malzeme talep genel bilgiler
        /// </summary>
        public MalzemeTalepGenelBilgiler MalzemeTalep { get; set; }

        /// <summary>
        /// Talep edilen miktar (SevkID + Statü bazında GRUPLANMIŞ toplam)
        /// </summary>
        public int TalepEdilenMiktar { get; set; }

        /// <summary>
        /// Bu kaydın kendi miktarı (Sadece bilgi amaçlı)
        /// </summary>
        public int BuKaydinMiktari { get; set; }

        /// <summary>
        /// İşlenen miktar (Hazırlanan/Onaylanan miktar)
        /// </summary>
        public decimal IslenenMiktar { get; set; }

        /// <summary>
        /// Hazırlanabilecek miktar (TalepEdilenMiktar - IslenenMiktar)
        /// </summary>
        public decimal HazirlanabilecekMiktar { get; set; }

        /// <summary>
        /// Toplam sevk edilen miktar
        /// </summary>
        public int ToplamSevkEdilenMiktar { get; set; }

        /// <summary>
        /// Kalan miktar
        /// </summary>
        public int KalanMiktar { get; set; }

        /// <summary>
        /// Parametre talep süreç statü ID'si
        /// </summary>
        public int ParamTalepSurecStatuID { get; set; }

        /// <summary>
        /// Süreç oluşturma tarihi (Grup içindeki en eski)
        /// </summary>
        public DateTime? SurecOlusturmaTarihi { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }

        /// <summary>
        /// Süreç statü bildirim tipi ID'si
        /// </summary>
        public int? SurecStatuBildirimTipiID { get; set; }

        /// <summary>
        /// Bildirim tipi tanımlama
        /// </summary>
        public string BildirimTipiTanimlama { get; set; }

        /// <summary>
        /// Sevk ID - Format: TF2025000000001
        /// </summary>
        public string SevkID { get; set; }

        /// <summary>
        /// Hazır ID - Format: HZ2025000000001
        /// </summary>
        public string HazirId { get; set; }

        /// <summary>
        /// Onay ID - Format: ON2025000000001
        /// </summary>
        public string OnayId { get; set; }

        /// <summary>
        /// Aynı SevkID + Statü'de toplam kayıt sayısı
        /// </summary>
        public int SevkIDBazindaKayitSayisi { get; set; }

        /// <summary>
        /// Departman bazında sevkiyat detayları
        /// </summary>
        public List<DepartmanSevkiyatDetay> DepartmanSevkiyatlari { get; set; }
    }

    /// <summary>
    /// Toplu malzeme talep item
    /// </summary>
    public class TopluMalzemeTalepItem
    {
        /// <summary>
        /// Malzeme talebi essiz ID
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }

        /// <summary>
        /// Sevk edilen miktar
        /// </summary>
        public int SevkEdilenMiktar { get; set; }

        /// <summary> Malzeme sevk talebi yapan departman ID </summary>
        public int MalzemeSevkTalebiYapanDepartmanID { get; set; }
    }

    /// <summary>
    /// Malzeme talep etme request modeli
    /// </summary>
    public class MalzemeTalepEtRequest
    {
        /// <summary>
        /// Malzeme talebi essiz ID'si
        /// </summary>
        public List<TopluMalzemeTalepItem> TalepItems { get; set; }
    }

    /// <summary>
    /// Malzemeleri hazırlama item
    /// </summary>
    public class MalzemeleriHazirlaItem
    {
        /// <summary>
        /// Malzeme talep süreç takip ID'si
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }

        /// <summary>
        /// Hazırlanan miktar
        /// </summary>
        public int HazirlananMiktar { get; set; }
    }

    /// <summary>
    /// Toplu malzemeleri hazırlama request modeli
    /// </summary>
    public class TopluMalzemeleriHazirlaRequest
    {
        /// <summary>
        /// Hazırlanacak malzeme item'ları listesi
        /// </summary>
        public List<MalzemeleriHazirlaItem> Items { get; set; }
    }

    /// <summary>
    /// Malzeme iade etme request modeli
    /// </summary>
    public class MalzemeIadeEtRequest
    {
        /// <summary>
        /// İade edilecek malzeme talep ID'si
        /// </summary>
        //public int MalzemeTalebiEssizID { get; set; }
        public int MalzemeTalepSurecTakipID { get; set; }

        /// <summary>
        /// Süreç statü bildirim tipi ID'si
        /// </summary>
        public int SurecStatuBildirimTipiID { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }
    }

    /// <summary>
    /// Malzemeyi hasarlı olarak işaretleme request modeli
    /// </summary>
    public class HasarliOlarakIşaretleRequest
    {
        /// <summary>
        /// İşaretlenecek malzeme talep ID'si
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }

        /// <summary>
        /// Süreç statü bildirim tipi ID'si
        /// </summary>
        public int SurecStatuBildirimTipiID { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }
    }

    /// <summary>
    /// SAT bilgisi güncelleme item
    /// </summary>
    public class SATBilgisiGuncellemeItem
    {
        /// <summary>
        /// Malzeme talebi essiz ID
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }

        /// <summary>
        /// Bu talebi karşılayan SAT seri no
        /// </summary>
        public string BuTalebiKarsilayanSATSeriNo { get; set; }

        /// <summary>
        /// Bu talebi karşılayan SAT sıra no
        /// </summary>
        public string BuTalebiKarsilayanSATSiraNo { get; set; }
    }

    /// <summary>
    /// Toplu SAT bilgisi güncelleme request modeli
    /// </summary>
    public class TopluSATBilgisiGuncellemeRequest
    {
        /// <summary>
        /// Güncellenecek SAT bilgileri listesi
        /// </summary>
        public List<SATBilgisiGuncellemeItem> Items { get; set; }
    }

    /// <summary>
    /// Toplu mal kabul etme request modeli
    /// </summary>
    public class TopluMalKabulEtRequest
    {
        /// <summary>
        /// Kabul edilecek malzeme talep süreç takip ID'leri listesi
        /// </summary>
        public List<int> MalzemeTalepSurecTakipIDler { get; set; }
    }
}