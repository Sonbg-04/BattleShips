﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect, missEffect;
    private Cell selectedCell;
    private int turn = 1, enemyShipCount = 17, playerShipCount = 17;
    private bool isCellSelected = false, enemyCellSelected = false, enemyCellsDiscovered = false, isAIShooting = false;
    private readonly List<GameObject> playerCells = new();
    private readonly List<GameObject> playerUI = new();
    private readonly List<GameObject> enemyUI = new();
    private readonly List<GameObject> enemyCells = new();
    private readonly List<GameObject> playerShips = new();
    private readonly Dictionary<string, List<GameObject>> shipDictionary = new();
    private AudioManager audioManager;
    private AIManager aiManager;
    private Scene3Manager scene3;

    private void Start()
    {
        selectedCell = null;

        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
        }
        else
        {
            Debug.Log("AudioManager not found!");
        }

        GameObject gameObject = GameObject.FindGameObjectWithTag("Scene3Manager");
        if (gameObject == null)
        {
            Debug.Log("Scene3Manager not found in the scene.");
        }
        else
        {
            scene3 = gameObject.GetComponent<Scene3Manager>();
        }

        GameObject Ai = GameObject.FindGameObjectWithTag("AIManager");
        if (Ai != null)
        {
            aiManager = Ai.GetComponent<AIManager>();
        }
        else
        {
            Debug.Log("AIManager not found in the scene.");
        }
        GameObject[] cells = GameObject.FindGameObjectsWithTag("PlayerCell");
        if (cells.Length > 0)
        {
            playerCells.AddRange(cells);
        }
        else
        {
            Debug.LogError("Không tìm thấy ô của người chơi.");
        }

        ShipFinding();
    }
    private void Update()
    {
        DiscoverEnemyCells();

        if (turn == 1)
        {
            HandlePlayerTurn();
        }

        if (turn == 2 && !isAIShooting)
        {
            HandleAITurn();
        }

        CheckForGameEnd();
    }

    private void DiscoverEnemyCells()
    {
        if (!enemyCellsDiscovered)
        {
            GameObject[] enemyCellsFound = GameObject.FindGameObjectsWithTag("AICell");
            if (enemyCellsFound.Length > 0)
            {
                enemyCells.AddRange(enemyCellsFound);
                enemyCellsDiscovered = true;
            }
            else
            {
                Debug.LogError("Không tìm thấy ô của kẻ thù.");
            }
        }
    }

    private void HandlePlayerTurn()
    {
        SetUI(true);
        if (!isCellSelected && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PlayerShootRoutine());
        }
    }

    private void HandleAITurn()
    {
        SetUI(false);
        if (!enemyCellSelected)
        {
            enemyCellSelected = true;
            isAIShooting = true;
            StartCoroutine(AIShootRoutine());
        }
    }

    private void CheckForGameEnd()
    {
        if (enemyShipCount == 0)
        {
            EndGamePanel(true);
        }

        if (playerShipCount == 0)
        {
            EndGamePanel(false);
        }
    }

    private void EndGamePanel(bool playerWon)
    {
        turn = 0;
        if (playerWon)
        {
            scene3.winPanel.SetActive(true);
            audioManager.PlayVictory();
        }
        else
        {
            scene3.losePanel.SetActive(true);
            audioManager.PlayDefeat();
        }
        SetUI(true);
    }

    private Cell GetCellAtPos(int row, int col, int rowNum)
    {
        int index = row + col * rowNum;
        return index >= 0 && index < playerCells.Count ? playerCells[index].GetComponent<Cell>() : null;
    }

    private void CheckIfHit(Cell selectedTile, List<GameObject> effectsList, out bool isHit)
    {
        isHit = false;

        if (selectedTile == null || selectedTile.GetIsHit())
            return;

        selectedTile.SetIsHit(true);

        if (selectedTile.GetHasShip())
        {
            GameObject newHit = Instantiate(hitEffect, selectedTile.transform.position, Quaternion.identity);
            effectsList.Add(newHit);
            isHit = true;

            if (turn == 1) enemyShipCount--;
            else if (turn == 2) playerShipCount--;
        }
        else
        {
            GameObject newMissed = Instantiate(missEffect, selectedTile.transform.position, Quaternion.identity);
            effectsList.Add(newMissed);
        }
    }

    private IEnumerator AIShootRoutine()
    {
        bool keepShooting = true;

        while (keepShooting)
        {
            yield return new WaitForSeconds(0.5f);

            var (row, col) = aiManager.AiChooseCell();
            Cell targetCell = GetCellAtPos(row, col, 10);

            CheckIfHit(targetCell, enemyUI, out bool isHit);
            aiManager.ProcessAIShotResult(row, col, isHit);

            if (!isHit) keepShooting = false;
            else yield return new WaitForSeconds(0.3f);
        }

        isAIShooting = false;
        StartCoroutine(Wait(1));
    }

    private IEnumerator PlayerShootRoutine()
    {
        isCellSelected = true;
        bool keepShooting = true;
        bool hitLastShot = false;

        while (keepShooting)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("AICell"))
            {
                Cell c = hit.collider.GetComponent<Cell>();
                if (!c.GetIsHit())
                {
                    selectedCell = c;
                    CheckIfHit(selectedCell, playerUI, out bool isHit);

                    hitLastShot = isHit;
                    if (!isHit) keepShooting = false;
                }
            }
        }

        StartCoroutine(Wait(2));
    }

    private void SetUI(bool isPlayerTurn)
    {
        scene3.yourTurnImg.gameObject.SetActive(isPlayerTurn);
        scene3.enemyTurnImg.gameObject.SetActive(!isPlayerTurn);
        SetActive(enemyCells, isPlayerTurn);
        SetActive(playerUI, isPlayerTurn);
        SetActive(enemyUI, !isPlayerTurn);
        SetActive(playerCells, !isPlayerTurn);
        SetActive(playerShips, !isPlayerTurn);
    }

    private void SetActive(List<GameObject> list, bool isActive)
    {
        foreach (var item in list)
        {
            item.SetActive(isActive);
        }
    }

    private void ShipFinding()
    {
        shipDictionary.Add("ShipCell_2", new List<GameObject>());
        shipDictionary.Add("ShipCell_3", new List<GameObject>());
        shipDictionary.Add("ShipCell_4", new List<GameObject>());
        shipDictionary.Add("ShipCell_5", new List<GameObject>());

        GameObject[] allShips = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var ship in allShips)
        {
            string shipTag = ship.tag;
            if (shipDictionary.ContainsKey(shipTag))
            {
                shipDictionary[shipTag].Add(ship);
            }
        }

        foreach (var shipList in shipDictionary.Values)
        {
            playerShips.AddRange(shipList);
        }
    }

    private IEnumerator Wait(int number)
    {
        yield return new WaitForSeconds(1.5f);
        turn = number;
        isCellSelected = false;
        enemyCellSelected = false;
    }
}