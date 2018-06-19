using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ground : MonoBehaviour
{

    public GameObject EndCanvas;
    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    private Text gamelife;
    public static int life = 10;
    // Use this for initialization
    void Start()
    {
        gamelife = GameObject.Find("/Canvas/gamelife/").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Fruit")
        {
            //扣命
            life--;
            gamelife.text = "x" + life.ToString();
            Destroy(collision.gameObject);
            //if (life == 2)
            //{
            //    Destroy(life3);
            //}
            //else if (life == 1)
            //{
            //    Destroy(life2);
            //}
            //else 
            if (life == 0)
            {
                //Destroy(life1);
                EndCanvas.SetActive(true);
                Instantiate(EndCanvas, Vector2.zero, Quaternion.identity);

                //輸了  停止遊戲
            }
            else if (life < 0)
            {
                gamelife.text = "x0";
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
