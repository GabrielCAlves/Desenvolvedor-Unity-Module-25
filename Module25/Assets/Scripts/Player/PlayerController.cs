using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;
using TMPro;
using DG.Tweening;

public class PlayerController : Singleton<PlayerController>
{
    //publics
    [Header("Lerp")]
    public Transform target;
    public float lerpSpeed = 1f;

    public float speed = 1f;
    public string tagToCheckEnemy = "Enemy";
    public string tagToCheckEndline = "Endline";
    public string tagToCheckCoin = "Coin";

    public GameObject startScreen;
    public GameObject endScreen;

    public bool invincible = false;

    [Header("TextMeshPro")]
    public TextMeshPro uiTextPowerUp;

    [Header("Coin Setup")]
    public GameObject coinCollector;

    [Header("Setup")]
    public SOPlayerSetup soPlayerSetup;

    [Header("Animator")]
    public Animator animator;

    [Header("VFX")]
    public ParticleSystem vfxDeath;

    //[Header("Animation")]
    //public AnimatorManager animatorManager;

    public Ease ease = Ease.OutBack;
    public float scaleDuration = .2f;

    [SerializeField] private BounceHelper _bounceHelper;

    //privates
    private Vector3 _pos;
    private bool _canRun;
    private float _currentSpeed;
    private Vector3 _startPosition;
    private bool validDeath = false;

    //private float _basespeedToAnimation = 7;
    private void Awake()
    {
        Instance = this;
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        _startPosition = transform.position;
        //ResetPosition();
        ResetSpeed();

        transform.DOScale(1, scaleDuration).SetEase(ease).SetLoops(1, LoopType.Yoyo).SetDelay(.5f);
    }

    void Update()
    {
        if (!_canRun)
        {
            return;
        }

        _pos = target.position;
        _pos.y = transform.position.y;
        _pos.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, _pos, lerpSpeed * Time.deltaTime);
        transform.Translate(transform.forward * _currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!validDeath)
        {
            bool isPlayerCollider = false;

            if (collision.transform.tag == tagToCheckEnemy)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.thisCollider.name.Equals("PlayerContainer"))
                    {
                        isPlayerCollider = true;
                    }
                }

                if (isPlayerCollider && !invincible)
                {
                    //animator.SetBool(soPlayerSetup.runBool, false);

                    MoveBack(collision.transform);
                    animator.SetTrigger(soPlayerSetup.triggerDeath);

                    if (vfxDeath != null)
                    {
                        vfxDeath.Play();
                    }

                    validDeath = true;
                    EndGame(/*AnimatorManager.AnimationType.DEATH*/);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == tagToCheckEndline)
        {
            EndGame();
            animator.SetBool(soPlayerSetup.runBool, false);
            //animatorManager.Play(AnimatorManager.AnimationType.IDLE);
        }
    }

    public void Bounce()
    {
        if(_bounceHelper != null)
        {
            _bounceHelper.Bounce();
        }
    }

    private void MoveBack(Transform t)
    {
        //transform.DOMoveZ(-1f, .3f).SetRelative(); //Mover o player pra trás
        t.DOMoveZ(1f, .3f).SetRelative(); //Mover o objeto colisor pra frente
    }

    public void EndGame(/*AnimatorManager.AnimationType animationType = AnimatorManager.AnimationType.IDLE*/)
    {
        _canRun = false;
        endScreen.SetActive(true);
        //animatorManager.Play(AnimatorManager.AnimationType.DEATH);
    }

    public void StartToRun()
    {
        animator.SetBool(soPlayerSetup.runBool, true);
        startScreen.SetActive(false);
        _canRun = true;
        //animatorManager.Play(AnimatorManager.AnimationType.RUN /* , _currentSpeed / _baseSpeedToAnimation */);
    }

    #region POWER UPS

    public void SetPowerUpText(string s)
    {
        uiTextPowerUp.text = s;
    }

    public void PowerUpSpeedUp(float f)
    {
        _currentSpeed = f;
    }

    public void ResetSpeed()
    {
        _currentSpeed = speed;
    }

    public void SetInvincible(bool b = true)
    {
        invincible = b;
    }

    public void ChangeHeight(float amount, float duration, float animationDuration, Ease ease)
    {
        //var p = transform.position;
        //p.y = _startPosition.y + amount;
        //transform.position = p;

        transform.DOMoveY(_startPosition.y + amount, animationDuration).SetEase(ease);
        Invoke(nameof(ResetHeight), duration);
    }

    public void ResetHeight(float animationDuration)
    {
        //var p = transform.position;
        //p.y = _startPosition.y;
        //transform.position = p;

        transform.DOMoveY(_startPosition.y, animationDuration);
    }

    public void ChangeCoinCollectorSize(float amount)
    {
        coinCollector.transform.localScale = Vector3.one * amount;
    }

    #endregion
}
