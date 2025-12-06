using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RepositionCamera : MonoBehaviour
{
    private struct Target
    {
        public Transform Waypoint;
        public float Distance;
        public float Angle;

    }

    public Transform initialPosition;
    public List<Transform> waypoints;
    public float speed = 2f;
    public bool returnToOriginBeforeMove = false;

    private Target _currentTarget;

    private List<Target> _queuedWaypoints = new();
    private bool _isMoving = false;

    private void OnEnable()
    {
        Keyboard.current.onTextInput += OnCharPressed;
    }

    private void OnDisable()
    {
        Keyboard.current.onTextInput -= OnCharPressed;
    }

    private void OnCharPressed(char character)
    {
        var num = char.GetNumericValue(character);

        if (num < 0 || num > waypoints.Count || _isMoving)
        {
            return;
        }

        if (num == 0)
        {
            _queuedWaypoints.Add(new Target
            {
                Waypoint = initialPosition,
                Distance = Vector3.Distance(gameObject.transform.position, initialPosition.position),
                Angle = Quaternion.Angle(gameObject.transform.rotation, initialPosition.rotation)
            });
        }
        else
        {
            if (returnToOriginBeforeMove)
            {
                _queuedWaypoints.Add(new Target
                {
                    Waypoint = initialPosition,
                    Distance = Vector3.Distance(gameObject.transform.position, initialPosition.position),
                    Angle = Quaternion.Angle(gameObject.transform.rotation, initialPosition.rotation)
                });
            }

            var currentWaypoint = waypoints[(int)num - 1];

            var distanceToWaypoint = Vector3.Distance(gameObject.transform.position, currentWaypoint.position);
            var angleToWaypoint = Quaternion.Angle(gameObject.transform.rotation, currentWaypoint.rotation);

            _queuedWaypoints.Add(new Target
            {
                Waypoint = currentWaypoint,
                Distance = distanceToWaypoint,
                Angle = angleToWaypoint
            });
        }
    }

    private void Start()
    {
        gameObject.transform.position = initialPosition.position;
        gameObject.transform.rotation = initialPosition.rotation;

        _currentTarget = new Target { Waypoint = initialPosition, Distance = 0, Angle = 0 };
    }

    private void Update()
    {
        if (!_isMoving && _queuedWaypoints.Count > 0)
        {
            _currentTarget = _queuedWaypoints[0];
            _isMoving = true;
        }

        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _currentTarget.Waypoint.position, Time.deltaTime * speed * _currentTarget.Distance);
        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, _currentTarget.Waypoint.rotation, Time.deltaTime * speed * _currentTarget.Angle);

        if (Vector3.Distance(gameObject.transform.position, _currentTarget.Waypoint.position) < 0.1f && _isMoving)
        {
            _queuedWaypoints.RemoveAt(0);
            _isMoving = false;
        }
    }
}
