using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera camera;

    void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        var positionY = camera.transform.position.y;
        var translate = transform.position;
        camera.transform.position = new Vector3(translate.x, positionY, translate.z);
    }
}
