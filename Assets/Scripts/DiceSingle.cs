using UnityEngine;

public class DiceSingle : MonoBehaviour
{
    public DiceObject diceObject;
    public int value = 0;
    public Vector2 direction;
    public float moveSpeed = 5f;
    public float rollSpeed = 3f;

    private Rigidbody2D body2D;
    private SpriteRenderer spriteRenderer;

    private bool animationStarted = false;
    public bool animationFinished = false;

    private const float ANIMATION_TIME = 1.5f;
    private const float CHANGE_SPRITE_TIME = .2f;
    private float animationTime = ANIMATION_TIME;
    private float changeSpriteTime = CHANGE_SPRITE_TIME;
    private bool changeSprite = false;
    private float currentMoveSpeed;
    private float currentRollSpeed;

    #region Unity API

    private void Awake()
    {
        body2D = transform.Find("Value").GetComponent<Rigidbody2D>();
        spriteRenderer = transform.Find("Value").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!animationStarted)
            return;

        if (!animationFinished)
        {
            animationTime -= Time.deltaTime;
            if (animationTime <= 0)
            {
                animationFinished = true;
                AfterAnimationFinish();
            }

            currentMoveSpeed -= Time.deltaTime * currentMoveSpeed;
            currentRollSpeed -= Time.deltaTime * currentRollSpeed;
        }

        if (!animationFinished && !changeSprite)
        {
            changeSpriteTime -= Time.deltaTime;
            if (changeSpriteTime <= 0)
            {
                changeSprite = true;
            }
        }

        if (!animationFinished && animationTime > CHANGE_SPRITE_TIME && changeSprite)
        {
            DisplayRandom();
            changeSprite = false;
            changeSpriteTime = CHANGE_SPRITE_TIME;
        }

        if (!animationFinished && animationTime < CHANGE_SPRITE_TIME)
            DisplayValue();
            
    }

    private void FixedUpdate()
    {
        if (!animationFinished && !changeSprite)
        {
            body2D.velocity = new Vector2(direction.x * currentMoveSpeed, direction.y * currentMoveSpeed);
            body2D.rotation = animationTime * 360 * currentRollSpeed;
        }
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

    private void AfterAnimationFinish()
    {
        body2D.velocity = Vector2.zero;
    }

    private void ResetFields()
    {
        animationTime = ANIMATION_TIME;
        animationFinished = false;
        changeSpriteTime = CHANGE_SPRITE_TIME;
        changeSprite = false;
        currentRollSpeed = rollSpeed;
        currentMoveSpeed = moveSpeed;
    }

    private void Roll()
    {
        value = Random.Range(1, 7);
    }

    public void RollAgain()
    {
        ResetFields();
        Roll();
        animationStarted = true;
    }
    public void RollAgain(Vector2 startPos)
    {
        body2D.position = new Vector3(startPos.x, startPos.y, 0);
        RollAgain();
    }

}
