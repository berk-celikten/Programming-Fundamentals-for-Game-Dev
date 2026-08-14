using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.CommandPattern
{
    /* 
    ====================================================================================================
    TASARIM DESENİ: COMMAND (KOMUT) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Command deseni, bir eylemi veya isteği (örneğin: karakter hareketi, ateş etme, buton tıklaması)
         parametreleriyle birlikte kendi başına bağımsız bir "NESNE" (Object) haline getiren tasarım desenidir.
       - Yapılacak işi doğrudan çağırmak yerine (`Player.Move()`), o işi bir pakete koyup (`new MoveCommand()`)
         bir yöneticiye (`CommandManager`) teslim eder.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Undo / Redo (Geri Al / Tekrar Et):** Eylemler birer nesneye dönüştüğü için bir `Stack` (yığın) içinde
         saklanabilir. Kullanıcı `Ctrl + Z` yaptığında son yapılan eylemin `Undo()` metodu çağrılarak işlem
         kolayca geri alınır.
       - **Decoupling (Bağımlılığı Azaltma):** İsteği tetikleyen yapı (Klavye, Gamepad, UI Butonu), işi yapan
         sınıfın (Karakter, Kamera vb.) iç detaylarını bilmek zorunda kalmaz.
       - **Komut Kuyrukları (Command Queue):** Sıra tabanlı (Turn-Based) oyunlarda veya network paketlerinde
         komutlar sıraya dizilip zamanı geldikçe sırayla çalıştırılabilir.
       - **Replay / Kayıt Sistemleri:** Oyuncunun yaptığı tüm hamleler (komut nesneleri) sırasıyla bir listede
         tutularak oyun sonunda tekrar izletilebilir.

    3. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **ICommand (Arayüz):** Tüm komutların uyması gereken şablondur (`Execute` ve `Undo` tanımlar).
       - **ConcreteCommand (Somut Komut):** Yapılacak eylemi temsil eden sınıftır (`MoveCommand`).
       - **Receiver (Alıcı):** Asıl fiziksel veya mantıksal işi yapan sınıftır (`CharacterMovement`).
       - **Invoker (Tetikleyici / Yönetici):** Komutları alan, çalıştıran ve Undo/Redo için Stack'te tutan sınıftır (`CommandManager`).
       - **Client / Input (İstemci):** Tuş girdilerini dinleyip komut nesnesini oluşturan sınıftır (`PlayerInputHandler`).
    ====================================================================================================
    */

    // =========================================================================
    // 1. ICOMMAND ARAYÜZÜ (Command Interface)
    // Sorumluluk: Tüm komutlar için standart bir çalışma ve geri alma şablonu sunar.
    // =========================================================================
    public interface ICommand
    {
        // Komut tetiklendiğinde yürütülecek ana mantık.
        void Execute();

        // Komut geri alındığında yapılacak tam tersi işlem.
        void Undo();
    }

    // =========================================================================
    // 2. RECEIVER (Alıcı / Asıl İşi Yapan Sınıf)
    // Sorumluluk: Komutun sonucunda fiziksel değişimi yaşayacak asıl nesnedir.
    // =========================================================================
    public class CharacterMovement : MonoBehaviour
    {
        // Karakteri verilen vektör yönünde hareket ettiren temel fonksiyon.
        public void MoveInDirection(Vector3 direction)
        {
            transform.position += direction;
            Debug.Log($"[Receiver] Karakter {direction} yönüne taşındı. Güncel Konum: {transform.position}");
        }
    }

    // =========================================================================
    // 3. CONCRETE COMMAND (Somut Komut)
    // Sorumluluk: Hareket eylemini kapsüller. Alıcıyı (CharacterMovement) ve
    // hareket verisini (Vector3) hafızasında tutarak Undo/Redo imkanı sağlar.
    // =========================================================================
    public class MoveCommand : ICommand
    {
        // Komutun üzerinde çalışacağı alıcı nesne.
        private readonly CharacterMovement _character;

        // Bu komut çalıştırıldığında uygulanacak olan hareket miktarı.
        private readonly Vector3 _displacement;

        // Yapıcı Metot (Constructor): Komut oluşturulurken parametreler nesneye kilitlenir.
        public MoveCommand(CharacterMovement character, Vector3 displacement)
        {
            _character = character;
            _displacement = displacement;
        }

        // Komut çalıştırıldığında alıcıya asıl hareketi yaptırır.
        public void Execute()
        {
            _character.MoveInDirection(_displacement);
        }

        // Geri alma işleminde hareketin tam tersini (-_displacement) uygular.
        public void Undo()
        {
            _character.MoveInDirection(-_displacement);
        }
    }

    // =========================================================================
    // 4. INVOKER (Tetikleyici / Komut Yöneticisi)
    // Sorumluluk: Komutları yürütür, hafızaya kaydeder ve Undo/Redo yönetimini yapar.
    // =========================================================================
    public class CommandManager : MonoBehaviour
    {
        // Geçmişte çalıştırılan komutlar (Undo için LIFO yapısı)
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();

        // Geri alınan komutlar (Redo için LIFO yapısı)
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        // Yeni bir komutu çalıştıran ve geçmişe ekleyen metot.
        public void ExecuteCommand(ICommand command)
        {
            // 1. Komutu çalıştır.
            command.Execute();

            // 2. Geri alınabilmesi için geçmiş yığınına ekle.
            _undoStack.Push(command);

            // 3. Yeni bir hamle yapıldığı için eski 'Redo' geçmişini temizle.
            _redoStack.Clear();
        }

        // En son yapılan işlemi geri alan (Undo) metot.
        public void Undo()
        {
            // Geçmişte komut yoksa işlem yapma.
            if (_undoStack.Count == 0) return;

            // 1. En son eklenen komutu al.
            ICommand lastCommand = _undoStack.Pop();

            // 2. Tersini uygula (Geri al).
            lastCommand.Undo();

            // 3. İleri geçmişine (Redo) aktar.
            _redoStack.Push(lastCommand);
        }

        // Geri alınan en son işlemi tekrar uygulayan (Redo) metot.
        public void Redo()
        {
            // İleri geçmişinde komut yoksa işlem yapma.
            if (_redoStack.Count == 0) return;

            // 1. Redo yığınından komutu al.
            ICommand redoCommand = _redoStack.Pop();

            // 2. Tekrar çalıştır.
            redoCommand.Execute();

            // 3. Tekrar ana geçmiş yığınına ekle.
            _undoStack.Push(redoCommand);
        }
    }

    // =========================================================================
    // 5. CLIENT / INPUT SYSTEM (Girdi Dinleyici)
    // Sorumluluk: Oyuncunun tuş basışlarını dinler ve uygun Komut nesnesini üretir.
    // =========================================================================
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private CharacterMovement character;
        [SerializeField] private CommandManager commandManager;

        private void Update()
        {
            // W - İleri Hareket Komutu Oluştur
            if (Input.GetKeyDown(KeyCode.W))
            {
                ICommand moveForward = new MoveCommand(character, Vector3.forward);
                commandManager.ExecuteCommand(moveForward);
            }

            // S - Geri Hareket Komutu Oluştur
            if (Input.GetKeyDown(KeyCode.S))
            {
                ICommand moveBack = new MoveCommand(character, Vector3.back);
                commandManager.ExecuteCommand(moveBack);
            }

            // Z - Geri Al (Undo)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                commandManager.Undo();
            }

            // Y - Tekrar Et (Redo)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                commandManager.Redo();
            }
        }
    }
}