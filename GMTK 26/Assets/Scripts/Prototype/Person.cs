using System.Collections;
using TMPro;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float Weight = 3f;
    public int targetFloor = 0;

    [SerializeField] private Material correctFloor;
    [SerializeField] private Material incorrectFloor;
    [SerializeField] private TextMeshPro targetFloorText;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private Vector2 waitRange = new Vector2(1.5f, 5f);
    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private AnimationCurve hoverCurve;
    [SerializeField] private float hoverTime = 0.8f;
    [SerializeField] private float hoverAmount = 0.1f;
    [SerializeField] private Transform mesh;
    [SerializeField] private GameObject small;
    [SerializeField] private GameObject medium;
    [SerializeField] private GameObject large;

    private int _currentFloor = 0;
    private Rigidbody rb;

    private int _walkDirection = 0;
    private float _waitTime;

    private GameObject _previousHover;

    private void Awake()
    {
        transform.localScale = (Vector3.one * Mathf.Pow(Weight, 0.5f) / 3f) * scaleFactor;
        targetFloorText.text = targetFloor.ToString();
        rb = GetComponent<Rigidbody>();
        NewTarget();

        GameObject meshInstance = Instantiate(Weight < 5f ? small : Weight < 10f ? medium : large, transform);
        mesh = meshInstance.transform;
    }

    private void Update()
    {
        _currentFloor = -1;
        foreach (Transform levelCheck in GameManager.Instance.LevelCheckers)
        {
            if (transform.position.y > levelCheck.position.y) _currentFloor++;
        }
        targetFloorText.color = _currentFloor == targetFloor ? Color.green : Color.black;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        GameObject currentHover = RayManager.Instance.MouseOver;
        if (currentHover != _previousHover && currentHover == gameObject) StartCoroutine(HoverRoutine());
        _previousHover = currentHover;
        if (GameManager.Instance.LeftElevator.EnteredObjects.Contains(gameObject) ||
            GameManager.Instance.RightElevator.EnteredObjects.Contains(gameObject) ||
            RayManager.Instance.PickedUp == gameObject)
            NewTarget(false);
        else
            Walk();

        mesh.transform.localEulerAngles = new Vector3(0f,
            Mathf.LerpAngle(mesh.transform.eulerAngles.y,
            -(float)_walkDirection * 90f, Time.deltaTime * 10f),
            0f);
    }

    private IEnumerator HoverRoutine()
    {
        float timer = 0f;
        while (timer < hoverTime)
        {
            timer +=  Time.deltaTime;
            float scale = 1f + hoverCurve.Evaluate(timer /  hoverTime) * hoverAmount;
            mesh.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }


private void Walk()
    {
        _waitTime -= Time.deltaTime;
        rb.position += new Vector3(_walkDirection * walkSpeed * Time.deltaTime, 0f, 0f);
        if (_walkDirection != 0f)
            rb.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(rb.rotation.eulerAngles.z, 0f, Time.deltaTime * 20f));
        if (_waitTime <= 0f) NewTarget();
    }

    private void NewTarget(bool walkAllowed = true)
    {
        _waitTime = Random.Range(waitRange.x, waitRange.y);
        _walkDirection = walkAllowed? Random.Range(-1, 2) : 0;
    }
    
    public void SetMoving(bool moving)
    {
        rb.isKinematic = !moving;
        rb.freezeRotation = !moving;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
    }
}
