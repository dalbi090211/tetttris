using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour {    //객체보단 구조체에 가까움
    [field : Header("Music")]
    [SerializeField] public EventReference TotalBGM;
    [SerializeField] public EventReference TitleBGM;

    [field : Header("SFX")]
    

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

    void Start()
    {
        // Start 메서드 제거
    }
}
