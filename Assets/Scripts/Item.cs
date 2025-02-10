using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private void Start()
    {
        GridManager.Instance.AddItem(this, transform.position);
        TimeManager.Instance.AddItem(this);
        Advance(TimePeriod.PRESENT);
    }

    public abstract void Advance(TimePeriod timePeriod);

    public abstract bool CanAdvance(TimePeriod timePeriod);

    private void OnDestroy()
    {
    }
}
