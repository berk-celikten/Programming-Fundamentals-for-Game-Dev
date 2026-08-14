using UnityEngine;

namespace OOPPrinciples.AbstractionRefactored
{

    public class AbstractionDemo : MonoBehaviour
    {
        private void Start()
        {
            CarController myCar = new CarController();

            // SOYUTLAMA (ABSTRACTION):
            // Sürücü (oyuncu) sadece kontağı çevirir / düğmeye basar.
            // Arka plandaki yakıt pompası veya enjeksiyon detaylarını bilmesine gerek yoktur.
            myCar.StartEngine("Spor Araba");
        }
    }

    // Araç kontrolcüsü sınıfı
    public class CarController
    {
        // AÇIK (PUBLIC) ARAYÜZ:
        // Dış dünyadan erişilebilen tek çalıştırma noktası.
        public void StartEngine(string modelName)
        {
            // Gizli iç süreçler sırasıyla tetiklenir:
            CheckBatteryVoltage();
            PumpFuel();
            IgniteSparkPlugs();

            Debug.Log(modelName + " çalıştırıldı ve sürüşe hazır!");
        }

        // GİZLİ (PRIVATE) SÜREÇLER:
        // Dışarıdaki sınıfların bu adımlara doğrudan erişmesi engellenir.

        private void CheckBatteryVoltage()
        {
            Debug.Log("Akü voltajı kontrol ediliyor...");
        }

        private void PumpFuel()
        {
            Debug.Log("Yakıt motor bloğuna pompalanıyor...");
        }

        private void IgniteSparkPlugs()
        {
            Debug.Log("Bujiler ateşleniyor...");
        }
    }
}