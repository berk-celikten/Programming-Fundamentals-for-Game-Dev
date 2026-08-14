using UnityEngine;

namespace SOLIDPrinciples.InterfaceSegregationRefactored 
{
    public class InterfaceSegregationDemo : MonoBehaviour
    {
        // Interface Segregation Principle (Arayüz Ayrıştırma Prensibi) Tanımı:
        // Büyük ve her işi yapan genel arayüzler yerine, belirli bir göreve odaklanmış 
        // daha küçük arayüzler tanımlanarak sınıfların gereksiz kod yükü taşıması engellenir.

        private void Start()
        {
            NPCWorker worker = new NPCWorker();
            WoodenChest chest = new WoodenChest();

            // Oyuncu işçiyle konuşur/çalıştırır:
            worker.Work();
            worker.TakeDamage(10f);

            // Oyuncu sandığa saldırır (Sandık çalışamaz, sadece hasar alabilir):
            chest.TakeDamage(25f);
        }
    }

    // 1. ODAKLANMIŞ ARAYÜZ: Hasar Alabilen Nesneler
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }

    // 2. ODAKLANMIŞ ARAYÜZ: Çalışabilen / Görev Yapabilen Birimler
    public interface IWorkable
    {
        void Work();
    }

    // 3. ODAKLANMIŞ ARAYÜZ: Tamir Edilebilen Nesneler
    public interface IRepairable
    {
        void Repair();
    }

    // SINIF 1: NPC İşçi Karakteri (Hem çalışabilir hem de hasar alabilir)
    public class NPCWorker : IDamageable, IWorkable
    {
        public void TakeDamage(float amount)
        {
            Debug.Log("İşçi " + amount + " hasar aldı!");
        }

        public void Work()
        {
            Debug.Log("İşçi maden toplamaya başladı.");
        }
    }

    // SINIF 2: Ahşap Sandık (Sadece hasar alabilir - İşçi veya Tamir arayüzlerini uygulamak zorunda kalmaz!)
    public class WoodenChest : IDamageable
    {
        public void TakeDamage(float amount)
        {
            Debug.Log("Sandık " + amount + " hasar aldı ve kırılmaya yaklaştı.");
        }
    }
}