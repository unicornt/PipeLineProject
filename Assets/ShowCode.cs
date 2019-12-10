using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool Open = false;
    public Animator target1, target2;
    public void OnClick()
    {
        Open = !Open;
        target1.SetBool("Open", Open);
        target2.SetBool("Open", Open);
    }
}
