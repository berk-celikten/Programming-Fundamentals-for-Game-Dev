using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace DesignPatterns.ObjectPoolRefactored
{
    public class ObjectPool : MonoBehaviour
    {
    }

    /* 
    ====================================================================================================
    TASARIM DESENİ: OBJECT POOL (NESNE HAVUZU) PATTERN
    ====================================================================================================
    
    1. NEDİR / NE YAPAR?
       - Object Pool, sıkça oluşturulup (Instantiate) yok edilen (Destroy) nesnelerin bellekte 
         yol açtığı yükü ve çöp toplayıcı (Garbage Collector - GC) takılmalarını (Lag spikes) 
         önlemek için kullanılan yaratımsal (creational) bir tasarım desenidir.
       - Çalışma Mantığı: Nesneleri yok etmek yerine devre dışı bırakıp (`SetActive(false)`) 
         bir "havuzda" saklar. Yeni bir nesne gerektiğinde `Instantiate` etmek yerine havuzdaki 
         hazır nesneyi aktif eder (`SetActive(true)`).

    2. NEDEN KULLANILIR? (HANGİ PROBLEMLERİ ÇÖZER?)
       - **Garbage Collection (GC) Kasmalarını Önler:** Unity'de sürekli `Instantiate` ve `Destroy` 
         çağırmak RAM'de bellek bloklarını parçalar (fragmentation) ve GC tetiklendiğinde oyunda 
         anlık FPS düşüşlerine (stuttering) sebep olur.
       - **Performans Optimizasyonu:** Özellikle mermiler, vuruş efektleri (VFX), düşman dalgaları veya 
         hasar yazıları (floating text) gibi çok sayıda kısa ömürlü nesne üreten sistemlerde hayati önem taşır.

    3. UNITY'NIN `UnityEngine.Pool` KÜTÜPHANESİ:
       - Unity 2021.1+ sürümüyle birlikte C# seviyesinde built-in `IObjectPool<T>` arabirimi ve `ObjectPool<T>` 
         sınıfı sunulmuştur. Bu sayede manuel liste/kuyruk yönetimi yazmaya gerek kalmadan thread-safe ve 
         optimaye bir havuzlama yapılabilir.

    4. MİMARİ BİLEŞENLERİ VE ROLLERİ:
       - **Pooled Item (`FireballProjectile`):** Havuzda saklanan ve tekrar tekrar kullanılan nesne.
       - **Pool Handler / Launcher (`MageCaster`):** Havuzu oluşturan (`Create`), havuzdan alan (`Get`), 
         havuza iade eden (`Release`) ve sınır aşılınca imha eden (`Destroy`) kontrolcü sınıf.
    ====================================================================================================
    */

    // =========================================================================
    // 1. POOLED ITEM (Havuzlanan Nesne - Alev Topu Projektili)
    // Sorumluluk: Hareket eder, süresi dolunca kendini ait olduğu havuza iade eder.
    // =========================================================================
    public class FireballProjectile : MonoBehaviour
    {
        private IObjectPool<FireballProjectile> _originPool;

        [SerializeField] private float _moveSpeed = 15f;
        [SerializeField] private float _lifeTimeSeconds = 2f;

        private Coroutine _autoReleaseRoutine;

        public void InitializePool(IObjectPool<FireballProjectile> pool)
        {
            _originPool = pool;
        }

        private void OnEnable()
        {
            // Nesne havuzdan alınıp aktif edildiğinde yaşam süresi sayacı başlar
            _autoReleaseRoutine = StartCoroutine(AutoReleaseTimer());
        }

        private void OnDisable()
        {
            // Nesne havuza geri döndüğünde çalışan coroutine'leri temizliyoruz
            if (_autoReleaseRoutine != null)
            {
                StopCoroutine(_autoReleaseRoutine);
                _autoReleaseRoutine = null;
            }
        }

        private void Update()
        {
            // İleriye doğru sabit hareket
            transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
        }

        private IEnumerator AutoReleaseTimer()
        {
            yield return new WaitForSeconds(_lifeTimeSeconds);
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            if (_originPool != null)
            {
                _originPool.Release(this);
            }
            else
            {
                // Eğer havuz referansı yoksa güvenli çıkış olarak yok et
                Destroy(gameObject);
            }
        }
    }

    // =========================================================================
    // 2. POOL MANAGER / LAUNCHER (Havuz Yöneticisi - Büyücü Sınıfı)
    // Sorumluluk: UnityEngine.Pool kütüphanesini yapılandırır ve yönetir.
    // =========================================================================
    public class MageCaster : MonoBehaviour
    {
        [SerializeField] private FireballProjectile _fireballPrefab;
        [SerializeField] private Transform _castPoint;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 20;

        private IObjectPool<FireballProjectile> _fireballPool;

        private void Awake()
        {
            InitializeObjectPool();
        }

        private void Update()
        {
            // Sol Tık veya Sol Ctrl tuşuna basıldığında büyü fırlatır
            if (Input.GetButtonDown("Fire1"))
            {
                CastFireball();
            }
        }

        private void InitializeObjectPool()
        {
            _fireballPool = new ObjectPool<FireballProjectile>(
                createFunc: OnCreatePoolItem,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolItem,
                collectionCheck: true, // Aynı nesnenin havuza 2 kez eklenmesini engeller (Debug için yararlı)
                defaultCapacity: _defaultCapacity,
                maxSize: _maxPoolSize
            );
        }

        // --- HAVUZ CALLBACK FONKSİYONLARI ---

        // 1. Havuzda hiç hazır nesne yoksa yeni bir tane Instantiate edilir
        private FireballProjectile OnCreatePoolItem()
        {
            FireballProjectile projectile = Instantiate(_fireballPrefab);
            projectile.InitializePool(_fireballPool);
            return projectile;
        }

        // 2. Havuzdan bir nesne talep edildiğinde (Get) çalışır
        private void OnGetFromPool(FireballProjectile projectile)
        {
            projectile.gameObject.SetActive(true);

            // Konum ve rotasyonu büyü noktasına sıfırla
            Transform spawnTransform = _castPoint != null ? _castPoint : transform;
            projectile.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
        }

        // 3. Nesne işi bitip havuza geri verildiğinde (Release) çalışır
        private void OnReleaseToPool(FireballProjectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        // 4. Havuz maxSize sınırını aşarsa veya havuz temizlenirse nesneyi bellekten siler
        private void OnDestroyPoolItem(FireballProjectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        // --- BÜYÜ TETİKLEME ---
        private void CastFireball()
        {
            if (_fireballPool != null)
            {
                // Instantiate yerine havuzdan nesne isteniyor
                _fireballPool.Get();
            }
        }
    }
}