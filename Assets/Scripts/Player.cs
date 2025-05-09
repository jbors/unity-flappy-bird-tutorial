using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class Player : Agent
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;


    public override void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public override void OnEpisodeBegin()
    {
        GameManager.Instance.Play();

        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);

        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var Actions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.Space))
        {
            Actions[0] = 1;
        } else
        {
            Actions[0] = 0;
        }
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        if (Mathf.FloorToInt(actions.DiscreteActions[0]) == 1)
        {
            //Hack to avoid 'no ceiling' bug
            if (transform.position.y < 5)
            {
                direction = Vector3.up * strength;
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Add current heigt as an input
        sensor.AddObservation(transform.position.y / 5);
    }

    //private void Awake()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //}

    //private void Start()
    //{
    //    InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    //}

    //private void OnEnable()
    //{
    //    Vector3 position = transform.position;
    //    position.y = 0f;
    //    transform.position = position;
    //    direction = Vector3.zero;
    //}

    private void Update()
    {
        //    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
        //        direction = Vector3.up * strength;
        //    }

        // Apply gravity and update the position
        direction.y += gravity* Time.deltaTime;
        transform.position += direction* Time.deltaTime;

        // Tilt the bird based on the direction
        // Vector3 rotation = transform.eulerAngles;
        // rotation.z = direction.y* tilt;
        // transform.eulerAngles = rotation;
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            //TODO: move this to the proper method
            Debug.Log("Game over");
            AddReward(-1.0f);
            EndEpisode();
            //GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            Debug.Log("Score!");
            AddReward(0.1f);
            GameManager.Instance.IncreaseScore();
        }
    }

}
