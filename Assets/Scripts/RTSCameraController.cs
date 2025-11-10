using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController Instance;
 
    // If we want to select an item to follow, inside the item script add:
    // public void OnMouseDown(){
    //   CameraController.instance.followTransform = transform;
    // }
 
    [Header("General")]
    [SerializeField] private Transform cameraTransform;
    public Transform followTransform;
    private Vector3 _newPosition;
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    
    [Header("Optional Functionality")]
    [SerializeField] private bool moveWithKeyboard;
    [SerializeField] private bool moveWithEdgeScrolling;
    [SerializeField] private bool moveWithMouseDrag;
 
    [Header("Keyboard Movement")]
    [SerializeField] private float fastSpeed = 0.05f;
    [SerializeField] private float normalSpeed = 0.01f;
    [SerializeField] private float movementSensitivity = 1f; // Hardcoded Sensitivity
    private float _movementSpeed;
 
    [Header("Edge Scrolling Movement")]
    [SerializeField] private float edgeSize = 50f;
    private bool _isCursorSet = false;
    public Texture2D cursorArrowUp;
    public Texture2D cursorArrowDown;
    public Texture2D cursorArrowLeft;
    public Texture2D cursorArrowRight;

    private CursorArrow _currentCursor = CursorArrow.Default;

    private enum CursorArrow
    {
        Up,
        Down,
        Left,
        Right,
        Default
    }
 
    private void Awake()
    {
        Instance = this;
 
        _newPosition = transform.position;
 
        _movementSpeed = normalSpeed;
    }
 
    private void Update()
    {
        // Allow Camera to follow Target
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        // Let us control Camera
        else
        {
            HandleCameraMovement();
        }
 
        if (Keyboard.current.escapeKey.isPressed)
        {
            followTransform = null;
        }
    }

    private void HandleCameraMovement()
    {
        // Mouse Drag
        if (moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }
 
        // Keyboard Control
        if (moveWithKeyboard)
        {
            _movementSpeed = Keyboard.current.shiftKey.isPressed ? fastSpeed : normalSpeed;
 
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                _newPosition += (transform.forward * _movementSpeed);
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                _newPosition += (transform.forward * -_movementSpeed);
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                _newPosition += (transform.right * _movementSpeed);
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                _newPosition += (transform.right * -_movementSpeed);
            }
        }
 
        // Edge Scrolling
        if (moveWithEdgeScrolling)
        {
 
            // Move Right
            if (Mouse.current.position.ReadValue().x > Screen.width - edgeSize)
            {
                _newPosition += (transform.right * _movementSpeed);
                ChangeCursor(CursorArrow.Right);
                _isCursorSet = true;
            }
            // Move Left
            else if (Mouse.current.position.ReadValue().x < edgeSize)
            {
                _newPosition += (transform.right * -_movementSpeed);
                ChangeCursor(CursorArrow.Left);
                _isCursorSet = true;
            }
            // Move Up
            else if (Mouse.current.position.ReadValue().y > Screen.height - edgeSize)
            {
                _newPosition += (transform.forward * _movementSpeed);
                ChangeCursor(CursorArrow.Up);
                _isCursorSet = true;
            }
            // Move Down
            else if (Mouse.current.position.ReadValue().y < edgeSize)
            {
                _newPosition += (transform.forward * -_movementSpeed);
                ChangeCursor(CursorArrow.Down);
                _isCursorSet = true;
            }
            else
            {
                if (_isCursorSet)
                {
                    ChangeCursor(CursorArrow.Default);
                    _isCursorSet = false;
                }
            }
        }
 
        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementSensitivity);
 
        Cursor.lockState = CursorLockMode.Confined; // If we have an extra monitor we don't want to exit screen bounds
    }
 
    private void ChangeCursor(CursorArrow newCursor)
    {
        // Only change cursor if it's not the same cursor
        if (_currentCursor == newCursor) return;
        switch (newCursor)
        {
            case CursorArrow.Up:
                Cursor.SetCursor(cursorArrowUp, Vector2.zero, CursorMode.Auto);
                break;
            case CursorArrow.Down:
                Cursor.SetCursor(cursorArrowDown, new Vector2(cursorArrowDown.width, cursorArrowDown.height), CursorMode.Auto); // So the Cursor will stay inside view
                break;
            case CursorArrow.Left:
                Cursor.SetCursor(cursorArrowLeft, Vector2.zero, CursorMode.Auto);
                break;
            case CursorArrow.Right:
                Cursor.SetCursor(cursorArrowRight, new Vector2(cursorArrowRight.width, cursorArrowRight.height), CursorMode.Auto); // So the Cursor will stay inside view
                break;
            case CursorArrow.Default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
 
        _currentCursor = newCursor;
    }
 
 
 
    private void HandleMouseDragInput()
    {
        if (Camera.main == null) return;
        
        if (Mouse.current.middleButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (plane.Raycast(ray, out var entry))
            {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Mouse.current.middleButton.isPressed && !EventSystem.current.IsPointerOverGameObject())
        {
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (plane.Raycast(ray, out var entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);
 
                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }
}