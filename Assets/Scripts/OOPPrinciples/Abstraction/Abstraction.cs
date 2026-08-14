using UnityEngine; 

namespace OOPPrinciples.Abstraction 
{
    
    public class Abstraction : MonoBehaviour
    {
        // Soyutlamanın (Abstraction) Tanımı:
        // Kodun ve anlaşılabilirliğin karmaşıklığını artırmadan 
        // büyük ve kapsamlı sistemler inşa edilmesini sağlar.

        private void Start() 
        {
            GameFactory gameFactory = new GameFactory(); // Oyun üretici sınıfımızdan yeni bir nesne türetiyoruz.

            // KULLANIM KOLAYLIĞI (SOYUTLAMA BURADA):
            // Dış dünyadan (Start içinden) sadece MakeGame() metodunu çağırıyoruz.
            // Arka planda hikayenin nasıl yazıldığı, kodun nasıl derlendiği veya seslerin nasıl üretildiğiyle 
            // buradaki kod ilgilenmez. Sadece "Oyun Yap" emrini verir.
            gameFactory.MakeGame("Love of Wisdom");
        }
    }

    // Oyun üretim süreçlerini yöneten fabrika sınıfı.
    public class GameFactory
    {
        // PUBLIC (DIŞA AÇIK) METOT:
        // Dış dünyanın erişebildiği TEK noktadır. 
        // Alt adımları kendi içinde sırasıyla çağırarak karmaşık süreci basitleştirir.
        public void MakeGame(string gameName)
        {
            // Gizlenmiş alt işlevler çağrılıyor:
            CreateStory();
            CreateDesign();
            CreateCode();
            CreateArt();
            CreateAudio();

            Debug.Log(gameName + " created."); // Oyunun tamamlandığını bildirir.
        }

        // GİZLİ (PRIVATE) İÇ DETAYLAR:
        // Aşağıdaki metotlar 'private' olduğu için dışarıdaki sınıflar (örneğin Abstraction sınıfı) 
        // bu metotları doğrudan çağıramaz (örneğin: gameFactory.CreateCode() yapılamaz).
        // Böylece iç süreçlerin yanlış sırayla veya zamansız çalıştırılması engellenir.

        private void CreateStory()
        {
            Debug.Log("Create Story!");
        }

        private void CreateDesign()
        {
            Debug.Log("Create Design!");
        }

        private void CreateCode()
        {
            Debug.Log("Create Code!");
        }

        private void CreateArt()
        {
            Debug.Log("Create Art!");
        }

        private void CreateAudio()
        {
            Debug.Log("Create Audio!");
        }
    }
}