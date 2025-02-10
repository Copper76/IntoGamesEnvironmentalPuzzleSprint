using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum TimePeriod
{
    PAST,
    PRESENT,
    FUTURE
}

public class TimeManager : RegulatorSingleton<TimeManager>
{
    [SerializeField] TimePeriod timePeriod = TimePeriod.PRESENT;
    public TimePeriod TimePeriod { get { return timePeriod; } }

    private List<Item> _items = new List<Item>();

    public bool Advance()
    {
        if (!CanAdvance()) return false;
        timePeriod = NextTimePeriod(timePeriod);
        foreach (Item item in _items)
        {
            item.Advance(timePeriod);
        }

        return true;
    }

    private bool CanAdvance()
    {
        foreach (Item item in _items) {
            if (!item.CanAdvance(timePeriod))
            {
                return false;
            }
        }

        return true;
    }

    public static TimePeriod NextTimePeriod(TimePeriod current)
    {
        return current switch
        {
            TimePeriod.PAST => TimePeriod.PRESENT,
            TimePeriod.PRESENT => TimePeriod.FUTURE,
            TimePeriod.FUTURE => TimePeriod.PAST,
            _ => TimePeriod.PRESENT
        };
    }

    public void AddItem(Item item)
    {
        _items.Add(item);
    }
}
