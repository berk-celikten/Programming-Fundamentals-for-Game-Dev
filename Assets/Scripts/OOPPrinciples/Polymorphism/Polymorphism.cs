using UnityEngine; 
using Object = System.Object; // C#'ın en temel kök sınıfı olan System.Object'e "Object" takma adını veriyoruz.

namespace OOPPrinciples.Polymorphism // Kod organizasyonunu sağlayan isim alanı.
{
    
    public class Polymorphism : MonoBehaviour
    {
        // Çok Biçimliliğin (Polymorphism) Tanımı:
        // Kalıtım alınan özellik veya metotların, birden fazla alt sınıf/soyutlama üzerinde 
        // farklı şekillerde (kendi biçimlerine göre) uygulanabilme yeteneğidir.

        private void Start() 
        {
            // HER ANIMAL BİR CAT DEĞİLDİR!
            // Aşağıdaki satır HATA VERİR çünkü temel (base) bir sınıf, özelleşmiş alt sınıf tipindeki referansa atanamaz:
            // Cat cat = new Animal(); 

            Upcasting(); // Yukarı tür dönüştürme örneği
            Downcasting(); // Aşağı tür dönüştürme örneği
            ReferenceToWrongSubtype(); // Hatalı cast (tür dönüşümü) örneği
            AnimalsArray(); // Polymorphism'in en yaygın ve güçlü kullanım alanı (Dizi/Liste)
        }

        // 1. UPCASTING (YUKARI DÖNÜŞTÜRME): Alt sınıf referansını Ata (Base) sınıfa çevirme.
        private void Upcasting()
        {
            // Upcasting, bu nesne üzerinden erişilebilen metot ve nitelikleri daraltır (sınırlandırır).

            Cat cat = new Cat(); // Bellekte bir Cat nesnesi oluşturuluyor.
            Animal animal = cat; // IMPLICIT (Örtük) UPCASTING: Cat nesnesi, Animal referansına atanıyor. (Casting gerekmez)

            animal.Eat(); // Çalışır! Çıktı: "Cat is eating!" (Override edildiği için Cat'in Eat() metodu çalışır).

            // animal.Meow(); -> HATA ALIRIZ! 
            // Çünkü 'animal' referansı Animal tipindedir ve Animal sınıfında Meow() metodu tanımlı değildir.
        }

        // 2. DOWNCASTING (AŞAĞI DÖNÜŞTÜRME): Ata sınıf referansını Alt sınıfa çevirme.
        private void Downcasting()
        {
            // Downcasting, nesneye erişebilecek metot ve nitelikleri tekrar genişletir.

            Animal animal = new Cat(); // Bellekte Cat nesnesi var, ama Animal referansı ile tutuluyor.

            // EXPLICIT (Açık) DOWNCASTING:
            // animal referansını zorla (Cat) tipine dönüştürüyoruz ki Cat'e özel metotlara erişebilelim.
            ((Cat)animal).Meow(); // Çıktı: "MEOW!"
        }

        // 3. HATALI TÜR DÖNÜŞÜMÜ (Wrong Subtype Casting)
        private void ReferenceToWrongSubtype()
        {
            Object o = new Animal(); // C#'taki her şey bir Object'tir. Animal nesnesi Object referansında tutuluyor.

            ((Animal)o).Eat(); // Güvenli dönüştürme: 'o' zaten arka planda bir Animal nesnesidir.

            // YANLIŞ TÜR DÖNÜŞÜMÜ ÖRNEĞİ:
            // Nesne özünde bir "Animal"dır, "Cat" DEĞİLDİR!
            // Aşağıdaki kod derlenirken (Compile-time) HATA VERMEZ.
            // Ancak oyun çalışırken (Runtime) "InvalidCastException" hatası verir ve ÇÖKER (Crash).
            // ((Cat)o).Eat();
            // ((Cat)o).Meow();
        }

        // 4. POLYMORPHISM'İN EN GÜÇLÜ KULLANIMI (Polimorfik Diziler)
        private void AnimalsArray()
        {
            Dog dog = new Dog(); // Dog tipinde nesne
            Cat cat = new Cat(); // Cat tipinde nesne

            // Farklı alt sınıfları, ortak ata sınıfları (Animal) türündeki tek bir dizide toplayabiliyoruz!
            Animal[] animals = { dog, cat };

            // Polymorphism sayesinde her nesne kendi Eat() davranışını sergiler:
            foreach (Animal animal in animals)
            {
                animal.Eat();
                // 1. Döngü Çıktısı: "Dog is eating!"
                // 2. Döngü Çıktısı: "Cat is eating!"
            }
        }
    }

    // ATA SINIF
    public class Animal
    {
        public virtual void Eat()
        {
            Debug.Log("Animal is eating!");
        }
    }

    // TÜRETİLMİŞ SINIF 1
    public class Dog : Animal
    {
        public override void Eat()
        {
            Debug.Log("Dog is eating!");
        }

        public void Bark() // Dog sınıfına özel ekstra metot
        {
            Debug.Log("BARK!");
        }
    }

    // TÜRETİLMİŞ SINIF 2
    public class Cat : Animal
    {
        public override void Eat()
        {
            Debug.Log("Cat is eating!");
        }

        public void Meow() // Cat sınıfına özel ekstra metot
        {
            Debug.Log("MEOW!");
        }
    }
}