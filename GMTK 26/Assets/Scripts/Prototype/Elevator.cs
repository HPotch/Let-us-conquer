using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float Weight;
    
    private Elevators _elevators;
    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();

    private void Awake()
    {
        _elevators = transform.parent.GetComponent<Elevators>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_rigidbodies.Contains(other.attachedRigidbody)) return;
        
        Weight += TryGetWeight(other.gameObject);
        _rigidbodies.Add(other.attachedRigidbody);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!_rigidbodies.Contains(other.attachedRigidbody)) return;
        
        Weight -= TryGetWeight(other.gameObject);
        _rigidbodies.Remove(other.attachedRigidbody);
    }

    private float TryGetWeight(GameObject go)
    {
        return go.TryGetComponent<WeightObject>(out WeightObject weightObject) ? weightObject.Weight : 0f;
    }

    private void Update()
    {
        if (_elevators.GetVelocity() == 0f) return;
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, _elevators.GetVelocity(), rb.linearVelocity.z);
        }
    }
}
