using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionBox : MonoBehaviour
{
    private Camera _myCam;
 
    [SerializeField] private RectTransform boxVisual;

    private Rect _selectionBox;

    private Vector2 _startPosition;
    private Vector2 _endPosition;
 
    private void Start()
    {
        _myCam = Camera.main;
        _startPosition = Vector2.zero;
        _endPosition = Vector2.zero;
        DrawVisual();
    }
 
    private void Update()
    {
        // When Clicked
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _startPosition = Mouse.current.position.ReadValue();
 
            // For selection the Units
            _selectionBox = new Rect();
        }
 
        // When Dragging
        if (Mouse.current.leftButton.isPressed)
        {
            if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
            {
                UnitSelectionManager.Instance.DeselectAll();
                SelectUnits();
            }
            
            _endPosition = Mouse.current.position.ReadValue();
            DrawVisual();
            DrawSelection();
        }
 
        // When Releasing
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;
            DrawVisual();
        }
    }

    private void DrawVisual()
    {
        // Calculate the starting and ending positions of the selection box.
        var boxStart = _startPosition;
        var boxEnd = _endPosition;
 
        // Calculate the center of the selection box.
        var boxCenter = (boxStart + boxEnd) / 2;
 
        // Set the position of the visual selection box based on its center.
        boxVisual.position = boxCenter;
 
        // Calculate the size of the selection box in both width and height.
        var boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
 
        // Set the size of the visual selection box based on its calculated size.
        boxVisual.sizeDelta = boxSize;
    }

    private void DrawSelection()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        
        if (mousePosition.x < _startPosition.x)
        {
            _selectionBox.xMin = mousePosition.x;
            _selectionBox.xMax = _startPosition.x;
        }
        else
        {
            _selectionBox.xMin = _startPosition.x;
            _selectionBox.xMax = mousePosition.x;
        }
 
 
        if (mousePosition.y < _startPosition.y)
        {
            _selectionBox.yMin = mousePosition.y;
            _selectionBox.yMax = _startPosition.y;
        }
        else
        {
            _selectionBox.yMin = _startPosition.y;
            _selectionBox.yMax = mousePosition.y;
        }
    }

    private void SelectUnits()
    {
        foreach (var unit in UnitSelectionManager.Instance.allUnitList)
        {
            if (_selectionBox.Contains(_myCam.WorldToScreenPoint(unit.transform.position)))
            {
                UnitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}