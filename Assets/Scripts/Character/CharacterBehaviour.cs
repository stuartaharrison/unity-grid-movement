using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour {

    private Stack<GameObject> _currentPath = new Stack<GameObject>();

    [Header("Movement Settings")]
    public int MaxMovements = 100;
    public float MoveSpeed = 10;

    void Update() {
        // check movement stack to see if we need to move the character
        if (_currentPath.Count > 0) {
            // peek at the next targets location & move towards
            var target = _currentPath.Peek();
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, MoveSpeed * Time.deltaTime);

            // remove from the path stack when we reach desired location
            if (target.transform.position.x == gameObject.transform.position.x && target.transform.position.z == gameObject.transform.position.z) {
                _currentPath.Pop();
            }
        }
    }

    public void MoveToPosition(int toX, int toY) {
        // find our grid manager in our scene
        var gridManager = GameObject.FindGameObjectWithTag("GridManager");
        if (gridManager) {
            // use our grid manager to calculate the best route to a specific position
            var path = gridManager.GetComponent<GridBehaviour>().GetPathToPosition(gameObject.transform, toX, toY, MaxMovements);
            for (int i = 0; i < path.Count; i++) {
                // push the values into our stack
                // then our update function within this script will begin to move our character!
                _currentPath.Push(path[i]);
            }
        } else {
            print("Could not find GridManager object within scene.");
        }
    }

    public void MoveToPosition(Transform targetPosition) {
        // find our grid manager in our scene
        var gridManager = GameObject.FindGameObjectWithTag("GridManager");
        if (gridManager) {
            // use our grid manager to calculate the best route to a specific position
            var path = gridManager.GetComponent<GridBehaviour>().GetPathToPosition(gameObject.transform, targetPosition, MaxMovements);
            for (int i = 0; i < path.Count; i++) {
                // push the values into our stack
                // then our update function within this script will begin to move our character!
                _currentPath.Push(path[i]);
            }
        } else {
            print("Could not find GridManager object within scene.");
        }
    }
}
