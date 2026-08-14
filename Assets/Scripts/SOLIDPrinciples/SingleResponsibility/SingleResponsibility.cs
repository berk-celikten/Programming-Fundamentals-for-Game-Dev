using UnityEngine; 

namespace SOLIDPrinciples.SingleResponsibility 
{
    
    public class SingleResponsibility : MonoBehaviour
    {
        // Single Responsibility Principle (Tek Sorumluluk Prensibi) Tanımı:
        // Bir sınıfın değişmek için tek bir nedeni olmalıdır. 
        // Yani bir sınıfın yalnızca tek bir görevi/sorumluluğu olmalıdır.

        private void Start() 
        {
            // Şekil nesnelerimizi kendi parametreleriyle oluşturuyoruz.
            // Bu sınıflar SADECE kendi verilerini tutmaktan sorumludur.
            Square square = new Square(5);
            Circle circle = new Circle(3);

            // Alan hesaplama işini yapmak üzere tek görevi alan hesaplamak olan sınıfı çağırıyoruz.
            AreaCalculator areaCalculator = new AreaCalculator();

            // Alanları hesaplatıp konsola yazdırıyoruz.
            Debug.Log("Area = " + areaCalculator.CalculateArea(square));
            Debug.Log("Area = " + areaCalculator.CalculateArea(circle));
        }
    }

    // TÜM ŞEKİLLER İÇİN ABSTRAKT (SOYUT) ATA SINIF
    public abstract class Shape
    {
        // Ortak bir üst tip (polimorfizm ve tip kontrolü) sağlamak için kullanılır.
    }

    // KARE SINIFI (Sorumluluğu: Sadece Kare verisini ve durumunu yönetmek)
    public class Square : Shape
    {
        private float _length; // Karenin kenar uzunluğu (Encapsulation)

        // Constructor (Yapıcı Metot) - Kenar uzunluğunu alır.
        public Square(float length)
        {
            _length = length;
        }

        // Kenar uzunluğunu dışarıya döndüren metot.
        public float GetLength()
        {
            return _length;
        }
    }

    // DAİRE SINIFI (Sorumluluğu: Sadece Daire verisini ve durumunu yönetmek)
    public class Circle : Shape
    {
        private float _radius; // Dairenin yarıçapı (Encapsulation)

        // Constructor (Yapıcı Metot) - Yarıçapı alır.
        public Circle(float radius)
        {
            _radius = radius;
        }

        // Yarıçapı dışarıya döndüren metot.
        public float GetRadius()
        {
            return _radius;
        }
    }

    // ALAN HESAPLAYICI (Sorumluluğu: Sadece şekillerin alanını hesaplamak)
    // Eğer alan hesaplama mantığı Square veya Circle sınıfının içine yazılsaydı,
    // şekil sınıfları hem veri tutmaktan hem de matematiksel hesap yapmaktan sorumlu olurdu (SRP ihlali).
    public class AreaCalculator
    {
        public float CalculateArea(Shape shape)
        {
            // C# Pattern Matching (Desen Eşleme) ile gelen şeklin türüne göre hesaplama yapılır:
            switch (shape)
            {
                case Square square:
                    // Kenarın karesi (length^2)
                    return Mathf.Pow(square.GetLength(), 2);

                case Circle circle:
                    // PI * r^2
                    return Mathf.PI * Mathf.Pow(circle.GetRadius(), 2);

                default:
                    return 0;
            }
        }
    }
}