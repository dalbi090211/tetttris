using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour {    //객체보단 구조체에 가까움
    [field : Header("Music")]
    [SerializeField] public EventReference TotalBGM;

    [field : Header("SFX")]
    [SerializeField] public EventReference cornifer;
    [SerializeField] public EventReference talkLoop;
    [SerializeField] public EventReference talkEvent;
    [SerializeField] public EventReference hiddenRoomReveal;
    [SerializeField] public EventReference playerAttack1;
    [SerializeField] public EventReference playerAttack2;
    [SerializeField] public EventReference playerAttack3;
    [SerializeField] public EventReference playerJump;
    [SerializeField] public EventReference playerJumpAttack;
    [SerializeField] public EventReference playerWalk;
    [SerializeField] public EventReference playerHitted;
    [SerializeField] public EventReference bossHitted;
    [SerializeField] public EventReference raccoonAtk;
    [SerializeField] public EventReference raccoonThrow;
    [SerializeField] public EventReference playerRoll;
    [SerializeField] public EventReference bench;

    public static FmodEvents instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
}
