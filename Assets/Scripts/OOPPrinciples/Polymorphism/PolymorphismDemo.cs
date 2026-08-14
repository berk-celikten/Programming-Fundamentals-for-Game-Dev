using UnityEngine;
using Object = System.Object; 

namespace OOPPrinciples.PolymorphismRefactored 
{
    
    public class PolymorphismDemo : MonoBehaviour
    {
        // Çok Biçimliliğin (Polymorphism) Tanımı:
        // Kalıtım alınan veya bir arayüzden gelen özellik ve metotların, 
        // türetilmiş farklı sınıflar üzerinde kendi biçimlerine özgü olacak şekilde 
        // (farklı şekillerde) çalışabilme yeteneğidir.

        private void Start()
        {
            // HER SPELL BİR HEALSPELL DEĞİLDİR!
            // Aşağıdaki satır HATA VERİR çünkü ata sınıf referansı alt sınıf tipine doğrudan atanamaz:
            // HealSpell heal = new Spell();

            Upcasting(); // Yukarı tür dönüştürme
            Downcasting(); // Aşağı tür dönüştürme
            ReferenceToWrongSubtype(); // Hatalı dönüşüm örneği
            SpellsArray(); // Polimorfik Dizi örneği
        }

        // 1. UPCASTING (YUKARI DÖNÜŞTÜRME): Alt sınıf referansını Ata (Base) sınıfa çevirme.
        private void Upcasting()
        {
            // Upcasting, nesne üzerinden erişilebilen metotları ata sınıf seviyesine daraltır.

            FireballSpell fireball = new FireballSpell(); // FireballSpell nesnesi türetiliyor.
            Spell spell = fireball; // IMPLICIT UPCASTING: FireballSpell, Spell referansına atanır.

            spell.Cast(); // Çalışır! Çıktı: "Alev topu fırlatıldı ve alan hasarı verildi!" (Override edildiği için çalışır)

            // spell.Explode(); -> HATA ALIRIZ! 
            // Çünkü 'spell' referansı Spell tipindedir ve Spell sınıfında Explode() metodu yoktur.
        }

        // 2. DOWNCASTING (AŞAĞI DÖNÜŞTÜRME): Ata sınıf referansını Alt sınıfa çevirme.
        private void Downcasting()
        {
            // Downcasting, nesneye özel metotlara erişebilmek için kapsamı tekrar genişletir.

            Spell spell = new FireballSpell(); // FireballSpell nesnesi Spell referansında tutuluyor.

            // EXPLICIT DOWNCASTING:
            // spell referansı zorla FireballSpell tipine dönüştürülür.
            ((FireballSpell)spell).Explode(); // Çıktı: "BOOM! Alev topu patladı!"
        }

        // 3. HATALI TÜR DÖNÜŞÜMÜ (Invalid Casting / Wrong Subtype)
        private void ReferenceToWrongSubtype()
        {
            Object obj = new Spell(); // Tüm sınıflar C#'ta Object'ten türer.

            ((Spell)obj).Cast(); // Güvenli dönüşüm: 'obj' zaten özünde bir Spell nesnesidir.

            // HATALI TÜR DÖNÜŞÜMÜ:
            // Nesne özünde bir "Spell"dir, "FireballSpell" DEĞİLDİR!
            // Derlenirken (Compile-time) hata vermez, fakat oyun çalışırken (Runtime) "InvalidCastException" verip ÇÖKER.
            // ((FireballSpell)obj).Cast();
            // ((FireballSpell)obj).Explode();
        }

        // 4. POLYMORPHISM'IN EN GÜÇLÜ KULLANIMI (Polimorfik Diziler)
        private void SpellsArray()
        {
            FireballSpell fireball = new FireballSpell();
            HealSpell heal = new HealSpell();

            // Farklı büyü türlerini tek bir 'Spell' dizisinde toplayabiliyoruz!
            Spell[] spellBook = { fireball, heal };

            // Polymorphism sayesinde her büyü kendi Cast() davranışını sergiler:
            foreach (Spell spell in spellBook)
            {
                spell.Cast();
                // 1. Döngü Çıktısı: "Alev topu fırlatıldı ve alan hasarı verildi!"
                // 2. Döngü Çıktısı: "İyileştirme büyüsü yapıldı ve can yenilendi!"
            }
        }
    }

    // ATA / TABAN SINIF (Base Class)
    public class Spell
    {
        public virtual void Cast()
        {
            Debug.Log("Temel büyü kullanıldı.");
        }
    }

    // TÜRETİLMİŞ SINIF 1 (Derived Class)
    public class FireballSpell : Spell
    {
        public override void Cast()
        {
            Debug.Log("Alev topu fırlatıldı ve alan hasarı verildi!");
        }

        public void Explode() // Fireball'a özel ekstra metot
        {
            Debug.Log("BOOM! Alev topu patladı!");
        }
    }

    // TÜRETİLMİŞ SINIF 2 (Derived Class)
    public class HealSpell : Spell
    {
        public override void Cast()
        {
            Debug.Log("İyileştirme büyüsü yapıldı ve can yenilendi!");
        }

        public void GrantBuff() // Heal'a özel ekstra metot
        {
            Debug.Log("Zırh güçlendirmesi uygulandı!");
        }
    }
}