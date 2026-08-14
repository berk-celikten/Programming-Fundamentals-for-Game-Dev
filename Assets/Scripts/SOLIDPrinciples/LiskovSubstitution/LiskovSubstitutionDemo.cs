using System.Collections.Generic;
using UnityEngine;

namespace SOLIDPrinciples.LiskovSubstitutionRefactored 
{
    public class LiskovSubstitutionDemo : MonoBehaviour
    {
        // Liskov Substitution Principle (Liskov'un Yerine Geçme Prensibi) Tanımı:
        // Alt sınıflar, üst sınıfların yerine geçtiğinde üst sınıfın vadettiği tüm 
        // davranışları eksiksiz sergilemeli, beklenmeyen hatalar veya istisnalar üretmemelidir.

        private void Start()
        {
            // Hareket edebilen tüm birimleri tek bir liste altında topluyoruz.
            List<IMovable> movableUnits = new List<IMovable>
            {
                new Player(),
                new Zombie()
            };

            // LSP UYUMU: Liste içerisindeki tüm nesneler IMovable davranışını bozmadan eksiksiz yerine getirir.
            foreach (IMovable unit in movableUnits)
            {
                unit.Move();
                // Çıktı 1: "Oyuncu klavye girdisine göre hareket etti."
                // Çıktı 2: "Zombi oyuncuya doğru yavaşça ilerledi."
            }
        }
    }

    // ARAYÜZ (Interface) - Yalnızca hareket edebilen birimler tarafından uygulanır.
    public interface IMovable
    {
        void Move();
    }

    // OYUN İÇİ KARAKTER (Tüm birimlerin ortak tabanı)
    public abstract class Character
    {
        public abstract void TakeDamage(float amount);
    }

    // 1. HAREKET EDEBİLEN BİRİM (Oyuncu)
    public class Player : Character, IMovable
    {
        public override void TakeDamage(float amount)
        {
            Debug.Log("Oyuncu hasar aldı.");
        }

        public void Move()
        {
            Debug.Log("Oyuncu klavye girdisine göre hareket etti.");
        }
    }

    // 2. HAREKET EDEBİLEN BİRİM (Düşman)
    public class Zombie : Character, IMovable
    {
        public override void TakeDamage(float amount)
        {
            Debug.Log("Zombi hasar aldı.");
        }

        public void Move()
        {
            Debug.Log("Zombi oyuncuya doğru yavaşça ilerledi.");
        }
    }

    // 3. HAREKET EDEMEYEN BİRİM (Sabit Savunma Kulesi)
    // Turret bir Character'dir ama IMovable'dan türetilmemiştir.
    // Böylece hareket ettirilmeye çalışıldığında hata verme veya mantığı bozma riski sıfıra indirilir (LSP Uyumlu).
    public class StaticTurret : Character
    {
        public override void TakeDamage(float amount)
        {
            Debug.Log("Sabit kule hasar aldı.");
        }
    }
}