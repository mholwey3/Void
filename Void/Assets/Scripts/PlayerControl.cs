using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] moveClips;
    [SerializeField]
    private AudioClip[] rotateClips;
    [SerializeField]
    private AudioClip[] attackClips;
    [SerializeField]
    private AudioClip[] hitEnemyClips;
    [SerializeField]
    private AudioClip[] jumpClips;
    [SerializeField]
    private AudioClip[] landClips;
    [SerializeField]
    private AudioClip[] hurtClips;
    [SerializeField]
    private AudioClip[] deadClips;
    [SerializeField]
    private AudioClip catchBreathClip;
    [SerializeField]
    private AudioClip enterVoidClip;
    [SerializeField]
    private AudioClip pickUpKeyClip;
    [SerializeField]
    private AudioClip lvlIndicatorClip;

    private bool enemyInRange = false;
    private bool canAttack = true;
    private bool canMove = true;
    private bool rightFoot = true;
    private bool canRotate = true;
    private bool canJump = true;
    public bool canInput = false;
    private bool gameOver = false;
    private bool winded = false;
    public bool isLanded = false;

    float moveSpeed = 5f;
    float health = 100f;
    private int stamina = 100;

    public int numKeys = 0;
    public float lvlIndicatorPitch = 1.0f;

    public int currentLevel;

    //FOR SWIPE INPUT
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
    private Vector2 touchEnd;
    private float x;
    private float y;
    private float swipeDistance;
    private const float MIN_SWIPE_DISTANCE = 50f;

    public Vector3 startingVector;

    public LevelManager levelManager;

    void Awake()
    {
        startingVector = transform.position;
        StartCoroutine(RegainStamina());
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.playerTakeDamage, PlayerTakeDamage);
        EventManager.StartListening(messages.pickUpKey, PickUpKey);
        EventManager.StartListening(messages.warpToStart, WarpToStart);
        EventManager.StartListening(messages.beatTheGame, BeatTheGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.playerTakeDamage, PlayerTakeDamage);
        EventManager.StopListening(messages.pickUpKey, PickUpKey);
        EventManager.StopListening(messages.warpToStart, WarpToStart);
        EventManager.StopListening(messages.beatTheGame, BeatTheGame);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag.Equals("Enemy"))
        {
            enemyInRange = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag.Equals("Enemy"))
        {
            enemyInRange = false;
        }
    }

	void Update ()
    {
        if (health <= 0 && !gameOver)
        {
            gameOver = true;
            StartCoroutine(GameOver());
        }

        if (stamina <= 0 && !winded)
        {
            winded = true;
            StartCoroutine(CatchBreath());
        }

        ////KEYBOARD
        //if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        //{
        //    if (canRotate && canInput && isLanded && !winded)
        //    {
        //        //turn 90 degrees to the right
        //        transform.Rotate(Vector3.up, 90);
        //        AudioManager.instance.PlayRandomEffect(false, rotateClips);
        //        canRotate = false;
        //        canInput = false;
        //        StartCoroutine(ToggleRotate());
        //        stamina -= 5;
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        //{
        //    if (canRotate && canInput && isLanded && !winded)
        //    {
        //        //turn 90 degrees to the left
        //        transform.Rotate(Vector3.up, -90);
        //        AudioManager.instance.PlayRandomEffect(false, rotateClips);
        //        canRotate = false;
        //        canInput = false;
        //        StartCoroutine(ToggleRotate());
        //        stamina -= 5;
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        //{
        //    if (canAttack && canInput && isLanded && !winded)
        //    {
        //        //attack

        //        if (enemyInRange)
        //        {
        //            AudioManager.instance.PlayRandomEffect(false, hitEnemyClips);
        //            EventManager.TriggerEvent(messages.killEnemy);
        //            enemyInRange = false;
        //        }
        //        else
        //            AudioManager.instance.PlayRandomEffect(false, attackClips);

        //        canAttack = false;
        //        canInput = false;
        //        StartCoroutine(ToggleAttack());
        //        stamina -= 25;
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        //{
        //    if (canJump && canInput && isLanded && !winded)
        //    {
        //        //jump back
        //        transform.Translate(Vector3.back * moveSpeed, transform);
        //        AudioManager.instance.PlayRandomEffect(false, jumpClips);
        //        canJump = false;
        //        canInput = false;
        //        StartCoroutine(ToggleJump());
        //        stamina -= 40;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        //{
        //    if (canMove && canInput && isLanded && !winded)
        //    {
        //        //walking
        //        transform.Translate(Vector3.forward * moveSpeed, transform);
        //        if (rightFoot)
        //        {
        //            AudioManager.instance.PlayRandomEffect(false, moveClips[0]);
        //            rightFoot = false;
        //        }
        //        else
        //        {
        //            AudioManager.instance.PlayRandomEffect(false, moveClips[1]);
        //            rightFoot = true;
        //        }

        //        canMove = false;
        //        canInput = false;
        //        StartCoroutine(ToggleMove());
        //        stamina -= 5;
        //    }
        //}


        //ANDROID
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                y = touchEnd.y - touchOrigin.y;

                //Calculate the distance the user has swiped their finger across the screen.
                swipeDistance = Vector2.Distance(touchOrigin, touchEnd);

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y) && swipeDistance >= MIN_SWIPE_DISTANCE)
                {
                    if (touchEnd.x > touchOrigin.x)
                    {
                        if (canRotate && canInput && isLanded)
                        {
                            //turn 90 degrees to the right
                            transform.Rotate(Vector3.up, 90);
                            AudioManager.instance.PlayRandomEffect(false, rotateClips);
                            canRotate = false;
                            canInput = false;
                            StartCoroutine(ToggleRotate());
                            stamina -= 5;
                        }
                    }
                    else
                    {
                        if (canRotate && canInput && isLanded)
                        {
                            //turn 90 degrees to the left
                            transform.Rotate(Vector3.up, -90);
                            AudioManager.instance.PlayRandomEffect(false, rotateClips);
                            canRotate = false;
                            canInput = false;
                            StartCoroutine(ToggleRotate());
                            stamina -= 5;
                        }
                    }
                }
                else if (Mathf.Abs(x) < Mathf.Abs(y) && swipeDistance >= MIN_SWIPE_DISTANCE)
                {
                    if (touchEnd.y > touchOrigin.y)
                    {
                        if (stamina > 0 && canAttack && canInput && isLanded)
                        {
                            //attack

                            if (enemyInRange)
                            {
                                AudioManager.instance.PlayRandomEffect(false, hitEnemyClips);
                                EventManager.TriggerEvent(messages.killEnemy);
                                enemyInRange = false;
                            }
                            else
                                AudioManager.instance.PlayRandomEffect(false, attackClips);

                            canAttack = false;
                            canInput = false;
                            StartCoroutine(ToggleAttack());
                            stamina -= 25;
                        }
                    }
                    else
                    {
                        if (stamina > 0 && canJump && canInput && isLanded)
                        {
                            //jump back
                            transform.Translate(Vector3.back * moveSpeed, transform);
                            AudioManager.instance.PlayRandomEffect(false, jumpClips);
                            canJump = false;
                            canInput = false;
                            StartCoroutine(ToggleJump());
                            stamina -= 40;
                        }
                    }
                }
                else
                {
                    if (stamina > 0 && canMove && canInput && isLanded)
                    {
                        //walking
                        transform.Translate(Vector3.forward * moveSpeed, transform);
                        if (rightFoot)
                        {
                            AudioManager.instance.PlayRandomEffect(false, moveClips[0]);
                            rightFoot = false;
                        }
                        else
                        {
                            AudioManager.instance.PlayRandomEffect(false, moveClips[1]);
                            rightFoot = true;
                        }

                        canMove = false;
                        canInput = false;
                        StartCoroutine(ToggleMove());
                        stamina -= 5;
                    }
                }
            }
        }

    }

    IEnumerator ToggleMove()
    {
        yield return new WaitForSeconds(0.2f);
        canMove = true;
        canInput = true;
    }

    IEnumerator ToggleRotate()
    {
        yield return new WaitForSeconds(0.5f);
        canRotate = true;
        canInput = true;
    }

    IEnumerator ToggleJump()
    {
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayRandomEffect(false, landClips);
        yield return new WaitForSeconds(0.25f);
        canJump = true;
        canInput = true;
    }

    IEnumerator ToggleAttack()
    {
        yield return new WaitForSeconds(0.25f);
        canAttack = true;
        canInput = true;
    }

    IEnumerator RegainStamina()
    {
        while (true)
        {
            if (stamina < 100)
            {
                stamina += 5;
                yield return new WaitForSeconds(0.25f);
            }
            else
                stamina = 100;

            yield return null;
        }
    }

    IEnumerator CatchBreath()
    {
        canInput = false;
        AudioManager.instance.PlayRandomEffect(false, catchBreathClip);
        yield return new WaitForSeconds(catchBreathClip.length);
        canInput = true;
        winded = false;
    }

    void PlayerTakeDamage()
    {
        AudioManager.instance.PlayRandomEffect(false, hurtClips);
        health -= 25f;
    }

    IEnumerator GameOver()
    {
        canInput = false;
        GetComponent<PlayerControl>().enabled = false;
        EventManager.TriggerEvent(messages.playerDead);
        AudioManager.instance.PlayRandomEffect(false, deadClips);
        yield return new WaitForSeconds(3.0f);
        Application.LoadLevel(Application.loadedLevel);
    }

    void WarpToStart()
    {
        StartCoroutine(EnterVoid());
    }

    IEnumerator EnterVoid()
    {
        levelManager.PlayEntityAudio(false);
        lvlIndicatorPitch = 1f;
        transform.position = startingVector;
        isLanded = false;
        yield return new WaitForSeconds(2);

        int lvl = currentLevel;
        while (lvl > 0)
        {
            AudioManager.instance.PlaySingle(false, lvlIndicatorPitch, lvlIndicatorClip);
            lvlIndicatorPitch += 0.5f;
            lvl--;
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        
        AudioManager.instance.PlayRandomEffect(false, jumpClips);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlaySingle(false, 1.0f, enterVoidClip);
        yield return new WaitForSeconds(enterVoidClip.length);

        AudioManager.instance.PlayRandomEffect(false, landClips);
        levelManager.PlayEntityAudio(true);
        isLanded = true;
        canInput = true;
    }

    void PickUpKey()
    {
        AudioManager.instance.PlaySingle(false, 1, pickUpKeyClip);
        numKeys++;

        //if player has aquired all of the keys in the level
        if (numKeys == currentLevel)
        {
            EventManager.TriggerEvent(messages.beatTheLevel);
        }
    }

    public void BeatTheGame()
    {
        GetComponent<PlayerControl>().enabled = false;
    }
}
