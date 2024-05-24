using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SphereAgent : Agent
{
    // create ml agent that will move towards the target
    public BallBehaviour ball;
    
    public float speed = 3.0f;
    public float horizontalSpeed = 3.0f;
    public float rotationSpeed = 3.0f;
    
    private Rigidbody rb;

    private List<GameObject> Guides = new List<GameObject>();
    
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        
        ball.OnScored += Ball_OnScored;
        ball.OnMissed += Ball_OnMissed;
        ball.OnGuideReached += Ball_OnGuideReached;
    }
    
    private void Ball_OnScored()
    {
        AddReward(1.0f);
        EndEpisode();
    }
    
    private void Ball_OnMissed()
    {
        // AddReward(-1.0f);
        // EndEpisode();
    }

    private void Ball_OnGuideReached(GameObject guide)
    {
        if (!Guides.Contains(guide))
        {
            Guides.Add(guide);
            guide.SetActive(false);
        }
        
        AddReward(0.1f);
    }
    
    // reset the agent
    public override void OnEpisodeBegin()
    {
        Guides.ForEach(g => g.SetActive(true));
        Guides.Clear();
        // Set random position
        this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-10f, 10.0f), 1.0f, UnityEngine.Random.Range(-4.0f, 4.0f));
        
        ball.transform.localPosition = new Vector3(UnityEngine.Random.Range(-10f, 10.0f), 1.0f, UnityEngine.Random.Range(-4.0f, 4.0f));
        // ball.transform.localPosition = new Vector3(0, 1, 0);
        ball.BallRigidbody.velocity = Vector3.zero;
        ball.BallRigidbody.angularVelocity = Vector3.zero;
    }
    
    private float distanceToTarget;
    
    // collect observations
    public override void CollectObservations(VectorSensor sensor)
    {
        
    }
    
    // test the agent's behavior
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut.Clear();
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Goal")
        {
             // AddReward(-0.01f);
            // EndEpisode();
        }

        if (other.gameObject.tag == "Ball")
        {
            
            // check if ball is in front
            if (Vector3.Dot(transform.forward, ball.transform.position - transform.position) > 0)
            {
                // AddReward(1.0f);
                // EndEpisode(); 
                // kick the ball
                ball.BallRigidbody.AddForce(transform.forward * 10.0f, ForceMode.Impulse);
            }
        }
    }
    
    // process actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.001f);
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        
        var discreteActionsOut = actions.DiscreteActions;
        var act = actions.DiscreteActions;
        
        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * speed;
                break;
            case 2:
                dirToGo = transform.forward * -speed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * horizontalSpeed;
                break;
            case 2:
                dirToGo = transform.right * -horizontalSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f * rotationSpeed;
                break;
            case 2:
                rotateDir = transform.up * 1f * rotationSpeed;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        rb.AddForce(dirToGo,
            ForceMode.VelocityChange);
        
    }
}
