using UnityEngine;
using UnityEngine.InputSystem;

public class RayManager : MonoBehaviour
{
    // Manager
    public static RayManager Instance;
    
    // Settings
    [SerializeField] private LayerMask pickUpMask;
    [SerializeField] private LayerMask collideMask;
    [SerializeField] private int disabledLayer;
    
    // Private
    private GameObject _mouseOver;
    private GameObject _pickedUp;
    private Vector3 _startPosition;
    private float _distance;
    
    // Static references
    private Camera _cam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _cam = Camera.main;
    }
    
    private void Update()
    {
        // Release object
        if (Mouse.current.leftButton.wasReleasedThisFrame) Release();
        CastRay();
        // Pick up object
        if (_mouseOver && Mouse.current.leftButton.wasPressedThisFrame) PickUp(_mouseOver);
    }

    private void CastRay()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        // Object picked up
        if (_pickedUp)
        {
            // Calculate target position, retaining the original Z axis
            Vector3 mousePosition = ray.GetPoint(_distance);
            Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, _startPosition.z);

            // Ray direction
            Vector3 direction = targetPosition - _startPosition;

            // Prevent self-collision
            int _previousLayer = _pickedUp.layer;
            _pickedUp.gameObject.layer = disabledLayer;

            // Send ray
            if (direction.magnitude > 0.001f && Physics.Raycast(_startPosition, direction.normalized, out RaycastHit hit, direction.magnitude, collideMask))
                // Snap to the point
                _pickedUp.transform.position = hit.point;
            else _pickedUp.transform.position = targetPosition; // Target is clear
            
            // Reset
            _pickedUp.gameObject.layer = _previousLayer;
            return;
        }
        
        // No object picked up
        _mouseOver = Physics.Raycast(ray, out RaycastHit hitSelect, 500f, pickUpMask) ? hitSelect.collider.gameObject : null;
    }

    private void PickUp(GameObject go)
    {
        _pickedUp = go;
        _startPosition = go.transform.position;
        _distance = (_startPosition - _cam.transform.position).magnitude;
        
        go.TryGetComponent<Rigidbody>(out var rb);
        rb.freezeRotation = true;
    }

    private void Release()
    {
        if (!_pickedUp.TryGetComponent<Rigidbody>(out var rb)) return;
        rb.freezeRotation = false;
        _pickedUp = null;
    }
}