using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extras : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyObject() {
        Destroy(gameObject);
    }

    public void DestroyParentObject() {
        Destroy(this.transform.parent.gameObject);
    }
}
