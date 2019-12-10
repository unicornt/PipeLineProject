using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToChangeValue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onclick()
    {
        PretendToRunAFunction();
    }

    static public string[] stageFull = {"Fetch", "Decode", "Excute", "Memory", "Write"};
    static public string[] stageshort = {"F", "D", "E", "M", "W"};
    static public void ChangeValue(int stage, string target, string targetValue)
    {
        Text text = GameObject.Find("UICanvas/Stage/" + stageFull[stage] + "Stage/" + target + "/value").GetComponent<Text>();
        if (text != null)
        {
            text.color = Color.white;
            text.text = targetValue;
        }
    }

    static public void ChangeCode(int stage, string targetValue)
    {
        print(targetValue);
        string deleteSpaceString;
        deleteSpaceString = "";
        for (int i = 0; i < targetValue.Length; i++)
        {
            if (targetValue[i] != ' ') deleteSpaceString += targetValue[i];
        }
        Text text = GameObject.Find("UICanvas/CodePanel/" + stageshort[stage] + "code").GetComponent<Text>();
        if (text != null)
        {
            text.text = deleteSpaceString;
        }
    }
    private void PretendToRunAFunction()
    {
        ChangeValue(1, "rA", "haha");
    }
}
