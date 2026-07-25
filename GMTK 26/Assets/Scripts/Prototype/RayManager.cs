using UnityEngine;
using UnityEngine.InputSystem;

public class RayManager : MonoBehaviour
{
    public static RayManager Instance;
    
    public GameObject PickedUp;
    
    [SerializeField] private LayerMask pickUpMask, collideMask;
    [SerializeField] private Elevator leftElevator, rightElevator;
    [SerializeField] private int disabledLayer;
    [SerializeField] private float throwAmount = 0.1f;
    [SerializeField] private float throwOutForce = 4f;
    
    private GameObject _mouseOver;
    private Vector3 _startPosition;
    private float _distance;
    private Vector3 _prevPosition;
    private Camera _cam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _cam = Camera.main;
    }

    private void Start()
    {
        PickedUp = null;
    }
    
    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame) Release();
        
        CastRay();
        
        if (_mouseOver && Mouse.current.leftButton.wasPressedThisFrame) 
            PickUp(_mouseOver);
        
    }

    private void CastRay()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        //  Object picked up
        if (PickedUp)
        {
            _prevPosition = PickedUp.transform.position;
            Vector3 mousePos = ray.GetPoint(_distance);

            //  Define target
            Vector3 targetPosition = RopeSim.Instance.SimulateRope(mousePos);
            Vector3 direction = targetPosition - _startPosition; 
            Vector3 ropeDir = targetPosition - mousePos;
            
            int previousLayer = PickedUp.layer; // Prevent clipping
            PickedUp.layer = disabledLayer; // Prevent self collision
            if (direction.magnitude > 0.001f && Physics.Raycast(_startPosition, direction.normalized, out RaycastHit hit, direction.magnitude, collideMask))
                PickedUp.transform.position = hit.point; // Snap to the point
            else PickedUp.transform.position = targetPosition; // Target is clear 

            // Handle rotation
            float ropeAngle = Mathf.Atan2(ropeDir.y, ropeDir.x) * Mathf.Rad2Deg;
            PickedUp.transform.localEulerAngles = new Vector3(0f, 0f, ropeAngle + 90f);
            
            // Cleanly exit
            PickedUp.layer = previousLayer;
            return;
        }
        
        // No object picked up
        _mouseOver = Physics.Raycast(ray, out RaycastHit hitSelect, 500f, pickUpMask) ? hitSelect.collider.gameObject : null;
    }

    private void PickUp(GameObject go)
    {
        go.TryGetComponent<Person>(out var person);
        
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            if (leftElevator.EnteredObjects.Contains(go))
            {
                if (person) person.SetMoving(true);
                rb.linearVelocity = new Vector3(-throwOutForce, 0, 0);
                leftElevator.Eject(go);
                return;
            }
            if (rightElevator.EnteredObjects.Contains(go))
            {
                if (person) person.SetMoving(true);
                rb.linearVelocity = new Vector3(throwOutForce, 0, 0);
                rightElevator.Eject(go);
                return;
            }
        }
        
        if (person) person.SetMoving(false);

        PickedUp = go;
        _startPosition = go.transform.position;
        _distance = (go.transform.position - _cam.transform.position).magnitude;
        
        // Setup positions for the rope
        Vector3 mousePos3D = _cam.ScreenPointToRay(Mouse.current.position.ReadValue()).GetPoint(_distance);
        Vector2 mousePos2D = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 objPos2D = new Vector2(go.transform.position.x, go.transform.position.y);
        
        RopeSim.Instance.StartSimulation(mousePos2D, objPos2D);
    }

    private void Release()
    {
        if (!PickedUp) return;
        if (PickedUp.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 throwForce = PickedUp.transform.position - _prevPosition;
            if (PickedUp.TryGetComponent<Person>(out var person))
                person.SetMoving(true);
            rb.AddForce((new Vector3(throwForce.x, throwForce.y, 0f) / Time.deltaTime)* throwAmount, ForceMode.Impulse);
        }
        
        PickedUp = null;
    }
}