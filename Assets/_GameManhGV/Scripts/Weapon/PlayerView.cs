﻿using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Basic control")]
    [SerializeField] private Transform _mainRoot;
    [SerializeField] private Transform _head;
    [SerializeField] private float _sensitivity = 15f;
    [SerializeField] private float _slerpFactor = 12.5f;
    [SerializeField] private Vector2 _viewHorizontalThreshold = new Vector2(-60f, 60f);
    [SerializeField] private Vector2 _viewVerticalThreshold = new Vector2(-89f, 89f);
    [SerializeField] private Vector2 _initRotate;
    [SerializeField] private Vector2 _totalRotate;
    [SerializeField] private bool WeaponView = false; // Biến Bool để chọn logic
    [SerializeField] private Transform WeaponTrans; // Biến Transform cho vũ khí
    [SerializeField] private Transform[] ListWeaponTrans; // Biến Transform cho vũ khí
    [SerializeField] private RectTransform CrossHair; // Biến RectTransform cho CrossHair
    [SerializeField] private Vector2 _crossHairMovementLimit = new Vector2(100f, 100f); // Giới hạn phạm vi di chuyển của CrossHair
    [SerializeField] private Vector2 _weaponMovementLimit = new Vector2(30f, 30f); // Giới hạn phạm vi di chuyển của súng
    [SerializeField] private Vector2 screenPosValue; // Thay đổi từ float thành Vector2
    [SerializeField] private Transform CameraTrans; // Thêm biến Transform cho Camera
    [SerializeField] private float CrossHairPos; // Thêm biến Transform cho Camera
    [SerializeField] private bool IsLimitRotate = true; // Biến để kiểm tra có giới hạn phạm vi di chuyển hay không

    private Quaternion originalCameraRotation;
    private Vector3 vectorCam;
    private Vector2 _previousRotate;

    private void Awake()
    {
        SetDefaultView();
        originalCameraRotation = CameraTrans.localRotation;
    }

    public void SetDefaultView()
    {
        _totalRotate = _initRotate;
        _previousRotate = _totalRotate;
        _mainRoot.localRotation = Quaternion.Euler(0, _previousRotate.x, 0);
        _head.localRotation = Quaternion.Euler(-_previousRotate.y, 0, 0);
    }

    private void Start()
    {
        if (WeaponView)
        {
            CrossHair.anchoredPosition = new Vector2(5.66243e-05f, 43.61921f);
        }
        else
        {
            CrossHair.anchoredPosition = new Vector2(0, CrossHairPos);
        }
    }

    private void OnCheckCamShake(Vector3 vector3)
    {
        vectorCam = vector3;
    }

    private void OnEnable()
    {
        EventManager.AddListener<Vector3>(EventName.OnCheckShakeCam, OnCheckCamShake);

        EventManager.AddListener<bool>(EventName.OnChangeWeapon, OnChangeMachineGun);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<Vector3>(EventName.OnCheckShakeCam, OnCheckCamShake);
        EventManager.RemoveListener<bool>(EventName.OnChangeWeapon, OnChangeMachineGun);
    }

    public void Update()
    {
        if(GameManager.Instance.GetGameState() != GameConstants.GameState.Playing)
            return;
        
        if (Input.GetMouseButton(0))
        {
            var input = new Vector2 { x = Input.GetAxis("Mouse X"), y = Input.GetAxis("Mouse Y") };
            if (Mathf.Abs(input.x) > 1000)
                input.x = 0;
            if (Mathf.Abs(input.y) > 1000)
                input.y = 0;

            var totalRotate = _totalRotate;
            var rotate = input * (_sensitivity * Time.timeScale);
            var slerpParam = _slerpFactor * Time.deltaTime;
            totalRotate += rotate;

            if (IsLimitRotate)
            {
                totalRotate.x = Mathf.Clamp(totalRotate.x, _viewHorizontalThreshold.x, _viewHorizontalThreshold.y);
                totalRotate.y = Mathf.Clamp(totalRotate.y, _viewVerticalThreshold.x, _viewVerticalThreshold.y);
            }

            //if (!UIEndGame.Instance.IsShowEndGame)
            {
                if (WeaponView && WeaponTrans != null)
                {
                    if (IsLimitRotate)
                    {
                        totalRotate.x = Mathf.Clamp(totalRotate.x, -_weaponMovementLimit.x, _weaponMovementLimit.x);
                        totalRotate.y = Mathf.Clamp(totalRotate.y, -_weaponMovementLimit.y, _weaponMovementLimit.y);
                    }

                    WeaponTrans.localRotation = Quaternion.Slerp(WeaponTrans.localRotation,
                        Quaternion.Euler(-totalRotate.y, totalRotate.x, 0), slerpParam);
                }
                else
                {
                    _mainRoot.localRotation = Quaternion.Slerp(_mainRoot.localRotation,
                        Quaternion.Euler(0, totalRotate.x, 0), slerpParam);
                    _head.localRotation = Quaternion.Slerp(_head.localRotation,
                        Quaternion.Euler(-totalRotate.y, 0, 0), slerpParam);
                }
            }

            UpdateCrossHair(totalRotate, slerpParam);

            _totalRotate = totalRotate;
            _previousRotate = totalRotate;
        }
    }

    private void OnChangeMachineGun(bool IsChange)
    {
        if (IsChange)
        {
            WeaponTrans = ListWeaponTrans[1];
        }
        else
        {
            WeaponTrans = ListWeaponTrans[0];
        }
    }

    private void UpdateCrossHair(Vector2 totalRotate, float slerpParam)
    {
        if (CrossHair != null && CameraTrans != null && WeaponView)
        {
            Vector2 screenPos = new Vector2(
                totalRotate.x / _viewHorizontalThreshold.y,
                totalRotate.y / _viewVerticalThreshold.y
            );

            Vector2 adjustedScreenPos = new Vector2(
                screenPos.x * screenPosValue.x,
                screenPos.y * screenPosValue.y
            );

            if (IsLimitRotate)
            {
                adjustedScreenPos.x = Mathf.Clamp(adjustedScreenPos.x, -_crossHairMovementLimit.x, _crossHairMovementLimit.x);
                adjustedScreenPos.y = Mathf.Clamp(adjustedScreenPos.y, -_crossHairMovementLimit.y, _crossHairMovementLimit.y);
            }

            Vector3 cameraRotation = originalCameraRotation.eulerAngles;
            float cameraTiltX = Mathf.Sin(cameraRotation.x * Mathf.Deg2Rad);
            float cameraTiltY = Mathf.Sin(cameraRotation.y * Mathf.Deg2Rad);

            adjustedScreenPos.x += cameraTiltY * screenPosValue.x;
            adjustedScreenPos.y += cameraTiltX * screenPosValue.y;

            CrossHair.anchoredPosition = Vector2.Lerp(CrossHair.anchoredPosition, adjustedScreenPos, slerpParam);
        }
    }
}
