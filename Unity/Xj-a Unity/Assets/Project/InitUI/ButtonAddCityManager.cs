using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAddCityManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Button button;

    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onClick() => CitiesViewManager.Instance.AddNewCityName(CitiesAddTextManager.Instance.Input);
}
