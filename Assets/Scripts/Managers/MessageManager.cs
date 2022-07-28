using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MessageManager : Singleton<MessageManager>
{
    public GameObject prefab;
    public ScrollView scrollView;
    public Transform msgHolder;
    //bool updatingScroll = false;

    public void AddMessage(string msgText)
    {
        TextMeshProUGUI tmpT = Instantiate(prefab, msgHolder).GetComponent<TextMeshProUGUI>();
        tmpT.text = msgText;

        //if(!updatingScroll) StartCoroutine(UpdateScrollView());
    }

    //public IEnumerator UpdateScrollView() 
    //{
    //    updatingScroll = true;
    //    yield return new WaitForEndOfFrame();
    //    Canvas.ForceUpdateCanvases();
    //    scrollView.scrollOffset = new Vector2(0,0);
    //    scrollView.verticalScroller.value = 0;
    //    Canvas.ForceUpdateCanvases();
    //    updatingScroll = false;
    //}
}
