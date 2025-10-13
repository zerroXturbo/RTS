using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    
    private Camera _cam;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _cam = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.rightButton.isPressed) return;
        
        var ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
        {
            _agent.SetDestination(hit.point);
        }

    }
}
