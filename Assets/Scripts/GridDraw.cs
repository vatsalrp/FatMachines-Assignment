using System.Collections;
using UnityEngine;

public class GridDraw : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mainCam;
    [SerializeField] private GameObject gridTilePrefab;
    [SerializeField] private TraySpawer traySpawer;

    [Header("Materials")]
    [SerializeField] private Material[] gridTileMaterials;

    [Header("Grid Settings")]
    public Vector2 GridSize;

    private void Start()
    {
        StartTheSystemJarvis();
    }

    [ContextMenu("Place Grid Tiles")]
    public void PlaceGridTiles()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                CreateGridTile(x, y);
            }
        }
    }

    [ContextMenu("Remove Grid Tiles")]
    public void RemoveGridTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void StartTheSystemJarvis()
    {
        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        RemoveGridTiles();
        CamPositionUpdate();
        PlaceGridTiles();
        yield return null;
        traySpawer.StartPlacingTrays();
    }

    private void CamPositionUpdate()
    {
        mainCam.transform.position = new Vector3(
            (GridSize.x / 2) - 0.5f,
            mainCam.transform.position.y,
            (GridSize.y / 2) - 0.5f
        );
    }

    private void CreateGridTile(int x, int y)
    {
        int materialIndex = (x + y) % 2;
        Vector3 position = new Vector3(x, 0, y);

        GameObject tile = Instantiate(gridTilePrefab, position, Quaternion.identity);
        tile.transform.SetParent(transform);

        Renderer tileRenderer = tile.transform.GetChild(0).GetComponent<Renderer>();
        tileRenderer.material = gridTileMaterials[materialIndex];
    }
}