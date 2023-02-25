using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using NTC.Global.Cache;
using UnityEngine.Events;


public enum EnemyBaceBehaviors
{
    Fly,
    Walk,
    Stand,
    Roll
}
public enum EnemyBaceActions
{
    Run,
    Attack,
    None
}

[Serializable] public class EnemyOnHpChanged : UnityEvent<float> { }
[Serializable] public class PlayerOnAmmoChanged : UnityEvent<float, float, float> { }

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoCache, IDamagable
{
    [Header("Inheritanced fields")]
    public string EntityName = "/n";
    [SerializeField]
    private TextMeshProUGUI nameText;
    public EnemyBaceActions enemyBaceAction;
    public bool Flipable = false;
    public bool Flip = false;
    public bool FlipLocalScale = false;
    [SerializeField]
    protected float health;
    [HideInInspector]
    public float maxHealth;
    [HideInInspector]
    public EnemyOnHpChanged OnHpChanged;
    [HideInInspector]
    public UnityEvent OnJumped;
    [HideInInspector]
    public UnityEvent OnDie;
    [HideInInspector]
    public UnityEvent OnInvItemRemoved;
    public bool dynamicMaxHp = true;

    public float hp
    {
        get => GetHp();
        set => SetHp(value);
    }
    protected virtual void SetHp(float value)
    {
        if (!dynamicMaxHp)
        {
            health = value;
            if (health > maxHealth)
                maxHealth = health;
        }
        else
        {
            health = Mathf.Min(maxHealth, value);
        }
    }
    protected virtual float GetHp() => health;

    [Min(0)]
    public float musicArm;
    [Min(0)]
    public float handArm;
    public DropObj[] Drop;
    public GameObject MusicNote;
    public Transform JumpTarget;
    public GameObject WingsPos;
    public LayerMask groundLayer;
    [HideInInspector]
    public Animator an;
    [HideInInspector]
    public Transform tr;
    [HideInInspector]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    [Header("Custom fields")]
    public bool dont_totch_me;

    public virtual void Awake()
    {
        maxHealth = health;
        tr = transform;
        rb = gameObject.GetComponent<Rigidbody2D>();
        an = gameObject.GetComponent<Animator>();
        if (nameText)
            nameText.transform.parent.gameObject.SetActive(true);
        OnHpChanged = new EnemyOnHpChanged();
        OnJumped = new UnityEvent();
        OnDie = new UnityEvent();
        NewAwake();
    }
    protected virtual void NewAwake() { }
    protected virtual void NewFixedUpdate() { }
    protected virtual void NewUpdate() { }
    protected virtual void NewLateUpdate() { }
    protected virtual void NewOnCollisionEnter2D(Collision2D collision) { }
    protected virtual void NewOnTriggerStay2D(Collider2D collision) { }
    protected virtual void NewOnTriggerEnter2D(Collider2D collision) { }
    protected virtual void NewOnTriggerExit2D(Collider2D collision) { }

    protected sealed override void FixedRun()
    {
        if (hp > 0)
        {
            NewFixedUpdate();
        }
    }
    protected sealed override void Run()
    {
        if (hp > 0)
        {
            NewUpdate();
        }
    }
    protected sealed override void LateRun()
    {
        if (hp > 0)
        {
            if (!gameObject.CompareTag("Player") && rb)
            {
                if (!FlipLocalScale)
                    sr.flipX = (Flip ? rb.velocity.x < 0 : rb.velocity.x > 0) && rb.velocity.x != 0 && Flipable;
                else
                    sr.gameObject.transform.localScale = new Vector3(sr.gameObject.transform.localScale.x * ((Flip ? rb.velocity.x < 0 : rb.velocity.x > 0) && rb.velocity.x != 0 && Flipable ? -1 : 1), sr.gameObject.transform.localScale.y, sr.gameObject.transform.localScale.z);
                nameText.text = EntityName == "/n" ? "" : EntityName;
            }
            NewLateUpdate();
        }
        if (hp <= 0 && hp > -100 && !gameObject.CompareTag("Player"))
        {
            an.Play("die");
            an.SetBool("daed", true);
            hp = -101;
        }
    }
    public virtual void SelfDestroy()
    {
        OnDie.Invoke();
        DropObj.Drop(transform.position, Drop);
        StopAllCoroutines();
        Destroy(gameObject);
    }
    public virtual void Jump(float jumpForce)
    {
        JumpTarget.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(45, 135));
        rb.AddForce(JumpTarget.right * jumpForce, ForceMode2D.Impulse);
        OnJumped.Invoke();
        an.Play("jump");
    }
    public virtual void AddDamage(float d, bool byHand)
    {
        an.Play("damage");
        d = Mathf.Max(d, 0);
        hp = hp - (d == 0 ? 1 : (d / (float)(byHand ? (handArm == 0 ? 1 : Math.Max(handArm, 1f)) : (musicArm == 0 ? 1 : Math.Max(musicArm, 1f)))));
        OnHpChanged.Invoke(hp);
    }
    public virtual void Heal(float d)
    {
        d = Mathf.Max(d, 0);
        hp = hp + d;
        OnHpChanged.Invoke(hp);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hp > 0)
        {
            NewOnCollisionEnter2D(collision);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hp > 0 && collision.gameObject.layer != 12)
        {
            MyTrigger mt = collision.GetComponent<MyTrigger>();
            if (mt && mt.LayerToActivate.Contains(gameObject.layer))
            {
                mt.Activate(this);
                mt.UpdateNebActivate(this);
            }
            NewOnTriggerEnter2D(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hp > 0)
        {
            MyTrigger mt = collision.GetComponent<MyTrigger>();
            if (mt && mt.LayerToActivate.Contains(gameObject.layer))
            {
                mt.Diactivate(this);
                mt.UpdateNebDiactivate();
            }
            NewOnTriggerExit2D(collision);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hp > 0)
        {
            NewOnTriggerStay2D(collision);
        }
    }
}

public interface IDamagable
{
    public void AddDamage(float damage, bool byHand);
}
