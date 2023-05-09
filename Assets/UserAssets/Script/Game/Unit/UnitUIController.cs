using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitUIController : MonoBehaviour
{
    [SerializeField] private Image mFillImage;
    [SerializeField] private bool mOnUpdate;

    private Transform mStatCanvas;
    private Slider mSlider;

    private void Awake()
    {
        mStatCanvas = GetComponent<Transform>();
        mSlider = transform.GetChild(0).GetComponent<Slider>();
    }

    private void Start() => mStatCanvas.rotation = Quaternion.LookRotation(GameManager.mMyCameraTransform.position);

    public void Init(int maxValue, bool isMine)
    {
        gameObject.SetActive(true);
        mSlider.maxValue = maxValue;
        mSlider.value = maxValue;
        mFillImage.color = isMine ? Color.blue : Color.red;
    }

    public void GetDamage(int value)
    {
        mSlider.value = value;
    }

    private void Update()
    {
        if (!mOnUpdate) return;
        mStatCanvas.rotation = Quaternion.LookRotation(GameManager.mMyCameraTransform.position);
    }
}
