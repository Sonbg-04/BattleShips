using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Collider2D dockCollider2D;
    [SerializeField] private List<Cell> allCells;
    [SerializeField] private List<ShipManager> allShips;

    public int shipCount;
    private bool isSelectShip = false;
    private ShipManager selectShip;
    private ShipManager.ShipType selectShipType;
    private Vector3 chosenPosition;

    private void Start()
    {
        selectShip = null;
        chosenPosition = Vector3.zero;

        allCells = new List<Cell>(FindObjectsOfType<Cell>());
        foreach(Cell c in allCells)
        {
            Vector3 pos = c.transform.position;
            int x = Mathf.RoundToInt(pos.x / 0.61f);
            int y = Mathf.RoundToInt(pos.y / 0.61f);
            c.SetCellPos(x, y);
        }

        allShips = new List<ShipManager>(FindObjectsOfType<ShipManager>());
        shipCount = allShips.Count;

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickSelectShip();
        }
    }
    private void ClickSelectShip()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider == null)
        {
            return;
        }
        else
        {
            if (!isSelectShip)
            {
                SelectShipFromDock(hit);
            }
            else
            {
                PlaceShipOnCell(hit);
            }
        }
    }
    private void SelectShipFromDock(RaycastHit2D hit)
    {
        ShipManager s = hit.collider.GetComponent<ShipManager>();
        if (s == null || dockCollider2D == null || !dockCollider2D.OverlapPoint(hit.point))
        {
            return;
        }
        else
        {
            selectShip = s;
            selectShipType = s.CheckType(hit.collider.gameObject);
            s.ChangeColorShip(hit.collider.gameObject, selectShipType);
            chosenPosition = hit.point;
            isSelectShip = true;
        }
    }
    private bool IsAdjacentToOtherShips(List<Cell> shipCells)
    {
        foreach (Cell cell in shipCells)
        {
            Vector2Int pos = cell.GetCellPos();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    Vector2Int neighborPos = new Vector2Int(pos.x + dx, pos.y + dy);

                    foreach (Cell other in allCells)
                    {
                        if (other.GetCellPos() == neighborPos && other.GetHasShip())
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    private void PlaceShipOnCell(RaycastHit2D hit)
    {
        if (!hit.collider.CompareTag("PlayerCell"))
        {
            return;
        }
        else
        {
            Cell c = hit.collider.GetComponent<Cell>();
            if (c == null || c.GetHasShip())
            {
                return;
            }
            else
            {
                chosenPosition = hit.transform.position;

                if (selectShip == null)
                {
                    return;
                }
                else
                {
                    selectShip.MoveShip(chosenPosition, selectShipType);
                    if (selectShip.GetPlaceShip())
                    {
                        List<Cell> occupiedCells = selectShip.GetOccupiedCells();

                        if (IsAdjacentToOtherShips(occupiedCells))
                        {
                            Debug.Log("Tàu không được đặt cạnh nhau!");
                            selectShip.StartFlashing();
                            return;
                        }
                        else
                        {
                            foreach (Cell occupiedCell in occupiedCells)
                            {
                                occupiedCell.SetHasShip(true);
                                occupiedCell.SetOccupiedShip(selectShip);
                            }
                            Debug.Log("Đặt tàu thành công!");
                            Debug.Log($"Trạng thái tàu sau khi đặt: {(selectShip.GetIsSunk() ? "Đã chìm" : "Chưa chìm")}");
                            selectShip.StopFlashing();
                            shipCount--;
                            isSelectShip = false;
                        }        
                    }
                    else
                    {
                        Debug.Log("Vị trí không hợp lệ! Đặt lại!");
                        chosenPosition = Vector3.zero;
                    }
                }
            }
        }
    }
    public void PlayerRotateShip()
    {
        if (isSelectShip && selectShip != null)
        {
            selectShip.RotateShip();
        }
    }
}