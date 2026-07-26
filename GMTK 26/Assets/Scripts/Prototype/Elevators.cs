using UnityEngine;

public class Elevators : MonoBehaviour
{
    public float Velocity = 0f;
    public Vector2 HeightRange;
    
    [SerializeField] private Elevator leftElevator;
    [SerializeField] private Elevator rightElevator;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float accelerationForce = 0.1f;
    [SerializeField] private float drag = 0.1f;
    
    private float _currentHeight = 0.5f;
    
    private void Update()
    {
        float weightDiff = rightElevator.Weight - leftElevator.Weight;
        float acceleration = weightDiff * accelerationForce;
        float targetVelocity = Velocity + acceleration * Time.deltaTime;
        _currentHeight += Velocity * Time.deltaTime;

        if (_currentHeight <= 0f) {
            _currentHeight = 0f;
            Velocity = 0f;
        } else if (_currentHeight >= 1f) {
            _currentHeight = 1f;
            Velocity = 0f;
        }

        Velocity = Mathf.Lerp(Velocity, targetVelocity, Mathf.Max(1f - drag * Time.deltaTime, 0f));
        Velocity = Mathf.Clamp(Velocity, -maxVelocity, maxVelocity);
        
        _currentHeight = Mathf.Clamp01(_currentHeight);
        
        leftElevator.transform.position = new Vector3(
            leftElevator.transform.position.x,
            Mathf.Lerp(HeightRange.x, HeightRange.y, _currentHeight),
            leftElevator.transform.position.z);
        rightElevator.transform.position = new Vector3(
            rightElevator.transform.position.x,
            Mathf.Lerp(HeightRange.x, HeightRange.y, 1f - _currentHeight),
            rightElevator.transform.position.z);
    }

    public float GetVelocity()
    {
        return Velocity * (HeightRange.y - HeightRange.x);
    }
}
