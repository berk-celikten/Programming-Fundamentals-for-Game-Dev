using UnityEngine; 

namespace SOLIDPrinciples.SingleResponsibilityRefactored 
{
    
    public class SingleResponsibilityDemo : MonoBehaviour
    {
        // Single Responsibility Principle (Tek Sorumluluk Prensibi):
        // Bir sınıfın yalnızca tek bir değişim nedeni ve görevi olmalıdır.

        private void Start() 
        {
            // Sadece kendi istatistik verilerini taşıyan karakter nesnelerini oluşturuyoruz.
            Warrior warrior = new Warrior(100f, 25f);
            Monster monster = new Monster(80f, 10f);

            // Hasar matematiksel işlemlerini yönetecek tek sorumlu sınıfı başlatıyoruz.
            DamageCalculator damageCalculator = new DamageCalculator();

            // Karakterlerin vereceği son hasar değerlerini hesaplatıp konsola yazdırıyoruz.
            Debug.Log("Savaşçı Hasarı: " + damageCalculator.CalculateDamage(warrior));
            Debug.Log("Canavar Hasarı: " + damageCalculator.CalculateDamage(monster));
        }
    }

    // TÜM KARAKTERLER İÇİN SOYUT ATA SINIF
    public abstract class Character
    {
        // Karakter türlerini ortak bir çatı altında toplamak için kullanılan taban sınıf.
    }

    // SAVAŞÇI KARAKTERİ (Sorumluluğu: Sadece Savaşçı istatistiklerini barındırmak)
    public class Warrior : Character
    {
        private float _health; // Karakter canı
        private float _baseAttack; // Temel saldırı gücü

        // Yapıcı metot (Constructor)
        public Warrior(float health, float baseAttack)
        {
            _health = health;
            _baseAttack = baseAttack;
        }

        public float GetBaseAttack()
        {
            return _baseAttack;
        }
    }

    // CANAVAR KARAKTERİ (Sorumluluğu: Sadece Canavar istatistiklerini barındırmak)
    public class Monster : Character
    {
        private float _health; // Canavar canı
        private float _bitePower; // Isırma gücü

        // Yapıcı metot (Constructor)
        public Monster(float health, float bitePower)
        {
            _health = health;
            _bitePower = bitePower;
        }

        public float GetBitePower()
        {
            return _bitePower;
        }
    }

    // HASAR HESAPLAYICI (Sorumluluğu: Sadece matematiksel hasar çarpanlarını hesaplamak)
    // Eğer hasar hesaplama kodları Warrior veya Monster içine yazılsaydı,
    // o sınıflar hem veri saklamaktan hem de oyun dengesi formüllerini çalıştırmaktan sorumlu olurdu (SRP ihlali).
    public class DamageCalculator
    {
        public float CalculateDamage(Character character)
        {
            // Karakter türüne göre özelleştirilmiş hasar mantığı uygulanır:
            switch (character)
            {
                case Warrior warrior:
                    // Savaşçı kritik vuruş çarpanı hesabı (Örn: Temel Saldırı * 1.5)
                    return warrior.GetBaseAttack() * 1.5f;

                case Monster monster:
                    // Canavar zehirli vuruş hesabı (Örn: Isırma Gücü + 5 sabit hasar)
                    return monster.GetBitePower() + 5f;

                default:
                    return 0f;
            }
        }
    }
}