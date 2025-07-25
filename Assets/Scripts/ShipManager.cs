using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public enum ShipType
    {
        ShipCell_2, ShipCell_3, ShipCell_4, ShipCell_5
    }

    [SerializeField] private ShipType shipType;
    [SerializeField] private float xOffset, yOffset;

    private readonly float xLeftBound = -5.5f, xRightBound = 1f,
                           yTopBound = 3.5f, yBottomBound = -3.2f;
    private bool isRotateShip = false, isPlaceShip = false, isSunk = false;
    private int rotateCount = 0;
    private Coroutine cor;
    private SpriteRenderer spriteRenderer;
    private readonly List<Cell> occupiedCells = new();

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("SpriteRenderer not found on the ship object.");
        }
    }
    public bool GetIsSunk()
    {
        return isSunk;
    }
    public void StartFlashing()
    {
        if (cor == null)
        {
            cor = StartCoroutine(FlashRoutine());
        }    
    }
    public void StopFlashing()
    {
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
            SetAlpha(1f);
        }
    }
    private IEnumerator FlashRoutine()
    {
        while(true)
        {
            SetAlpha(0.5f);
            yield return new WaitForSeconds(0.2f);
            SetAlpha(1f);
            yield return new WaitForSeconds(0.2f);
        }    
    }    
    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        else
        {
            Debug.Log("SpriteRenderer not found on the ship object.");
        }
    }
    public bool GetPlaceShip()
    {
        return isPlaceShip;
    }
    public ShipType CheckType(GameObject obj)
    {
        return obj.tag switch
        {
            "ShipCell_2" => ShipType.ShipCell_2,
            "ShipCell_3" => ShipType.ShipCell_3,
            "ShipCell_4" => ShipType.ShipCell_4,
            _ => ShipType.ShipCell_5
        };
    }    
    public void ChangeColorShip(GameObject obj, ShipType st)
    {
        isPlaceShip = false;

        Color cl = st switch
        {
            ShipType.ShipCell_2 => new Color(0.83f, 0.68f, 0.46f, 0.8f),
            ShipType.ShipCell_3 => new Color(0.71f, 0.40f, 0.16f, 1.0f),
            ShipType.ShipCell_4 => new Color(0.2f, 0.6f, 0.4f, 1.0f),
            _ => new Color(0.2f, 0.4f, 0.6f, 0.8f)
        };
        if (obj.TryGetComponent<Renderer>(out var rd))
        {
            StartCoroutine(Blink(rd.material, cl));
        }
    }
    private IEnumerator Blink(Material mt, Color cl)
    {
        Color clr = mt.color;
        float blinkDuration = 0.2f;
        while (!isPlaceShip)
        {
            mt.color = cl;
            yield return new WaitForSeconds(blinkDuration);
            mt.color = clr;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
    public void MoveShip(Vector3 pos, ShipType st)
    {
        transform.position = isRotateShip ? new Vector3(pos.x + xOffset, pos.y, pos.z)
                                          : new Vector3(pos.x, pos.y + yOffset, pos.z);

        Physics2D.SyncTransforms();

        isPlaceShip = IsWithinBounds() && !IsOverlappingOtherShips() && !IsOccupiedByOtherShipCells(); 
    }
    private bool IsOccupiedByOtherShipCells()
    {
        List<Cell> c = GetOccupiedCells();

        foreach (Cell cell in c)
        {
            if (cell.GetHasShip())
            {
                return true;
            }
        }
        return false;
    }
    private bool IsWithinBounds()
    {
        Renderer rd = GetComponentInChildren<Renderer>();
        if (rd == null)
        {
            return false;
        }
        else
        {
            Bounds b = rd.bounds;
            return b.min.x >= xLeftBound 
                && b.max.x <= xRightBound 
                && b.min.y >= yBottomBound 
                && b.max.y <= yTopBound;
        }    
    }
    public List<Cell> GetOccupiedCells()
    {
        occupiedCells.Clear();

        Collider2D[] shipParts = GetComponentsInChildren<Collider2D>();

        foreach (var part in shipParts)
        {
            Vector2 checkPosition = part.bounds.center;
            Vector2 size = part.bounds.size;

            Collider2D[] results = Physics2D.OverlapBoxAll(checkPosition, size, 0f, LayerMask.GetMask("Cell"));
            foreach (var hit in results)
            {
                if (hit.CompareTag("PlayerCell"))
                {
                    Cell cell = hit.GetComponent<Cell>();
                    if (cell != null && !occupiedCells.Contains(cell))
                    {
                        occupiedCells.Add(cell);
                    }
                }
            }
        }
        return occupiedCells;
    }
    private bool IsOverlappingOtherShips()
    {
        Collider2D[] shipParts = GetComponentsInChildren<Collider2D>();

        foreach (var part in shipParts)
        {
            Vector2 center = part.bounds.center;
            Vector2 size = part.bounds.size;

            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, LayerMask.GetMask("ShipPlayer"));

            foreach (var hit in hits)
            {
                if (hit != part && !hit.transform.IsChildOf(this.transform))
                {
                    Debug.Log("Va chạm với: " + hit.name);
                    return true;
                }
            }
        }
        return false;
    }
    public void RotateShip()
    {
        transform.Rotate(Vector3.forward, -90f);
        rotateCount++;

        isRotateShip = rotateCount % 2 != 0;
    }
}
