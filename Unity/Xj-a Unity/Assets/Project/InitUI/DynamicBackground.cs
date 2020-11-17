using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    [SerializeField] private float speed = 0.10f;
    [SerializeField] private float delta = 0.3f;
    [SerializeField] private GameObject background;
    private Vector2 normalSpeed;
    private Vector2 bounds;
    private Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rectBackground = background.GetComponent<SpriteRenderer>().bounds.size;
        Vector3 rectCamera = GetComponent<Collider2D>().bounds.size;
        bounds = new Vector2(rectBackground.x - delta, rectBackground.y - delta);
        size = new Vector2(rectCamera.x, rectCamera.y);
        Debug.Log(size.x);
        Debug.Log(size.y);
        normalSpeed = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 previousPos = transform.position;
        transform.position = new Vector3(normalSpeed.x * speed * Time.deltaTime + previousPos.x, normalSpeed.y * speed * Time.deltaTime + previousPos.y, previousPos.z);
        if(transform.position.x - size.x/2 <= -bounds.x/2 || transform.position.x + size.x / 2 >= bounds.x / 2)
        {
            float delta = Random.Range(-0.1f, 0.1f);
            if(Mathf.Abs(normalSpeed.x + delta) > 1.00f)
            {
                normalSpeed.x -= delta;
            }
            else
            {
                
                normalSpeed.x += delta;
            }
            normalSpeed.x = -normalSpeed.x;
        }
        if (transform.position.y - size.y / 2 <= -bounds.y / 2 || transform.position.y + size.y / 2 >= bounds.y / 2)
        {
            float delta = Random.Range(-0.1f, 0.1f);
            if (Mathf.Abs(normalSpeed.y + delta) > 1.00f)
            {

                normalSpeed.y -= delta;
            }
            else
            {
                normalSpeed.y += delta;
            }
            normalSpeed.y = -normalSpeed.y;
        }
    }
}
