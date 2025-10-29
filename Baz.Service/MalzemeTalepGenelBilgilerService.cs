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
    }

    /// <summary>
    /// MalzemeTalepGenelBilgiler ile ilgili işlemleri yöneten servis sınıfı
    /// </summary>
    public class MalzemeTalepGenelBilgilerService : Service<MalzemeTalepGenelBilgiler>, IMalzemeTalepGenelBilgilerService
    {
        private readonly IRepository<MalzemeTalepSurecTakip> _surecTakipRepository;
        private readonly IRepository<MalzemeTalepMiktarTarihcesi> _miktarTarihcesiRepository;
        private readonly IRepository<MalzemeTalepSurecTakipNotlari> _surecTakipNotlariRepository;
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
        /// <param name="loginUser"></param>
        public MalzemeTalepGenelBilgilerService(
            IRepository<MalzemeTalepGenelBilgiler> repository,
            IDataMapper dataMapper,
            IServiceProvider serviceProvider,
            ILogger<MalzemeTalepGenelBilgilerService> logger,
            IRepository<MalzemeTalepSurecTakip> surecTakipRepository,
            IRepository<MalzemeTalepMiktarTarihcesi> miktarTarihcesiRepository,
            IRepository<MalzemeTalepSurecTakipNotlari> surecTakipNotlariRepository,
            ILoginUser loginUser) : base(repository, dataMapper, serviceProvider, logger)
        {
            _surecTakipRepository = surecTakipRepository;
            _miktarTarihcesiRepository = miktarTarihcesiRepository;
            _surecTakipNotlariRepository = surecTakipNotlariRepository;
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

                    detayliSonuclar.Add(new MalzemeTalepDetayResponse
                    {
                        MalzemeTalep = malzeme,
                        ToplamSevkEdilenMiktar = sevkEdilenToplam,
                        KalanMiktar = Math.Max(0, kalanMiktar) // Negatif değer olmasın
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
                        SevkTalepEdenKisiID = request.SevkTalepEdenKisiID,
                        MalzemeSevkTalebiYapanDepartmanID = request.MalzemeSevkTalebiYapanDepartmanID,
                        MalzemeSevkTalebiYapanKisiID = request.MalzemeSevkTalebiYapanKisiID,
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
                                SurecStatuBildirimTipiID = request.SurecStatuBildirimTipiID,
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

        /// <summary>
        /// Süreç statüsü bildirim tipi ID'si
        /// </summary>
        public int SurecStatuBildirimTipiID { get; set; }
    }
}