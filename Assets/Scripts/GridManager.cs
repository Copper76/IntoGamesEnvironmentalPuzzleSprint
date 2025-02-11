using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct Connection : IEquatable<Connection>
{
    public Vector3 A { get; }
    public Vector3 B { get; }

    public Connection(Vector3 a, Vector3 b)
    {
        // Ensure consistent ordering (lower first for hashing)
        if (a.GetHashCode() < b.GetHashCode())
        {
            A = a;
            B = b;
        }
        else
        {
            A = b;
            B = a;
        }
    }

    public override int GetHashCode()
    {
        return A.GetHashCode() ^ B.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Connection other && Equals(other);
    }

    public bool Equals(Connection other)
    {
        return A.Equals(other.A) && B.Equals(other.B);
    }
}

public class GridManager : RegulatorSingleton<GridManager>
{
    private HashSet<Vector3> validPositions = new HashSet<Vector3>();
    [SerializeField] private List<Vector3> validPositionList = new List<Vector3>();

    private HashSet<Vector3> blockedPositions = new HashSet<Vector3>();

    private Dictionary<Vector3, Item> itemPlacements = new Dictionary<Vector3, Item>();

    private Vector3 _playerPosition;
    private Vector3 _playerTargetPosition;

    private void Start()
    {
        validPositionList = validPositions.ToList();
    }

    public void SetPlayerPosition(Vector3 playerPosition)
    {
        _playerPosition = RoundToInt(playerPosition);
    }

    public void SetPlayerTargetPosition(Vector3 playerPosition)
    {
        _playerTargetPosition = RoundToInt(playerPosition);
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerPosition;
    }

    public Vector3 GetPlayerTargetPosition()
    {
        return _playerTargetPosition;
    }

    public bool IsPlayerInPosition(Vector3 position)
    {
        return (position == _playerPosition || position == _playerTargetPosition);
    }

    public bool IsPlayerInPosition(Vector3[] potentialPositions)
    {
        foreach (Vector3 position in potentialPositions)
        {
            Vector3 newPosition = RoundToInt(position);
            if (newPosition == _playerPosition || newPosition == _playerTargetPosition) return true;
        }
        return false;
    }

    public void AddValidPosition(Vector3 newPosition)
    {
        newPosition = RoundToInt(newPosition);
        validPositions.Add(newPosition);
    }

    public bool TryAddValidPosition(Vector3 newPosition)
    {
        newPosition = RoundToInt(newPosition);
        if (!IsFree(newPosition)) return false;

        validPositions.Add(newPosition);

        return true;
    }

    public void TryAddValidPositions(Vector3[] newPositions)
    {
        foreach (Vector3 position in newPositions)
        {
            Vector3 newPosition = RoundToInt(position);
            if (blockedPositions.Contains(newPosition)) continue;

            validPositions.Add(newPosition);
        }
    }

    public void RemoveValidPosition(Vector3 removedPosition)
    {
        removedPosition = RoundToInt(removedPosition);
        validPositions.Remove(removedPosition);
    }

    public void AddBlockedPosition(Vector3 newPosition)
    {
        newPosition = RoundToInt(newPosition);
        blockedPositions.Add(newPosition);
    }

    public void RemoveBlockedPosition(Vector3 removedPosition)
    {
        removedPosition = RoundToInt(removedPosition);
        blockedPositions.Remove(removedPosition);
    }

    public bool IsValid(Vector3 checkedPosition)
    {
        checkedPosition = RoundToInt(checkedPosition);
        return validPositions.Contains(checkedPosition) && !blockedPositions.Contains(checkedPosition);
    }

    public bool IsFree(Vector3 checkedPosition)
    {
        checkedPosition = RoundToInt(checkedPosition);
        return !blockedPositions.Contains(checkedPosition) && checkedPosition != _playerPosition;
    }

    public bool IsAllFree(Vector3[] checkedPositions)
    {
        foreach (Vector3 position in checkedPositions)
        {
            Vector3 roundedPosition = RoundToInt(position);
            if (blockedPositions.Contains(roundedPosition) || roundedPosition == _playerPosition)
            {
                return false;
            }
        }
        return true;
    }

    public bool GetValidTarget(Vector3 startPosition, ref Vector3 target)
    {
        target = RoundToInt(target);
        Vector3 originalTarget = target;

        if (IsValid(target)) return true;

        target += Vector3.up / 2.0f; 

        if (IsValid(target)) return true;

        if (!IsFree(originalTarget)) return false;

        Vector3 upperRamp = RoundToInt(originalTarget + Vector3.up);
        if (IsValid(upperRamp) && _playerPosition.y % 1 != 0)
        {
            target = upperRamp;
            return true;
        }

        target -= Vector3.up;

        if (!IsFree(target)) return false;

        if (IsValid(target)) return true;

        target -= Vector3.up / 2.0f;

        return IsValid(target);//Allow dropping for a block
    }

    public bool AddItem(Item item, Vector3 position)
    {
        position = RoundToInt(position);
        if (IsValid(position) && !itemPlacements.ContainsKey(position))
        {
            itemPlacements.Add(position, item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryPickUp(Vector3 position, ref Item heldItem)
    {
        position = RoundToInt(position);
        if (IsValid(position) && itemPlacements.ContainsKey(position))
        {

            heldItem = itemPlacements[position];
            itemPlacements.Remove(position);
            return true;
        }
        else
        {
            heldItem = null;
        }

        return false;
    }

    private Vector3 RoundToInt(Vector3 v)
    {
        return new Vector3(
            Mathf.Round(v.x * 2.0f) / 2.0f,
            Mathf.Round(v.y * 2.0f) / 2.0f,
            Mathf.Round(v.z * 2.0f) / 2.0f
        );
    }

    private void OnDrawGizmos()
    {
        foreach (var position in validPositions)
        {
            if (!blockedPositions.Contains(position))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(position, 0.2f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(position, 0.2f);
            }
        }
        foreach (var position in blockedPositions)
        {
            if (!validPositions.Contains(position))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(position, 0.2f);
            }
        }
    }
}
