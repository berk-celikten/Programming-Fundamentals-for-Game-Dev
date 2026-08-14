using UnityEngine;

namespace SOLIDPrinciples.OpenClosedRefactored 
{
    public class OpenClosedDemo : MonoBehaviour
    {
        private void Start()
        {
            // Farklı silah türlerini türetiyoruz.
            Weapon sword = new Sword(35f);
            Weapon bow = new Bow(20f, 12f);

            // Yeni bir silah (örneğin MagicWand) eklemek istediğimizde 
            // ne Weapon sınıfına dokunuruz ne de mevcut çalışan sistemleri değiştiririz.
            Debug.Log("Kılıç Hasarı: " + sword.CalculateDamage());
            Debug.Log("Yay Hasarı: " + bow.CalculateDamage());
        }
    }

    // SOYUT TABAN SINIF (Arayüz / Sözleşme)
    public abstract class Weapon
    {
        // Her silahın kendi hasarını hesaplama yöntemi farklıdır ama
        // dışarıya sunduğu 'CalculateDamage' çağrısı ortaktır.
        public abstract float CalculateDamage();
    }

    // KILIÇ SILAHI (Gelişime açık: Kendi hasar mantığını yazar)
    public class Sword : Weapon
    {
        private float _rawDamage;

        public Sword(float rawDamage)
        {
            _rawDamage = rawDamage;
        }

        public override float CalculateDamage()
        {
            // Kılıç için düz fiziksel hasar döndürülür
            return _rawDamage;
        }
    }

    // YAY SILAHI (Gelişime açık: Kendi hasar ve ok mantığını yazar)
    public class Bow : Weapon
    {
        private float _arrowDamage;
        private float _distanceBonus;

        public Bow(float arrowDamage, float distanceBonus)
        {
            _arrowDamage = arrowDamage;
            _distanceBonus = distanceBonus;
        }

        public override float CalculateDamage()
        {
            // Yay için ok hasarı ve mesafe bonusu toplanır
            return _arrowDamage + _distanceBonus;
        }
    }
}