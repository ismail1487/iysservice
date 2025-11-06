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
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Hazırlanacak malzeme talep süreç takip ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        Result<bool> MalzemeleriHazirla(int malzemeTalepSurecTakipID);

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
                // ⭐ YENİ: Süreç takip tablosundan başla (önceki: MalzemeTalepGenelBilgiler'den başlıyordu)
                IQueryable<MalzemeTalepSurecTakip> surecQuery = _surecTakipRepository.List(x => x.AktifMi == 1);

                // Statü filtresi
                if (request.MalzemeTalepEtGetir)
                {
                    var statusOneOrTwo = surecQuery.Where(x =>
                        x.ParamTalepSurecStatuID == 1 ||
                        x.ParamTalepSurecStatuID == 2);

                    var allMalzemeler = _repository.List(x => x.AktifMi == 1).ToList();
                    var kalanMiktarVarIds = new List<int>();

                    foreach (var malzeme in allMalzemeler)
                    {
                        var sevkEdilenToplam = _miktarTarihcesiRepository.List(x =>
                            x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID && x.AktifMi == 1)
                            .Sum(x => (int?)x.SevkEdilenMiktar) ?? 0;

                        if (malzeme.MalzemeOrijinalTalepEdilenMiktar > sevkEdilenToplam)
                        {
                            kalanMiktarVarIds.Add(malzeme.MalzemeTalebiEssizID);
                        }
                    }

                    var kalanMiktarSurecler = _surecTakipRepository.List(x =>
                        kalanMiktarVarIds.Contains(x.MalzemeTalebiEssizID) && x.AktifMi == 1)
                        .GroupBy(x => x.MalzemeTalebiEssizID)
                        .Select(g => g.OrderByDescending(x => x.TabloID).First())
                        .ToList();

                    var combinedIds = statusOneOrTwo.Select(x => x.TabloID)
                        .Union(kalanMiktarSurecler.Select(x => x.TabloID))
                        .ToList();

                    surecQuery = _surecTakipRepository.List(x =>
                        combinedIds.Contains(x.TabloID) && x.AktifMi == 1);
                }
                else if (request.TalepSurecStatuIDs != null && request.TalepSurecStatuIDs.Any())
                {
                    surecQuery = surecQuery.Where(x => request.TalepSurecStatuIDs.Contains(x.ParamTalepSurecStatuID));
                }

                var surecKayitlari = surecQuery.ToList();

                // Proje kodu filtresi
                if (request.ProjeKodu.HasValue && request.ProjeKodu.Value > 0)
                {
                    var projeKodundakiMalzemeler = _repository.List(x =>
                        x.ProjeKodu == request.ProjeKodu.Value && x.AktifMi == 1)
                        .Select(x => x.MalzemeTalebiEssizID)
                        .ToList();

                    surecKayitlari = surecKayitlari
                        .Where(x => projeKodundakiMalzemeler.Contains(x.MalzemeTalebiEssizID))
                        .ToList();
                }

                // Response oluştur
                var detayliSonuclar = new List<MalzemeTalepDetayResponse>();

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

                    // ⭐ YENİ: Bu süreç için talep edilen miktar (FK ile)
                    var talepEdilenMiktar = _miktarTarihcesiRepository.List(x =>
                        x.MalzemeTalepSurecTakipID == surecTakip.TabloID &&
                        x.AktifMi == 1)
                        .FirstOrDefault()?.SevkEdilenMiktar ?? 0;

                    var toplamSevkEdilen = _miktarTarihcesiRepository.List(x =>
                        x.MalzemeTalebiEssizID == malzeme.MalzemeTalebiEssizID &&
                        x.AktifMi == 1)
                        .Sum(x => (int?)x.SevkEdilenMiktar) ?? 0;

                    var kalanMiktar = Math.Max(0, malzeme.MalzemeOrijinalTalepEdilenMiktar - toplamSevkEdilen);

                    var enSonNot = _surecTakipNotlariRepository.List(n =>
                        n.MalzemeTalepSurecTakipID == surecTakip.TabloID && n.AktifMi == 1)
                        .AsEnumerable()
                        .OrderByDescending(n => n.TabloID)
                        .FirstOrDefault();

                    string bildirimTipiTanimlama = null;
                    if (enSonNot?.SurecStatuBildirimTipiID != null && enSonNot.SurecStatuBildirimTipiID > 0)
                    {
                        bildirimTipiTanimlama = _surecStatuleriBildirimTipleriRepository.List(bt =>
                            bt.TabloID == enSonNot.SurecStatuBildirimTipiID && bt.AktifMi == 1)
                            .FirstOrDefault()?.BildirimTipiTanimlama;
                    }

                    detayliSonuclar.Add(new MalzemeTalepDetayResponse
                    {
                        MalzemeTalepSurecTakipID = surecTakip.TabloID,
                        MalzemeTalebiEssizID = malzeme.MalzemeTalebiEssizID,
                        MalzemeTalep = malzeme,
                        TalepEdilenMiktar = talepEdilenMiktar,        
                        ToplamSevkEdilenMiktar = toplamSevkEdilen,
                        KalanMiktar = kalanMiktar,
                        ParamTalepSurecStatuID = surecTakip.ParamTalepSurecStatuID,
                        SurecOlusturmaTarihi = surecTakip.SurecTetiklenmeZamani,
                        SurecStatuGirilenNot = enSonNot?.SurecStatuGirilenNot,
                        SurecStatuBildirimTipiID = enSonNot?.SurecStatuBildirimTipiID,
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
                throw new Exception("Malzeme talep edilirken hata oluştu: " + ex.Message + innerMessage);
            }
        }

        /// <summary>
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için metod
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Hazırlanacak malzeme talep ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        public Result<bool> MalzemeleriHazirla(int malzemeTalepSurecTakipID)
        {
            try
            {
                if (malzemeTalepSurecTakipID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // İlgili MalzemeTalepSurecTakip kaydını bul
                var surecTakip = _surecTakipRepository
                    .List(x => x.TabloID == malzemeTalepSurecTakipID && x.AktifMi == 1)
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
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
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
        /// <param name="malzemeTalepSurecTakipID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID)
        {
            try
            {
                if (malzemeTalepSurecTakipID <= 0)
                {
                    throw new Exception("Geçerli bir malzeme talep ID'si gereklidir.");
                }

                // İlgili MalzemeTalepSurecTakip kaydını bul
                var surecTakip = _surecTakipRepository
                    .List(x => x.TabloID == malzemeTalepSurecTakipID && x.AktifMi == 1)
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
                    throw new Exception("Süreç takip notu kaydı sırasında hata: " + ex.Message + (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
                }

                return true.ToResult();
            }
            catch (Exception ex)
            {
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
    /// Malzeme talep detay response modeli
    /// </summary>
    public class MalzemeTalepDetayResponse
    {
        /// <summary>
        /// Malzeme talep süreç takip ID'si
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }

        /// <summary>
        /// Malzeme talebi essiz ID'si
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }
        /// <summary>
        /// Malzeme talep genel bilgiler
        /// </summary>
        public MalzemeTalepGenelBilgiler MalzemeTalep { get; set; }

        /// <summary>
        /// Talep edilen miktar
        /// </summary>
        public int TalepEdilenMiktar { get; set; }

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
        /// Süreç oluşturma tarihi
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
}