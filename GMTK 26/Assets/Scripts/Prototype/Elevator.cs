using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Elevator : MonoBehaviour
{
    public float Weight;
    public List<GameObject> EnteredObjects = new List<GameObject>();

    [SerializeField] private AnimationCurve enterCurve;
    [SerializeField] private float enterTime = 1f;
    [SerializeField] private float enterMultiplier = 0.1f;
    [SerializeField] private Transform visual;
    
    private Elevators _elevators;
    private Dictionary<Rigidbody, Vector3> _bodies = new Dictionary<Rigidbody, Vector3>();
    private GameObject _mightEnter;
    private List<GameObject> _ignore = new List<GameObject>();

    private void Awake()
    {
        _elevators = transform.parent.GetComponent<Elevators>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!EnteredObjects.Contains(other.gameObject)) EnteredObjects.Add(other.gameObject);
        if (other.isTrigger) return;
        if (_bodies.ContainsKey(other.attachedRigidbody) || _mightEnter == other.gameObject) return;
        StartCoroutine(AddWeightRoutine(other));
    }

    private IEnumerator AddWeightRoutine(Collider other)
    {
        _mightEnter = other.gameObject;
        yield return new WaitUntil(() => RayManager.Instance.PickedUp != other.gameObject);

        if (EnteredObjects.Contains(other.gameObject) && !_ignore.Contains(other.gameObject))
        {
            Weight += TryGetWeight(other.gameObject);
            Vector3 difference = other.attachedRigidbody.position - transform.position;
            difference.y = 3.6f;
            _bodies.Add(other.attachedRigidbody, difference);
            if (other.TryGetComponent<Person>(out var person))
                person.SetMoving(false);
            StartCoroutine(EnterAnimation());
        }

        _mightEnter = null;
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
        if (EnteredObjects.Contains(other.gameObject)) EnteredObjects.Remove(other.gameObject);
        if (other.isTrigger) return;
        if (!_bodies.ContainsKey(other.attachedRigidbody)) return;
        
        Weight -= TryGetWeight(other.gameObject);
        _bodies.Remove(other.attachedRigidbody);
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
            if (RayManager.Instance.PickedUp == rb.Key.gameObject || _ignore.Contains(rb.Key.gameObject))  continue;
            rb.Key.position = Vector3.Lerp(rb.Key.position, transform.position + rb.Value, 20f * Time.deltaTime);
        }
    }

    public void Eject(GameObject go)
    {
        EnteredObjects.Remove(go);
        Weight -= TryGetWeight(go);
        _bodies.Remove(go.GetComponent<Rigidbody>());
        StartCoroutine(IgnoreRoutine(go));
    }

    private IEnumerator IgnoreRoutine(GameObject go)
    {
        _ignore.Add(go);
        yield return new WaitForSeconds(0.3f);
        _ignore.Remove(go);
    }
}
