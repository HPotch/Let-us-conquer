using UnityEngine;

public class HotelGenerator : MonoBehaviour
{
    [SerializeField] private bool generate = false;
    
    [Header("Variables")]
    [SerializeField] private int numberOfFloors;
    [SerializeField] private float floorHeight;
    [SerializeField] private float width;
    [SerializeField] private float depth;
    [SerializeField] private float floorWallThickness;
    
    [Header("References")]
    [SerializeField] private Transform ground;
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform centerWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Transform floors;
    [SerializeField] private Transform roof;
    
    private void Awake()
    {
        Generate();
    }

    private void Update()
    {
        if (generate)
        {
            Generate();
            //generate = false;
        }
    }

    private void Generate()
    {
        // Reset
        foreach (Transform floor in floors.transform)
        {
            Destroy(floor.gameObject);
        }
        
        // Set ground and walls
        ground.localScale = new Vector3(width, floorWallThickness, depth);
        float hotelHeight = (float)(numberOfFloors + 1) * (floorHeight + floorWallThickness);
        Vector3 wallScale = new Vector3(floorWallThickness, hotelHeight, depth);
        leftWall.localScale = wallScale;
        leftWall.localPosition = new Vector3(-width / 2, 0, 0);
        rightWall.localScale = wallScale;
        rightWall.localPosition = new Vector3(width / 2, 0, 0);
        centerWall.localScale = wallScale;
        roof.localScale = new Vector3(width, floorWallThickness, depth);
        roof.localPosition = new Vector3(0,  (floorWallThickness + floorHeight) * (numberOfFloors + 1), 0);
        
        // Create floors
        for (int i = 0; i < numberOfFloors; i++)
        {
            GameObject floor = Instantiate(floorPrefab, floors);
            floor.transform.position = new Vector3(0, (floorWallThickness + floorHeight) * (i + 1), 0);
            floor.transform.localScale = new Vector3(width, floorWallThickness, depth);
        }
    }
}
