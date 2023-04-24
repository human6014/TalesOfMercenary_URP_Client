using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [Tooltip("제어할 유닛 + 움직일 위치로 감지할 레이어")]
    [SerializeField] private LayerMask player1Layer;
    [SerializeField] private LayerMask player2Layer;
    
    private Unit clickedUnit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, player1Layer))
            {
                if (hit.collider.TryGetComponent(out Unit currentUnit))
                {
                    clickedUnit = currentUnit;
                    currentUnit.IsClicked = true;
                }
                else if (clickedUnit != null)
                {
                    clickedUnit.PointMove(hit.point);
                    clickedUnit.IsClicked = false;
                    clickedUnit = null;
                }
            }
            //else clickedUnit.IsClicked = false;
        }
    }
}
