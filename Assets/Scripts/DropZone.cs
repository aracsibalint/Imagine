using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking;


public class DropZone : NetworkBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData) {
		if (eventData.pointerDrag == null) {
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.EnteringDropZone (this.transform);
		}
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		//Debug.Log("OnPointerExit");
		if (eventData.pointerDrag == null) {
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.LeavingDropZone(this.transform);
		}
	}

    public void disableCardsMovement() {
        foreach (Transform child in transform) {
            child.GetComponent<Draggable>().enabled = false;
        }
    }
	
	public void OnDrop(PointerEventData eventData) {
		//Debug.Log (eventData.pointerDrag.name + " was dropped on " + gameObject.name);

//		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		//if(d != null) {
		//	d.targetDropZone = this.transform;
//		}
	}
	
	/*
	private void Update() {
        if (Input.GetMouseButton(1)) {
            foreach (var item in selectedCards) {
                item.GetComponent<RotateCard>().Rotate();
            }
        }
    }
	*/
	
}
