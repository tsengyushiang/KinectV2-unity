using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class BodyContorller : MonoBehaviour {

    // GUI
    private Text Score;
    private Text gamelife;
    public static int ExistBodyCount = 0;
    public static int DeadBodyCount = 0;

    public Camera mainCamera;
    // Kinect Body Control
    public GameObject MainBody;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject leftLeg;
    public GameObject rightLeg;
    public GameObject Neck;
    public Windows.Kinect.HandState HandRightState;
    public Windows.Kinect.HandState HandLeftState;

    // Animation
    public GameObject explosion;
    public GameObject smoke;

    //
    float HP=100;
    float MP=100;
    float EXP = 0;
    
    void Start ()
    {
        //Camera for world Text
        this.gameObject.transform.Find("Main/Canvas").GetComponent<Canvas>().worldCamera=mainCamera;

        //Body Index
        this.gameObject.transform.Find("Main/Canvas/Name").GetComponent<Text>().text =ExistBodyCount.ToString();
        ExistBodyCount++;

        //Information
        Score = GameObject.Find("/Canvas/Scores/").GetComponent<Text>();
        gamelife = GameObject.Find("/Canvas/gamelife/").GetComponent<Text>();
        this.gameObject.transform.Find("Main/Canvas/HP").GetComponent<Slider>().value = HP;
        this.gameObject.transform.Find("Main/Canvas/MP").GetComponent<Slider>().value = MP;
        this.gameObject.transform.Find("Main/Canvas/EXP").GetComponent<Slider>().value = EXP;
     }
	
	// Update is called once per frame
	void Update () {
        GameObject s = GameObject.Find("/" + name + "/Main/DeformationSystem/Root_M/SpineControll/");

        //Information----------------------------------------------------------------------------------------------
        this.gameObject.transform.Find("Main/Canvas/HP").GetComponent<Slider>().value = HP;
        this.gameObject.transform.Find("Main/Canvas/MP").GetComponent<Slider>().value = MP;
        this.gameObject.transform.Find("Main/Canvas/EXP").GetComponent<Slider>().value = EXP;

        
        //BodyContorl----------------------------------------------------------------------------------------------
        if (MainBody == null) return;
        if (leftHand == null) return;
        if (rightHand == null) return;
        if (rightLeg == null) return;
        if (leftLeg == null) return;
        if (Neck == null) return;

     
        GameObject lh = s.gameObject.transform.Find("Spine1_M/Chest_M/Scapula_L/ControllLeftHand").gameObject;
        GameObject rh = s.gameObject.transform.Find("Spine1_M/Chest_M/Scapula_R/ControllRightHand").gameObject;
        GameObject ll = GameObject.Find("/" + name + "/Main/DeformationSystem/Root_M/Hip_L/ControllLeftLeg");
        GameObject rl = GameObject.Find("/" + name + "/Main/DeformationSystem/Root_M/Hip_R/ControllRightLeg");
        GameObject b = GameObject.Find("/" + name + "Main");

        lh.transform.LookAt(leftHand.transform, lh.transform.up);
        rh.transform.LookAt(rightHand.transform, rh.transform.up);      
        ll.transform.LookAt(leftLeg.transform, ll.transform.up);
        rl.transform.LookAt(rightLeg.transform, rl.transform.up);

        if (Neck.transform.position.y > s.gameObject.transform.position.y)
        {            
            s.transform.LookAt(Neck.transform, s.transform.up);
        }

        
         b.transform.position =new Vector3(MainBody.transform.position.x,
                                            0,
                                              MainBody.transform.position.z);

        // weapon controll---------------------------------------------------------------------------------------
        GameObject leftHandFireGun = lh.gameObject.transform.Find("Flames").gameObject;
        if (HandRightState == Windows.Kinect.HandState.Closed && MP>0)
        {
            leftHandFireGun.GetComponent<ParticleSystem>().Play();
            MP-=0.5F;
        }
        else
        {
            leftHandFireGun.GetComponent<ParticleSystem>().Stop();
        }       

    }

    void AddScore()
    {
        try
        {
            string temp = Score.text.ToString();            

            int CurrentScore = Int32.Parse(temp);
            CurrentScore++;
            Score.text = CurrentScore.ToString() ;
        }
        catch (FormatException e)
        {
            print(e.Message);
            Score.text = "*";
        }
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if (ground.life>0)
        {
            if (collision.transform.tag == "Fruit")
            {
                //print(3);
                GameObject Animation = Instantiate(explosion);
                Animation.transform.position = collision.transform.position;
                Destroy(Animation, 0.5f);

                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2000);
                collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Destroy(collision.gameObject);

                AddScore();
                HP--;
                if (MP < 100)
                    MP += 10;
            }
            else if (collision.transform.tag == "heart")
            {
                Destroy(collision.gameObject);
                ground.life++;
                gamelife.text = "x" + ground.life.ToString();
            }
            else if (collision.transform.tag == "spell")
            {

                //print(1);
            }
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (HandLeftState == Windows.Kinect.HandState.Closed)
        {
            if (collision.transform.tag == "Restart")
            {
                //print(1);
                Application.LoadLevel(0);
            }
            else if (collision.transform.tag == "End")
            {
                //print(2);
                Application.Quit();
            }
        }
    }

    public void FlamesHit(GameObject other)
    {
        if (other.transform.tag == "Fruit")
        {
            EXP+=10f;
            GameObject Animation = Instantiate(smoke);
            Animation.transform.position = other.transform.localPosition;
            Destroy(Animation, 1f);

            Destroy(other);

            AddScore();
        }
    }
    

}
