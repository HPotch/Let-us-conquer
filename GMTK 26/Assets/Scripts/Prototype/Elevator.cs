using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float Weight;
  

    private void OnTriggerEnter(Collider other)
    {
        Weight += TryGetWeight(other.gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        Weight -= TryGetWeight(other.gameObject);
    }

    private float TryGetWeight(GameObject go)
    {
        return go.TryGetComponent<WeightObject>(out WeightObject weightObject) ? weightObject.Weight : 0f;
    }
}
