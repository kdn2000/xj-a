using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CitiesAddTextManager : Singleton<CitiesAddTextManager>
{
    // Start is called before the first frame update
    private InputField textField;

    public string Input
    {
        get
        {
            return Instance.textField.text;
        }
    }

    void Start()
    {
        textField = this.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
