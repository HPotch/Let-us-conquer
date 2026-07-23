using UnityEngine;

public class WeightObject : MonoBehaviour
{
    public float Weight = 3f;

    private void Start()
    {
        transform.localScale *= Weight;
    }
}
