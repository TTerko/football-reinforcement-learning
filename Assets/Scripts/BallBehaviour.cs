using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallBehaviour : MonoBehaviour
{
    public Rigidbody BallRigidbody;

    public Action OnScored = delegate { };
    public Action OnMissed = delegate { };
    public Action<GameObject> OnGuideReached = delegate { };
    
    void Start()
    {
        this.BallRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            OnMissed.Invoke();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Guide")
        {
            OnGuideReached.Invoke(other.gameObject);
        }
        
        if (other.gameObject.tag == "Goal")
        {
            OnScored.Invoke();
        }
    }
}
