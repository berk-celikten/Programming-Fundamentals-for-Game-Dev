using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.CompositeRefactored
{

    public class Composıte : MonoBehaviour
    {

    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: COMPOSITE (BİLEŞİK) PATTERN
    (Ek Olarak: Decorator Pattern İçerir)
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Composite deseni, nesneleri **Ağaç Yapısı (Tree Structure)** şeklinde hiyerarşik olarak gruplamaya
         yarayan yapısal (structural) bir tasarım desenidir.
       - Tek bir nesne (Yaprak / Leaf) ile bu nesnelerin oluşturduğu grubu (Bileşik / Composite) **aynı arayüz (Interface)**
         üzerinden yönetebilmemizi sağlar.
       - Sistem, tetiklenen eylemin tek bir etki mi yoksa birden fazla etkinin birleşimi mi olduğunu bilmek 
         zorunda kalmaz. Hepsi `ISpell` (veya `IAbility`) arayüzü üzerinden tek bir metotla çağrılır.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Karmaşık Kombinasyonlar / Kombolar:** Büyü, yetenek veya envanter sistemlerinde tekil efektleri
         (örn: Hasar, Ses, Görsel Efekt) bir araya getirip tek bir "Zincirleme Büyü" gibi çalıştırmayı sağlar.
       - **Esneklik (Flexibility):** Yeni bir büyü zinciri oluştururken mevcut sınıfların koduna dokunmadan
         sadece nesne gruplamasını/dizilimini değiştirmek yeterlidir.
       - **Polimorfizm (Çok Biçimlilik):** Büyüyü tetikleyen sınıf (`SpellCaster`), arka planda 1 efektin mi
         yoksa 10 zincirleme efektin mi çalışacağını umursamadan sadece `Cast()` metodunu çağırır.

    3. KODDAKİ EK PATTERN: DECORATOR (SÜSLEYİCİ)
       - `ManaCostDecorator` sınıfı **Decorator Pattern** örneğidir. 
       - Var olan bir büyünün/yetenek nesnesinin orijinal kodunu değiştirmeden, dışını sarmalayarak ona 
         "Mana Kontrolü / Tüketimi" gibi ekstra bir davranış kazandırır.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Component (ISpell Arayüzü):** Tüm tekil efektlerin ve grup sınıflarının uymak zorunda olduğu ortak şablon.
       - **Leaf (Tekil Yaprak Nesneler):** `ApplyDamageEffect`, `PlayVisualEffect`, `ApplyShieldEffect`.
         Sadece kendi sorumluluğundaki tekil işi yapan en alt sınıflardır.
       - **Composite (Bileşik Grup):** `SpellSequenceComposite`. İçerisinde birden fazla `ISpell` tutar 
         ve `Cast()` çağrıldığında listedekileri sırayla tetikler.
       - **Decorator (Süsleyici):** `ManaCostDecorator`. Bir `ISpell` nesnesini sarmalar; önce mana 
         miktarını kontrol eder, mana yeterliyse içindeki büyüyü tetikler.
       - **Client (İstemci):** `SpellCaster`. Büyüyü başlatan sınıftır, arkadaki karmaşık yapıyla uğraşmaz.
    ====================================================================================================
    */

    // =========================================================================
    // 1. COMPONENT INTERFACE (Ortak Arayüz)
    // Sorumluluk: Tekil efektlerin ve grup nesnelerinin ortak sözleşmesi.
    // =========================================================================
    public interface ISpell
    {
        void Cast(GameObject target);
    }

    // =========================================================================
    // 2. LEAF CLASSES (Tekil Yaprak Nesneler - Somut Efektler)
    // Sorumluluk: Başka nesne barındırmayan, doğrudan hedefe etki eden sınıflar.
    // =========================================================================

    [Serializable]
    public class ApplyDamageEffect : ISpell
    {
        private readonly float _damageAmount;

        public ApplyDamageEffect(float damageAmount)
        {
            _damageAmount = damageAmount;
        }

        public void Cast(GameObject target)
        {
            Debug.Log($"[Efekt] {target.name} adlı hedefe {_damageAmount} puan doğrudan hasar verildi.");
        }
    }

    [Serializable]
    public class ApplyShieldEffect : ISpell
    {
        private readonly float _shieldAmount;

        public ApplyShieldEffect(float shieldAmount)
        {
            _shieldAmount = shieldAmount;
        }

        public void Cast(GameObject target)
        {
            Debug.Log($"[Efekt] {target.name} adlı hedefe {_shieldAmount} puan koruma kalkanı eklendi.");
        }
    }

    [Serializable]
    public class PlayVisualEffect : ISpell
    {
        private readonly string _vfxName;

        public PlayVisualEffect(string vfxName)
        {
            _vfxName = vfxName;
        }

        public void Cast(GameObject target)
        {
            Debug.Log($"[Görsel] {target.name} üzerinde '{_vfxName}' efekti oynatılıyor.");
        }
    }

    // =========================================================================
    // 3. COMPOSITE (Bileşik Grup Nesnesi)
    // Sorumluluk: İçerisinde birden fazla ISpell tutar ve hepsini sırayla çalıştırır.
    // =========================================================================
    [Serializable]
    public class SpellSequenceComposite : ISpell
    {
        [SerializeReference] private List<ISpell> _spells = new List<ISpell>();

        public SpellSequenceComposite(List<ISpell> spells)
        {
            _spells = spells;
        }

        public void Cast(GameObject target)
        {
            Debug.Log("--- Zincirleme Büyü Başlatıldı ---");
            foreach (var spell in _spells)
            {
                spell.Cast(target);
            }
            Debug.Log("--- Zincirleme Büyü Tamamlandı ---");
        }
    }

    // =========================================================================
    // 4. DECORATOR (Süsleyici / Ek Özellik Katıcı Sınıf)
    // Sorumluluk: Sarmaladığı büyünün önüne Mana Tüketimi kontrolü ekler.
    // =========================================================================
    [Serializable]
    public class ManaCostDecorator : ISpell
    {
        [SerializeReference] private ISpell _wrappedSpell;
        private readonly float _requiredMana;
        private float _currentMana;

        public ManaCostDecorator(ISpell wrappedSpell, float requiredMana, float currentMana)
        {
            _wrappedSpell = wrappedSpell;
            _requiredMana = requiredMana;
            _currentMana = currentMana;
        }

        public void Cast(GameObject target)
        {
            if (_currentMana >= _requiredMana)
            {
                _currentMana -= _requiredMana;
                Debug.Log($"[Decorator] {_requiredMana} mana harcandı. Kalan Mana: {_currentMana}");
                _wrappedSpell.Cast(target);
            }
            else
            {
                Debug.LogWarning("[Decorator] Yetersiz mana! Büyü başarısız oldu.");
            }
        }
    }

    // =========================================================================
    // 5. CLIENT / RUNNER (İstemci Sınıf)
    // Sorumluluk: Büyüyü tetikler. Tek bir efekt mi yoksa karmaşık bir zincir mi
    // olduğunu bilmeden sadece Cast() metodunu çağırır.
    // =========================================================================
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private GameObject targetDummy;

        [SerializeReference]
        private ISpell _comboSpell;

        private void Start()
        {
            // Büyü zincirimizi oluşturuyoruz (Composite Yapı)
            var spellSequence = new List<ISpell>
            {
                new PlayVisualEffect("Fire_Explosion_VFX"),
                new ApplyDamageEffect(75f),
                new ApplyShieldEffect(20f)
            };

            ISpell baseCompositeSpell = new SpellSequenceComposite(spellSequence);

            // Büyü zincirini Mana Kontrolü ile sarmalıyoruz (Decorator Yapı)
            _comboSpell = new ManaCostDecorator(baseCompositeSpell, requiredMana: 50f, currentMana: 100f);
        }

        public void CastSpell()
        {
            if (targetDummy != null)
            {
                _comboSpell.Cast(targetDummy);
            }
        }
    }
}