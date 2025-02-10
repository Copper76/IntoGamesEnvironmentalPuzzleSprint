using UnityEngine;

public class BlockedPathInfo : PathInfo
{
    protected override void OnPlace()
    {
        GridManager.Instance.AddBlockedPosition(transform.position);
    }

    protected override void OnMove()
    {
        GridManager.Instance.RemoveBlockedPosition(transform.position);
    }
}
