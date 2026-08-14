using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPatterns.ModelViewPresenterRefactored
{
    public class ModelViewPresenter : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: MODEL-VIEW-PRESENTER (MVP) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - MVP, kullanıcı arayüzü (UI) mantığını, iş/oyun mantığından (Business Logic) ayıran bir mimari desendir.
       - Üç ana bileşene ayrılır:
         • **Model:** Veriyi ve oyun mantığını tutar. UI'dan tamamen habersizdir.
         • **View:** Ekrandaki buton, metin ve barları yönetir. Sadece Presenter'dan gelen komutları çizer.
         • **Presenter:** Model ve View arasındaki köprüdür. Model'deki olayları (event) dinler, View'ı günceller;
           View'dan gelen kullanıcı girdilerini (Button click vb.) alıp Model'e aktarır.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Spagetti Kodu Önler:** Can, XP veya Stamina değerlerinin tutulduğu sınıflara `Text`, `Image` veya 
         `Slider` kütüphanelerini entegre etmek kilitlenmeye ve karmaşaya yol açar. MVP bunu tamamen ayırır.
       - **UI Değişikliklerine Karşı Esneklik:** NGUI'den UI Toolkit'e veya uGUI'ye geçildiğinde Model koduna 
         hiç dokunulmaz. Sadece View/Presenter tarafı güncellenir.
       - **Test Edilebilirlik (Unit Testing):** Model sınıfları Unity UI bileşenlerine bağımlı olmadığı için 
         UI olmadan da kolayca otomatik birim testlerine tabi tutulabilir.

    3. KODDA YAPILAN MİMARİ İYİLEŞTİRME (PASSIVE VIEW):
       - Ham kodda Presenter doğrudan `Text` ve `Slider` bileşenlerine erişiyordu. Bu durum Presenter'ı 
         UI framework'üne bağımlı kılar.
       - Özgün tasarımda `StaminaView` gibi **View Sınıfları** oluşturularak Presenter tamamen View 
         arayüzleri üzerinden haberleşecek şekilde soyutlaştırılmıştır.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Model (`StaminaModel`, `EnergyModel`):** Veriyi saklar, harcar, yeniler. Event fırlatır.
       - **View (`StaminaView`, `EnergyView`):** Sadece ekrandaki elemanları günceller ve tıklamaları iletir.
       - **Presenter (`StaminaPresenter`, `EnergyPresenter`):** Model ve View'ı birleştirir.
    ====================================================================================================
    */

    // =========================================================================
    // 1. MODEL KATMANI (Veri & Oyun Mantığı - UI'dan Tamamen Bağımsız)
    // =========================================================================

    // Model 1: Oyuncunun Enerjisi (XP yerine)
    public class EnergyModel : MonoBehaviour
    {
        public event Action OnEnergyChanged;
        public event Action OnEnergyMaxedOut;

        [SerializeField] private int _maxEnergy = 100;
        private int _currentEnergy;

        public int CurrentEnergy => _currentEnergy;
        public int MaxEnergy => _maxEnergy;

        public void AddEnergy(int amount)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0, _maxEnergy);
            OnEnergyChanged?.Invoke();

            if (_currentEnergy >= _maxEnergy)
            {
                OnEnergyMaxedOut?.Invoke();
            }
        }

        public void ResetEnergy()
        {
            _currentEnergy = _maxEnergy;
            OnEnergyChanged?.Invoke();
        }
    }

    // Model 2: Oyuncunun Stamina / Dayanıklılığı (Health yerine)
    public class StaminaModel : MonoBehaviour
    {
        public event Action OnStaminaChanged;

        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _drainRatePerSecond = 5f;
        private float _currentStamina;

        public float NormalizedStamina => _currentStamina / _maxStamina;

        private void Awake()
        {
            RestoreFullStamina();
            StartCoroutine(DrainRoutine());
        }

        private void OnEnable()
        {
            // Model-Model Arası Etkileşim: Enerji dolduğunda Stamina yenilenir
            var energyModel = GetComponent<EnergyModel>();
            if (energyModel != null)
            {
                energyModel.OnEnergyMaxedOut += RestoreFullStamina;
            }
        }

        private void OnDisable()
        {
            var energyModel = GetComponent<EnergyModel>();
            if (energyModel != null)
            {
                energyModel.OnEnergyMaxedOut -= RestoreFullStamina;
            }
        }

        public void RestoreFullStamina()
        {
            _currentStamina = _maxStamina;
            OnStaminaChanged?.Invoke();
        }

        private IEnumerator DrainRoutine()
        {
            while (_currentStamina > 0)
            {
                _currentStamina = Mathf.Max(0, _currentStamina - _drainRatePerSecond);
                OnStaminaChanged?.Invoke();
                yield return new WaitForSeconds(1f);
            }
        }
    }

    // =========================================================================
    // 2. VIEW KATMANI (Arayüz / Görsel Sınıflar - Sadece UI İşlerini Yapar)
    // =========================================================================

    public class EnergyView : MonoBehaviour
    {
        [SerializeField] private Text _energyValueText;
        [SerializeField] private Button _chargeEnergyButton;

        public Button ChargeEnergyButton => _chargeEnergyButton;

        public void DisplayEnergy(int current, int max)
        {
            if (_energyValueText != null)
            {
                _energyValueText.text = $"Enerji: {current} / {max}";
            }
        }
    }

    public class StaminaView : MonoBehaviour
    {
        [SerializeField] private Slider _staminaBar;

        public void UpdateStaminaBar(float fillAmount)
        {
            if (_staminaBar != null)
            {
                _staminaBar.value = fillAmount;
            }
        }
    }

    // =========================================================================
    // 3. PRESENTER KATMANI (Model ve View Arasındaki Köprü)
    // =========================================================================

    public class EnergyPresenter : MonoBehaviour
    {
        [SerializeField] private EnergyModel _model;
        [SerializeField] private EnergyView _view;

        private void Start()
        {
            // View'daki buton tıklamasını Model'deki fonksiyona bağlar
            if (_view.ChargeEnergyButton != null)
            {
                _view.ChargeEnergyButton.onClick.AddListener(OnChargeButtonClicked);
            }

            // Model'deki veri değişimini View güncellemesine bağlar
            _model.OnEnergyChanged += RefreshView;
            RefreshView();
        }

        private void OnDestroy()
        {
            if (_view.ChargeEnergyButton != null)
            {
                _view.ChargeEnergyButton.onClick.RemoveListener(OnChargeButtonClicked);
            }

            _model.OnEnergyChanged -= RefreshView;
        }

        private void OnChargeButtonClicked()
        {
            _model.AddEnergy(25);
        }

        private void RefreshView()
        {
            _view.DisplayEnergy(_model.CurrentEnergy, _model.MaxEnergy);
        }
    }

    public class StaminaPresenter : MonoBehaviour
    {
        [SerializeField] private StaminaModel _model;
        [SerializeField] private StaminaView _view;

        private void Start()
        {
            _model.OnStaminaChanged += RefreshView;
            RefreshView();
        }

        private void OnDestroy()
        {
            _model.OnStaminaChanged -= RefreshView;
        }

        private void RefreshView()
        {
            _view.UpdateStaminaBar(_model.NormalizedStamina);
        }
    }
}