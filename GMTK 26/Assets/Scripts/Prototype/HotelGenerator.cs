using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class HotelGenerator : MonoBehaviour
{
    [SerializeField] private bool generate = false;
    [SerializeField] private bool clear = false;
    
    
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
    [SerializeField] private Transform levelCheckers;
    [SerializeField] private Transform roof;
    [SerializeField] private Elevators elevators;
    
    private void Update()
    {
        if (clear) Clear();
        if (generate)
        {
            Generate();
            generate = false;
        }
    }

    private void Generate()
    {
        generate = false;
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
        
        // Reset
        foreach (Transform floor in floors.transform) DestroyImmediate(floor.gameObject);
        foreach (Transform levelCheck in levelCheckers.transform) DestroyImmediate(levelCheck.gameObject);
        
        for (int i = 0; i < numberOfFloors + 1; i++)
        {
            // Create floors
            GameObject floor = Instantiate(floorPrefab, floors);
            floor.transform.position = new Vector3(0, (floorWallThickness + floorHeight) * i, 0);
            floor.transform.localScale = new Vector3(width, floorWallThickness, depth);
            
            // Create levelChecks
            levelCheckers.transform.position = new Vector3(0f, 0f, depth * -0.5f);
            GameObject levelCheck = new GameObject();
            levelCheck.name = (i + 1).ToString();
            levelCheck.transform.parent = levelCheckers.transform;
            
            // Text
            GameObject textObject = new GameObject();
            textObject.transform.parent = levelCheck.transform;
            textObject.AddComponent<TextMeshPro>();
            TextMeshPro text = textObject.GetComponent<TextMeshPro>();
            text.text = i.ToString();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 10f;
            text.color = Color.black;
            
            levelCheck.transform.localPosition = floor.transform.position;
        }
        
        elevators.HeightRange = new Vector2(-2.75f, ((floorWallThickness + floorHeight) * numberOfFloors) + (2f * floorWallThickness) - 2.75f);
    }

    private void Clear()
    {
        // Reset
        foreach (Transform floor in floors.transform) DestroyImmediate(floor.gameObject);
        foreach (Transform levelCheck in levelCheckers.transform) DestroyImmediate(levelCheck.gameObject);
    }
}
