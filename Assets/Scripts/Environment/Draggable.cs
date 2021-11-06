using UnityEngine;
using UnityEngine.EventSystems;

namespace Environment {
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Vector2 clickOffset = Vector2.zero;
        private Camera mainCamera;

        [SerializeField] private bool canDrag = false;
        [SerializeField] public Color dragColor;

        // Use this for initialization
        void Start()
        {
            // //Comment this Section if EventSystem system is already in the Scene
            // addEventSystem();
            
            mainCamera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            clickOffset = transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
            if(canDrag) this.GetComponent<SpriteRenderer>().color = dragColor;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Use Offset To Prevent Sprite from Jumping to where the finger is
            Vector2 tempVec = (Vector2) Camera.main.ScreenToWorldPoint(eventData.position) + clickOffset;

            
            if(canDrag) transform.position = tempVec;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void doDrag(bool state) {
            canDrag = state;
        }
    }
}
