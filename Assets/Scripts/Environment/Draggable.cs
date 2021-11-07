using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Environment {
    public class Draggable : MonoBehaviour
    {
        Vector2 clickOffset = Vector2.zero;
        private Camera mainCamera;

        [SerializeField] private bool canDrag = false;
        [SerializeField] public Color draggableColor;
        [SerializeField] public Color dragColor;
        private bool hasCollisionOverlap;

        [SerializeField] private Vector2 localPositionBounds;

        private bool doOnceOnDragStart;
        
        // Use this for initialization
        void Start()
        {
            // //Comment this Section if EventSystem system is already in the Scene
            // addEventSystem();
            
            mainCamera = Camera.main;
        }

        private void OnMouseEnter() {
            hasCollisionOverlap = true;
        }

        private void OnMouseExit() {
            hasCollisionOverlap = false;
        }

        private void OnMouseDrag() {
            if (hasCollisionOverlap && canDrag) {
                if (!doOnceOnDragStart) {
                    this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                    clickOffset =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    doOnceOnDragStart = true;
                    print(clickOffset);
                }
                Vector2 tempVec = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickOffset;
                this.GetComponent<SpriteRenderer>().color = dragColor;
                transform.position = tempVec;
            }
        }

        private void OnMouseUp() {
            if (canDrag) {
                this.GetComponent<SpriteRenderer>().color = draggableColor;
            }
        }

        public void doDrag(bool state) {
            canDrag = state;
            if (state) {
                this.GetComponent<SpriteRenderer>().color = draggableColor;
            } else {
                this.GetComponent<SpriteRenderer>().color = Color.white;
                clickOffset = Vector2.zero;
                this.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}
