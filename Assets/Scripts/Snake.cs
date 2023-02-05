using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    SpriteRenderer mySpriteRenderer; 
    ParticleSystem myParticleSystem;
    [SerializeField] ParticleSystem digParticles;
    [SerializeField] Image waterMeter;
    [SerializeField] float waterFillAmount = 1;

    [SerializeField] float waterLoseAmount = 0.0625f;
    [SerializeField] AudioSource conductor;
    [SerializeField] AudioSource waterSip;

    private bool fillWater = false;

    [SerializeField] Vector3 rootStartingPosition;

    private List<Transform> segments = new List<Transform>();
    public Transform segmentPrefab;
    public Vector2 direction = Vector2.down;
    private Vector2 input;
    public int initialSize = 10;
    public Sprite[] rootSprites;
    public SpriteRenderer segmentPrefabSprite;
   // public Spriterenderer prefabSpriteRenderer = prefab.GetComponent<SpriteRenderer>();

    private int lastBeat = 0;

    private void Start()
    {
        ResetState();
        myRigidBody = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        waterMeter.fillAmount = 1;
        direction = Vector2.down;
        RootController.instance.PlaceRoot();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            SpriteRenderer spriteRend = segmentPrefab.GetComponentInChildren<SpriteRenderer>();

            //Linjen nedenfor inneholder bevegelse oppover.
            //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) { input = Vector2.up; }

            //Linjen nedenfor hadde originalt "else if".
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                spriteRend.sprite = rootSprites[3];
                input = Vector2.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            SpriteRenderer spriteRend = segmentPrefab.GetComponentInChildren<SpriteRenderer>();
            
            
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2.right;
                spriteRend.sprite = rootSprites[0];
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2.left;
                spriteRend.sprite = rootSprites[0];
            }
        }

        if (Conductor.instance.songPositionInBeatsInt > lastBeat)
        {
            BeatUpdate();

            lastBeat = Conductor.instance.songPositionInBeatsInt;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);

        if (movementDirection != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, -1000f * Time.deltaTime);
        } 
    }

    private void BeatUpdate()
    {
        // Set the new direction based on the input
        if (input != Vector2.zero) {
            direction = input;
        }


        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        float x = Mathf.Round(transform.position.x) + direction.x;
        float y = Mathf.Round(transform.position.y) + direction.y;
        

        transform.position = new Vector2(x, y);
        RootController.instance.PlaceRoot();

        digParticles.Play();
        waterMeter.fillAmount = waterMeter.fillAmount - waterLoseAmount;

        if (fillWater)
        {
            waterMeter.fillAmount = waterMeter.fillAmount+waterFillAmount;
            waterSip.Play();
            fillWater = false;
        }

        if (waterMeter.fillAmount == 0)
        {
            ResetState();
        }

    }

    public void Grow()
    {

        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        
        transform.position = rootStartingPosition;
        direction = Vector2.down;
        waterMeter.fillAmount = 1;
        Conductor.instance.ResetMusic();

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food")) {
            Grow();
        } else if (other.gameObject.CompareTag("Obstacle")) {
            ResetState();
        } else if (other.gameObject.CompareTag("Water")) {
            fillWater = true;
        } else if (other.gameObject.CompareTag("Music")) {
            Conductor.instance.playNextTrack = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            fillWater = true;
        }
    }

    private void ThirstDed()
    {
        if (waterMeter.fillAmount == 0)
        {
            ResetState();
        }

    }

        void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x), 1f);
        }

    }

}
