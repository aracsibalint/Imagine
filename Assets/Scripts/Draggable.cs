using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Draggable : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject Hand;

    Vector3 rotationEuler;
    bool IsOnDrag = false;
    private Transform originalParent = null;
    private int originalIndex;
    private bool returnToOriginal = false;

    //	private Transform placeholderParent = null;
    static GameObject placeholder = null;

    void Start() {
        if (isServer) {
            if (placeholder == null) {
                placeholder = GameObject.FindGameObjectWithTag("DropIndicator");
                placeholder.SetActive(false);
            }
        }

        if (!isServer) {
            this.GetComponent<LayoutElement>().enabled = false;
            this.transform.SetParent(GameObject.Find("Canvas").transform);
            this.transform.position = new Vector3(-269, 648, 0);
            //GameObject.Find("Hand").SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (isServer) {
            originalParent = this.transform.parent;
            originalIndex = this.transform.GetSiblingIndex();
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);

            // same location as the dragable
            placeholder.SetActive(true);
            placeholder.transform.SetParent(originalParent);
            placeholder.transform.SetSiblingIndex(originalIndex);

            //		placeholderParent = originalParent;

            Canvas canvas = (Canvas)GameObject.FindObjectOfType(typeof(Canvas));

            this.transform.SetParent(canvas.transform);
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (isServer) {
            IsOnDrag = true;
            this.transform.position = eventData.position;
            var parent = placeholder.transform.parent;
            if (parent != null && !returnToOriginal) {
                int newSiblingIndex = parent.childCount;

                for (int i = 0; i < parent.childCount; i++) {
                    if (this.transform.position.x < parent.GetChild(i).position.x) {

                        newSiblingIndex = i;

                        if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                            newSiblingIndex--;

                        break;
                    }
                }

                placeholder.transform.SetSiblingIndex(newSiblingIndex);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (isServer) { 
        IsOnDrag = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        this.transform.SetParent(placeholder.transform.parent);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);

        Canvas canvas = (Canvas)GameObject.FindObjectOfType(typeof(Canvas));
        placeholder.transform.SetParent(canvas.transform);
        placeholder.SetActive(false);
        }
    }

    public void EnteringDropZone(Transform dropZone) {
        //		Debug.Log ("entering drop zone " + dropZone.name);
        if (isServer) { 
        returnToOriginal = false;
        placeholder.transform.SetParent(dropZone);
        placeholder.transform.SetSiblingIndex(0);
        }
    }

    public void LeavingDropZone(Transform dropZone) {
        if (isServer) { 
        //		Debug.Log ("Leaving drop zone " + dropZone.name);
        placeholder.transform.SetParent(originalParent);
        //		Debug.Log("Set new sibling index " + originalIndex);
        placeholder.transform.SetSiblingIndex(originalIndex);
        returnToOriginal = true;
        }
    }

    private void Update() {
        if (isServer) {
            if (IsOnDrag && (Input.GetMouseButton(1) || Input.GetKey("left"))) {
                rotationEuler += Vector3.forward * 80 * Time.deltaTime; //increment 30 degrees every second
                transform.rotation = Quaternion.Euler(rotationEuler);

                //To convert Quaternion -> Euler, use eulerAngles
                print(transform.rotation.eulerAngles);
            }
            if (IsOnDrag && Input.GetKey("right")) {
                rotationEuler -= Vector3.forward * 80 * Time.deltaTime; //increment 30 degrees every second
                transform.rotation = Quaternion.Euler(rotationEuler);

                //To convert Quaternion -> Euler, use eulerAngles
                print(transform.rotation.eulerAngles);
            }


            if (transform.parent.name == "Hand") {
                transform.Rotate(0, 0, 0);
            }
        }
    }
}