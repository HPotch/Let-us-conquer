using UnityEngine;

public class Elevators : MonoBehaviour
{
    public float Velocity = 0f;
    
    [SerializeField] private Elevator leftElevator;
    [SerializeField] private Elevator rightElevator;
    [SerializeField] private Vector2 heightRange;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float minVelocity = 0.01f;
    [SerializeField] private float accelerationForce = 0.1f;
    [SerializeField] private float normalDrag = 0.1f;
    [SerializeField] private float emptyElevatorDragModifier = 2;

    private float _currentHeight = 0.5f;
    private float previousVelocity = 0f;

    private void Update()
    {
        
        // Weightdiff is the difference in weight between the two elevators
        // AccelerationForce is how much the weightDiff impacts the acceleration
        // As long as there is a weightdiff, the velocity increases


        float weightDiff = rightElevator.Weight - leftElevator.Weight;
        
        float acceleration = weightDiff * accelerationForce;

        float drag = weightDiff == 0f ? normalDrag * emptyElevatorDragModifier : normalDrag;

        float targetVelocity = Velocity + acceleration * Time.deltaTime - drag * Velocity * Velocity;        
        

        if (_currentHeight <= 0f) {
            _currentHeight = 0f;
            Velocity = 0f;
        } else if (_currentHeight >= 1f) {
            _currentHeight = 1f;
            Velocity = 0f;
        }


        // Drag = how quickly we lerp between the velocity and target velocity
        Velocity = Mathf.Lerp(Velocity, targetVelocity, Mathf.Max(1f - normalDrag * Time.deltaTime, 0f));
        Velocity = Mathf.Clamp(Velocity, -maxVelocity, maxVelocity);

        // snap the elevator to 0 velocity when it is basically 0
        if (-minVelocity <= Velocity && Velocity <= minVelocity && weightDiff == 0) Velocity = 0f;
        


        _currentHeight += Velocity * Time.deltaTime;

        _currentHeight = Mathf.Clamp01(_currentHeight);
        
        leftElevator.transform.position = new Vector3(
            leftElevator.transform.position.x,
            Mathf.Lerp(heightRange.x, heightRange.y, _currentHeight),
            leftElevator.transform.position.z);
        rightElevator.transform.position = new Vector3(
            rightElevator.transform.position.x,
            Mathf.Lerp(heightRange.x, heightRange.y, 1f - _currentHeight),
            rightElevator.transform.position.z);

        previousVelocity = Velocity;
    }

    public float GetVelocity()
    {
        return Velocity * (heightRange.y - heightRange.x);
    }
}
