using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class SceneMoveManager : MonoBehaviour
{
    public CancellationTokenSource tokenSource;
    public static SceneMoveManager instance;
    public int current_scene = 1;   //원래는 튜토리얼씬이 0번인데 없어서 1부터 시작

    //================================ player =================================
    [SerializeField] private Animator playerAnimator;

    //================================ scene1 =================================
    [SerializeField] private Animator scene1_goblin;
    [SerializeField] private Animator scene1_mushroom;
    [SerializeField] private Animator scene1_skeleton;

    // //================================ scene2 =================================
    // [SerializeField] private Animator scene2_goblin;
    // [SerializeField] private Animator scene2_mushroom;
    // [SerializeField] private Animator scene2_skeleton;


    private void Awake()
    {
        instance = this;
        tokenSource = new CancellationTokenSource();
        moveNextStage(tokenSource.Token).Forget();
        // stopMove();
    }

    public async UniTask moveNextStage(CancellationToken token)
    {
        playerAnimator.Play("HeroKnight_Run");
        while(true)
        {
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
    }

    public void stopMove()
    {
        playerAnimator.Play("HeroKnight_Idle");
        if(tokenSource != null)
        {
            startFight(current_scene);  //현재는 멈추는 즉시 전투진입, 이동중에 테트리스가 비활성화 되어있다가 전투시 활성화 되는 연출이 들어갈수도 있고 전투 전 컷신이 필요시 옮길수도 있음
            tokenSource.Cancel();
        }
        tokenSource = new CancellationTokenSource();
    }

    public void startFight(int scene_num){
        BattleManager.instance.setBattle(scene_num);
        switch(scene_num){
            case 1:
                playerAnimator.Play("HeroKnight_Attack1");  //공격 시에만 들어갈수도?
                scene1_goblin.Play("goatk1");
                scene1_mushroom.Play("mushatk1");
                scene1_skeleton.Play("skeletonatk1");
                break;
        }
    }

    public void winFight(int scene_num){
        switch(scene_num){
            case 1:
                scene1_goblin.Play("go_death");
                scene1_mushroom.Play("mushdeath");
                scene1_skeleton.Play("skeletondeath");
                moveNextStage(tokenSource.Token).Forget();
                break;
        }
    }

    private void OnDestroy()
    {
        if(tokenSource != null)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
    }
}


