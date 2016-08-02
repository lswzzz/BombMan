using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour {

    private Animator m_Animator;
    private PlayerDirection direction;
    private PlayerState state;
    private string DirectionString = "Direction";
    private string StateString = "State";
    private string TrapLevelString = "TrapLevel";
    public float trap1Time = 4f;
    public float trap3Time = 2f;
    public int blinkLevel;
    // Use this for initialization
    void Start() {
        m_Animator = GetComponent<Animator>();
        direction = PlayerDirection.Down;
        state = PlayerState.Idle;
        m_Animator.SetInteger(DirectionString, (int)direction);
        m_Animator.SetInteger(StateString, (int)state);
        blinkLevel = 1;
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {
    }

    public PlayerDirection Direction {
        get { return direction; }
    }

    public PlayerState State
    {
        get { return state; }
    }

    public TrapLevel ITrapLevel {
        get { return (TrapLevel)m_Animator.GetInteger(TrapLevelString); }
    }

    public void AcceptInput(PlayerDirection IDirection, PlayerState IState)
    {
        direction = IDirection;
        m_Animator.SetInteger(DirectionString, (int)direction);
        switch (IState) {
            case PlayerState.Idle:
                if (CanIdle) state = PlayerState.Idle;
                break;
            case PlayerState.Movement:
                if (CanMove) state = PlayerState.Movement;
                break;
        }
        m_Animator.SetInteger(StateString, (int)state);
    }

    public void AcceptSpecialState(PlayerDirection IDirection, PlayerState IState)
    {
        direction = IDirection;
        m_Animator.SetInteger(DirectionString, (int)direction);
        switch (IState)
        {
            case PlayerState.Save:
                if (CanSave)
                {
                    StopAllCoroutines();
                    state = PlayerState.Save;
                }
                break;
            case PlayerState.Trap:
                if (CanTrap)
                {
                    state = PlayerState.Trap;
                    StartCoroutine(Trap1());
                }
                break;
            case PlayerState.Death:
                if (CanDeath)
                {
                    state = PlayerState.Death;
                }
                break;
            case PlayerState.Idle:
                state = PlayerState.Idle;
                break;
            case PlayerState.Movement:
                state = PlayerState.Movement;
                break;
        }
        m_Animator.SetInteger(StateString, (int)state);
    }

    public bool CanDeath {
        get
        {
            if (state == PlayerState.Trap) return true;
            return false;
        }
    }

    public bool CanTrap
    {
        get
        {
            if (state == PlayerState.Death || state == PlayerState.Trap || state == PlayerState.Save) return false;
            return true;
        }
    }

    public bool CanSave
    {
        get
        {
            if(state == PlayerState.Trap)
            {
                TrapLevel level = ITrapLevel;
                if (level != TrapLevel.Trap3) return true;
            }
            return false;
        }
    }

    public bool CanMove
    {
        get
        {
            if (state == PlayerState.Death || state == PlayerState.Trap || state == PlayerState.Save) return false;
            return true;
        }
    }

    public bool CanIdle
    {
        get
        {
            if (state == PlayerState.Death || state == PlayerState.Trap || state == PlayerState.Save) return false;
            return true;
        }
    }

    IEnumerator Trap1()
    {
        yield return new WaitForSeconds(trap1Time);
        m_Animator.SetInteger(TrapLevelString, (int)TrapLevel.Trap2);
    }

    public void Trap2()
    {
        StartCoroutine(Trap3());
        m_Animator.SetInteger(TrapLevelString, (int)TrapLevel.Trap3);
    }

    IEnumerator Trap3()
    {
        yield return new WaitForSeconds(trap3Time);
        //不可以在DeathStartBlink中设置因为那个方法可能是其他玩家杀死当前玩家的时候用的
        //这里要注意如果是robot的话他自身无法变成death的状态需要接受到服务器的转发数据才能进行变换
        PlayerBehaviourCollector collector = GetComponent<PlayerBehaviourCollector>();
        if(collector != null)
        {
            collector.SendDeathState();
            AcceptSpecialState(PlayerDirection.Down, PlayerState.Death);
        }
    }

    public void DeathStartBlink()
    {
        StopAllCoroutines();
        StartCoroutine(DeathBlink());
        if (GetComponent<PlayerInputController>() != null) GetComponent<PlayerInputController>().startAccept = false;
    }

    IEnumerator DeathBlink()
    {
        yield return new WaitForSeconds(0.1f);
        float value = blinkLevel / 1f;
        GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, value);
        blinkLevel--;
        if (blinkLevel < 0) blinkLevel = 1;
        StartCoroutine(DeathBlink());
    }

    public void clearPlayer()
    {
        StopAllCoroutines();
        AcceptSpecialState(PlayerDirection.Down, PlayerState.Idle);
        GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
    }

    public void SaveStartBlink()
    {
        StartCoroutine(SaveBlink());
        m_Animator.SetInteger(TrapLevelString, 0);
        if (GetComponent<PlayerInputController>() != null) GetComponent<PlayerInputController>().startAccept = false;
    }

    IEnumerator SaveBlink()
    {
        yield return new WaitForSeconds(0.1f);
        float value = blinkLevel / 1f;
        GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, value);
        blinkLevel--;
        if (blinkLevel < 0) blinkLevel = 1;
        StartCoroutine(SaveBlink());
    }

    public void BeSaveOK()
    {
        StopAllCoroutines();
        GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
        if (GetComponent<PlayerInputController>() != null) GetComponent<PlayerInputController>().startAccept = true;
        AcceptSpecialState(PlayerDirection.Down, PlayerState.Idle);
    }
}
