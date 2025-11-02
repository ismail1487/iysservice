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
        Result<bool> MalzemeTalepEt(MalzemeTalepEtRequest request);

        /// <summary>
        /// Malzeme taleplerini koşullu filtreleme ile getiren metod
        /// </summary>
        /// <param name="malzemeTalepEtGetir">True ise: [1,2] statüleri veya kalan miktar > 0 olanları getirir</param>
        /// <param name="talepSurecStatuIDs">Talep süreç statü ID'leri</param>
        /// <returns>Filtrelenmiş malzeme talep listesi</returns>
        Result<List<MalzemeTalepGenelBilgiler>> MalzemeTalepList(bool malzemeTalepEtGetir = false, List<int> talepSurecStatuIDs = null);

        /// <summary>
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Hazırlanacak malzeme talep ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        Result<bool> MalzemeleriHazirla(int malzemeTalebiEssizID);

        /// <summary>
        /// Malzeme talebini iade etme metodu
        /// </summary>
        /// <param name="request">İade parametreleri</param>
        /// <returns>İade işlemi sonucu</returns>
        Result<bool> MalzemeIadeEt(MalzemeIadeEtRequest request);

        /// <summary>
        /// Malzeme kabul etme metodu
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        Result<bool> MalKabulEt(int malzemeTalebiEssizID);

        /// <summary>
        /// Malzemeyi hasarlı olarak işaretleme metodu
        /// </summary>
        /// <param name="request">Hasarlı işaretleme parametreleri</param>
        /// <returns>İşaretleme işlemi sonucu</returns>
        Result<bool> HasarliOlarakIsaretle(HasarliOlarakIşaretleRequest request);
    }

    /// <summary>
    /// MalzemeTalepGenelBilgiler ile ilgili işlemleri yöneten servis sınıfı
    /// </summary>
    public class MalzemeTalepGenelBilgilerService : Service<MalzemeTalepGenelBilgiler>, IMalzemeTalepGenelBilgilerService
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

        /// <summary>
        /// Malzeme taleplerini filtreli olarak getiren metod
        /// </summary>
        /// <param name="request">Filtre parametreleri</param>
        /// <returns>Filtrelenmiş malzeme talep listesi (detaylı miktar bilgileri ile)</returns>
        public Result<List<MalzemeTalepDetayResponse>> MalzemeTalepleriniGetir(MalzemeTalepFiltreleRequest request)
        {
            try
            {
                IQueryable<MalzemeTalepGenelBilgiler> query;

                // Süreç statü filtresi - Yeni mantık
                if (request.MalzemeTalepEtGetir)
                {
                    // malzemeTalepEtGetir = true ise: [1,2] statüleri VEYA kalan miktar > 0 olanları getir
                    // TalepSurecStatuIDs parametresi görmezden gelinir
                    
                    // 1. Tüm aktif malzemeleri al
                    var baseQuery = _repository.List(x => x.AktifMi == 1);

                    // 2. Statü [1,2] olan malzeme taleplerinin ID'lerini al
                    var statusOneOrTwoIds = _surecTakipRepository.List(st =>
                        (st.ParamTalepSurecStatuID == 1 || st.ParamTalepSurecStatuID == 2) && st.AktifMi == 1)
                        .Select(st => st.MalzemeTalebiEssizID)
                        .Distinct()
                        .ToList();

                    // 3. Kalan miktarı > 0 olan malzeme taleplerinin ID'lerini al
                    var allMalzemeTaleps = baseQuery.ToList();
                    var remainingAmountIds = new List<int>();

                    foreach (var malzeme in allMalzemeTaleps)
                    {
                        var sevkEdilenToplam = _miktarTarihcesiRepository.List(x =>
                            x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .ToList()
                            .Sum(x => x.SevkEdilenMiktar);

                        var kalanMiktar = malzeme.MalzemeOrijinalTalepEdilenMiktar - sevkEdilenToplam;
                        
                        if (kalanMiktar > 0)
                        {
                            remainingAmountIds.Add(malzeme.MalzemeTalebiEssizID);
                        }
                    }

                    // 4. VEYA mantığı: Statü [1,2] VEYA kalan miktar > 0
                    var combinedIds = statusOneOrTwoIds.Union(remainingAmountIds).Distinct().ToList();
                    query = _repository.List(x => x.AktifMi == 1).Where(x => combinedIds.Contains(x.MalzemeTalebiEssizID));
                }
                else if (request.TalepSurecStatuIDs != null && request.TalepSurecStatuIDs.Any())
                {
                    // malzemeTalepEtGetir = false ise: Sadece TalepSurecStatuIDs'e bakılır (MIKTAR KONTROLÜ YOK)
                    query = _repository.List(x => x.AktifMi == 1);
                    
                    // Sadece statü filtresine bak - miktar kontrolü yapma
                    var malzemeTalepIds = _surecTakipRepository.List(st =>
                        request.TalepSurecStatuIDs.Contains(st.ParamTalepSurecStatuID) && st.AktifMi == 1)
                        .Select(st => st.MalzemeTalebiEssizID)
                        .Distinct()
                        .ToList();

                    query = query.Where(x => malzemeTalepIds.Contains(x.MalzemeTalebiEssizID));
                }
                else
                {
                    // Hiçbir filtre yok ise tüm aktif malzemeleri getir
                    query = _repository.List(x => x.AktifMi == 1);
                }

                // Statü filtresinden SONRA liste al
                var filteredResult = query.ToList();

                // Proje kodu filtresi - listelenen veriler üzerinde uygula
                if (request.ProjeKodu.HasValue && request.ProjeKodu.Value > 0)
                {
                    filteredResult = filteredResult.Where(x => x.ProjeKodu == request.ProjeKodu.Value).ToList();
                }

                // Search text filtresi - listelenen veriler üzerinde uygula
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    var searchTerm = request.SearchText.ToLower();
                    filteredResult = filteredResult.Where(x =>
                        x.MalzemeKodu.ToLower().Contains(searchTerm) ||
                        x.MalzemeIsmi.ToLower().Contains(searchTerm) ||
                        x.SATSiraNo.ToLower().Contains(searchTerm)).ToList();
                }

                // Sonucu MalzemeTalepDetayResponse listesine dönüştür (miktar hesaplamaları ile)
                var detayliSonuclar = new List<MalzemeTalepDetayResponse>();

                foreach (var malzeme in filteredResult)
                {
                    // Her malzeme için sevk edilen toplam miktarı hesapla
                    var sevkEdilenToplam = _miktarTarihcesiRepository.List(x =>
                        x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && x.AktifMi == 1)
                        .ToList()
                        .Sum(x => x.SevkEdilenMiktar);

                    // Kalan miktarı hesapla
                    var kalanMiktar = malzeme.MalzemeOrijinalTalepEdilenMiktar - sevkEdilenToplam;

                    // MalzemeTalepSurecTakip kaydını bul
                    var surecTakip = _surecTakipRepository.List(st =>
                        st.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && st.AktifMi == 1)
                        .FirstOrDefault();

                    // En son notları bul (varsa)
                    string surecStatuGirilenNot = null;
                    int? surecStatuBildirimTipiID = null;
                    string bildirimTipiTanimlama = null;

                    if (surecTakip != null)
                    {
                        var enSonNot = _surecTakipNotlariRepository.List(n =>
                            n.MalzemeTalepSurecTakipID == surecTakip.TabloID && n.AktifMi == 1)
                            .OrderByDescending(n => n.TabloID)
                            .FirstOrDefault();

                        if (enSonNot != null)
                        {
                            surecStatuGirilenNot = enSonNot.SurecStatuGirilenNot;
                            surecStatuBildirimTipiID = enSonNot.SurecStatuBildirimTipiID;

                            // Bildirim tipi tanımlamasını bul (varsa)
                            if (surecStatuBildirimTipiID.HasValue && surecStatuBildirimTipiID.Value > 0)
                            {
                                var bildirimTipi = _surecStatuleriBildirimTipleriRepository.List(bt =>
                                    bt.TabloID == surecStatuBildirimTipiID.Value && bt.AktifMi == 1)
                                    .FirstOrDefault();

                                if (bildirimTipi != null)
                                {
                                    bildirimTipiTanimlama = bildirimTipi.BildirimTipiTanimlama;
                                }
                            }
                        }
                    }

                    detayliSonuclar.Add(new MalzemeTalepDetayResponse
                    {
                        MalzemeTalep = malzeme,
                        ToplamSevkEdilenMiktar = sevkEdilenToplam,
                        KalanMiktar = Math.Max(0, kalanMiktar), // Negatif değer olmasın
                        ParamTalepSurecStatuID = surecTakip?.ParamTalepSurecStatuID,
                        SurecStatuGirilenNot = surecStatuGirilenNot,
                        SurecStatuBildirimTipiID = surecStatuBildirimTipiID,
                        BildirimTipiTanimlama = bildirimTipiTanimlama
                    });
                }

                return detayliSonuclar.ToResult();
            }
            catch (Exception ex)
            {
                throw new Exception("Malzeme talepleri getirilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzeme talebinde bulunma metodu
        /// </summary>
        /// <param name="request">Talep parametreleri</param>
        /// <returns>Talep işlemi sonucu</returns>
        public Result<bool> MalzemeTalepEt(MalzemeTalepEtRequest request)
        {
            try
            {
                // 1. MalzemeTalepGenelBilgiler'den orijinal miktarı al
                var malzemeTalep = List(x =>
                    x.MalzemeTalebiEssizID == request.MalzemeTalebiEssizID && x.AktifMi == 1).Value.FirstOrDefault();

                if (malzemeTalep == null)
                {
                    throw new Exception("Malzeme talebi bulunamadı.");
                }

                // 2. Önceki sevk edilen miktarları hesapla
                var oncekiSevklerResult = _miktarTarihcesiRepository.List(x =>
                    x.MalzemeTalebiEssizID == request.MalzemeTalebiEssizID && x.AktifMi == 1).ToList();

                var toplamSevkEdilen = oncekiSevklerResult.Sum(x => x.SevkEdilenMiktar);

                // 3. Mevcut kalan miktarı hesapla
                var mevcutKalanMiktar = malzemeTalep.MalzemeOrijinalTalepEdilenMiktar - toplamSevkEdilen;

                // 4. Talep edilen miktar mevcut kalan miktardan fazla olamaz
                var aktarılacakMiktar = Math.Min(request.SevkEdilenMiktar, Math.Max(0, mevcutKalanMiktar));
                var kalanMiktar = Math.Max(0, mevcutKalanMiktar - aktarılacakMiktar);

                // Validasyon: Eğer hiç aktarılacak miktar yoksa hata ver
                if (aktarılacakMiktar <= 0)
                {
                    throw new Exception($"Talep edilen miktar ({request.SevkEdilenMiktar}) mevcut kalan miktardan ({mevcutKalanMiktar}) fazla. Aktarılabilir miktar: {Math.Max(0, mevcutKalanMiktar)}");
                }

                // 5. MalzemeTalepMiktarTarihcesi tablosuna yeni kayıt ekle
                try
                {
                    var miktarTarihcesi = new MalzemeTalepMiktarTarihcesi
                    {
                        MalzemeTalebiEssizID = request.MalzemeTalebiEssizID,
                        SevkEdilenMiktar = aktarılacakMiktar, // Düzeltilmiş miktar
                        KalanMiktar = kalanMiktar,
                        SevkZamani = DateTime.Now, // Local time
                        SevkTalepEdenKisiID = _loginUser.KisiID,
                        MalzemeSevkTalebiYapanDepartmanID = _loginUser.KurumID,
                        MalzemeSevkTalebiYapanKisiID = _loginUser.KisiID,
                        // BaseModel zorunlu alanları
                        AktifMi = 1,
                        SilindiMi = 0,
                        DilID = 1,
                        KisiID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID,
                        KurumID = _loginUser?.KurumID ?? 1,
                        KayitTarihi = DateTime.Now,
                        KayitEdenID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID,
                        GuncellenmeTarihi = DateTime.Now,
                        GuncelleyenKisiID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID
                    };

                    var tarihceResult = _miktarTarihcesiRepository.Add(miktarTarihcesi);
                    var tarihceSaveResult = _miktarTarihcesiRepository.SaveChanges();
                    if (tarihceSaveResult <= 0)
                    {
                        throw new Exception("Miktar tarihçesi kaydedilemedi.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Miktar tarihçesi kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                // 5. MalzemeTalepSurecTakip tablosundaki kaydı güncelle (ParamTalepSurecStatuID = 3)
                try
                {
                    var surecTakipList = _surecTakipRepository.List(x =>
                        x.MalzemeTalebiEssizID == request.MalzemeTalebiEssizID && x.AktifMi == 1).ToList();

                    if (surecTakipList.Any())
                    {
                        var ilkSurecTakip = surecTakipList.First();
                        ilkSurecTakip.ParamTalepSurecStatuID = 3; // Hazırlandı

                        var surecUpdateResult = _surecTakipRepository.Update(ilkSurecTakip);
                        var surecSaveResult = _surecTakipRepository.SaveChanges();
                        if (surecSaveResult <= 0)
                        {
                            throw new Exception("Süreç takip güncellenemedi.");
                        }

                        try
                        {
                            var surecNot = new MalzemeTalepSurecTakipNotlari
                            {
                                MalzemeTalepSurecTakipID = ilkSurecTakip.TabloID,
                                SurecStatuGirilenNot = request.SurecStatuGirilenNot,
                                SurecStatuBildirimTipiID = 0, // Default olarak 0
                                // BaseModel zorunlu alanları
                                AktifMi = 1,
                                SilindiMi = 0,
                                DilID = 1,
                                KisiID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID,
                                KurumID = _loginUser?.KurumID ?? 1,
                                KayitTarihi = DateTime.Now,
                                KayitEdenID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID,
                                GuncellenmeTarihi = DateTime.Now,
                                GuncelleyenKisiID = _loginUser?.KisiID ?? request.SevkTalepEdenKisiID
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
                            throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Süreç takip güncellemesi sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                // Inner exception detayını da logla
                var innerMessage = ex.InnerException != null ? " Inner Exception: " + ex.InnerException.Message : "";
                _logger.LogError(ex, "MalzemeTalepEt hatası: {Message}{InnerMessage}", ex.Message, innerMessage);
                throw new Exception("Malzeme talep edilirken hata oluştu: " + ex.Message + innerMessage);
            }
        }

        /// <summary>
        /// Malzeme taleplerini koşullu filtreleme ile getiren metod
        /// </summary>
        /// <param name="malzemeTalepEtGetir">True ise: [1,2] statüleri veya kalan miktar > 0 olanları getirir</param>
        /// <param name="talepSurecStatuIDs">Talep süreç statü ID'leri</param>
        /// <returns>Filtrelenmiş malzeme talep listesi</returns>
        public Result<List<MalzemeTalepGenelBilgiler>> MalzemeTalepList(bool malzemeTalepEtGetir = false, List<int> talepSurecStatuIDs = null)
        {
            try
            {
                var query = _repository.List(x => x.AktifMi == 1);

                if (malzemeTalepEtGetir)
                {
                    // malzemeTalepEtGetir = true ise: [1,2] statüleri VEYA kalan miktar > 0 olanları getir
                    
                    // 1. Statü [1,2] olan malzeme taleplerinini ID'lerini al
                    var statusOneOrTwoIds = _surecTakipRepository.List(st =>
                        (st.ParamTalepSurecStatuID == 1 || st.ParamTalepSurecStatuID == 2) && st.AktifMi == 1)
                        .Select(st => st.MalzemeTalebiEssizID)
                        .Distinct()
                        .ToList();

                    // 2. Kalan miktarı > 0 olan malzeme taleplerinin ID'lerini al
                    var allMalzemeTaleps = query.ToList();
                    var remainingAmountIds = new List<int>();

                    foreach (var malzeme in allMalzemeTaleps)
                    {
                        var sevkEdilenToplam = _miktarTarihcesiRepository.List(x =>
                            x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .ToList()
                            .Sum(x => x.SevkEdilenMiktar);

                        var kalanMiktar = malzeme.MalzemeOrijinalTalepEdilenMiktar - sevkEdilenToplam;
                        
                        if (kalanMiktar > 0)
                        {
                            remainingAmountIds.Add(malzeme.MalzemeTalebiEssizID);
                        }
                    }

                    // 3. VEYA mantığı: Statü [1,2] VEYA kalan miktar > 0
                    var combinedIds = statusOneOrTwoIds.Union(remainingAmountIds).Distinct().ToList();
                    query = query.Where(x => combinedIds.Contains(x.MalzemeTalebiEssizID));
                }
                else if (talepSurecStatuIDs != null && talepSurecStatuIDs.Any())
                {
                    // malzemeTalepEtGetir = false ise: sadece talepSurecStatuIDs'e bakarak filtrele
                    var malzemeTalepIds = _surecTakipRepository.List(st =>
                        talepSurecStatuIDs.Contains(st.ParamTalepSurecStatuID) && st.AktifMi == 1)
                        .Select(st => st.MalzemeTalebiEssizID)
                        .Distinct()
                        .ToList();

                    query = query.Where(x => malzemeTalepIds.Contains(x.MalzemeTalebiEssizID));
                }

                return query.ToList().ToResult();
            }
            catch (Exception ex)
            {
                throw new Exception("Malzeme talepleri getirilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Hazırlanacak malzeme talep ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        public Result<bool> MalzemeleriHazirla(int malzemeTalebiEssizID)
        {
            try
            {
                if (malzemeTalebiEssizID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // İlgili MalzemeTalepSurecTakip kaydını bul
                var surecTakip = _surecTakipRepository.List(st =>
                    st.MalzemeTalebiEssizID == malzemeTalebiEssizID && st.AktifMi == 1)
                    .FirstOrDefault();

                if (surecTakip == null)
                {
                    throw new Exception("Belirtilen malzeme talebine ait kayıt bulunamadı.");
                }

                // Kaydın statüsünü 4 olarak güncelle
                surecTakip.ParamTalepSurecStatuID = 4;
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
                _logger.LogError(ex, "MalzemeleriHazirla hatası: {Message}", ex.Message);
                throw new Exception("Malzeme hazırlama işlemi sırasında hata oluştu: " + ex.Message);
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
                if (request == null || request.MalzemeTalebiEssizID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // 1. MalzemeTalepSurecTakip kaydını bul ve statüsünü 5 olarak güncelle
                var surecTakip = _surecTakipRepository.List(st =>
                    st.MalzemeTalebiEssizID == request.MalzemeTalebiEssizID && st.AktifMi == 1)
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
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MalzemeIadeEt hatası: {Message}", ex.Message);
                throw new Exception("Malzeme iade işlemi sırasında hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Malzeme kabul etme metodu
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        public Result<bool> MalKabulEt(int malzemeTalebiEssizID)
        {
            try
            {
                if (malzemeTalebiEssizID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // İlgili MalzemeTalepSurecTakip kaydını bul
                var surecTakip = _surecTakipRepository.List(st =>
                    st.MalzemeTalebiEssizID == malzemeTalebiEssizID && st.AktifMi == 1)
                    .FirstOrDefault();

                if (surecTakip == null)
                {
                    throw new Exception("Belirtilen malzeme talebine ait kayıt bulunamadı.");
                }

                // Kaydın statüsünü 6 olarak güncelle (Mal Kabul Edildi)
                surecTakip.ParamTalepSurecStatuID = 6;
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
        /// Malzemeyi hasarlı olarak işaretleme metodu
        /// </summary>
        /// <param name="request">Hasarlı işaretleme parametreleri</param>
        /// <returns>İşaretleme işlemi sonucu</returns>
        public Result<bool> HasarliOlarakIsaretle(HasarliOlarakIşaretleRequest request)
        {
            try
            {
                if (request == null || request.MalzemeTalebiEssizID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // 1. MalzemeTalepSurecTakip kaydını bul ve statüsünü 7 olarak güncelle
                var surecTakip = _surecTakipRepository.List(st =>
                    st.MalzemeTalebiEssizID == request.MalzemeTalebiEssizID && st.AktifMi == 1)
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
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HasarliOlarakIsaretle hatası: {Message}", ex.Message);
                throw new Exception("Malzemeyi hasarlı olarak işaretleme işlemi sırasında hata oluştu: " + ex.Message);
            }
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
    }

    /// <summary>
    /// Malzeme talep detaylı response modeli
    /// </summary>
    public class MalzemeTalepDetayResponse
    {
        /// <summary>
        /// Malzeme talep genel bilgileri
        /// </summary>
        public MalzemeTalepGenelBilgiler MalzemeTalep { get; set; }

        /// <summary>
        /// Toplam sevk edilen miktar
        /// </summary>
        public int ToplamSevkEdilenMiktar { get; set; }

        /// <summary>
        /// Kalan miktar (Orijinal - Sevk Edilen)
        /// </summary>
        public int KalanMiktar { get; set; }

        /// <summary>
        /// Talep süreç statü ID'si
        /// </summary>
        public int? ParamTalepSurecStatuID { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen son not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }

        /// <summary>
        /// Süreç statü bildirim tipi ID'si
        /// </summary>
        public int? SurecStatuBildirimTipiID { get; set; }

        /// <summary>
        /// Bildirim tipi tanımlaması
        /// </summary>
        public string BildirimTipiTanimlama { get; set; }
    }

    /// <summary>
    /// Malzeme talep etme request modeli
    /// </summary>
    public class MalzemeTalepEtRequest
    {
        /// <summary>
        /// Malzeme talebi essiz ID'si
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }

        /// <summary>
        /// Sevk edilen miktar
        /// </summary>
        public int SevkEdilenMiktar { get; set; }

        /// <summary>
        /// Sevk talebinde bulunan kişi ID'si
        /// </summary>
        public int SevkTalepEdenKisiID { get; set; }

        /// <summary>
        /// Malzeme sevk talebinde bulunan departman ID'si
        /// </summary>
        public int MalzemeSevkTalebiYapanDepartmanID { get; set; }

        /// <summary>
        /// Malzeme sevk talebinde bulunan kişi ID'si
        /// </summary>
        public int MalzemeSevkTalebiYapanKisiID { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }
    }

    /// <summary>
    /// Malzemeleri hazırlama request modeli
    /// </summary>
    public class MalzemeleriHazirlaRequest
    {
        /// <summary>
        /// Hazırlanacak malzeme talep ID'si
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }
    }

    /// <summary>
    /// Malzeme iade etme request modeli
    /// </summary>
    public class MalzemeIadeEtRequest
    {
        /// <summary>
        /// İade edilecek malzeme talep ID'si
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }

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
        public int MalzemeTalebiEssizID { get; set; }

        /// <summary>
        /// Süreç statü bildirim tipi ID'si
        /// </summary>
        public int SurecStatuBildirimTipiID { get; set; }

        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }
    }
}