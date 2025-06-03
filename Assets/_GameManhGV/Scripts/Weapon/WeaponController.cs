// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using Random = UnityEngine.Random;
// using UnityEngine.EventSystems;
// using static GameConstants;
//
// public class WeaponController : Singleton<WeaponController>
// {
//     [SerializeField] public WeaponInfo weaponInfo;
//     [SerializeField] private LayerMask botLayerMask;
//     [SerializeField] private LayerMask armorBossLayerMask;
//     [SerializeField] private LayerMask groundLayerMask;
//     [SerializeField] private LayerMask rewardLayerMask;
//     [SerializeField] private LayerMask gasLayerMask;
//     [SerializeField] private Transform _muzzleTrans;
//     [SerializeField] private Transform _muzzleTrans2;
//     [SerializeField] public Transform[] Gunbarrel; // Nòng súng xoay (dùng cho súng 6 nòng)
//     [SerializeField] private Animation _animation;
//     [SerializeField] private PoolType _bulletType; // Loại đạn sẽ được bắn
//     [SerializeField] private ParticleSystem[] _fireEffect;
//     [SerializeField] private AudioSource _audioSource;
//     [SerializeField] private AudioSource _audioSourceHit;
//     [SerializeField] private bool _isShowCard;
//     [SerializeField] private bool shootBasedOnGunDirection = false; // Chế độ bắn: true = bắn theo hướng súng, false = bắn theo hướng camera
//     [SerializeField] private bool isDoubleMuzzle = false;
//     [SerializeField] private Transform shakeCam; // Biến để tham chiếu đến MainCamera
//     [SerializeField] private float shakeCamMin;
//     [SerializeField] private float shakeCamMax;
//     [SerializeField] private bool IsShowLunaEndGame;
//
//     private Transform _cameraTransform;
//     private Camera _camera;
//     private float _timeSinceLastShoot = 0f; // Thời gian từ lần bắn cuối cùng
//     private int _currentBulletCount; // Số lượng đạn hiện tại trong băng
//     private int _defaultBulletCount; // Số lượng đạn hiện tại trong băng
//     public int CurrentBulletCount => _currentBulletCount;
//     public int DefaultBulletCount => _defaultBulletCount;
//     private bool _isReloading = false; // Trạng thái đang nạp đạn
//     private float currentRotationSpeed = 0f; // Tốc độ quay hiện tại của nòng súng
//     private bool isShooting = false; // Trạng thái đang bắn
//     private bool canShoot = false; // Trạng thái có thể bắn
//     private bool isBarrelSpinning = false; // Trạng thái nòng súng đang quay
//     private Coroutine shootingCoroutine;
//     
//     [Header("DrawGizmod")]
//
//     [SerializeField] private float distance;
//     
//     [Header("Tutorial")]
//     public bool instructReload = false;
//     public bool instructRoket = false;
//     
//     // kiểm tra có ang ấn vào UI không
//     // Bộ nhớ tạm cho kiểm tra UI - static để tái sử dụng
//     private static readonly PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
//     private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();
//         
//     protected override void Awake()
//     {
//         base.Awake();
//         _bulletType = PoolType.Projectile_Bullet_Norman;
//         _camera = Camera.main;
//         _cameraTransform = _camera.transform;
//         _currentBulletCount = weaponInfo.bulletCount; // Khởi tạo số lượng đạn
//         _defaultBulletCount = _currentBulletCount;
//         AssignAnimationClips();
//         UIManager.Instance.GetUI<Canvas_GamePlay>().Init();
//         Invoke(nameof(Init),.1f);
//     }
//
//     private void Start()
//     {
//         Instance = this;
//     }
//
//     private void Init()
//     {
//         EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
//         EventManager.Invoke(EventName.UpdateBulletCountDefault, _defaultBulletCount);
//     }
//     
//     private void OnEnable()
//     {
//         EventManager.AddListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
//         EventManager.AddListener<bool>(EventName.OnChangeFireRate, OnChangeFireRate);
//     }
//     private void OnDisable()
//     {
//         EventManager.RemoveListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
//         EventManager.RemoveListener<bool>(EventName.OnChangeFireRate, OnChangeFireRate);
//     }
//
//     private void OnShowLunaEndGame(bool IsShow)
//     {
//         IsShowLunaEndGame = IsShow;
//     }
//     
//     public bool IsReloadFull()
//     {
//         return _currentBulletCount >= weaponInfo.bulletCount;
//     }
//
//     private void Update()
//     {
// #if UNITY_EDITOR
//         GizmodTuVe();
// #endif
//         if(GameManager.Instance.GetGameState()!= GameConstants.GameState.Playing)
//             return;
//         
//         HandleGatlingGunRotation();
//         // if (!UIEndGame.Instance.IsShowEndGame)
//         // {
//         if(!IsPointerOverUI())
//             OnShooting();
//         // }
//         // if (UIEndGame.Instance.IsShowEndGame )
//         // {
//         //     isShooting = false;
//         //     StopGunEffect();
//         // }
//
//
//
//
//         // Thêm phím tắt để chuyển đổi chế độ bắn
//         if (Input.GetKeyDown(KeyCode.T)) // Ví dụ: nhấn phím T để chuyển đổi
//         {
//             shootBasedOnGunDirection = !shootBasedOnGunDirection;
//             Debug.Log("Shoot based on gun direction: " + shootBasedOnGunDirection);
//         }
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             InstructRocket();
//         }
//         
//     }
//
//     public void InstructRocket()
//     {
//         //print("Hướng dẫn rocket");
//         if(!instructRoket)
//         {
//             instructRoket = true;
//             EventManager.Invoke(EventName.InstructRocket, true);
//         }
//     }
//
//     public void InstructReload()
//     {
//         //TODO: nếu chưa hướng dẫn reload thì hướng dẫn reload
//         if (!instructReload)
//         {
//             instructReload = true;
//             EventManager.Invoke(EventName.InstructReload, true);
//         }
//     }
//
//     /// <summary>
//     /// Kiểm tra xem con trỏ chuột có đang hover trên UI element không
//     /// </summary>
//     /// <returns>True nếu pointer đang trên UI, false nếu không</returns>
//     public static bool IsPointerOverUI()
//     {
//         if (EventSystem.current == null) return false;
//
//         // Cập nhật vị trí con trỏ hiện tại
//         PointerEventData.position = Input.mousePosition;
//         RaycastResults.Clear();
//
//         // Thực hiện raycast để kiểm tra UI elements
//         EventSystem.current.RaycastAll(PointerEventData, RaycastResults);
//         return RaycastResults.Count > 0;
//     }
//
//     private void AssignAnimationClips()
//     {
//         if (_animation != null && weaponInfo != null)
//         {
//             _animation.AddClip(weaponInfo.Fire, "Fire");
//             _animation.AddClip(weaponInfo.Idle, "Idle");
//             _animation.AddClip(weaponInfo._reloadAnimIn, "ReloadIn");
//             _animation.AddClip(weaponInfo._reloadAnimOn, "ReloadOn");
//             _animation.AddClip(weaponInfo._reloadAnimOut, "ReloadOut");
//         }
//     }
//   
//     private void OnShooting()
//     {
//         if (_isReloading)//IsIngameGUI || 
//             return;
//
//         _timeSinceLastShoot += Time.deltaTime;
//
//         if (Input.GetMouseButton(0))
//         {
//             UICrosshairItem.Instance.Narrow_Crosshair();
//             if (!isShooting)
//             {
//                 isShooting = true;
//                 if (shootingCoroutine == null)
//                 {
//                     shootingCoroutine = StartCoroutine(StartShootingAfterDelay());
//                 }
//                 if (!isBarrelSpinning )//&& !EventSystem.current.IsPointerOverGameObject()
//                 {
//                     _audioSource.clip = weaponInfo.AudioStartBarrel;
//                     _audioSource.Play();
//                     isBarrelSpinning = true;
//                 }
//             }
//
//             if (canShoot && _timeSinceLastShoot >= weaponInfo.FireRate)
//             {
//                 if (_currentBulletCount <= 0 && !weaponInfo.infiniteBullet)
//                 {
//                     OnReload();
//                 }
//                 else
//                 {
//                     Shoot();
//                     _timeSinceLastShoot = 0f;
//
//                     if (!weaponInfo.infiniteBullet)
//                     {
//                         _currentBulletCount--;
//                         if (_currentBulletCount <= 3 && !instructReload)
//                         {
//                             instructReload = true;
//                             EventManager.Invoke(EventName.InstructReload, true);
//                         }
//                         
//                         //Debug.Log("Bullet fired. Remaining bullets: " + _currentBulletCount);
//                         EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
//                     }
//                     PlayGunEffect(); // Kích hoạt hiệu ứng nổ súng
//                 }
//             }
//         }
//         else
//         {
//             if (isShooting)
//             {
//                 StopShootingSound();
//                 isShooting = false;
//                 canShoot = false; // Reset canShoot when stopping shooting
//                 if (shootingCoroutine != null)
//                 {
//                     StopCoroutine(shootingCoroutine);
//                     shootingCoroutine = null;
//                 }
//                 if (isBarrelSpinning)
//                 {
//                     _audioSource.clip = weaponInfo.AudioEndBarrel;
//                     _audioSource.Play();
//                     isBarrelSpinning = false;
//                 }
//                 StopGunEffect(); // Dừng hiệu ứng nổ súng
//             }
//         }
//     }
//     public void OnReload()
//     {
//
//         if (_isReloading || _currentBulletCount == _defaultBulletCount)
//             return;
//         _isReloading = true;
//         StartCoroutine(Reload());
//         StopGunEffect();
//         UICrosshairItem.Instance.ResetCorosshair();
//     }
//     public int GetCurrentAmmo()
//     {
//         return _currentBulletCount;
//     }
//     private void HandleGatlingGunRotation()
//     {
//         if (weaponInfo.isGatlingGun)
//         {
//             if (isShooting)
//             {
//                 currentRotationSpeed += (weaponInfo.MaxSpeedRotaBarrel / Mathf.Max(1, weaponInfo.WaitToShoot)) * Time.deltaTime;
//                 if (currentRotationSpeed >= weaponInfo.MaxSpeedRotaBarrel)
//                 {
//                     currentRotationSpeed = weaponInfo.MaxSpeedRotaBarrel;
//                 }
//             }
//             else if (currentRotationSpeed > weaponInfo.MinSpeedRotaBarrel)
//             {
//                 currentRotationSpeed -= (weaponInfo.MaxSpeedRotaBarrel / weaponInfo.TimeMinSpeed) * Time.deltaTime;
//                 if (currentRotationSpeed <= weaponInfo.MinSpeedRotaBarrel)
//                 {
//                     currentRotationSpeed = weaponInfo.MinSpeedRotaBarrel;
//                 }
//             }
//
//             RotateGunbarrels();
//         }
//     }
//
//     private IEnumerator StartShootingAfterDelay()
//     {
//         yield return new WaitForSeconds(weaponInfo.WaitToShoot);
//         canShoot = true;
//         shootingCoroutine = null;
//     }
//
//     private void RotateGunbarrels()
//     {
//         if (IsShowLunaEndGame) return;
//         foreach (var barrel in Gunbarrel)
//         {
//             var currentRotation = barrel.localRotation.eulerAngles;
//             var newRotationZ = currentRotation.z + currentRotationSpeed * Time.deltaTime;
//             // Debug.Log(currentRotation);
//             // Debug.Log(newRotationZ);
//             // Debug.Log(currentRotationSpeed);
//             var newRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newRotationZ);
//               
//             
//             barrel.localRotation = newRotation;
//         }
//     }
//     
//     [SerializeField] LayerMask boxLayer;
//     public Vector3 GizmodTuVe()
//     {
//         Ray ray = new Ray(shakeCam.position, shakeCam.forward);
//         RaycastHit hit;
//
//         Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);
//
//         // Bắn raycast
//         if (Physics.Raycast(ray, out hit, distance, boxLayer))
//         {
//             // Debug.Log("Va chạm tại vị trí: " + hit.point);
//             // Debug.Log("Khoảng cách đến box: " + hit.distance);
//
//             Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.1f, Color.green);
//             return hit.point;
//         }
//
//         return Vector3.one;
//     }
//     
//     private void Shoot()
//     {
//         if (this == null || _cameraTransform == null) return;
//
//         Vector3 forward;
//         if (shootBasedOnGunDirection)
//         {
//             forward = _cameraTransform.forward;
//             StartCoroutine(ShakeCamera(0.1f, 0.05f));
//         }
//         else
//         {
//             forward = _cameraTransform.forward;
//             var targetPoint = FindPointedTransform();
//             if (targetPoint != null)
//             {
//                 if (Vector3.SqrMagnitude(targetPoint.position - _cameraTransform.position) > 0)
//                     forward = (targetPoint.position - _cameraTransform.position).normalized;
//             }
//         }
//
// //        print(_cameraTransform.forward + "Forward: " + forward);
//         
//         forward += new Vector3(
//             Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
//             Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
//             Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount)
//         );
//
//         // Bắn từ nòng đầu tiên
//         FireFromMuzzle(_muzzleTrans, forward);
//
//         // Nếu isDoubleMuzzle là true, bắn thêm từ nòng thứ hai
//         if (isDoubleMuzzle)
//         {
//             FireFromMuzzle(_muzzleTrans2, forward);
//         }
//
//         _animation.Play("Fire");
//         _animation["Fire"].speed = 2.0f;
//         _audioSource.clip = weaponInfo.audioClip;
//         _audioSource.Play();
//
//         UICrosshairItem.Instance.Expand_Crosshair(15);
//
//         PlayGunEffect();
//     }
//
//     private bool IsInBotLayer(GameObject obj)
//     {
//         return ((1 << obj.layer) & botLayerMask ) != 0;
//     }
//     
//     private bool IsInArmorBossLayer(GameObject obj)
//     {
//         return ((1 << obj.layer) & armorBossLayerMask ) != 0;
//     }
//     
//     public static class LayerConstants
//     {
//         public static readonly int WeakPointLayer = 10; // Giả sử layer 10 là WeakPoint
//         public static readonly LayerMask WeakPointMask = 1 << WeakPointLayer;
//     }
//
//     private bool IsInGroundLayerMask(GameObject obj)
//     {
//         return ((1 << obj.layer) & (groundLayerMask | LayerConstants.WeakPointMask)) != 0;
//     }
//
//     private bool IsInRewardLayer(GameObject obj)
//     {
//         return ((1 << obj.layer) & rewardLayerMask) != 0;
//     }
//     
//     private bool IsInGasLayer(GameObject obj)
//     {
//         return ((1 << obj.layer) & gasLayerMask ) != 0;
//     }
//     
//     private void FireFromMuzzle(Transform muzzle, Vector3 forward)
//     {
//         var shotRotation = Quaternion.Euler(Random.insideUnitCircle * weaponInfo.inaccuracy) * forward;
//         var ray = new Ray(_cameraTransform.position, shotRotation);
//
//         BulletTrail bullet = SimplePool.Spawn<BulletTrail>(_bulletType, muzzle.position, muzzle.rotation);
//         Vector3 posGizmod = GizmodTuVe();
//         bullet.Init((posGizmod - muzzle.position).normalized,posGizmod);
//         
//         bool CheckRayCast = Physics.Raycast(ray, out var hit, Mathf.Infinity, botLayerMask | armorBossLayerMask |gasLayerMask| rewardLayerMask| groundLayerMask | LayerConstants.WeakPointMask);
//         PoolType typeEffect = PoolType.vfx_ConcreteImpact;
//         if (CheckRayCast)
//         {
//             //var damageType = hit.collider.CompareTag("WeakPoint")? DamageType.Weekness:DamageType.Normal;
//             var damageType = hit.collider.gameObject.layer == LayerConstants.WeakPointLayer 
//                 ? DamageType.Weekness 
//                 : DamageType.Normal;
//             //Debug.Log($"Raycast hit object: {hit.collider.gameObject.name}, Layer: {hit.collider.gameObject.layer}");
//
//             var damageInfo = new DamageInfo() 
//             {
//                 damageType = damageType,
//                 damage = weaponInfo.damage,
//                 name = hit.collider.name,
//             };
//             
//             if (IsInBotLayer(hit.collider.gameObject))
//             {
//                 var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
//                 if (takeDamageController == null)
//                 {
//                     takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
//                 }
//                 if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
//                 typeEffect = PoolType.vfx_BloodEffectZom;
// //                Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
//             }
//             else if (IsInArmorBossLayer(hit.collider.gameObject))
//             {
//                 Detector detector = hit.transform.gameObject.GetComponent<Detector>();
//                 if (detector != null)
//                     detector.SetHealthImage(damageInfo.damage);
//                 
//                 damageInfo.damage /= 3;
//                 var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
//                 if (takeDamageController == null)
//                 {
//                     takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
//                 }
//                 if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
//
//             }
//             else if (IsInRewardLayer(hit.collider.gameObject))
//             {
//                 var rewardController = hit.transform.gameObject.GetComponent<IReward>();
//                 if (rewardController == null)
//                 {
//                     rewardController = hit.transform.root.gameObject.GetComponent<IReward>();
//                 }
//                 if (rewardController != null) rewardController.TakeCollect(weaponInfo.damage);
//                 PlayRandomAttackSound();
//                 typeEffect = PoolType.vfx_ConcreteImpact;
//             } 
//             else if(IsInGroundLayerMask(hit.collider.gameObject))
//             {
//                  var takeDamageController1 = hit.transform.gameObject.GetComponent<ITakeDamage>();
//                  if (takeDamageController1 == null)
//                  {
//                      takeDamageController1 = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
//                  }
//                  if (takeDamageController1 != null) takeDamageController1.TakeDamage(damageInfo);
//                  //Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
//                  //PlayRandomAttackSound();
//                  typeEffect = PoolType.vfx_ConcreteImpact;
//             }else if (IsInGasLayer(hit.collider.gameObject))
//             {
//                 OxygenTanks oxygenTanks = null;
//                 oxygenTanks = hit.transform.gameObject.GetComponent<OxygenTanks>();
//                 if(oxygenTanks !=null)
//                     oxygenTanks.Explosion();
//                 //PlayRandomAttackSound();
//                 typeEffect = PoolType.vfx_ConcreteImpact;
//                 // print("Ban vao Ga");
//             }
//
//             // Tạo hiệu ứng va chạm
//             SimplePool.Spawn<Effect>(typeEffect, hit.point, Quaternion.identity).Init();
//         }
//         EventManager.Invoke(EventName.OnCheckBotTakeDamage, CheckRayCast);
//     }
//
//     
//     void PlayRandomAttackSound()
//     {
//         // AudioClip clip = AudioManager.Instance.GetAudioAttackClip();
//         // if (clip != null)    
//         // {
//         //     //_audioSource.clip = clip;
//         //     _audioSourceHit.PlayOneShot(clip);
//         // }
//     }
//     private void OnChangeFireRate(bool IsChange)
//     {
//         if (IsChange)
//         {
//             _bulletType = PoolType.Projectile_Bullet_BBQ;
//         }
//         else
//         {
//             _bulletType = PoolType.Projectile_Bullet_Norman;
//         }
//     }
//
//
//     private IEnumerator Reload()
//     {
//         StopShootingSound();
//         float reloadTime = weaponInfo.reloadTime;
//         EventManager.Invoke(EventName.OnReloading, reloadTime);
//         //Debug.Log("Reloading...");
//         _audioSource.PlayOneShot(weaponInfo.AudioReloadIn);
//
//         _animation.Play("ReloadIn");
//         yield return new WaitForSeconds(reloadTime / 3);
//
//         _animation.Play("ReloadOn");
//         yield return new WaitForSeconds(reloadTime / 3);
//
//         _audioSource.PlayOneShot(weaponInfo.AudioReloadOut);
//         _animation.Play("ReloadOut");
//         yield return new WaitForSeconds(reloadTime / 3);
//         _currentBulletCount = weaponInfo.bulletCount;
//         
//         _isReloading = false;
//         
//         EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
//
//         // Phát âm thanh khi súng ngừng xoay nếu đang nạp đạn
//         if (isBarrelSpinning)
//         {
//             _audioSource.clip = weaponInfo.AudioEndBarrel;
//             _audioSource.Play();
//             isBarrelSpinning = false;
//         }
//     }
//
//     private IEnumerator DecreaseRotationSpeed()
//     {
//         while (currentRotationSpeed > weaponInfo.MinSpeedRotaBarrel)
//         {
//             currentRotationSpeed -= (weaponInfo.MaxSpeedRotaBarrel / weaponInfo.TimeMinSpeed) * Time.deltaTime;
//             if (currentRotationSpeed < weaponInfo.MinSpeedRotaBarrel)
//             {
//                 currentRotationSpeed = weaponInfo.MinSpeedRotaBarrel;
//             }
//             yield return null;
//         }
//     }
//
//     private Transform FindPointedTransform()
//     {
//         // var minCrossHairDistance = float.MaxValue;
//         Transform pointedTransform = null;
//
//         // var bots = BotManager.Instance.BotNetworks;
//         // foreach (var bot in bots.Where(bot => bot != null && !bot.IsDead))
//         // {
//         //     CheckFireAssistCheckPos(bot.FireAssistCheckPos, ref minCrossHairDistance, ref pointedTransform);
//         // }
//         //
//         // var rewards = RewardManager.Instance.RewardNetworks;
//         // foreach (var reward in rewards.Where(reward => reward != null && !reward.IsCollected))
//         // {
//         //     CheckFireAssistCheckPos(reward.FireAssistCheckPos, ref minCrossHairDistance, ref pointedTransform);
//         // }
//
//         return pointedTransform;
//     }
//
//     private void CheckFireAssistCheckPos(List<Transform> fireAssistCheckPos, ref float minCrossHairDistance, ref Transform pointedTransform)
//     {
//         foreach (var checkPoint in fireAssistCheckPos)
//         {
//             var checkPosition = checkPoint.position;
//
//             if (!SatisfyAutoFireCondition(checkPosition, out var crossHairDistance) ||
//                 crossHairDistance > minCrossHairDistance) continue;
//
//             minCrossHairDistance = crossHairDistance;
//             pointedTransform = checkPoint;
//         }
//     }
//
//     [SerializeField] private float radius = 33f;
//     private const float ReferenceWidth = 887;
//
//     private bool SatisfyAutoFireCondition(Vector3 target, out float distance)
//     {
//         var viewPosition = _camera.WorldToScreenPoint(target);
//         if (viewPosition.z < 0)
//         {
//             distance = float.MaxValue;
//             return false;
//         }
//         viewPosition.x -= Screen.width / 2f;
//         viewPosition.y -= Screen.height / 2f;
//
//         viewPosition *= ReferenceWidth / Screen.width;
//
//         distance = Mathf.Sqrt(viewPosition.x * viewPosition.x + viewPosition.y * viewPosition.y);
//         return distance < radius && IsClearShot(_cameraTransform.position, target);
//     }
//
//     private bool IsClearShot(Vector3 origin, Vector3 target)
//     {
//         var distance = Vector3.Distance(origin, target);
//         var ray = new Ray(origin, target - origin);
//         return !Physics.Raycast(ray, out _, distance, botLayerMask| armorBossLayerMask  | rewardLayerMask | groundLayerMask);
//     }
//
//     // Thêm phương thức dừng âm thanh bắn
//   
//     private void StopShootingSound()
//     {
//         if (_audioSource.isPlaying && _audioSource.clip == weaponInfo.audioClip)
//         {
//             _audioSource.Stop();
//         }
//     }
//
//     // Thêm phương thức nhận AnimationEvent
//     public void AnimationAudioEvent()
//     {
//         // Thực hiện hành động khi sự kiện AnimationAudioEvent được gọi
//         //Debug.Log("AnimationAudioEvent called");
//     }
//
//     // Thêm hàm rung lắc camera
//     private IEnumerator ShakeCamera(float duration, float magnitude)
//     {
//         Quaternion originalRot = shakeCam.localRotation;
//         float elapsed = 0.0f;
//
//         while (elapsed < duration)
//         {
//             float x = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
//             float y = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
//
//             shakeCam.localRotation = originalRot * Quaternion.Euler(x, y, 0);
//
//             elapsed += Time.deltaTime;
//
//             yield return null;
//         }
//
//         shakeCam.localRotation = originalRot;
//         EventManager.Invoke(EventName.OnCheckShakeCam, shakeCam.localEulerAngles);
//     }
//
//
//     private void PlayGunEffect()
//     {
//         foreach (ParticleSystem fireEffect in _fireEffect)
//         {
//             if (fireEffect != null && !fireEffect.isPlaying)
//             {
//                 fireEffect.Play();
//             }    
//         }
//     }
//     
//     private void StopGunEffect()
//     {
//         foreach (ParticleSystem fireEffect in _fireEffect)
//         {
//             if (fireEffect != null && fireEffect.isPlaying)
//             {
//                 fireEffect.Stop();
//             }
//         }
//     }
// }
