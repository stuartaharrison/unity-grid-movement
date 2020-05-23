using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class GridBehaviour : MonoBehaviour {

    public enum Direction {
        up,
        right,
        down,
        left
    }

    public enum GridType {
        passable,
        nonPassable
    }

    public int Rows = 0;
    public int Columns = 0;
    public int Scale = 1;
    public GameObject GridPrefab;    
    public GameObject[,] GridArray;

    void SetDistance(int fromX, int fromY) {
        // reset the current grid to make them all 'unvisited'
        foreach (GameObject obj in GridArray) {
            if (obj) {
                obj.GetComponent<GridStats>().Visited = -1;
            }
        }

        // set the start position as steps to get there == 0
        GridArray[fromX, fromY].GetComponent<GridStats>().Visited = 0;

        // work out the number of steps to get to each grid position object
        for (int step = 1; step < Rows * Columns; step++) {
            foreach (GameObject obj in GridArray) {
                if (obj && obj.GetComponent<GridStats>().Visited == step - 1) {
                    TestFourDirections(obj.GetComponent<GridStats>().X, obj.GetComponent<GridStats>().Y, step);
                }
            }
        }
    }
    
    void SetVisited(int x, int y, int step) {
        if (GridArray[x, y]) {
            GridArray[x, y].GetComponent<GridStats>().Visited = step;
        }
    }

    void TestFourDirections(int x, int y, int step) {
        if (TestDirection(x, y, -1, Direction.up)) {
            SetVisited(x, y + 1, step);
        }

        if (TestDirection(x, y, -1, Direction.right)) {
            SetVisited(x + 1, y, step);
        }

        if (TestDirection(x, y, -1, Direction.down)) {
            SetVisited(x, y - 1, step);
        }

        if (TestDirection(x, y, -1, Direction.left)) {
            SetVisited(x - 1, y, step);
        }
    }

    bool TestDirection(int x, int y, int step, Direction direction) {
        switch (direction) {
            case Direction.up:
                return (y + 1 < Rows && GridArray[x, y + 1] && GridArray[x, y + 1].GetComponent<GridStats>().Visited == step);
            case Direction.right:
                return (x + 1 < Columns && GridArray[x + 1, y] && GridArray[x + 1, y].GetComponent<GridStats>().Visited == step);
            case Direction.down:
                return (y - 1 > -1 && GridArray[x, y - 1] && GridArray[x, y - 1].GetComponent<GridStats>().Visited == step);
            case Direction.left:
                return (x - 1 > -1 && GridArray[x - 1, y] && GridArray[x - 1, y].GetComponent<GridStats>().Visited == step);
            default:
                return false;
        }
    }

    GameObject FindClosest(Transform targetLocation, List<GameObject> list) {
        float currentDistance = Scale * Rows * Columns;
        int indexNumber = 0;
        
        for (int  i =0; i < list.Count; i++) {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance) {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }

        return list[indexNumber];
    }

    List<GameObject> GetPath(int toX, int toY) {
        int step;
        int x = toX;
        int y = toY;

        var path = new List<GameObject>();
        var tempList = new List<GameObject>();

        if (GridArray[toX, toY] && GridArray[toX, toY].GetComponent<GridStats>().Visited > 0) {
            path.Add(GridArray[x, y]);
            step = GridArray[x, y].GetComponent<GridStats>().Visited - 1;
        } else {
            print("Can't reach the desired location.");
            return null;
        }

        for (int i = step; step > -1; step--) {
            if (TestDirection(x, y, step, Direction.up)) {
                tempList.Add(GridArray[x, y + 1]);
            }
            if (TestDirection(x, y, step, Direction.right)) {
                tempList.Add(GridArray[x + 1, y]);
            }
            if (TestDirection(x, y, step, Direction.down)) {
                tempList.Add(GridArray[x, y - 1]);
            }
            if (TestDirection(x, y, step, Direction.left)) {
                tempList.Add(GridArray[x - 1, y]);
            }

            GameObject tempObj = FindClosest(GridArray[toX, toY].transform, tempList);
            path.Add(tempObj);

            x = tempObj.GetComponent<GridStats>().X;
            y = tempObj.GetComponent<GridStats>().Y;

            tempList.Clear();
        }

        return path;
    }

    List<GameObject> GetPath(int fromX, int fromY, int toX, int toY) {
        SetDistance(fromX, fromY);
        return GetPath(toX, toY);
    }

    public void GenerateGrid(int[,] map, Vector3 startPosition) {
        // check that we can instantiate a grid object
        if (!GridPrefab) {
            print("Missing GridPrefab, please assign.");
            return;
        }

        // assign the rows & columns properties based on our grid map size
        Rows = map.GetLength(1);
        Columns = map.GetLength(0);

        // setup the grid array that will hold references to our grid objects
        GridArray = new GameObject[Columns, Rows];

        // build our grid map for our level based on the map object values
        for (int i = 0; i < Columns; i++) {
            for (int j = 0; j < Rows; j++) {
                // find out what should be at this position
                int mapObj = map[i, j];
                switch (mapObj) {
                    case (int)GridType.passable:
                        // instantiate our grid object & assign position
                        GameObject obj = Instantiate(GridPrefab, new Vector3(startPosition.x + Scale * i, startPosition.y, startPosition.z + Scale * j), Quaternion.identity);
                        obj.name = $"grid-x{i}-y{j}";
                        obj.transform.SetParent(gameObject.transform);
                        obj.GetComponent<GridStats>().X = i;
                        obj.GetComponent<GridStats>().Y = j;

                        // add to grid once instantiated
                        GridArray[i, j] = obj;
                        break;
                    case (int)GridType.nonPassable:
                    default:
                        break;
                }
            }
        }
    }

    public List<GameObject> GetPathToPosition(Transform from, Transform to, int maximumSteps) {
        var startX = (int)from.position.x;
        var startY = (int)from.position.z;
        var endX = (int)to.position.x;
        var endY = (int)to.position.y;

        return GetPath(startX, startY, endX, endY);
    }

    public List<GameObject> GetPathToPosition(Transform from, int toX, int toY, int maximumSteps) {
        var startX = (int)from.position.x;
        var startY = (int)from.position.z;

        return GetPath(startX, startY, toX, toY);
    }

    public List<GameObject> GetAvailablePositions(Transform currentPosition, int maximumSteps) {
        var startX = (int)currentPosition.position.x;
        var startY = (int)currentPosition.position.z;

        SetDistance(startX, startY);

        return null; // todo;
    }
}
