using UnityEngine;
using System.Collections;

public enum PlayerDirection
{
    Left,
    Right,
    Up,
    Down,
}

public enum PlayerState {
    Idle,
    Movement,
    Trap,
    Save,
    Death,
}

public enum TrapLevel
{
    Trap1 = 1,
    Trap2,
    Trap3,
}

public enum PlayerAction {
    None,
    Bubble,
}


