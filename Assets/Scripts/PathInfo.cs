using UnityEngine;

public class PathInfo : MonoBehaviour
{
    protected Vector3 _waypoint;

    protected void Awake()
    {
        OnPlace();
    }

    protected virtual void CalculateWaypoint()
    {
        _waypoint = transform.position + transform.up;
    }

    protected virtual void OnPlace()
    {
        CalculateWaypoint();
        GridManager.Instance.AddValidPosition(_waypoint);
        GridManager.Instance.AddBlockedPosition(transform.position);
    }

    protected virtual void OnMove()
    {
        GridManager.Instance.RemoveValidPosition(_waypoint);
        GridManager.Instance.RemoveBlockedPosition(transform.position);
    }
}
