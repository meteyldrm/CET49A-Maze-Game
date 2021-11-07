using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Environment {
    public class MovableMaterialController : MonoBehaviour {
        [SerializeField] private Color droneActiveColor;
        [SerializeField] private Color droneSelectedColor;

        private Coroutine droneSetActive;
        private Coroutine droneSetPassive;

        private void Start() {
            StartCoroutine(startCoroutine());
        }

        public void SetMovableState(bool state) {
            if (state) {
                if(droneSetPassive != null) StopCoroutine(droneSetPassive);
                droneSetActive = StartCoroutine(droneActive());
            } else {
                if(droneSetActive != null) StopCoroutine(droneSetActive);
                droneSetPassive = StartCoroutine(dronePassive());
            }
        }

        IEnumerator droneActive() {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<Draggable>().doDrag(true);
            }

            yield return null;
        }
    
        IEnumerator dronePassive() {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<Draggable>().doDrag(false);
            }

            yield return null;
        }

        IEnumerator startCoroutine() {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<Draggable>().dragColor = droneSelectedColor;
                child.gameObject.GetComponent<Draggable>().draggableColor = droneActiveColor;
            }
            
            yield return null;
        }
    
    }
}
