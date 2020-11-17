using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CityTextManager : Singleton<CityTextManager>, IPointerClickHandler
{
    // Start is called before the first frame update
    [SerializeField] private UnityEvent unityEvent;
    [SerializeField] private Color colorChangeOn;
    private TextMeshPro textComponent;
    public bool changed = false;

    void Start()
    {
        textComponent = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        unityEvent.Invoke();
    }

    public void onClick()
    {
        textComponent.text = System.String.Format("<mark={0}>{1}</mark>", colorChangeOn.ToString(), textComponent.text);
        changed = true;
    }
}
