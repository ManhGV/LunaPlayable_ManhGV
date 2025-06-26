using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponBase : Singleton<WeaponBase>
{
    [Header("Data Weapon")]
    [SerializeField] public WeaponInfo weaponInfo;

    [Header("Layer Target")] [Tooltip("0: Bot, 1:Weakpoint, 2:Reward, 3:Ground, 4:Gas")]
    [SerializeField] protected LayerMask[] LayerMasks;
    
    [Header("Effect")]
    [SerializeField] private ParticleSystem[] _fireEffect;
    
    [Header("Audio")]
    [SerializeField] protected AudioSource _audioSource;
    
    [Header("DrawGizmod Caculator Point Shoot")]
    [SerializeField] protected Transform _cameraTransform;
    [SerializeField] LayerMask _layerTarget;
    [SerializeField] float _distanceGizMod;
    
    public bool readyShoot;
    
    // kiểm tra có ang ấn vào UI không
    // Bộ nhớ tạm cho kiểm tra UI - static để tái sử dụng
    private static readonly PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
    private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

    #region BaseUnity

    protected override void Awake()
    {
        base.Awake();
        AddAnimationClips();
    }

    protected virtual void Start()
    {
        this.readyShoot = WeaponBase.Instance.readyShoot;
        Instance = this;
    }

    protected virtual void Update()
    {
#if UNITY_EDITOR
      GizmodCaculatorPointShoot();
#endif
        if (GameManager.Instance.GetGameState() != GameConstants.GameState.Playing)
        {
            StopGunEffect();
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            readyShoot = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            readyShoot = false;
        }
    }

    #endregion
    
    
    protected virtual void OnInit()
    {
        
    }
    
    /// <summary>
    /// Kiểm tra xem con trỏ chuột có đang hover trên UI element không
    /// </summary>
    /// <returns>True nếu pointer đang trên UI, false nếu không</returns>
    private static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Cập nhật vị trí con trỏ hiện tại
        PointerEventData.position = Input.mousePosition;
        RaycastResults.Clear();

        // Thực hiện raycast để kiểm tra UI elements
        EventSystem.current.RaycastAll(PointerEventData, RaycastResults);
        return RaycastResults.Count > 0;
    }

    /// <summary>
    /// Add animation clips to the weapon
    /// </summary>
    protected virtual void AddAnimationClips()
    {
        
    }
    
    /// <summary>
    /// Check Layer target  
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_layerIndex"></param>
    /// <returns></returns>
    protected bool IsInLayerIndex(GameObject _obj,int _layerIndex)
    {
        return ((1 << _obj.layer) & LayerMasks[_layerIndex]) != 0;
    }
    
    protected int CombinedLayerMask() => LayerMasks.Aggregate(0, (mask, lm) => mask | lm.value);
    
    // Thêm phương thức dừng âm thanh bắn
    protected void StopShootingSound()
    {
        if (_audioSource.isPlaying && _audioSource.clip == weaponInfo.audioClip)
            _audioSource.Stop();
    }
    
    public Vector3 GizmodCaculatorPointShoot()
    {
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * _distanceGizMod, Color.red);

        // Bắn raycast
        if (Physics.Raycast(ray, out hit, _distanceGizMod, _layerTarget))
        {
            // Debug.Log("Va chạm tại vị trí: " + hit.point);
            // Debug.Log("Khoảng cách đến box: " + hit.distance);

            Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.1f, Color.green);
            return hit.point;
        }

        return Vector3.one;
    }
    
    #region Play - Stop Gun Effect
    protected void PlayGunEffect()
    {
        foreach (ParticleSystem fireEffect in _fireEffect)
            if (fireEffect != null && !fireEffect.isPlaying)
                fireEffect.Play();
    }

    public void StopGunEffect()
    {
        readyShoot = false;
        
        foreach (ParticleSystem fireEffect in _fireEffect)
            if (fireEffect != null && fireEffect.isPlaying)
                fireEffect.Stop();
    }
    #endregion
}
