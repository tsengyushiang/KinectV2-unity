using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFruit : MonoBehaviour {

    public GameObject[] Fruits;
    public float GenerateInterval;
    public int  rightBound;
    public int lefBound;
	// Use this for initialization
	void Start () {
        InvokeRepeating("GererateFruit",3f, GenerateInterval);
    }

	// Update is called once per frame
	void Update () {        
       }

    void GererateFruit()
    {
        GameObject tmp = Instantiate(Fruits[(int)Random.Range(0, Fruits.Length)],
            new Vector3(transform.position.x+Random.Range(lefBound, rightBound), transform.position.y,
            transform.position.z), transform.rotation);   

        //tmp.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward + Vector3.up) * 500);
        tmp.gameObject.GetComponent<Rigidbody>().AddTorque((Vector3.forward + Vector3.up) * 500);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

}
