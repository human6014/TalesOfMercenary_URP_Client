using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitUIController : MonoBehaviour
{
    private Transform mStatCanvas;
    [SerializeField] private bool mOnUpdate;

    private void Awake() => mStatCanvas = GetComponent<Transform>();

    private void Start() => mStatCanvas.rotation = Quaternion.LookRotation(GameManager.mMyCameraTransform.position);

    private void Update()
    {
        if (!mOnUpdate) return;
        mStatCanvas.rotation = Quaternion.LookRotation(GameManager.mMyCameraTransform.position);
    }
    
}
