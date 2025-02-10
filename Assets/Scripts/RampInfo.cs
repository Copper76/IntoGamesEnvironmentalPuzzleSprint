using UnityEngine;

public class RampInfo : PathInfo
{
    protected override void CalculateWaypoint()
    {
        _waypoint = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
    }
}
