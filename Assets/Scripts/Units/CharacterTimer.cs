using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterTimer : MonoBehaviour
{
    List<Timer> timers;
    Character character;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        timers = new List<Timer>();

        timers.Add(new BlockMovingTimer(character));
        timers.Add(new InvincibilityTimer(character));
        timers.Add(new DisappearTimer(character));
    }

    public void StartTimer(string type)
    {
        int index = TypeToIndex(type);

        if (index == -1)
        {
            Debug.Log("Invalid timer type");
        }
        else
        {
            timers[index].StartTimer();
        }
    }

    void Update()
    {
        foreach (Timer timer in timers)
        {
            if (timer.IsTimerWorking())
            {
                timer.WorkTimer(Time.deltaTime);
            }
        }
    }

    static private int TypeToIndex(string type)
    {
        if (type == "BlockMoving")
        {
            return 0;
        }
        if (type == "Invincibility")
        {
            return 1;
        }
        else if (type == "DisappearBeforeDie")
        {
            return 2;
        }

        return -1;
    }
}

class Timer
{
    protected Character character;
    protected float time = 0.0f;
    protected float remainingTime = 0.0f;

    public Timer(Character character)
    {
        this.character = character;
    }

    public float RemainingTime
    {
        get { return remainingTime; }
        set { remainingTime = value; }
    }

    public virtual void StartTimer()
    {
        remainingTime = time;
    }

    public bool IsTimerWorking()
    {
        return remainingTime > 0.0f;
    }

    public virtual void WorkTimer(float deltaTime)
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0.0f)
        {
            EndTimer();
            remainingTime = 0.0f;
        }
    }

    protected virtual void EndTimer()
    {
    }
}

class BlockMovingTimer : Timer
{
    public BlockMovingTimer(Character character) :
           base(character)
    {
        time = character.BlockMovingTime;
    }

    public override void StartTimer()
    {
        base.StartTimer();
        character.IsMovingBlocked = true;
    }

    protected override void EndTimer()
    {
        character.IsMovingBlocked = false;
    }
}

class InvincibilityTimer : Timer
{
    public InvincibilityTimer(Character character):
           base(character)
    {
        time = character.InvincibleAfterDamageTime;
    }

    public override void StartTimer()
    {
        base.StartTimer();
        character.IsInvincible = true;
    }

    public override void WorkTimer(float deltaTime)
    {
        float multiplier = Mathf.Min(remainingTime - 0.3f, time - remainingTime);
        if (multiplier < 0.4f)
        {
            character.Sprite.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0.5f),
                                                multiplier / 0.4f);
        }

        base.WorkTimer(deltaTime);
    }

    protected override void EndTimer()
    {
        character.IsInvincible = false;
    }
}

class DisappearTimer : Timer
{
    public DisappearTimer(Character character):
           base(character)
    {
        time = character.DisappearBeforeDieTime;
    }

    public override void WorkTimer(float deltaTime)
    {
        character.Sprite.color = Color.Lerp(new Color(1, 0.2f, 0.2f, 0), Color.white,
                                            remainingTime / time);
        
        base.WorkTimer(deltaTime);
    }

    protected override void EndTimer()
    {
        character.Die();
    }
}
