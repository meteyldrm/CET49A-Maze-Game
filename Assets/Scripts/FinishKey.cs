using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishKey : MonoBehaviour {
    private TextMeshProUGUI tmp;
    
    private void Start() {
        tmp = GameObject.FindWithTag("FinishText").GetComponent<TextMeshProUGUI>();

        StartCoroutine(T1(tmp));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            StartCoroutine(T2(tmp));
            Destroy(this.gameObject);
        }
    }

    IEnumerator T1(TextMeshProUGUI t) {
        t.text = "You are probably dead...";
        yield return new WaitForSeconds(3);
        t.gameObject.SetActive(false);
    }
    
    IEnumerator T2(TextMeshProUGUI t) {
        t.text = "You are now definitely dead.";
        t.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
