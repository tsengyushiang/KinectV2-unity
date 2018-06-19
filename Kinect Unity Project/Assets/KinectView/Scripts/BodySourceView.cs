using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;


using UnityEngine.UI;
using System;
using System.Linq;

public class BodySourceView : MonoBehaviour 
{
    public Camera mainCamera;

    public GameObject BodyMesh;

    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject jointMesh;

    // Effect
    public GameObject explosion;
    public GameObject smoke;

    private static int Viewer=-1;// 1:lookfront -1:lookforback

    //儲存目前Tracked body 對應的GameObject
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
        
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    private Kinect.JointType[] visialbeJoints = 
        new Kinect.JointType[] { Kinect.JointType.SpineBase,
                                 Kinect.JointType.Neck,
                                 Kinect.JointType.HandLeft,
                                 Kinect.JointType.HandRight,
                                 Kinect.JointType.KneeLeft,
                                 Kinect.JointType.KneeRight };
    void Update ()
    {
       
            //GameObject.Find("/Game/FruitCreater").transform.GetComponent<CreateFruit>().GenerateInterval -= 0.0005f;
            //if (GameObject.Find("/Game/FruitCreater").transform.GetComponent<CreateFruit>().GenerateInterval < 0.001)
            //    GameObject.Find("/Game/FruitCreater").transform.GetComponent<CreateFruit>().GenerateInterval = 0.001f;



        if (BodySourceManager == null)
        {
            return;
        }       
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
      
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {                
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }
      
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                RefreshBodyObject(body, _Bodies[body.TrackingId]);            
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {       
        GameObject body = Instantiate(BodyMesh);
        body.name="Body:" + id;

        body.GetComponent<BodyContorller>().mainCamera = mainCamera;
        body.GetComponent<BodyContorller>().smoke = smoke;
        body.GetComponent<BodyContorller>().explosion = explosion;

        foreach (Kinect.JointType jt in visialbeJoints)
        {
            GameObject jointObj = Instantiate(jointMesh);

            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }


        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {       
        GameObject Barbarian = GameObject.Find(bodyObject.name);      

        
         Barbarian.GetComponent<BodyContorller>().HandLeftState = body.HandLeftState;
         Barbarian.GetComponent<BodyContorller>().HandRightState = body.HandRightState;
        
        foreach (Kinect.JointType jt in visialbeJoints)
        {

            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];               
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            Vector3 SourceJointposition = GetVector3FromJoint(sourceJoint);
            jointObj.localPosition = SourceJointposition;

            // MeshControl
            if (jt == Kinect.JointType.HandLeft)
                Barbarian.GetComponent<BodyContorller>().rightHand = jointObj.gameObject;
            if (jt == Kinect.JointType.HandRight)
                Barbarian.GetComponent<BodyContorller>().leftHand = jointObj.gameObject;
            if (jt == Kinect.JointType.KneeLeft)
                Barbarian.GetComponent<BodyContorller>().rightLeg = jointObj.gameObject;
            if (jt == Kinect.JointType.KneeRight)
                Barbarian.GetComponent<BodyContorller>().leftLeg = jointObj.gameObject;
            if (jt == Kinect.JointType.SpineBase)
            {
                Barbarian.GetComponent<BodyContorller>().MainBody = jointObj.gameObject;
                  
            }
            if (jt == Kinect.JointType.Neck)
                Barbarian.GetComponent<BodyContorller>().Neck = jointObj.gameObject;

        }
    }      
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        float cameraCorrection = -0.36f;
        Vector3 bias = new Vector3(5, 0, 0);

        Vector3 SourceJointposition=new Vector3(joint.Position.X*10 , joint.Position.Y*20, joint.Position.Z*10);
        Vector3 JointPosition =
             new Vector3(SourceJointposition.x * Viewer, //鏡像效果
                          SourceJointposition.y - cameraCorrection * SourceJointposition.z, //修正Kinect視錐造成的Y軸位移
                          0.8f) //2d效果 固定z軸
                                  - bias;//回原點
        return JointPosition;
    }
}
