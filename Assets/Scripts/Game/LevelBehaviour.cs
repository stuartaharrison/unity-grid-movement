using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour {

    private readonly int[,] _tempMap = new int[12, 12] {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0 },
        { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0 },
        { 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0 },
        { 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0 },
        { 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    public GameObject GridManagerPrefab;
    public GameObject CharacterPrefab;
    
    void Awake() {
        // generate the grid for movement
        if (GridManagerPrefab) {
            GenerateLevel();
        } else {
            print("Missing Grid Manager, please assign.");
        }

        // place character/player into the world
        if (CharacterPrefab) {
            GeneratePlayer();
        } else {
            print("Missing Character, please assign.");
        }

        // setup camera
        SetupCamera();
    }

    void GeneratePlayer() {
        // todo; we want to use our level id or something to place player/character into world
        Instantiate(CharacterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void GenerateLevel() {
        // create our grid manager object into the world
        GameObject gridManager = Instantiate(GridManagerPrefab);
        gridManager.name = "Grid Manager";

        // todo; want to grab some kind of game-manager object and load map based on level id

        gridManager.GetComponent<GridBehaviour>().GenerateGrid(_tempMap, new Vector3(0, 0, 0));
    }

    void SetupCamera() {
        // we just want to grab the main camera and move it to be within view for now
        // get the center of the grid
        var rows = _tempMap.GetLength(1);
        var columns = _tempMap.GetLength(0);

        Camera.main.transform.position = new Vector3(columns / 2, 20, rows / 2);
    }
}
