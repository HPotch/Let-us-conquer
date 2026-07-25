using TMPro;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float Weight = 3f;
    public int targetFloor = 0;
    
    [SerializeField] private Material correctFloor;
    [SerializeField] private Material incorrectFloor;
    [SerializeField] private TextMeshPro targetFloorText;
    
    private MeshRenderer _meshRenderer;
    private int _currentFloor = 0;
    private Rigidbody rb;

    private void Awake()
    {
        transform.localScale = Vector3.one * Mathf.Pow(Weight, 0.5f) / 3f;
        _meshRenderer = GetComponent<MeshRenderer>();
        targetFloorText.text = targetFloor.ToString();
        rb =  GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _currentFloor = 0;
        foreach (Transform levelCheck in GameManager.Instance.LevelCheckers)
        {
            if (transform.position.y > levelCheck.position.y) _currentFloor++;
        }
        
        _meshRenderer.material = _currentFloor == targetFloor ? correctFloor : incorrectFloor;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            0f);
    }

    public void SetMoving(bool moving)
    {
        rb.isKinematic = !moving;
        rb.freezeRotation = !moving;
    }
}
