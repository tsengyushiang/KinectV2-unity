using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


public class Flames : MonoBehaviour {  

    private void OnParticleCollision(GameObject other)
    {
        SendMessageUpwards("FlamesHit", other);
    }
}
