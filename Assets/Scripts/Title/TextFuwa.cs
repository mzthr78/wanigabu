using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFuwa : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();

        StartCoroutine(Fuwa());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fuwa()
    {
        yield return null;
    }
}
