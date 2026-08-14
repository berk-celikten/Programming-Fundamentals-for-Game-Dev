using UnityEngine;

namespace DesignPatterns.StateMachineRefactored
{
    public class StateMachine : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: STATE (DURUM) PATTERN / FINITE STATE MACHINE (FSM)
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - State deseni, bir nesnenin iç durumu (state) değiştiğinde davranışını da değiştirmesini sağlayan 
         davranışsal (behavioral) bir tasarım desenidir.
       - Nesne, dışarıdan bakıldığında sanki sınıfını değiştiriyormuş gibi davranır. Her durum ayrı bir 
         sınıf olarak kapsüllenir (encapsulation).

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Spagetti `switch-case` veya `if-else` Yapılarını Engeller:** Karakter hareketlerinde (Yürüme, 
         Zıplama, Eğilme, Tırmanma) tüm durumları tek bir `Update` içinde devasa `switch` bloklarıyla 
         yönetmek kod büyüdükçe bakımı imkansız hale getirir.
       - **Genişletilebilirlik (Open/Closed Principle):** Oyuna yeni bir durum ekleneceğinde (örneğin "Kayma / Sliding" 
         veya "Tırmanma / Climbing"), mevcut karakter kodunu değiştirmeden sadece yeni bir `IState` sınıfı 
         yazmak yeterlidir.

    3. KLASİK ENUM SWITCH İLE CLASS-BASED STATE PATTERN KARŞILAŞTIRMASI:
       - **Enum tabanlı FSM:** Küçük ve durumu az nesneler için hızlıdır. Ancak her durumda özel bir zamanlayıcı 
         veya animasyon çalıştırmak gerektiğinde kod karmaşıklaşır.
       - **Class-based State Pattern:** Her durumun kendi `Enter()`, `Exit()` ve `LogicUpdate()` 
         fonksiyonları bulunur. Duruma özel değişkenler ve animasyonlar o sınıfın içinde temiz bir şekilde tutulur.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **`ICharacterState`:** Bütün durumların türediği arayüz (Enter, Exit, Update).
       - **Concrete States (`GroundedState`, `InAirState`, `CrouchState`):** Duruma özel davranışlar.
       - **Context (`CharacterLocomotionFSM`):** Mevcut durumu tutar, geçişleri yönetir.
    ====================================================================================================
    */

    // =========================================================================
    // 1. STATE INTERFACE (Durum Arayüzü)
    // Sorumluluk: Her durumun barındırması gereken yaşam döngüsü metotlarını tanımlar.
    // =========================================================================
    public interface ICharacterState
    {
        void EnterState(CharacterLocomotionFSM context);
        void LogicUpdate();
        void PhysicsUpdate();
        void ExitState();
    }

    // =========================================================================
    // 2. CONTEXT (Bağlam Sınıfı - Karakter Hareket Kontrolcüsü)
    // Sorumluluk: Mevcut durumu yönetir ve dış girdileri duruma iletir.
    // =========================================================================
    public class CharacterLocomotionFSM : MonoBehaviour
    {
        // Durumlar (GC Alloc oluşturmamak için bir kez oluşturulup saklanır)
        public GroundedState GroundedState { get; private set; }
        public InAirState InAirState { get; private set; }
        public CrouchState CrouchState { get; private set; }

        private ICharacterState _currentState;
        public string CurrentStateName => _currentState != null ? _currentState.GetType().Name : "None";

        private void Awake()
        {
            // State nesnelerini bir defa belleğe alıyoruz
            GroundedState = new GroundedState(this);
            InAirState = new InAirState(this);
            CrouchState = new CrouchState(this);
        }

        private void Start()
        {
            // Başlangıç durumu
            TransitionTo(GroundedState);
        }

        private void Update()
        {
            _currentState?.LogicUpdate();
            HandleInput();
        }

        private void FixedUpdate()
        {
            _currentState?.PhysicsUpdate();
        }

        public void TransitionTo(ICharacterState newState)
        {
            if (newState == null || newState == _currentState) return;

            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState(this);

            Debug.Log($"<color=cyan>[FSM]</color> Yeni Duruma Geçildi: <b>{CurrentStateName}</b>");
        }

        private void HandleInput()
        {
            // Tuş girdilerini test amaçlı dinliyoruz
            if (Input.GetKeyDown(KeyCode.Space)) ExecuteJump();
            if (Input.GetKeyDown(KeyCode.C)) ExecuteCrouch();
        }

        public void ExecuteJump()
        {
            if (_currentState is GroundedState)
            {
                TransitionTo(InAirState);
            }
            else if (_currentState is CrouchState)
            {
                TransitionTo(GroundedState);
            }
        }

        public void ExecuteCrouch()
        {
            if (_currentState is GroundedState)
            {
                TransitionTo(CrouchState);
            }
            else if (_currentState is CrouchState)
            {
                TransitionTo(GroundedState);
            }
        }

        public void ExecuteLand()
        {
            if (_currentState is InAirState)
            {
                TransitionTo(GroundedState);
            }
        }
    }

    // =========================================================================
    // 3. CONCRETE STATES (Somut Durum Sınıfları)
    // =========================================================================

    // Durum 1: Yerde Olma Durumu (Grounded)
    public class GroundedState : ICharacterState
    {
        private readonly CharacterLocomotionFSM _context;

        public GroundedState(CharacterLocomotionFSM context)
        {
            _context = context;
        }

        public void EnterState(CharacterLocomotionFSM context)
        {
            // Örn: Yürüme animasyonunu başlat, sürtünmeyi normale çek
        }

        public void LogicUpdate() { }

        public void PhysicsUpdate() { }

        public void ExitState() { }
    }

    // Durum 2: Havada Olma Durumu (InAir)
    public class InAirState : ICharacterState
    {
        private readonly CharacterLocomotionFSM _context;

        public InAirState(CharacterLocomotionFSM context)
        {
            _context = context;
        }

        public void EnterState(CharacterLocomotionFSM context)
        {
            // Örn: Dikey kuvvet (Impulse) uygula, Zıplama animasyonunu tetikle
        }

        public void LogicUpdate()
        {
            // Örn: Yere temas edilip edilmediğini kontrol et (Raycast / GroundCheck)
        }

        public void PhysicsUpdate() { }

        public void ExitState() { }
    }

    // Durum 3: Eğilme Durumu (Crouch)
    public class CrouchState : ICharacterState
    {
        private readonly CharacterLocomotionFSM _context;

        public CrouchState(CharacterLocomotionFSM context)
        {
            _context = context;
        }

        public void EnterState(CharacterLocomotionFSM context)
        {
            // Örn: Karakter Kolider boyutunu yarıya indir, hareket hızını düşür
        }

        public void LogicUpdate() { }

        public void PhysicsUpdate() { }

        public void ExitState()
        {
            // Örn: Kolider boyutunu tekrar eski haline getir
        }
    }
}