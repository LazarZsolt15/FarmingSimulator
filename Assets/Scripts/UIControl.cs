using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControl : MonoBehaviour
{

    public int score = 0;

    void Start()
    {
        
    }

    
    void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score;

    }
 
}
