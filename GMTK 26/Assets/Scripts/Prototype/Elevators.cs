using TMPro;
using UnityEngine;

public class Elevators : MonoBehaviour
{
    [SerializeField] private Elevator leftElevator;
    [SerializeField] private Elevator rightElevator;

    [SerializeField] TextMeshProUGUI weightTextLeftElevator;
    [SerializeField] TextMeshProUGUI weightTextRightElevator;


    [SerializeField] private Vector2 heightRange;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float accelerationForce = 0.1f;
    [SerializeField] private float drag = 0.1f;
    
    private float _currentHeight = 0.5f;
    private float _velocity = 0f;
    
    private void Update()
    {
        float weightDiff = rightElevator.Weight - leftElevator.Weight;
        float acceleration = weightDiff * accelerationForce;
        _velocity += acceleration * Time.deltaTime;
        _velocity = Mathf.Clamp(_velocity, -maxVelocity, maxVelocity);
        _currentHeight += _velocity * Time.deltaTime;

        if (_currentHeight <= 0f) {
            _currentHeight = 0f;
            _velocity = 0f;
        } else if (_currentHeight >= 1f) {
            _currentHeight = 1f;
            _velocity = 0f;
        }

        _velocity *= Mathf.Max(1f - drag * Time.deltaTime, 0f);
        
        _currentHeight = Mathf.Clamp01(_currentHeight);
        
        leftElevator.transform.position = new Vector3(
            leftElevator.transform.position.x,
            Mathf.Lerp(heightRange.x, heightRange.y, _currentHeight),
            leftElevator.transform.position.z);
        rightElevator.transform.position = new Vector3(
            rightElevator.transform.position.x,
            Mathf.Lerp(heightRange.x, heightRange.y, 1f - _currentHeight),
            rightElevator.transform.position.z);


        weightTextLeftElevator.text = leftElevator.Weight.ToString();
        weightTextRightElevator.text = rightElevator.Weight.ToString();

    }

}
