using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Animator target;
    // Update is called once per frame
    void Update()
    {
        
    }

    private bool viewEnable = false;
    public void BeginTextView()
    {
        viewEnable = !viewEnable;
        target.SetBool("View", viewEnable);
    }
}
