using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private readonly List<Vector2Int> usedIndices = new();
    private readonly GameObject[,] enemyCells = new GameObject[10, 10];
    [SerializeField] private GameObject cellPrefab;
    private readonly bool[,] hitCells = new bool[10, 10];
    private readonly List<Vector2Int> currentHits = new();
    private List<Vector2Int> potentialTargets = new();
    private Vector2Int? firstHit = null, lastHit = null;
    private int huntDirection = -1;
    private bool isHunting = false, directionConfirmed = false, reversedDirectionTried = false;

    private void Start()
    {
        CreateBoard();
        Generate(1, 5);
        Generate(1, 4);
        Generate(2, 3);
        Generate(1, 2);
        RenderBoard();
    }
    private void Generate(int shipsToGenerate, int shipSize)
    {
        for (int i = 0; i < shipsToGenerate; i++)
        {
            bool shipPlaced = false;
            int attempts = 0;
            const int maxAttempts = 100;

            while (!shipPlaced && attempts < maxAttempts)
            {
                attempts++;
                int x = Random.Range(0, 10);
                int y = Random.Range(0, 10);
                Vector2Int start = new(x, y);

                if (IsPositionOccupied(start) || HasAdjacentShips(start)) continue;

                List<int> directions = new() { 0, 1, 2, 3 };
                Shuffle(directions);

                foreach (int dir in directions)
                {
                    List<Vector2Int> temp = new() { start };
                    Vector2Int next = start;
                    bool isValid = true;

                    for (int j = 1; j < shipSize; j++)
                    {
                        next += DirectionToVector(dir);

                        if (!IsValidPosition(next) || IsPositionOccupied(next) || HasAdjacentShips(next))
                        {
                            isValid = false;
                            break;
                        }
                        temp.Add(next);
                    }

                    if (isValid)
                    {
                        usedIndices.AddRange(temp);
                        shipPlaced = true;
                        break;
                    }
                }
            }

        }
    }
    private bool IsValidPosition(Vector2Int pos) => pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 10;
    private bool IsPositionOccupied(Vector2Int pos) => usedIndices.Contains(pos);
    private bool HasAdjacentShips(Vector2Int pos)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int check = new(pos.x + dx, pos.y + dy);
                if (IsValidPosition(check) && IsPositionOccupied(check)) return true;
            }
        }
        return false;
    }
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    private void CreateBoard()
    {
        float xOffset = 0.61f, yOffset = 0.61f;

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                float xPos = col * xOffset;
                float yPos = row * yOffset;
                Vector3 cellPosition = new(xPos, yPos, 0);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                Cell c = cell.GetComponent<Cell>();
                enemyCells[row, col] = cell;
                c.transform.SetParent(transform);
            }
        }
    }
    private void RenderBoard()
    {
        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                GameObject cell = enemyCells[row, col];
                Cell cellComp = cell.GetComponent<Cell>();
                cellComp.SetCellPos(row, col);
                cellComp.SetHasShip(usedIndices.Contains(new Vector2Int(row, col)));
            }
        }
    }
    public (int row, int col) AiChooseCell()
    {
        if (isHunting)
        {
            while (potentialTargets.Count > 0)
            {
                Vector2Int target = potentialTargets[0];
                potentialTargets.RemoveAt(0);

                if (IsValidPosition(target) && !hitCells[target.x, target.y])
                {
                    Debug.Log($"AI chọn mục tiêu săn bắn: {target}");
                    return (target.x, target.y);
                }
            }

            Debug.Log("Hết mục tiêu săn, dừng săn.");
            StopHunting();
        }

        int longestShipRemaining = GetLongestRemainingShipSize();

        List<Vector2Int> candidates = new();
        List<Vector2Int> parityCandidates = new();
        List<Vector2Int> fallbackCandidates = new();

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (hitCells[row, col]) continue;

                Vector2Int pos = new(row, col);

                bool canFitLargeShip = CanFitShip(row, col, longestShipRemaining);

                bool hasGoodParity = (row + col) % 2 == 0;

                if (canFitLargeShip)
                {
                    if (hasGoodParity)
                    {
                        parityCandidates.Add(pos);
                    }
                    else
                    {
                        candidates.Add(pos);
                    }
                }
                else if (!hitCells[row, col])
                {
                    fallbackCandidates.Add(pos);
                }
            }
        }

        if (parityCandidates.Count > 0)
        {
            candidates.AddRange(parityCandidates);
        }

        if (candidates.Count == 0)
        {
            candidates = fallbackCandidates;
        }

        if (candidates.Count > 0)
        {
            var bestCandidates = candidates
                .OrderByDescending(pos => CalculateShipPotential(pos.x, pos.y, longestShipRemaining))
                .Take(5)
                .ToList();

            Vector2Int chosen = bestCandidates[Random.Range(0, bestCandidates.Count)];
            Debug.Log($"AI chọn ngẫu nhiên từ 5 ô tốt nhất: {chosen}");
            return (chosen.x, chosen.y);
        }

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (!hitCells[row, col])
                    return (row, col);
            }
        }

        return (-1, -1);
    }
    private void StopHunting()
    {
        Debug.Log("Dừng chế độ săn.");
        isHunting = false;
        currentHits.Clear();
        firstHit = null;
        lastHit = null;
        huntDirection = -1;
        directionConfirmed = false;
        reversedDirectionTried = false;
    }
    private int GetLongestRemainingShipSize()
    {
        if (isHunting && currentHits.Count > 0)
        {
            return Mathf.Max(5, currentHits.Count + 1);
        }

        int maxPossible = 2;

        for (int size = 5; size >= 2; size--)
        {
            if (!IsShipSunk(size))
            {
                maxPossible = size;
                break;
            }
        }

        return maxPossible;
    }
    private int CalculateShipPotential(int row, int col, int shipSize)
    {
        int potential = 0;

        for (int i = 0; i < shipSize; i++)
        {
            if (col + i < 10 && !hitCells[row, col + i])
                potential++;
            else
                break;
        }

        for (int i = 0; i < shipSize; i++)
        {
            if (row + i < 10 && !hitCells[row + i, col])
                potential++;
            else
                break;
        }

        return potential;
    }
    private bool IsShipSunk(int shipSize)
    {
        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (hitCells[row, col])
                {
                    bool rowComplete = true;
                    for (int i = 0; i < shipSize; i++)
                    {
                        if (col + i >= 10 || !hitCells[row, col + i])
                        {
                            rowComplete = false;
                            break;
                        }
                    }
                    if (rowComplete) return true;
                    bool colComplete = true;
                    for (int i = 0; i < shipSize; i++)
                    {
                        if (row + i >= 10 || !hitCells[row + i, col])
                        {
                            colComplete = false;
                            break;
                        }
                    }
                    if (colComplete) return true;
                }
            }
        }
        return false;
    }

    private bool CanFitShip(int row, int col, int shipSize)
    {
        bool result = false;
        if (row + shipSize <= 10 && Enumerable.Range(0, shipSize).All(i => !hitCells[row + i, col]))
            result = true;

        if (col + shipSize <= 10 && Enumerable.Range(0, shipSize).All(i => !hitCells[row, col + i]))
            result = true;

        if (row - shipSize + 1 >= 0 && Enumerable.Range(0, shipSize).All(i => !hitCells[row - i, col]))
            result = true;

        if (col - shipSize + 1 >= 0 && Enumerable.Range(0, shipSize).All(i => !hitCells[row, col - i]))
            result = true;

        return result;
    }
    public void ProcessAIShotResult(int row, int col, bool isHit)
    {
        hitCells[row, col] = true;
        Vector2Int pos = new(row, col);
        Debug.Log($"Kết quả bắn tại ({row},{col}): {(isHit ? "TRÚNG" : "TRƯỢT")}");

        if (isHit)
        {
            RegisterHit(pos);
            lastHit = pos;

            if (directionConfirmed && lastHit.HasValue)
            {
                potentialTargets.Clear();
                Vector2Int next = lastHit.Value + DirectionToVector(huntDirection);

                if (IsValidPosition(next) && !hitCells[next.x, next.y])
                {
                    AddPotentialTarget(next);
                    return;
                }
                else
                {
                    next = firstHit.Value - DirectionToVector(huntDirection);
                    if (IsValidPosition(next) && !hitCells[next.x, next.y])
                    {
                        AddPotentialTarget(next);
                    }

                }
            }
        }
        else
        {
            if (isHunting && directionConfirmed)
            {
                if (!reversedDirectionTried)
                {
                    huntDirection = GetOppositeDirection(huntDirection);
                    reversedDirectionTried = true;
                    Debug.Log("Hướng bắn sai, thử hướng ngược lại.");


                    potentialTargets.Clear();
                    if (firstHit.HasValue)
                    {
                        Vector2Int nextTry = firstHit.Value + DirectionToVector(huntDirection);
                        if (IsValidPosition(nextTry) && !hitCells[nextTry.x, nextTry.y])
                        {
                            AddPotentialTarget(nextTry);
                        }
                    }
                }
                else
                {
                    Debug.Log("Cả hai hướng đều sai. Thử lại từ đầu từ ô trúng đầu tiên.");
                    directionConfirmed = false;
                    huntDirection = -1;
                    reversedDirectionTried = false;
                    lastHit = firstHit;
                    potentialTargets.Clear();

                    if (firstHit.HasValue)
                    {
                        foreach (int dir in new[] { 0, 1, 2, 3 })
                        {
                            Vector2Int newTarget = firstHit.Value + DirectionToVector(dir);
                            if (IsValidPosition(newTarget) && !hitCells[newTarget.x, newTarget.y])
                            {
                                AddPotentialTarget(newTarget);
                            }
                        }
                    }
                }
            }
        }
    }
    private int GetOppositeDirection(int dir)
    {
        return dir switch
        {
            0 => 1,
            1 => 0,
            2 => 3,
            3 => 2,
            _ => -1
        };
    }
    private Vector2Int DirectionToVector(int dir)
    {
        return dir switch
        {
            0 => Vector2Int.up,
            1 => Vector2Int.down,
            2 => Vector2Int.right,
            3 => Vector2Int.left,
            _ => Vector2Int.zero
        };
    }
    public void RegisterHit(Vector2Int pos)
    {
        currentHits.Add(pos);
        if (!firstHit.HasValue) firstHit = pos;
        lastHit = pos;
        isHunting = true;
        Debug.Log($"Ghi nhận trúng tàu tại {pos}");

        if (currentHits.Count == 1)
        {
            potentialTargets.Clear();

            foreach (int dir in new[] { 0, 1, 2, 3 })
                AddPotentialTarget(pos + DirectionToVector(dir));
        }
        else if (currentHits.Count == 2 && !directionConfirmed)
        {
            Vector2Int delta = lastHit.Value - firstHit.Value;

            if (delta != Vector2Int.zero)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    huntDirection = delta.x > 0 ? 2 : 3;
                else if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    huntDirection = delta.y > 0 ? 0 : 1;

                directionConfirmed = true;
                reversedDirectionTried = false;
                Debug.Log($"Xác nhận hướng: {huntDirection}");

                Vector2Int axis = DirectionToVector(huntDirection);
                List<Vector2Int> filteredTargets = potentialTargets.FindAll(target =>
                {
                    Vector2Int offset = target - firstHit.Value;
                    return (axis.x != 0 && offset.x != 0 && offset.y == 0) ||
                           (axis.y != 0 && offset.y != 0 && offset.x == 0);
                });

                potentialTargets = filteredTargets;

                AddPotentialTarget(lastHit.Value + axis);
                AddPotentialTarget(firstHit.Value - axis);
            }
        }
    }
    private void AddPotentialTarget(Vector2Int pos)
    {
        if (IsValidPosition(pos) && !hitCells[pos.x, pos.y])
        {
            if (!potentialTargets.Contains(pos))
            {
                potentialTargets.Add(pos);
                Debug.Log($"Thêm mục tiêu mới: {pos}");
            }
        }
    }
   
}
