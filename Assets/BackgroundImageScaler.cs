using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImageScaler : MonoBehaviour
{
    int k = 0;
    float sign = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.localScale += new Vector3(sign*0.00001f, sign * 0.00001f, 0);

        k++;
        if(k == 10000)
        {
            sign *= -1.0f;
        }

        k %= 10000;

        //Debug.Log("k scale : " + k);
    }
}
