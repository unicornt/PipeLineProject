using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorHappens : MonoBehaviour
{
    static public Text errorText = GameObject.Find("DialogCanvas/ErrorDialog/Text").GetComponent<Text>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void OnClick(string s)
    {
        errorText.text = s;
    }
}
