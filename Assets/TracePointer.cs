using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class TracePointer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Toggle target;
    public ParticleSystem targetPs;
    public Text text;
    public Animator btnpanel;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {    
            Vector2 moveToward = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPs.transform.position = moveToward;
            targetPs.Play();
        }

        if (target.isOn == true)
        {
            if (btnpanel.GetBool("Move") == false) btnpanel.SetBool("Move", true);
        }
        else
        {
            Vector2 mousePosition = Input.mousePosition;
            if(mousePosition.x > UnityEngine.Screen.width - 100) btnpanel.SetBool("Move", true);
            if(mousePosition.x < UnityEngine.Screen.width - 250) btnpanel.SetBool("Move", false);
        }
    }
}
