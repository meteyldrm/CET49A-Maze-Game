using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishKey : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI tmp;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            StartCoroutine(T2(tmp));
            Destroy(this.gameObject);
        }
    }
    
    IEnumerator T2(TextMeshProUGUI t) {
        t.text = "The secrets of the universe are yours. You have won.";
        t.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
