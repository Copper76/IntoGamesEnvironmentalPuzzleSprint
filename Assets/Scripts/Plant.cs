using UnityEngine;

public class Plant : Item
{
    [SerializeField] private GameObject bloom;
    [SerializeField] private GameObject wither1;
    [SerializeField] private GameObject wither2;
    [SerializeField] private int timePeriodOffset = 0;

    public override void Advance(TimePeriod timePeriod)
    {
        for (int i = 0; i < timePeriodOffset; i++)
        {
            timePeriod = TimeManager.NextTimePeriod(timePeriod);
        }
        switch (timePeriod)
        {
            case TimePeriod.PAST: 
                transform.localScale = Vector3.one * 0.2f;
                GridManager.Instance.AddValidPosition(transform.position);
                GridManager.Instance.RemoveValidPosition(transform.position + Vector3.up);
                GridManager.Instance.RemoveBlockedPosition(transform.position);
                if (wither1.activeInHierarchy)
                {
                    GridManager.Instance.RemoveValidPosition(transform.position + Vector3.up + transform.forward);
                    GridManager.Instance.RemoveBlockedPosition(transform.position + transform.forward);
                    wither1.SetActive(false);
                }
                if (wither2.activeInHierarchy)
                {
                    GridManager.Instance.RemoveValidPosition(transform.position + Vector3.up + transform.forward * 2.0f);
                    GridManager.Instance.RemoveBlockedPosition(transform.position + transform.forward * 2.0f);
                    wither2.SetActive(false);
                }
                break;
            case TimePeriod.PRESENT:
                transform.localScale = Vector3.one;
                GridManager.Instance.RemoveValidPosition(transform.position);
                if (GridManager.Instance.IsFree(transform.position + Vector3.up))
                {
                    bloom.SetActive(true);
                    GridManager.Instance.AddValidPosition(transform.position + Vector3.up * 2.0f);
                }
                break;
            case TimePeriod.FUTURE:
                if (bloom.activeInHierarchy)
                {
                    GridManager.Instance.RemoveValidPosition(transform.position + Vector3.up * 2.0f);
                    bloom.SetActive(false);
                }
                GridManager.Instance.TryAddValidPosition(transform.position + Vector3.up);
                GridManager.Instance.AddBlockedPosition(transform.position);
                if (GridManager.Instance.IsFree(transform.position + transform.forward))
                {
                    GridManager.Instance.TryAddValidPosition(transform.position + Vector3.up + transform.forward);
                    GridManager.Instance.AddBlockedPosition(transform.position + transform.forward);
                    wither1.SetActive(true);
                }
                if (wither1.activeInHierarchy && GridManager.Instance.IsFree(transform.position + transform.forward * 2.0f))
                {
                    GridManager.Instance.TryAddValidPosition(transform.position + Vector3.up + transform.forward * 2.0f);
                    GridManager.Instance.AddBlockedPosition(transform.position + transform.forward * 2.0f);
                    wither2.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public override bool CanAdvance(TimePeriod timePeriod)
    {
        switch (timePeriod)
        {
            case TimePeriod.PAST:
                return true;
            case TimePeriod.PRESENT:
                return !(bloom.activeInHierarchy && GridManager.Instance.GetPlayerPosition() == transform.position + Vector3.up * 2.0f);
            case TimePeriod.FUTURE:
                Vector3 playerPosition = GridManager.Instance.GetPlayerPosition();
                if (playerPosition == transform.position + Vector3.up) return false;
                if (wither1.activeInHierarchy && playerPosition == transform.position + Vector3.up + transform.forward) return false;
                if (wither2.activeInHierarchy && playerPosition == transform.position + Vector3.up + transform.forward * 2.0f) return false;
                return true;
            default:
                return false;
        }
    }
}
