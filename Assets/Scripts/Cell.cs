using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private int x, y;
    private bool hasShip = false, isHit = false;
    private ShipManager occupiedShip;

    public ShipManager GetOccupiedShip()
    {
        return occupiedShip;
    }
    public void SetOccupiedShip(ShipManager ship)
    {
        occupiedShip = ship;
    }
    public Vector2Int GetCellPos()
    {
        return new Vector2Int(x, y);
    }
    public void SetCellPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public bool GetHasShip()
    {
        return hasShip;
    }
    public void SetHasShip(bool value)
    {
        hasShip = value;
    }
    public bool GetIsHit()
    {
        return isHit;
    }
    public void SetIsHit(bool value)
    {
        isHit = value;
    }
}