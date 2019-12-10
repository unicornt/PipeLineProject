using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerHighlight : MonoBehaviour
,IPointerExitHandler,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static readonly int View = Animator.StringToHash("View");
    private Vector2 deltaV = new Vector2(0,25);
   private void OpenView(string target)
    {
        Text text = GameObject.Find("UICanvas/CodePanel/" + target).GetComponent<Text>();
        text.rectTransform.offsetMax += deltaV;
        text.rectTransform.offsetMin -= deltaV;
        text.fontSize += 30;
        //target.SetBool(View, true);
    }
    
    private void CloseView(string target)
    {
        Text text = GameObject.Find("UICanvas/CodePanel/" + target).GetComponent<Text>();
        text.rectTransform.offsetMax -= deltaV;
        text.rectTransform.offsetMin += deltaV;
        text.fontSize -= 30;
    }

    public string thisName;

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseView(thisName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OpenView(thisName);
    }
}
