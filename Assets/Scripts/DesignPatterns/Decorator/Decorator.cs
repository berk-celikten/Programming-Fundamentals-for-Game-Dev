using System;
using UnityEngine;

namespace DesignPatterns.DecoratorRefactored
{

    public class Decorator : MonoBehaviour
    {

    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: DECORATOR (SÜSLEYİCİ / SARMALAYICI) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Decorator, var olan bir nesneye **kodunu değiştirmeden** ve **kalıtım (inheritance) hiyerarşisini 
         karmaşıklaştırmadan** dinamik olarak yeni sorumluluklar/özellikler eklemeyi sağlayan yapısal (structural) 
         bir tasarım desenidir.
       - Nesneyi başka bir nesnenin içine sarmalayarak (wrapping) bir "soğan katmanı" yapısı oluşturur.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Sınıf Patlamasını (Class Explosion) Önler:** Örneğin bir silahınız var (M4A1). Buna Susturucu, 
         Dürbün ve Alev Gizleyen eklenebilsin. Kalıtım kullansaydınız `M4A1WithScope`, `M4A1WithScopeAndSilencer` 
         gibi onlarca alt sınıf türetmeniz gerekirdi. Decorator ile bu özellikleri run-time'da esnekçe birleştirirsiniz.
       - **Açık/Kapalı Prensibi (Open/Closed Principle):** Mevcut çalışan temel koda (`BaseRifle`) dokunmadan 
         sisteme yeni modifikasyonlar (`SilencerDecorator`, `ScopeDecorator`) ekleyebilirsiniz.

    3. COMPOSITE PATTERN ILE FARKI NEDİR?
       - **Composite:** Nesneleri ağaç yapısında toplar (1-to-Many). Amacı gruptaki elemanları TEK BİR nesneymiş 
         gibi sırayla çalıştırmaktır.
       - **Decorator:** Tek bir nesneyi katman katman sarmalar (1-to-1 zinciri). Amacı orijinal nesnenin davranışını 
         değiştirmek, genişletmek veya önüne/arkasına ön koşul (zamanlayıcı, mana, ses vb.) eklemektir.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Component (IWeapon):** Temel nesnelerin ve dekoratörlerin paylaştığı ortak arayüz.
       - **Concrete Component (BaseRifle):** Süsleme yapılmamış, saf/temel işi yapan somut sınıf.
       - **Base Decorator (WeaponDecorator):** IWeapon arayüzünü uygular ve içinde başka bir IWeapon nesnesi tutar.
       - **Concrete Decorators (SilencerDecorator, CooldownDecorator):** Temel nesneye ekstra güç/özellik katan 
         somut katmanlar.
       - **Client (WeaponTester):** Sarmalanmış silahı tetikleyen istemci sınıf.
    ====================================================================================================
    */

    // =========================================================================
    // 1. COMPONENT INTERFACE (Ortak Arayüz)
    // Sorumluluk: Hem yalın silahın hem de süsleyici eklentilerin ortak sözleşmesi.
    // =========================================================================
    public interface IWeapon
    {
        void Fire(GameObject shooter);
        float GetDamage();
    }

    // =========================================================================
    // 2. CONCRETE COMPONENT (Temel / Yalın Nesne)
    // Sorumluluk: Eklentisiz, en yalın haliyle ateş etme işini yapan ana sınıf.
    // =========================================================================
    [Serializable]
    public class BaseRifle : IWeapon
    {
        private readonly float _baseDamage = 25f;

        public void Fire(GameObject shooter)
        {
            Debug.Log($"[Ateş] {shooter.name} varsayılan tüfekle ateş etti. Standart ses efekti oynatılıyor.");
        }

        public float GetDamage()
        {
            return _baseDamage;
        }
    }

