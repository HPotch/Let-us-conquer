using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RopeSim : MonoBehaviour
{
    public static RopeSim Instance;

    public class Point
    {
        public Vector2 position, prevPosition;
        public bool locked = false;
    }

    public class Stick
    {
        public Point p1, p2;
        public float length;
    }
    
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private int iterations = 5; // Increased slightly for a stiffer rope
    [SerializeField] private float _ropeLength = 1f;
    [SerializeField] private float holdDistance = 0.5f;

    private List<Point> _points = new List<Point>();
    private List<Stick> _sticks = new List<Stick>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Initialize with real world positions
    public void StartSimulation(Vector2 anchorPos, Vector2 tailPos)
    {
        _points.Clear();
        _sticks.Clear();

        _points.Add(new Point { locked = true, position = anchorPos, prevPosition = anchorPos }); // Mouse point
        _points.Add(new Point { locked = false, position = tailPos, prevPosition = tailPos }); // Hanging object

        _sticks.Add(new Stick { p1 = _points[0], p2 = _points[1], length = _ropeLength });
    }

    // Pass the mouse position, return where the object should be
    public Vector2 SimulateRope(Vector2 newAnchorPos)
    {
        _points[0].position = newAnchorPos;
        Simulate();
        return newAnchorPos + ((_points[1].position - newAnchorPos) / _ropeLength) * holdDistance; 
    }

    // Used if the object hits a wall, so the physics don't clip through it
    public void SyncTailPosition(Vector2 collisionPos)
    {
        _points[1].position = collisionPos;
    }

    private void Simulate()
    {
        // Simulate points
        foreach (var p in _points)
        {
            if (p.locked) continue;
            Vector2 posBeforeUpdate = p.position;
            p.position += p.position - p.prevPosition;
            p.position += Vector2.down * (gravity * Time.deltaTime * Time.deltaTime);
            p.prevPosition = posBeforeUpdate;
        }

        // Simulate sticks
        for (int i = 0; i < iterations; i++)
        {
            foreach (var s in _sticks)
            {
                Vector2 centre = (s.p1.position + s.p2.position) / 2f;
                Vector2 dir = (s.p1.position - s.p2.position).normalized;
                
                if (!s.p1.locked) s.p1.position = centre + dir * s.length / 2;
                if (!s.p2.locked) s.p2.position = centre - dir * s.length / 2;
            }
        }
    }
}