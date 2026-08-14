using UnityEngine;

namespace OOPPrinciples.InheritanceRefactored 
{
    
    public class InheritanceDemo : MonoBehaviour
    {
        // Kalıtımın (Inheritance) Tanımı:
        // Bir sınıfın (alt sınıf / child class), başka bir sınıfın (ata sınıf / base class) 
        // tüm özelliklerini, değişkenlerini ve metotlarını devralarak (miras alarak) 
        // kod tekrarını önlemesi ve üzerine yeni özellikler ekleyebilmesidir.

        private void Start()
        {
            // ATA (BASE) SINIFTAN NESNE OLUŞTURMA:
            BaseEnemy genericEnemy = new BaseEnemy();

            // TÜRETİLMİŞ (CHILD) SINIFLARDAN NESNE OLUŞTURMA:
            Goblin goblin = new Goblin(); // Goblin sınıfı BaseEnemy'den miras alır.
            Dragon dragon = new Dragon(); // Dragon sınıfı da BaseEnemy'den miras alır.

            // METOT ÇAĞIRILARI VE EKRAN ÇIKTILARI:

            genericEnemy.Attack();
            // Çıktı: "Düşman oyuncuya saldırdı!"
            // Temel sınıftaki virtual metot doğrudan çalışır.

            goblin.Attack();
            // Çıktı: "Goblin bıçağıyla hızlıca saldırdı!" 
            // Goblin sınıfı Attack() metodunu tamamen ezdiği (override) için ata sınıftaki kod çalışmaz.

            dragon.Attack();
            // Çıktı 1: "Düşman oyuncuya saldırdı!" (base.Attack() sayesinde üst sınıftaki metot çalıştı)
            // Çıktı 2: "Ejderha alev püskürttü!" (Ardından kendi eklediği log çalıştı)
        }
    }

    // TABAN / ATA SINIF (Base Class / Parent Class)
    public class BaseEnemy
    {
        // VIRTUAL KEYWORD (Sanal Metot):
        // "virtual", bu metodun alt sınıflar (Goblin, Dragon) tarafından 
        // ezilebileceğini (override edilebileceğini) belirtir.
        public virtual void Attack()
        {
            Debug.Log("Düşman oyuncuya saldırdı!");
        }
    }

    // TÜRETİLMİŞ SINIF 1 (Derived Class / Child Class)
    // "Goblin : BaseEnemy" sözdizimi Goblin sınıfının BaseEnemy sınıfından miras aldığını gösterir.
    public class Goblin : BaseEnemy
    {
        // OVERRIDE KEYWORD (Metot Ezme):
        // "override", ata sınıftaki virtual Attack() metodunu tamamen iptal eder 
        // ve yerine bu sınıfın kendi Attack() davranışını koyar.
        public override void Attack()
        {
            Debug.Log("Goblin bıçağıyla hızlıca saldırdı!");
        }
    }

    // TÜRETİLMİŞ SINIF 2 (Derived Class / Child Class)
    public class Dragon : BaseEnemy
    {
        // OVERRIDE & BASE KEYWORD:
        // Dragon sınıfı da metodu ezer ancak ebeveyn davranışını tamamen terk etmez.
        public override void Attack()
        {
            // BASE KEYWORD:
            // "base.Attack()", ata sınıf olan BaseEnemy içindeki orijinal Attack() metodunu çağırır.
            // Böylece hem üst sınıfın davranışı korunur hem de üzerine yeni davranış eklenir.
            base.Attack();

            Debug.Log("Ejderha alev püskürttü!");
        }
    }
}