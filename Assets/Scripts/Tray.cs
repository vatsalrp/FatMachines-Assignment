using UnityEngine;

[System.Serializable]
public class ShapeRow
{
    public bool[] columns;
}

public class Tray : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color[] color;

    [Header("Physics Settings")]
    [SerializeField] private float baseForce = 1000f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float gridSize = 1f;

    [Header("Shape Configuration")]
    public int Width = 1;
    public int Height = 1;
    public ShapeRow[] shapeRows;

    private Rigidbody rb;
    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragOffset;

    private void Awake()
    {
        InitializeShape();
    }

    private void Start()
    {
        InitializeComponents();
        ApplyRandomColor();
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseDrag()
    {
        HandleDragging();
    }

    private void OnMouseUp()
    {
        StopDragging();
    }

    private void OnValidate()
    {
        ValidateDimensions();
        InitializeShape();
    }

    // Public Methods
    [ContextMenu("Initialize Shape")]
    public void InitializeShape()
    {
        InitializeShapeRows();
    }

    // Private Methods
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.drag = 2f;
        rb.angularDrag = 5f;

        cam = Camera.main;
    }

    private void ApplyRandomColor()
    {
        int randomColorIndex = Random.Range(0, color.Length);
        Color baseColor = color[randomColorIndex];
        Color darkerColor = baseColor - new Color(55f / 255f, 55f / 255f, 55f / 255f);

        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer childRenderer = transform.GetChild(i).GetComponent<Renderer>();
            childRenderer.materials[0].color = baseColor;
            childRenderer.materials[1].color = darkerColor;
        }
    }

    private void StartDragging()
    {
        rb.isKinematic = false;
        isDragging = true;
        dragOffset = transform.position - GetMouseWorldPosition();
    }

    private void HandleDragging()
    {
        if (!isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPos = mouseWorldPos + dragOffset;
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        ApplyMovementForce(direction);
        ClampVelocity();
    }

    private void StopDragging()
    {
        isDragging = false;
        SnapToGrid();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    private void ApplyMovementForce(Vector3 direction)
    {
        float distance = direction.magnitude;
        Vector3 force = baseForce * distance * Time.deltaTime * direction.normalized;
        rb.AddForce(force, ForceMode.Force);
    }

    private void ClampVelocity()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void SnapToGrid()
    {
        Vector3 pos = transform.position;
        float snappedX = Mathf.Round(pos.x / gridSize) * gridSize;
        float snappedZ = Mathf.Round(pos.z / gridSize) * gridSize;

        transform.position = new Vector3(snappedX, pos.y, snappedZ);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void InitializeShapeRows()
    {
        if (shapeRows == null || shapeRows.Length != Height)
        {
            CreateNewShapeRows();
        }
        else
        {
            ValidateExistingShapeRows();
        }
    }

    private void CreateNewShapeRows()
    {
        shapeRows = new ShapeRow[Height];
        for (int y = 0; y < Height; y++)
        {
            shapeRows[y] = new ShapeRow();
            shapeRows[y].columns = new bool[Width];
            for (int x = 0; x < Width; x++)
            {
                shapeRows[y].columns[x] = true;
            }
        }
    }

    private void ValidateExistingShapeRows()
    {
        for (int y = 0; y < Height; y++)
        {
            if (shapeRows[y].columns == null || shapeRows[y].columns.Length != Width)
            {
                ResizeShapeRow(y);
            }
        }
    }

    private void ResizeShapeRow(int y)
    {
        bool[] newColumns = new bool[Width];

        if (shapeRows[y].columns != null)
        {
            for (int x = 0; x < Mathf.Min(Width, shapeRows[y].columns.Length); x++)
            {
                newColumns[x] = shapeRows[y].columns[x];
            }
            for (int x = shapeRows[y].columns.Length; x < Width; x++)
            {
                newColumns[x] = true;
            }
        }
        else
        {
            for (int x = 0; x < Width; x++)
            {
                newColumns[x] = true;
            }
        }

        shapeRows[y].columns = newColumns;
    }

    private void ValidateDimensions()
    {
        Width = Mathf.Max(1, Width);
        Height = Mathf.Max(1, Height);
    }
}
