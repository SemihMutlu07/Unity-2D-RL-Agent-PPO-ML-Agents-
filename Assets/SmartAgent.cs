using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SmartAgent : Agent
{
    [Header("Refs")] public Transform target;
    [Header("Move")] public float moveSpeed = 5f;
    [Header("Bounds")] public Vector2 minXY = new Vector2(-4.5f, -2.8f);
    public Vector2 maxXY = new Vector2(4.5f, 2.8f);

    private Rigidbody2D rb;
    private float lastDistance;
    private int stepCounter;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector2(
            Random.Range(minXY.x + 0.1f, maxXY.x - 0.1f),
            Random.Range(minXY.y + 0.1f, maxXY.y - 0.1f)
        );
        target.position = new Vector2(
            Random.Range(minXY.x + 0.1f, maxXY.x - 0.1f),
            Random.Range(minXY.y + 0.1f, maxXY.y - 0.1f)
        );

        lastDistance = Vector2.Distance(transform.position, target.position);
        stepCounter = 0;

        // ilk frame beklemesin
        RequestDecision();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(target.position.x);
        sensor.AddObservation(target.position.y);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var movement = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
        movement = Vector2.ClampMagnitude(movement, 1f);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        AddReward(-0.001f); // step penalty

        float d = Vector2.Distance(transform.position, target.position);
        AddReward(lastDistance - d); // yaklaþýyorsa +, uzaklaþýyorsa -
        lastDistance = d;

        Vector2 p = transform.position;
        if (p.x < minXY.x || p.x > maxXY.x || p.y < minXY.y || p.y > maxXY.y)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void FixedUpdate()
    {
        stepCounter++;
        if (stepCounter >= 1000) { AddReward(-0.2f); EndEpisode(); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var a = actionsOut.ContinuousActions;
        a[0] = Input.GetAxisRaw("Horizontal");
        a[1] = Input.GetAxisRaw("Vertical");
    }
}
