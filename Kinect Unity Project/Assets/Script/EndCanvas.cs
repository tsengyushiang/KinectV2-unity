using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCanvas : MonoBehaviour
{
    public GameObject fruit;
    void OnEnable()
    {
        //print(11);
        fruit.SetActive(false);
    }

    void OnDisable()
    {
        ground.life = 10;
        BodyContorller.ExistBodyCount = 0;
        //print(22);
        //fruit.SetActive(true);
    }
}