using UnityEngine;
using UnityEngine.SceneManagement;

namespace DesignPatterns.SingletonRefactored
{
    public class Singleton : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: SINGLETON (TEKİL) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Singleton, bir sınıfın çalışma zamanında (runtime) **yalnızca tek bir örneğinin (instance)** 
         oluşturulmasını garanti altına alan ve bu örneğe küresel bir erişim noktası (`Instance`) sağlayan 
         yaratımsal (creational) bir tasarım desenidir.
       - Unity'de genellikle `DontDestroyOnLoad` fonksiyonu ile birleştirilerek sahneler değişse dahi 
         yok olmayan yöneticiler (Manager/Controller) oluşturmak için tercih edilir.

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Küresel Erişim:** Oyunun herhangi bir yerinden (örneğin bir düşman öldüğünde veya buton tıklandığında) 
         `AudioManager.Instance.PlaySFX(...)` şeklinde doğrudan erişim kolaylığı sağlar.
       - **Sahneler Arası Veri Kalıcılığı:** Sahne yeniden yüklendiğinde (`SceneManager.LoadScene`) veya 
         yeni sahneye geçildiğinde nesnenin silinmesini ve durum verisinin kaybolmasını önler.
       - **Çift Nesne Oluşumunu Engeller:** Sahneye yanlışlıkla aynı Manager prefab'ından iki tane 
         yerleştirilirse veya sahne tekrar yüklendiğinde eski nesne silinmeyip yenisi gelirse, mükerrer 
         oluşan örneği tespit edip anında imha eder (`Destroy(gameObject)`).

    3. UNITY SINGLETON YAKLAŞIMLARI VE DİKKAT EDİLMESİ GEREKENLER:
       - **Klassik MonoBehaviour Singleton:** Sahnedeki var olan nesneye tutunur. Eğer sahnede yoksa `null` döner.
       - **Lazy Persistent Generic Singleton:** Sahnede nesne yoksa otomatik olarak yeni bir `GameObject` 
         oluşturur, bileşeni ekler ve `DontDestroyOnLoad` ile korur. Esnek ve yeniden kullanılabilirdir.
       - **Prefab Spawner (Alternative Approach):** Bir `Bootstrap` veya `Initializer` sahnesi aracılığıyla 
         kalıcı sistem prefab'larını oyunun başında bir kere `Instantiate` eder.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **`PersistentSingleton<T>`:** Jenerik, kalıcı ve türetilebilir temel Singleton tabanı.
       - **`AudioManager`:** Ses efektlerini ve arka plan müziğini sahneler arası kesintisiz yöneten Singleton.
       - **`GameSessionTracker`:** Sahne yeniden yüklendiğinde skoru ve tur sayısını koruyan sistem.
       - **`SceneReloader`:** Sahne yüklemelerini test etmek için girdi dinleyici.
    ====================================================================================================
    */

    // =========================================================================
    // 1. JENERİK SINGLETON TABANI (Generic Base Class for Reusability)
    // Sorumluluk: Herhangi bir MonoBehaviour sınıfını tek satırla Singleton yapar.
    // =========================================================================
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get;
            private set;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                // Çiftleşen (Duplicate) nesne tespit edildiğinde kendisini yok eder
                Destroy(gameObject);
            }
        }
    }

    // =========================================================================
    // 2. KULLANIM ÖRNEĞİ 1: Ses Yöneticisi (Audio Manager)
    // Sorumluluk: Sahneler arası müziği kesintisiz çalar ve ses efektlerini yürütür.
    // =========================================================================
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        public void PlaySoundEffect(AudioClip clip)
        {
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayBackgroundMusic(AudioClip musicClip)
        {
            if (_musicSource != null && musicClip != null)
            {
                _musicSource.clip = musicClip;
                _musicSource.loop = true;
                _musicSource.Play();
            }
        }
    }

    // =========================================================================
    // 3. KULLANIM ÖRNEĞİ 2: Oyun Oturumu Takipçisi (Game Session Tracker)
    // Sorumluluk: Sahne yenilense bile skoru ve tur sayısını saklar.
    // =========================================================================
    public class GameSessionTracker : PersistentSingleton<GameSessionTracker>
    {
        public int CurrentScore { get; private set; }
        public int ReloadCount { get; private set; }

        public void IncrementScore(int amount)
        {
            CurrentScore += amount;
            Debug.Log($"<color=green>[GameSession]</color> Skor Güncellendi: {CurrentScore}");
        }

        public void RegisterSceneReload()
        {
            ReloadCount++;
            Debug.Log($"<color=yellow>[GameSession]</color> Sahne {ReloadCount}. kez yeniden yüklendi. Mevcut Skor: {CurrentScore}");
        }
    }

    // =========================================================================
    // 4. TEST BİLEŞENİ: Sahne Yenileyici (Scene Reloader)
    // Sorumluluk: Test amacıyla R veya Space tuşuna basıldığında sahneyi tekrar yükler.
    // =========================================================================
    public class SceneReloader : MonoBehaviour
    {
        private void Update()
        {
            HandleSceneReloadInput();
            HandleScoreAdditionInput();
        }

        private void HandleSceneReloadInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (GameSessionTracker.Instance != null)
                {
                    GameSessionTracker.Instance.RegisterSceneReload();
                }

                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }
        }

        private void HandleScoreAdditionInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GameSessionTracker.Instance != null)
                {
                    GameSessionTracker.Instance.IncrementScore(100);
                }
            }
        }
    }
}