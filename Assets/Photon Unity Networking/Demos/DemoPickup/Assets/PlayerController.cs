using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Timers;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool movementDisabled = false;
    public bool isDashing = false;
    public bool canDash = true;
    public bool dazed = false;
    public float speed;
    public float dashSpeed;
    public int count = 0;
    public int score = 0;
    public int enemyScore = 0;
    public int enemyScore1 = 0;
    public int enemyScores = 0;
    public int initialNumCubes = 8;
    public int numCubes;
    public float dashCooldown = 5.0f;
    public float currentDashCooldown;
    public int dazedTime = 2;
    public float Health;
    public Text countText;
    public Text winText;
    public GameObject EndScreen;
    public Text scoreText;
    public Text enemyScoreText;
    public Text dashCooldownText;
    public Slider cooldownSlider;
    public GameObject cam;
    public GameObject droppedCube;
    public GameObject dazedEffect;
    public GameObject basePlatform;

    // Audio stuff
    public AudioClip dazedSound;
    public AudioClip dashSound;
    public AudioClip respawnSound;
    public AudioClip collectAtGoalSound;

    private bool dashHeld = false;
    private Vector3 movement;
    private Rigidbody rb;
    private Transform cameraTransform;
    private GameObject dazedEffectInstance;

    // Audio stuff
    private AudioSource audioSource;

    public GameObject enemy;
    public GameObject enemy1;
    public GameObject deathPlane;
    public Vector3 initialPosition;

    public PhotonView photonView;


    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera");
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        // Audio stuff
        audioSource = GetComponent<AudioSource>();

        photonView = GetComponent<PhotonView>();
        Time.timeScale = 1;
        //Cursor.visible = false;
       // Cursor.lockState = CursorLockMode.Locked;
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        currentDashCooldown = 0;
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 10.0f;
        rb.drag = 0.5f;
        //winText.text = "";
        //EndScreen.SetActive(false);
        numCubes = initialNumCubes;
       // dazedEffectInstance = Instantiate(dazedEffect);
       // dazedEffectInstance.SetActive(false);
    }
   
    void Update()
    {
        

        // Set the Dash Cooldown text to reflect the current dash cooldown
        //dashCooldownText.text = "Dash Cooldown: " + currentDashCooldown;
        //cooldownSlider.value = currentDashCooldown/dashCooldown;

        if (isDashing)
        {
            // While the player is dashing, their mass is increased, so when they hit an enemy, it goes flying
            rb.mass = 100;
            // If you miss the enemy, you are "confused" for 2 seconds, meaning you cannot move. This also prevents adjusting trajectory during dash.
            StartCoroutine("DashCoroutine", 2);
        }

        // If you are not dashing and the spacebar is not held down, your mass returns to 1 and your movement is re-enabled
        else if (!isDashing && !dashHeld && !dazed)
        {
            rb.mass = 1;
            movementDisabled = false;
        }
        
        if (enemy1 != null)
        {

        }
        //enemyScores = enemyScore + enemyScore1;
        // Win Text
        if (numCubes == 0)
        {
            // If you have fewer than half of the cubes when they have all been brought to a goal, you lose
            if (score < enemyScores)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
               // Cursor.lockState = CursorLockMode.None;
               // winText.text = "You Lose!";
                //EndScreen.SetActive(true);
            }

            // If you have exactly half, you tie
            else if (score == enemyScores)
            {
                Time.timeScale = 0;
                //Cursor.visible = true;
               // Cursor.lockState = CursorLockMode.None;
//winText.text = "Tie!";
                //EndScreen.SetActive(true);
            }

            // If you have more than half, you win
            else
            {
                Time.timeScale = 0;
              //  Cursor.visible = true;
               // Cursor.lockState = CursorLockMode.None;
               // winText.text = "You Win!";
               // EndScreen.SetActive(true);
            }
        }

        // Set score text and count text
       // scoreText.text = "Score: " + score.ToString();
       // countText.text = "Count: " + count.ToString();
      //  enemyScoreText.text = "Enemy Score: " + enemyScores.ToString();

        // If the dash cooldown is zero, you are not currently dashing, and you hold down both space and forward, you will not be able to move until you let go of space
        // The camera zooms in slightly to indicate that you are about to perform a dash; camera zoom control is taken from the player
        if (canDash && !isDashing && Input.GetKeyDown(KeyCode.Space) && Input.GetAxis("Vertical") > 0)
        {
            dashHeld = true;
            movementDisabled = true;
            cam.transform.position += cam.transform.forward * 3;
        }

        if (dashHeld && Input.GetKeyUp(KeyCode.Space))
        {
            // If you simply let go of space without holding forward, the dash is canceled
            if (Input.GetAxis("Vertical") == 0) { }

            // If you let go of space and are still holding forward, you get launched in the direction the camera is facing, and your dash goes on cooldown
            else
            {
                // Audio stuff
               // audioSource.PlayOneShot(dashSound);

                //dashCooldownText.text="Wait to dash...";
                rb.AddForce(new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z) * dashSpeed);
                isDashing = true;
                canDash = false;
                StartCoroutine("DashCooldownCoroutine", dashCooldown);
            }

            // The camera zooms back out, and camera zoom control is returned to the player
            dashHeld = false;
            cam.transform.position -= cam.transform.forward * 3;
        }
    }

    void FixedUpdate()
    {
        // Set horizontal and vertical inputs to their respective axes
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Set movement to the horizontal and vertical inputs
        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Set movement to the value returned by RotateWithView

        // If movement is not disabled, move according to inputs
        if (!movementDisabled)
        {
            rb.AddForce(movement * speed);
        }
    }

    // Rotate input with view (i.e. forward direction changes depending on which way you are facing)
  

    // When you touch the Goal, if you have cubes, exchange them for points
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            if (count > 0)
            {
                // Audio stuff
                audioSource.PlayOneShot(collectAtGoalSound);

                score += count;
                numCubes-= count;
                count = 0;
            }
        }
        if(other.CompareTag("Death Area"))
        {
            // Audio stuff
            audioSource.PlayOneShot(respawnSound);
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity=Vector3.zero;
            transform.position = initialPosition;

            if (count > 0)
            {
                StartCoroutine("RespawnCubes");
            }
        }
    }

    // When you collide with an enemy while you are dashing, stop moving and re-enable movement
    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.collider.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            StopCoroutine("DashCoroutine");
            isDashing = false;
        }

        if (collision.collider.tag == "Enemy" && !isDashing)
        {
            Debug.Log("Pog");

            if (!isDashing && count > 0)
            {
                StartCoroutine("DropCubes");
            }
        }
    }

    // If you miss the enemy while dashing, you cannot move for two seconds
    private IEnumerator DashCoroutine(int time)
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1);
        }

        if (time == 0)
        {
            isDashing = false;
        }
    }

    // Dash cooldown timer
    private IEnumerator DashCooldownCoroutine(int time)
    {
        while (time > 0)
        {
            currentDashCooldown = time;
            time--;
            yield return new WaitForSeconds(1);
        }

        if (time == 0)
        {
            currentDashCooldown = time;
            canDash = true;
            //dashCooldownText.text= "Press 'Space' to dash!";
        }
    }

    private IEnumerator DazedCountdown(int time)
    {
        // Audio stuff
        audioSource.PlayOneShot(dazedSound);

        //dazedEffectInstance.SetActive(true);

        while (time > 0)
        {
            dazed = true;
            movementDisabled = true;
            canDash = false;
            time--;
            yield return new WaitForSeconds(1.0f);
        }

        if (time == 0)
        {
            dazed = false;
            movementDisabled = false;
            canDash = true;
           // dazedEffectInstance.SetActive(false);
        }
    }

    private IEnumerator DropCubes()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject droppedCubeInstance;
            droppedCubeInstance = Instantiate(droppedCube, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), transform.rotation) as GameObject;
            droppedCubeInstance.GetComponent<Rigidbody>().AddForce(droppedCube.transform.up * 5.0f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.1f);
        }

        count = 0;
    }

    private IEnumerator RespawnCubes()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject droppedCubeInstance;
            droppedCubeInstance = Instantiate(droppedCube, new Vector3(basePlatform.transform.position.x - i * 2, basePlatform.transform.position.y + 3.0f, basePlatform.transform.position.z), transform.rotation) as GameObject;
            droppedCubeInstance.GetComponent<Rigidbody>().AddForce(droppedCube.transform.up * 5.0f, ForceMode.Impulse);
            yield return new WaitForSeconds(0.1f);
        }

        count = 0;
    }
}
