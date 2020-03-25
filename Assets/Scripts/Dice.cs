using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Dice : MonoBehaviour, IThrowable
{
    [Header("Prefabs")]
    [SerializeField]
    private DiceObject diceObject;
    public Vector2 direction;

    [Header("Speed")]
    public float moveSpeed = 5f;
    public float rollSpeed = 3f;

    private Rigidbody2D body2D;
    private SpriteRenderer spriteRenderer;

    private bool animationStarted = false;
    private bool animationFinished = false;

    private const float CHANGE_SPRITE_TIME = .2f;
    private float changeSpriteTime = CHANGE_SPRITE_TIME;
    private bool changeSprite = false;

    private int value = 0;

    private bool AnimationStarted
    {
        get { return animationStarted; }
        set
        {
            if (value)
                OnAnimationStart();
            animationStarted = value;
        }
    }

    public bool AnimationFinished
    {
        get { return animationFinished; }
        set
        {
            if (value)
                OnAnimationFinish();
            animationFinished = value;
        }
    }


    private bool IsStopped
    {
        get
        {
            return
                body2D.velocity.x > -.1f && body2D.velocity.x < .1f &&
                body2D.velocity.y > -.1f && body2D.velocity.y < .1f;
        }
    }

    private bool IsLastFrame
    {
        get { return body2D.velocity.magnitude < 1f; }
    }

    #region Unity API

    private void Awake()
    {
        body2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // do not continue when animation not started or finished
        if (!AnimationStarted || AnimationFinished)
            return;

        //------------------------
        // calculations
        //------------------------

        {
            // if dice is stopped
            if (IsStopped)
            {
                // finish animation
                AnimationFinished = true;
                return;
            }
        }

        // when not changing sprite
        if (!changeSprite)
        {
            // decrease changing sprite to random
            changeSpriteTime -= Time.fixedDeltaTime;

            // change sprite when timer finished
            if (changeSpriteTime <= 0)
            {
                changeSprite = true;
            }
        }

        //------------------------
        // drawing calls
        //------------------------

        // animation is not last frame
        if (!IsLastFrame && changeSprite)
        {
            // display random value
            DisplayRandom();

            // reset changing sprite
            changeSprite = false;
            changeSpriteTime = CHANGE_SPRITE_TIME;
        }

        // animation is last frame
        if (IsLastFrame)
            DisplayValue();

    }

    #endregion

    #region Draw Methods

    private void DisplayValue()
    {
        spriteRenderer.sprite = diceObject.valueSprites[value - 1];
    }

    private void DisplayRandom()
    {
        spriteRenderer.sprite = diceObject.valueSprites[Random.Range(0, diceObject.valueSprites.Length)];
    }

    #endregion

    #region Animation

    private void ResetAnimation()
    {
        AnimationStarted = false;
        AnimationFinished = false;

        changeSpriteTime = CHANGE_SPRITE_TIME;
        changeSprite = false;
    }

    private void OnAnimationStart()
    {
        // move body to direction
        body2D.AddForce(direction * moveSpeed, ForceMode2D.Impulse);

        // give starting rotation velocity
        body2D.angularVelocity = 360 * rollSpeed;
        body2D.angularDrag = rollSpeed;
    }
    private void OnAnimationFinish()
    {
        body2D.velocity = Vector2.zero;
        body2D.angularVelocity = 0;
    }

    #endregion

    #region Dice Functions

    private void Roll()
    {
        value = Random.Range(1, 7);
    }

    public void Throw()
    {
        ResetAnimation();
        Roll();

        AnimationStarted = true;
    }

    public void Throw(int value)
    {
        ResetAnimation();
        this.value = value;

        AnimationStarted = true;
    }

    public void Throw(Vector2 startPos)
    {
        body2D.position = new Vector3(startPos.x, startPos.y, 0);
        Throw();
    }

    public void Throw(int value, Vector2 startPos)
    {
        body2D.position = new Vector3(startPos.x, startPos.y, 0);
        Throw(value);
    }

    #endregion
}
