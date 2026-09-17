using System;
using UnityEngine;

public sealed class LevelGenerator : MonoBehaviour
{
    public int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    public GameObject[] tilePrefabs = new GameObject[9];
    public string manualLevelName = "Level01";
    public Camera levelCamera;
    public Transform previewGallery;
    public int GeneratedTileCount { 
        get; 
        private set; 
    }
    public int FullRows { 
        get; 
        private set; 
    }
    public int FullColumns { 
        get; 
        private set; 
    }
    public GameObject GeneratedRoot { 
        get; 
        private set; 
    }
    float lastAspect;

    void Start()
    {
        if (levelCamera == null) {
            levelCamera = Camera.main;
        }
        if (previewGallery == null) {
            previewGallery = GameObject.Find("PreviewGallery")?.transform;
        }
        Regenerate(levelMap);
    }

    public void Regenerate(int[,] quadrant)
    {
        int[,] orientations = TileTopology.Solve(quadrant);
        int rows = quadrant.GetLength(0), columns = quadrant.GetLength(1);
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < columns; c++) 
            {
                int tile = quadrant[r, c];
            }
        }
        GameObject manual = GameObject.Find(manualLevelName);
        if (manual != null) { 
            manual.SetActive(false); 
            Destroy(manual); 
        }
        if (GeneratedRoot != null) { 
            GeneratedRoot.SetActive(false); 
            Destroy(GeneratedRoot); 
        }
        GeneratedRoot = new GameObject("GeneratedLevel01");
        FullRows = rows * 2 - 1; FullColumns = columns * 2; GeneratedTileCount = 0;
        for (int vertical = 0; vertical < 2; vertical++)
            for (int horizontal = 0; horizontal < 2; horizontal++)
            {
                var group = new GameObject((vertical == 0 ? "Top" : "Bottom") + (horizontal == 0 ? "Left" : "Right"));
                group.transform.SetParent(GeneratedRoot.transform, false);
                group.transform.localPosition = new Vector3(horizontal == 0 ? 0 : FullColumns - 1, vertical == 0 ? 0 : -(FullRows - 1), 0);
                group.transform.localScale = new Vector3(horizontal == 0 ? 1 : -1, vertical == 0 ? 1 : -1, 1);
            
                int rowLimit = vertical == 0 ? rows : rows - 1;
                for (int r = 0; r < rowLimit; r++) {
                    for (int c = 0; c < columns; c++)
                    {
                        int tile = quadrant[r, c];
                        if (tile == 0) {
                            continue;
                        }
                        GameObject piece = Instantiate(tilePrefabs[tile], group.transform);
                        piece.name = $"Tile_{r:D2}_{c:D2}_{tile}";
                        piece.transform.localPosition = new Vector3(c, -r, 0);
                        piece.transform.localRotation = Quaternion.Euler(0, 0, TileTopology.IsWall(tile) ? TileTopology.RotationDegrees(tile, orientations[r, c]) : 0);
                        piece.transform.localScale = Vector3.one;
                        GeneratedTileCount++;
                    }
                }
            }
        if (previewGallery != null) previewGallery.position = new Vector3(FullColumns + 2, -1, 0);
        FitCamera();
    }
    void LateUpdate()
    {
        if (levelCamera != null) {
            if (!Mathf.Approximately(lastAspect, levelCamera.aspect)){
                FitCamera();
            }
        }
    }

    public void FitCamera()
    {
        if (levelCamera == null || FullRows == 0) {
            return;
        }
        
        lastAspect = Mathf.Max(0.1f, levelCamera.aspect);
        float left = -2f, right = FullColumns - 1 + (previewGallery == null ? 2f : 13f);
        float top = 3f, bottom = -Mathf.Max(FullRows - 1, previewGallery == null ? 0 : 24) - 2f;
        levelCamera.orthographic = true;
        levelCamera.transform.position = new Vector3((left + right) * .5f, (top + bottom) * .5f, -10);
        levelCamera.orthographicSize = Mathf.Max((top - bottom) * .5f, (right - left) * .5f / lastAspect);
    }
}
