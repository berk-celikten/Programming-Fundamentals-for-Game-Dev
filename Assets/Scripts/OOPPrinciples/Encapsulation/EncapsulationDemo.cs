using UnityEngine;

namespace OOPPrinciples.EncapsulationRefactored
{

    public class EncapsulationDemo : MonoBehaviour
    {

        // Kapsüllemenin (Encapsulation) Tanımı:
        // Bir nesnenin iç durumunu (verilerini) ve işlevselliğini saklayıp,
        // bu verilere yalnızca halka açık (public) bir fonksiyon kümesi aracılığıyla erişilmesine izin vermektir.

        private void Start()
        {
            PlayerHealth player = new PlayerHealth(100f);

            // player._currentHealth = -50f; -> HATA ALIRDIK! Değişken gizlidir.

            // Güvenli metotlar üzerinden etkileşim:
            player.TakeDamage(30f); // 30 hasar veriyoruz
            player.Heal(15f);       // 15 can yeniliyoruz

            // Güncel canı okuma:
            Debug.Log("Karakterin Kalan Canı: " + player.GetHealth());
        }
    }

    // Kapsüllemenin uygulandığı sınıf
    public class PlayerHealth
    {
        // GİZLİ (PRIVATE) DEĞİŞKEN:
        // Can değerinin doğrudan dışarıdan bozulmasını (örn: negatif yapılmasını) engelliyoruz.
        private float _currentHealth;
        private readonly float _maxHealth;

        // Yapıcı Metot (Constructor)
        public PlayerHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        // SETTER / KONTROLLÜ VERİ DEĞİŞTİRME:
        // Hasar alma işlemini doğrulamalarla yapıyoruz.
        public void TakeDamage(float damageAmount)
        {
            if (damageAmount <= 0) return; // Geçersiz hasar girdisini engelle

            _currentHealth -= damageAmount;

            // Canın 0'ın altına düşmesini engelliyoruz (Kapsülleme avantajı)
            if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }
        }

        // SETTER / KONTROLLÜ CAN YENİLEME:
        public void Heal(float healAmount)
        {
            if (healAmount <= 0) return;

            _currentHealth += healAmount;

            // Canın maksimum değeri aşmasını engelliyoruz
            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        // GETTER / VERİ OKUMA:
        // Sadece mevcut can değerini okumak için kullanılır.
        public float GetHealth()
        {
            return _currentHealth;
        }
    }
}