    // =========================================================================
    // 3. BASE DECORATOR (Temel Süsleyici Soyut Sınıf / Arayüz Sarmalayıcı)
    // Sorumluluk: İçinde sarmalanacak IWeapon nesnesini tutar ve çağrıları ona aktarır.
    // =========================================================================
    [Serializable]
    public abstract class WeaponDecorator : IWeapon
    {
        [SerializeReference] protected IWeapon _wrappedWeapon;

        protected WeaponDecorator(IWeapon weapon)
        {
            _wrappedWeapon = weapon;
        }

        public virtual void Fire(GameObject shooter)
        {
            _wrappedWeapon.Fire(shooter);
        }

        public virtual float GetDamage()
        {
            return _wrappedWeapon.GetDamage();
        }
    }

    // =========================================================================
    // 4. CONCRETE DECORATORS (Somut Süsleyiciler / Eklentiler)
    // =========================================================================

    // Süsleyici 1: Susturucu Eklentisi (Sesi ve Görünürlüğü Değiştirir)
    [Serializable]
    public class SilencerDecorator : WeaponDecorator
    {
        public SilencerDecorator(IWeapon weapon) : base(weapon) { }

        public override void Fire(GameObject shooter)
        {
            Debug.Log("[Susturucu] Ateş sesi bastırıldı. Düşman radarında görünme ihtimali azaldı.");
            base.Fire(shooter); // Sarmalanan asıl silah çalışır
        }

        public override float GetDamage()
        {
            // Susturucu mermi hızını biraz düşürdüğü için hasardan 2 puan eksiltir
            return base.GetDamage() - 2f;
        }
    }

    // Süsleyici 2: Gecikme / Bekleme Süresi Eklentisi (Ateş Etmeden Önce Bekletir)
    [Serializable]
    public class CooldownDecorator : WeaponDecorator
    {
        private readonly float _delaySeconds;

        public CooldownDecorator(IWeapon weapon, float delaySeconds) : base(weapon)
        {
            _delaySeconds = delaySeconds;
        }

        public override void Fire(GameObject shooter)
        {
            Debug.Log($"[Geciktirici] {_delaySeconds} saniyelik kurma/tetik mekanizması gecikmesi uygulanıyor...");
            base.Fire(shooter);
        }
    }

    // Süsleyici 3: Lazer Görüşü Eklentisi (Hasarı / Hassasiyeti Artırır)
    [Serializable]
    public class LaserSightDecorator : WeaponDecorator
    {
        public LaserSightDecorator(IWeapon weapon) : base(weapon) { }

        public override void Fire(GameObject shooter)
        {
            Debug.Log("[Lazer] Hedef kilitlendi, hassas nişan alındı.");
            base.Fire(shooter);
        }

        public override float GetDamage()
        {
            // Lazer nişangah kritik vuruş ihtimalini artırıp hasara 10 puan ekler
            return base.GetDamage() + 10f;
        }
    }

    // =========================================================================
    // 5. CLIENT / RUNNER (İstemci Sınıf)
    // Sorumluluk: Silahı kullanır. Silahın kaç katman sarmalandığını umursamaz.
    // =========================================================================
    public class WeaponTester : MonoBehaviour
    {
        [SerializeReference]
        private IWeapon _equippedWeapon;

        private void Start()
        {
            // 1. Yalın bir tüfek oluşturulur
            IWeapon rifle = new BaseRifle();

            // 2. Dinamik olarak katman katman süslenir (Susturucu + Lazer + Geciktirici)
            IWeapon silencedRifle = new SilencerDecorator(rifle);
            IWeapon upgradedRifle = new LaserSightDecorator(silencedRifle);
            _equippedWeapon = new CooldownDecorator(upgradedRifle, delaySeconds: 0.5f);
        }

        public void Shoot()
        {
            if (_equippedWeapon != null)
            {
                _equippedWeapon.Fire(gameObject);
                Debug.Log($"[Toplam Hasar] {_equippedWeapon.GetDamage()} PT");
            }
        }
    }
}