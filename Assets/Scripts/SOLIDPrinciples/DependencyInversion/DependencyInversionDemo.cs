using UnityEngine;

namespace SOLIDPrinciples.DependencyInversionRefactored
{
    public class DependencyInversionDemo : MonoBehaviour
    {
        // Dependency Inversion Principle (Bağımlılıkların Tersine Çevrilmesi Prensibi) Tanımı:
        // Üst seviye iş mantığı içeren sınıfların, alt seviye teknik detay içeren sınıflara 
        // sıkı sıkıya bağlanmasını engeller. Değişime açık, esnek ve test edilebilir mimariler sağlar.

        private void Start()
        {
            IInputProvider keyboard = new KeyboardInput();
            IInputProvider vrController = new VRControllerInput();

            PlayerCharacter player = new PlayerCharacter();

            // Oyuncu klavye ile oynuyor:
            player.SetInputSource(keyboard);
            player.UpdateMovement();

            // Oyuncu VR Cihazına geçtiğinde PlayerCharacter sınıfında HİÇBİR KOD DEĞİŞMEZ:
            player.SetInputSource(vrController);
            player.UpdateMovement();
        }
    }

    // SOYUTLAMA (Abstraction)
    public interface IInputProvider
    {
        Vector2 GetMovementInput();
    }

    // DÜŞÜK SEVİYELİ MODÜL 1: Klavye girdisi
    public class KeyboardInput : IInputProvider
    {
        public Vector2 GetMovementInput()
        {
            Debug.Log("Klavye WASD girdisi okundu.");
            return new Vector2(1f, 0f);
        }
    }

    // DÜŞÜK SEVİYELİ MODÜL 2: VR Kontrolör girdisi
    public class VRControllerInput : IInputProvider
    {
        public Vector2 GetMovementInput()
        {
            Debug.Log("VR Joystick eksen girdisi okundu.");
            return new Vector2(0f, 1f);
        }
    }

    // YÜKSEK SEVİYELİ MODÜL: Karakter Yönetimi
    // Karakter, girdinin Klavyeden mi, VR'dan mı yoksa Gamepad'den mi geldiğini bilmez ve umursamaz.
    public class PlayerCharacter
    {
        private IInputProvider _inputProvider;

        public void SetInputSource(IInputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

        public void UpdateMovement()
        {
            if (_inputProvider == null) return;

            Vector2 moveVector = _inputProvider.GetMovementInput();
            Debug.Log("Karakter " + moveVector + " yönüne doğru hareket ettirildi.");
        }
    }
}