using System;
using UnityEngine;

public class WeightObject : MonoBehaviour
{
    public float Weight = 3f;

    private void Awake()
    {
        transform.localScale = Vector3.one * Mathf.Pow(Weight, 0.5f) / 3f;
    }
}
