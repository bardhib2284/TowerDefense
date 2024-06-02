using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragable : MonoBehaviour
{
    Vector3 StartPosition;
    Vector3 MousePositionOffset;
    public bool WantToMerge = false;
    private void Start() {
        StartPosition = transform.position;
    }
    private Vector3 GetMouseWorldPosition() {
        var modifiedPosition = new Vector3(Input.mousePosition.x,Input.mousePosition.y,-2);
        return Camera.main.ScreenToWorldPoint(modifiedPosition);
    }

    void OnMouseDown() {
        WantToMerge = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        MousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    void OnCollisionExit2D(Collision2D collision) {
        if(WantToMerge) {
            this.WantToMerge = false;
            collision.transform.GetComponent<Dragable>().WantToMerge = false;
            Debug.Log(gameObject.name + " wants to merge with : " + collision.gameObject.name);
            if(gameObject.name == collision.gameObject.name) {
                if(gameObject.GetComponentInChildren<Character>().MergeLevel == collision.gameObject.GetComponentInChildren<Character>().MergeLevel) {
                    gameObject.SetActive(false);
                    collision.gameObject.SetActive(false);
                    var mergeLevel = gameObject.GetComponentInChildren<Character>().MergeLevel + 1;
                    var spawnPoint = collision.gameObject.transform.parent;
                    FindObjectOfType<GameManager>().MergeCharacters(this.gameObject, collision.gameObject, spawnPoint, mergeLevel);
                    return;
                }
                gameObject.transform.position = new Vector3(StartPosition.x, StartPosition.y, 0);
            }
            gameObject.transform.position = new Vector3(StartPosition.x, StartPosition.y, 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        
    }
    private void OnMouseDrag() {
        WantToMerge = false;
        var newPosition = GetMouseWorldPosition() + MousePositionOffset;
        gameObject.transform.position = new Vector3(newPosition.x,newPosition.y,-2);
    }

    private void OnMouseUp() {
        WantToMerge = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.transform.position = new Vector3(StartPosition.x, StartPosition.y, 0);

    }
}
