using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitiesViewManager : Singleton<CitiesViewManager>
{
    // Start is called before the first frame update

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddNewCityName(string text)
    {
        Debug.Log("Add cname;;;;");
        var content = this.transform.GetChild(0).GetChild(0).gameObject;
        var textPrefab = content.transform.GetChild(0).gameObject;
        var contentRect = content.GetComponent<RectTransform>();
        //contentRect.size = new Vector2(contentRect.size.x, contentRect.size.y + 20);
        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentRect.rect.height + 20);
        var textObject = Instantiate(textPrefab);
        textObject.transform.SetParent(content.transform, false);
        textObject.GetComponent<Text>().text = text;
    }
}
