using UnityEngine;

namespace DesignPatterns.StrategyRefactored
{
    public class Strategy : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: STRATEGY (STRATEJİ) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Strategy deseni, bir algoritma ailesini tanımlayan, her birini ayrı bir sınıfta kapsülleyen (encapsulate) 
         ve bunların çalışma zamanında (runtime) birbirlerinin yerine kullanılabilmesini (interchangeable) 
         sağlayan davranışsal (behavioral) bir tasarım desenidir.
       - Algoritmanın uygulanma şeklini (strateji), onu kullanan sınıftan (context) tamamen soyutlar.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Spagetti `if-else` / `switch-case` Kontrollerini Önler:** Karakter farklı silahlar (Kılıç, Yay, 
         Büyü Asası) kullandığında `if (weaponType == Sword)` gibi devasa durum kontrolleri yazmak yerine, 
         her saldırı tipini kendi strateji sınıfına böler.
       - **Açık/Kapalı İlkesi (Open/Closed Principle):** Yeni bir saldırı veya yetenek türü eklenmek 
         istendiğinde mevcut karakter veya yetenek çalıştırıcı koduna dokunulmaz. Sadece yeni bir `IStrategy` 
         sınıfı türetilir.
       - **Çalışma Zamanında Davranış Değişimi:** Oyuncu bir tuşa basarak veya envanterden silah değiştirerek 
         saldırı davranışını anında dinamik olarak değiştirebilir.

    3. UNITY KULLANIM NOTU:
       - Standart C# arayüzleri (`interface`) Unity Inspector'ında varsayılan olarak serileştirilemez (`[SerializeField]`).
       - Bu sorunu çözmek için Strategy deseni Unity'de genellikle `ScriptableObject` sınıfları ile harmanlanır. 
         Böylece hem stratejiler sürükle-bırak yapılabilir hem de çalışma zamanında dinamik atanabilir.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Strategy Interface (`IAttackStrategy`):** Tüm algoritma çeşitlerinin uyguladığı ortak arayüz.
       - **Concrete Strategies (`MeleeSlashAttack`, `RangedBowAttack`, `MagicSpellAttack`):** Farklı saldırı algoritmaları.
       - **Context (`HeroCombatExecutor`):** Seçili stratejiyi tutar ve gerektiğinde çalıştırır.
    ====================================================================================================
    */

    // =========================================================================
    // 1. STRATEGY INTERFACE (Strateji Arayüzü)
    // Sorumluluk: Saldırı stratejilerinin ortak imzasını belirler.
    // =========================================================================
    public interface IAttackStrategy
    {
        void ExecuteAttack(Transform attackerTransform, Transform targetTransform, float baseDamage);
    }

    // =========================================================================
    // 2. CONCRETE STRATEGIES (Somut Strateji Sınıfları)
    // =========================================================================

    // Strateji 1: Yakın Dövüş Kılıç Saldırısı
    public class MeleeSlashAttack : IAttackStrategy
    {
        public void ExecuteAttack(Transform attackerTransform, Transform targetTransform, float baseDamage)
        {
            float finalDamage = baseDamage * 1.5f; // %50 yakın dövüş bonusu
            Debug.Log($"<color=red>[Melee Attack]</color> {attackerTransform.name}, kılıçla saldırdı! Verilen Hasar: {finalDamage}");
        }
    }

    // Strateji 2: Uzak Mesafe Ok Saldırısı
    public class RangedBowAttack : IAttackStrategy
    {
        public void ExecuteAttack(Transform attackerTransform, Transform targetTransform, float baseDamage)
        {
            if (targetTransform != null)
            {
                float distance = Vector3.Distance(attackerTransform.position, targetTransform.position);
                Debug.Log($"<color=green>[Ranged Attack]</color> {distance:F1} metre mesafedeki hedefe ok fırlatıldı! Hasar: {baseDamage}");
            }
            else
            {
                Debug.Log("<color=green>[Ranged Attack]</color> Boşluğa ok fırlatıldı!");
            }
        }
    }

    // Strateji 3: Büyü / Element Saldırısı
    public class MagicSpellAttack : IAttackStrategy
    {
        public void ExecuteAttack(Transform attackerTransform, Transform targetTransform, float baseDamage)
        {
            Debug.Log($"<color=cyan>[Magic Attack]</color> Alan etkili büyü yapıldı! Büyüsel Hasar: {baseDamage * 2f}");
        }
    }

    // =========================================================================
    // 3. CONTEXT (Bağlam Sınıfı - Savaş Kontrolcüsü)
    // Sorumluluk: Aktif stratejiyi barındırır ve saldırı emrini verir.
    // =========================================================================
    public class HeroCombatExecutor : MonoBehaviour
    {
        [SerializeField] private float _baseDamage = 25f;
        [SerializeField] private Transform _currentTarget;

        // Mevcut Strateji
        private IAttackStrategy _activeAttackStrategy;

        private void Awake()
        {
            // Varsayılan Strateji: Yakın Dövüş
            SetStrategy(new MeleeSlashAttack());
        }

        private void Update()
        {
            // Test için klavye girdileriyle strateji değiştirme
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetStrategy(new MeleeSlashAttack());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetStrategy(new RangedBowAttack());
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetStrategy(new MagicSpellAttack());
            }

            // Saldırı Tetikleme
            if (Input.GetButtonDown("Fire1"))
            {
                PerformAttack();
            }
        }

        public void SetStrategy(IAttackStrategy newStrategy)
        {
            _activeAttackStrategy = newStrategy;
            Debug.Log($"<color=yellow>[Combat System]</color> Yeni Saldırı Stratejisi Atandı: <b>{newStrategy.GetType().Name}</b>");
        }

        public void PerformAttack()
        {
            if (_activeAttackStrategy != null)
            {
                _activeAttackStrategy.ExecuteAttack(transform, _currentTarget, _baseDamage);
            }
            else
            {
                Debug.LogWarning("Atanmış bir saldırı stratejisi bulunamadı!");
            }
        }
    }

    // =========================================================================
    // 4. SCRIPTABLEOBJECT TABANLI STRATEJİ (UNITY ALTERNATİFİ)
    // Sorumluluk: Stratejilerin Inspector'dan sürüklenebilmesini sağlar.
    // =========================================================================
    public abstract class ScriptableAttackStrategy : ScriptableObject, IAttackStrategy
    {
        public abstract void ExecuteAttack(Transform attackerTransform, Transform targetTransform, float baseDamage);
    }
}