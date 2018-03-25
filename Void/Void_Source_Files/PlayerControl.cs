using UnityEngine;
using System.Collections;

public class PlayerControl : EntityControl
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

    private bool enemyInRange = false;
    private bool canAttack = true;
    private bool canMove = true;
    private bool rightFoot = true;
    private bool canRotate = true;
    private bool canJump = true;
    private bool canInput = true;
    private bool gameOver = false;
    private bool winded = false;
    private int stamina = 100;

    //FOR SWIPE INPUT
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
    private Vector2 touchEnd;
    private float x;
    private float y;
    private float swipeDistance;
    private const float MIN_SWIPE_DISTANCE = 50f;

    void Awake()
    {
        StartCoroutine(RegainStamina());
    }

    void OnEnable()
    {
        EventManager.StartListening(messages.playerTakeDamage, PlayerTakeDamage);
        EventManager.StartListening(messages.beatTheGame, BeatTheGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(messages.playerTakeDamage, PlayerTakeDamage);
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

        /*STANDALONE
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (canRotate && canInput)
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
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (canRotate && canInput)
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
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (stamina > 0 && canAttack && canInput)
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
                stamina -= 20;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if(stamina > 0 && canJump && canInput)
            {
                //jump back
                transform.Translate(Vector3.back * moveSpeed, transform);
                AudioManager.instance.PlayRandomEffect(false, jumpClips);
                canJump = false;
                canInput = false;
                StartCoroutine(ToggleJump());
                stamina -= 25;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (stamina > 0 && canMove && canInput)
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
        */

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
				swipeDistance = Vector2.Distance (touchOrigin, touchEnd);
				
				//Check if the difference along the x axis is greater than the difference along the y axis.
				if (Mathf.Abs(x) > Mathf.Abs(y) && swipeDistance >= MIN_SWIPE_DISTANCE)
				{
					if(touchEnd.x > touchOrigin.x)
					{
						if (canRotate && canInput)
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
						if (canRotate && canInput)
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
				else if(Mathf.Abs (x) < Mathf.Abs (y) && swipeDistance >= MIN_SWIPE_DISTANCE)
				{
					if(touchEnd.y > touchOrigin.y)
					{
						if (stamina > 0 && canAttack && canInput)
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
                            stamina -= 20;
                        }
					}
					else
					{
						if(stamina > 0 && canJump && canInput)
                        {
                            //jump back
                            transform.Translate(Vector3.back * moveSpeed, transform);
                            AudioManager.instance.PlayRandomEffect(false, jumpClips);
                            canJump = false;
                            canInput = false;
                            StartCoroutine(ToggleJump());
                            stamina -= 25;
                        }
					}
				}
				else
				{
					if (stamina > 0 && canMove && canInput)
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
        AudioManager.instance.PlayRandomEffect(false, catchBreathClip);
        yield return new WaitForSeconds(catchBreathClip.length);
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
        yield return new WaitForSeconds(5.0f);
        Application.LoadLevel(Application.loadedLevel);
    }

    void BeatTheGame()
    {
        canInput = false;
        GetComponent<PlayerControl>().enabled = false;
    }
}
