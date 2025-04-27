using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GridDraw gridDraw;

    public void BtnSpawn()
    {
        gridDraw.StartTheSystemJarvis();
    }
}
