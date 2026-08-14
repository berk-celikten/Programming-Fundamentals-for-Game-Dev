using UnityEngine; 

namespace OOPPrinciples.Inheritance 
{
    
    public class Inheritance : MonoBehaviour
    {
        // Kalıtımın (Inheritance) Tanımı:
        // Mevcut soyutlamalara (sınıflara) dayanarak yeni soyutlamalar/sınıflar oluşturabilme yeteneğidir.
        // Alt sınıflar, üst sınıfın tüm ortak özelliklerini ve davranışlarını miras alır.

        private void Start() // Oyun başladığında Unity tarafından çağrılan ilk metot.
        {
            // ATA (BASE) SINIFTAN NESNE OLUŞTURMA:
            Animal animal = new Animal();

            // TÜRETİLMİŞ (CHILD) SINIFLARDAN NESNE OLUŞTURMA:
            Dog dog = new Dog(); // Dog sınıfı Animal'dan miras almıştır.
            Cat cat = new Cat(); // Cat sınıfı da Animal'dan miras almıştır.

            // METOT ÇAĞIRILARI VE EKRAN ÇIKTILARI:

            animal.Yell();
            // Çıktı: "I am an animal!"
            // Temel sınıftaki virtual metot doğrudan çalışır.

            dog.Yell();
            // Çıktı: "I am a dog!"
            // Dog sınıfı, Yell() metodunu tamamen ezdiği (override) için ata sınıftaki kod çalışmaz.

            cat.Yell();
            // Çıktı 1: "I am an animal!" (base.Yell() sayesinde üst sınıftaki metot çalıştı)
            // Çıktı 2: "I am a cat!" (ardından Cat sınıfının kendi eklediği log çalıştı)
        }
    }

    // ATA / EBEVEYN SINIF (Base / Parent Class)
    public class Animal
    {
        // VIRTUAL KEYWORD (Sanal Metot):
        // "virtual" anahtar kelimesi, bu metodun türetilen alt sınıflar (Dog, Cat) tarafından 
        // ezilebileceğini (override edilebileceğini) belirtir.
        public virtual void Yell()
        {
            Debug.Log("I am an animal!");
        }
    }

    // TÜRETİLMİŞ SINIF 1 (Derived / Child Class)
    // "Dog : Animal" sözdizimi Dog sınıfının Animal sınıfından miras aldığını gösterir.
    public class Dog : Animal
    {
        // OVERRIDE KEYWORD (Metot Ezme):
        // "override", ata sınıftaki virtual Yell() metodunu tamamen iptal eder 
        // ve yerine bu sınıfın kendi Yell() davranışını koyar.
        public override void Yell()
        {
            Debug.Log("I am a dog!");
        }
    }

    // TÜRETİLMİŞ SINIF 2 (Derived / Child Class)
    public class Cat : Animal
    {
        // OVERRIDE KEYWORD:
        // Cat sınıfı da Yell() metodunu ezer ancak ebeveyn davranışını tamamen terk etmez.
        public override void Yell()
        {
            // BASE KEYWORD:
            // "base.Yell()", ata sınıf olan Animal içindeki orijinal Yell() metodunu çağırır.
            // Böylece hem üst sınıfın davranışı korunur hem de üzerine yeni davranış eklenir.
            base.Yell();

            Debug.Log("I am a cat!");
        }
    }
}