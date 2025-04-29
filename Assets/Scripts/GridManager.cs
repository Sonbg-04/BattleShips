using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    private readonly float xOffset = 0.61f, yOffset = 0.61f;
    private readonly int gridWidth = 10, gridHeight = 10;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int row = 0; row < gridWidth; row++)
        {
            for (int col = 0; col < gridHeight; col++)
            {
                float xPos = row * xOffset;
                float yPos = col * yOffset;
                Vector3 cellPosition = new Vector3(xPos, yPos, 0);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cell.transform.SetParent(transform);
            }
        }
    }
}
