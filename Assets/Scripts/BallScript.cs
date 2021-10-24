using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BallScript : MonoBehaviour {
    public GameObject wall;
    public GameObject restaurant;
    public TextMeshProUGUI tmp;
    private int score = 0;
    private bool canWin = false;

    private void Start() {
        StartCoroutine(Wait(3));
    }

    IEnumerator Wait(int time) {
        yield return new WaitForSeconds(time);
        tmp.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        other.gameObject.SetActive(false);
        score += 1;

        if (canWin) {
            tmp.text = "You beat the game! Congratulations!";
            tmp.gameObject.SetActive(true);
        }
    }

    private void Update() {
        if (score >= 2) {
            wall.gameObject.SetActive(false);
            canWin = true;
        }
    }
}