using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionMovement : MonoBehaviour
{
    public static UnitSelectionMovement Instance;
    
    [SerializeField] public List<GameObject> allUnitList = new List<GameObject>();
    [SerializeField] private List<GameObject> unitsSelected = new List<GameObject>();
    
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject groundMarker;
    
    private Camera _cam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        _cam = Camera.main;        
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            var ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickableLayer))
            {
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    MultiSelection(hit.collider.gameObject);
                }
                else
                {
                    SelectedByClicking(hit.collider.gameObject);
                }                
            }
            else
            {
                if (!Keyboard.current.leftShiftKey.isPressed)
                {
                    DeselectAll();
                }
            }
        }

        if (Mouse.current != null && Mouse.current.rightButton.isPressed && unitsSelected.Count > 0)
        {
            var ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
            {
                groundMarker.transform.position = new Vector3(hit.point.x, groundMarker.transform.position.y, hit.point.z);
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }
    }

    private void MultiSelection(GameObject unit)
    {
        if (unitsSelected.Contains(unit))
        {
            EnableUnitMovement(unit, false);
            unitsSelected.Remove(unit);
        }
        else
        {
            unitsSelected.Add(unit);
            EnableUnitMovement(unit, true);
        }
    }

    private void SelectedByClicking(GameObject unit)
    {
        DeselectAll();
        unitsSelected.Add(unit);
        EnableUnitMovement(unit, true);
    }

    private void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            EnableUnitMovement(unit, false);
        }
        unitsSelected.Clear();
        groundMarker.SetActive(false);
    }
    
    private void EnableUnitMovement(GameObject unit, bool isMove)
    {
        unit.GetComponent<Unit>().indicator.SetActive(isMove);
        unit.GetComponent<UnitMovement>().enabled = isMove;
    }
}
