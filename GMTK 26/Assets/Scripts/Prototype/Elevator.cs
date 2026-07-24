using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float Weight;

    [SerializeField] private AnimationCurve enterCurve;
    [SerializeField] private float enterTime = 1f;
    [SerializeField] private float enterMultiplier = 0.1f;
    [SerializeField] private Transform visual;
    
    private Elevators _elevators;
    private Dictionary<Rigidbody, Vector3> _bodies = new Dictionary<Rigidbody, Vector3>();
    private List<GameObject> _enteredObjects = new List<GameObject>();

    private void Awake()
    {
        _elevators = transform.parent.GetComponent<Elevators>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_enteredObjects.Contains(other.gameObject)) _enteredObjects.Add(other.gameObject);
        if (other.isTrigger) return;
        if (_bodies.ContainsKey(other.attachedRigidbody)) return;
        StartCoroutine(AddWeightRoutine(other));
    }

    private IEnumerator AddWeightRoutine(Collider other)
    {
        yield return new WaitUntil(() => RayManager.Instance.PickedUp != other.gameObject);

        if (_enteredObjects.Contains(other.gameObject))
        {
            Weight += TryGetWeight(other.gameObject);
            Vector3 difference = other.attachedRigidbody.position - transform.position;
            difference.y = 3.6f;
            _bodies.Add(other.attachedRigidbody, difference);
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.freezeRotation = true;
            StartCoroutine(EnterAnimation());
        }
    }

    private IEnumerator EnterAnimation()
    {
        float timer = 0f;
        while (timer < enterTime)
        {
            timer += Time.deltaTime;
            visual.transform.localPosition = new Vector3(0f, enterCurve.Evaluate(timer / enterTime), 0f) * enterMultiplier;
            yield return null;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_enteredObjects.Contains(other.gameObject)) _enteredObjects.Remove(other.gameObject);
        if (other.isTrigger) return;
        if (!_bodies.ContainsKey(other.attachedRigidbody)) return;
        
        Weight -= TryGetWeight(other.gameObject);
        _bodies.Remove(other.attachedRigidbody);
        other.attachedRigidbody.useGravity = true;
        other.attachedRigidbody.freezeRotation = false;
    }

    private float TryGetWeight(GameObject go)
    {
        return go.TryGetComponent<Person>(out Person weightObject) ? weightObject.Weight : 0f;
    }

    private void Update()
    {
        if (_elevators.GetVelocity() == 0f) return;
        foreach (var rb in _bodies)
        {
            if (RayManager.Instance.PickedUp == rb.Key.gameObject)  continue;
            rb.Key.position = Vector3.Lerp(rb.Key.position, transform.position + rb.Value, 20f * Time.deltaTime);
        }
    }
}
