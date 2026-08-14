using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns.ObserverRefactored
{
    public class Observer : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: OBSERVER (GÖZLEMCİ) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Observer deseni, bir nesnede (Subject / Publisher) meydana gelen durum değişikliklerini, 
         bu nesneye bağımlı olan diğer nesnelere (Observer / Subscriber) otomatik olarak bildiren 
         davranışsal (behavioral) bir tasarım desenidir.
       - C# dilinde bu desen `delegate`, `event` ve `System.Action` / `System.Func` yapılarıyla 
         yerleşik olarak desteklenir. Unity tarafında ayrıca Inspector'dan sürüklenebilen `UnityEvent` 
         seçeneği de bulunur.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Gevşek Bağlantı (Loose Coupling):** Yayıncı sınıf (Subject), kendisini dinleyen gözlemcilerin 
         kim olduğunu veya ne iş yaptığını bilmek zorunda değildir. Sadece "Bir şey oldu!" diye bildirim fırlatır.
       - **Update Spagettisinden Kurtarır:** Her karede (`Update` içinde) `if (ship.IsOverheated)` 
         şeklinde durum kontrolü (polling) yapmak yerine, olay gerçekleştiğinde tepki vermeyi sağlar (Event-Driven Architecture).

    3. C# EVENT vs. UNITYEVENT KARŞILAŞTIRMASI:
       - **C# `event Action`:** Bellek dostudur, sıfır GC Alloc üretir, çok hızlıdır. Yalnızca kod tarafında abone olunur.
       - **UnityEvent:** Unity Inspector penceresinden sürükle-bırak ile görsel bağlantı yapmayı sağlar. 
         Geliştirici olmayan tasarımcılar için harikadır ancak hafif bir performans maliyeti vardır.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Subject (Yayıncı - `ShipReactor`):** Enerji biriktirir ve aşırı ısınma/aşırı şarj olaylarını yayınlar.
       - **Observer 1 (Gözlemci - `ShieldGenerator`):** Reaktör şarj olduğunda kalkanı otomatik olarak yeniler.
       - **Observer 2 (Gözlemci - `FlightTelemetryLogger`):** Reaktör durumunu konsola / arayüze loglar.
    ====================================================================================================
    */

    // =========================================================================
    // 1. SUBJECT / PUBLISHER (Yayıncı Katmanı - Uzay Gemisi Reaktörü)
    // Sorumluluk: Veriyi günceller ve olay gerçekleştiğinde aboneleri bilgilendirir.
    // =========================================================================
    public class ShipReactor : MonoBehaviour
    {
        // Parametresiz C# Event
        public event Action OnOverheat;

        // Parametre Taşıyan C# Event (Güncel Şarj Yüzdesini Gönderir)
        public event Action<float> OnEnergyRecharged;

        // Unity Inspector'dan görsel olarak bağlanabilen UnityEvent
        [SerializeField] private UnityEvent _onReactorCriticalUnityEvent;

        [SerializeField] private float _maxEnergy = 100f;
        [SerializeField] private float _chargeRatePerSecond = 15f;
        private float _currentEnergy;

        public float CurrentEnergy => _currentEnergy;
        public float MaxEnergy => _maxEnergy;

        private void Start()
        {
            StartCoroutine(RechargeRoutine());
        }

        private IEnumerator RechargeRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                AccumulateEnergy(_chargeRatePerSecond);
            }
        }

        public void AccumulateEnergy(float amount)
        {
            _currentEnergy += amount;

            // Abonelere enerji değişimini duyur
            OnEnergyRecharged?.Invoke(_currentEnergy / _maxEnergy);

            // Limit aşıldıysa ısınma olayını fırlat
            if (_currentEnergy >= _maxEnergy)
            {
                _currentEnergy = 0f; // Reaktör sıfırlanır
                OnOverheat?.Invoke();
                _onReactorCriticalUnityEvent?.Invoke();
            }
        }
    }

    // =========================================================================
    // 2. OBSERVER 1 (Gözlemci Katmanı - Kalkan Üreteci)
    // Sorumluluk: Reaktördeki olaylara yanıt vererek kalkan seviyesini korur.
    // =========================================================================
    public class ShieldGenerator : MonoBehaviour
    {
        [SerializeField] private ShipReactor _reactor;
        [SerializeField] private float _maxShieldCapacity = 100f;
        private float _currentShield;

        public float CurrentShield => _currentShield;

        private void OnEnable()
        {
            if (_reactor != null)
            {
                // Event'lere Abone Olma (Subscription)
                _reactor.OnOverheat += RestoreShieldToFull;
            }
        }

        private void OnDisable()
        {
            if (_reactor != null)
            {
                // Bellek Sızıntılarını (Memory Leak) Önlemek İçin Abonelikten Çıkma (Unsubscription)
                _reactor.OnOverheat -= RestoreShieldToFull;
            }
        }

        private void RestoreShieldToFull()
        {
            _currentShield = _maxShieldCapacity;
            Debug.Log("<color=cyan>[ShieldGenerator]</color> Reaktör aşırı şarj oldu! Kalkanlar %100 seviyesine yenilendi.");
        }
    }

    // =========================================================================
    // 3. OBSERVER 2 (Gözlemci Katmanı - Uçuş Telemetri Loglayıcı)
    // Sorumluluk: Reaktörün verilerini takip eder ve loglar.
    // =========================================================================
    public class FlightTelemetryLogger : MonoBehaviour
    {
        [SerializeField] private ShipReactor _reactor;

        private void OnEnable()
        {
            if (_reactor != null)
            {
                _reactor.OnEnergyRecharged += HandleEnergyRecharged;
                _reactor.OnOverheat += HandleReactorOverheat;
            }
        }

        private void OnDisable()
        {
            if (_reactor != null)
            {
                _reactor.OnEnergyRecharged -= HandleEnergyRecharged;
                _reactor.OnOverheat -= HandleReactorOverheat;
            }
        }

        private void HandleEnergyRecharged(float fillPercentage)
        {
            Debug.Log($"<color=yellow>[Telemetry]</color> Reaktör Doluluk Oranı: %{fillPercentage * 100f:F0}");
        }

        private void HandleReactorOverheat()
        {
            Debug.LogWarning("<color=red>[Telemetry WARNING]</color> KKRİTİK UYARI: Reaktör AŞIRI ISINDI!");
        }
    }
}