using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float speedRotate = 1.0f;

    private void Start()
    {
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        var translate = new Vector3(0, 0, 0);
        translate.x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        translate.y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 rotation = transform.eulerAngles;

            rotation.y -= speedRotate * Time.deltaTime;

            transform.eulerAngles = rotation;
        }
        if(Input.GetKey(KeyCode.E))
        {
            Vector3 rotation = transform.eulerAngles;

            rotation.y += speedRotate * Time.deltaTime;

            transform.eulerAngles = rotation;
        }
        transform.Translate(translate, Space.Self);
    }
}
