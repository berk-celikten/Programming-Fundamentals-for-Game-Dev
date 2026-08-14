using UnityEngine; // Unity Engine kütüphanesini projeye dahil ediyoruz (MonoBehaviour ve Debug.Log için gereklidir).

namespace OOPPrinciples.Encapsulation // Kod organizasyonu sağlayan ve çakışmaları önleyen kapsül/isim alanı.
{
    // Unity'de bir GameObject üzerine eklenebilen ana Script sınıfımız.
    public class Encapsulation : MonoBehaviour
    {
        // Kapsüllemenin (Encapsulation) Tanımı:
        // Bir nesnenin iç durumunu (verilerini) ve işlevselliğini saklayıp,
        // bu verilere yalnızca halka açık (public) bir fonksiyon kümesi aracılığıyla erişilmesine izin vermektir.

        private void Start() // Oyun başladığında Unity tarafından otomatik çağrılan ilk metot.
        {
            Foo foo = new Foo(); // Foo sınıfından "foo" adında yeni bir nesne (instance) üretiyoruz.

            // foo._name = "Boo"; -> HATA ALIRDIK! Çünkü _name değişkeni 'private' yani gizlidir.

            foo.SetName("Boo"); // Saklı olan _name değişkenine güvenli bir şekilde değer atamak için public metodu kullanıyoruz.

            Debug.Log(foo.GetName()); // Saklı olan _name değişkeninin değerini güvenli şekilde okuyup Unity konsoluna yazdırıyoruz.
        }
    }

    // Kapsülleme mantığının uygulandığı sınıf.
    public class Foo
    {
        // PRIVATE (GİZLİ) DEĞİŞKEN:
        // Bu değişkene sadece Foo sınıfının içinden erişilebilir. 
        // Dışarıdaki hiçbir sınıf bu değişkeni doğrudan değiştiremez veya okuyamaz.
        private string _name;

        // SETTER (ATAYICI) METOT:
        // Dış dünyadan gelen veriyi alır ve private olan _name değişkenine atar.
        public void SetName(string name)
        {
            // İstenirse buraya doğrulama/şartlar eklenebilir. 
            // Örneğin: if (!string.IsNullOrEmpty(name)) { _name = name; }
            _name = name;
        }

        // GETTER (GETİRİCİ) METOT:
        // Private olan _name değişkeninin değerini dış dünyaya döndürür.
        public string GetName()
        {
            // İstenirse buraya güvenlik veya yetki kontrolleri eklenebilir.
            return _name;
        }
    }
}