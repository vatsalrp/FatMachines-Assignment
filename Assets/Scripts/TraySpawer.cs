using System.Collections.Generic;
using UnityEngine;

public class TraySpawer : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] trayPrefabs;

    [Header("References")]
    [SerializeField] private GridDraw gridSystem;

    [Header("Settings")]
    [SerializeField] private int maxAttempts = 100;

    private bool[,] occupiedGrid;
    private List<GameObject> spawnedTrays = new List<GameObject>();

    public void StartPlacingTrays()
    {
        InitializeOccupiedGrid();
        SpawnRandomTrays();
    }

    public void SpawnRandomTrays()
    {
        ClearExistingTrays();
        InitializeOccupiedGrid();

        for (int i = 0; i < trayPrefabs.Length; i++)
        {
            GameObject randomTrayPrefab = trayPrefabs[Random.Range(0, trayPrefabs.Length)];
            TrySpawnTrayRandomly(randomTrayPrefab);
        }
    }

    private void InitializeOccupiedGrid()
    {
        occupiedGrid = new bool[(int)gridSystem.GridSize.x, (int)gridSystem.GridSize.y];
    }

    private void ClearExistingTrays()
    {
        foreach (var tray in spawnedTrays)
        {
            Destroy(tray);
        }
        spawnedTrays.Clear();
    }

    private void TrySpawnTrayRandomly(GameObject trayPrefab)
    {
        Tray trayInfo = trayPrefab.GetComponent<Tray>();
        if (trayInfo == null) return;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int randomPosition = GetRandomValidPosition(trayInfo);

            if (CanPlaceTray(trayInfo, randomPosition))
            {
                PlaceTray(trayPrefab, randomPosition);
                return;
            }
        }

        Debug.LogWarning($"Failed to place tray after {maxAttempts} attempts");
    }

    private Vector2Int GetRandomValidPosition(Tray trayInfo)
    {
        return new Vector2Int(
            Random.Range(0, (int)gridSystem.GridSize.x - trayInfo.Width + 1),
            Random.Range(0, (int)gridSystem.GridSize.y - trayInfo.Height + 1)
        );
    }

    private bool CanPlaceTray(Tray trayInfo, Vector2Int gridPosition)
    {
        for (int x = 0; x < trayInfo.Width; x++)
        {
            for (int y = 0; y < trayInfo.Height; y++)
            {
                if (trayInfo.shapeRows[y].columns[x] && IsPositionInvalid(gridPosition, x, y))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsPositionInvalid(Vector2Int gridPosition, int x, int y)
    {
        return gridPosition.x + x >= gridSystem.GridSize.x ||
               gridPosition.y + y >= gridSystem.GridSize.y ||
               occupiedGrid[gridPosition.x + x, gridPosition.y + y];
    }

    private void PlaceTray(GameObject trayPrefab, Vector2Int gridPosition)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0, gridPosition.y);
        GameObject tray = Instantiate(trayPrefab, worldPosition, Quaternion.identity);
        spawnedTrays.Add(tray);

        Tray trayInfo = tray.GetComponent<Tray>();
        MarkGridOccupied(trayInfo, gridPosition, true);
    }

    private void MarkGridOccupied(Tray trayInfo, Vector2Int gridPosition, bool occupied)
    {
        for (int x = 0; x < trayInfo.Width; x++)
        {
            for (int y = 0; y < trayInfo.Height; y++)
            {
                if (trayInfo.shapeRows[y].columns[x])
                {
                    occupiedGrid[gridPosition.x + x, gridPosition.y + y] = occupied;
                }
            }
        }
    }
}