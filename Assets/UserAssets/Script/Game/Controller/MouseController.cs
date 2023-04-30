using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MouseController : MonoBehaviour
{
    public static Action<bool> ClickAction { get; set; }
    private Unit clickedUnit;
    [SerializeField] private LayerMask unitLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer))
            {
                if (hit.collider.TryGetComponent(out Unit currentUnit))
                {
                    clickedUnit = currentUnit;
                    currentUnit.IsClicked = true;

                    ClickAction?.Invoke(true);
                }
                else if (clickedUnit != null)
                {
                    clickedUnit.PointMove(hit.point);
                    clickedUnit.IsClicked = false;
                    clickedUnit = null;

                    ClickAction?.Invoke(false);
                }
                else ClickAction?.Invoke(false);
            }
            else ClickAction?.Invoke(false);
            //else clickedUnit.IsClicked = false;
        }
    }
}
