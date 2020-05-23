using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBehaviour : MonoBehaviour {

    [Header("XY Inputs")]
    public InputField XInput;
    public InputField YInput;

    public void MoveToPosition() {
        // get values of x and y inputs
        int toX = int.Parse(XInput.text);
        int toY = int.Parse(YInput.text);

        print($"Go to: ({XInput.text}, {YInput.text})");

        // get the player on the field
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) {
            player.GetComponent<CharacterBehaviour>().MoveToPosition(toX, toY);
        }
    }
}
