using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class SceneMoveManager : MonoBehaviour
{
    public CancellationTokenSource tokenSource;
    public static SceneMoveManager instance;
    [SerializeField] private float animation_speed = 0.7f;
    public int current_scene = 1;   //원래는 튜토리얼씬이 0번인데 없어서 1부터 시작
    [SerializeField] private GameObject boss_hp_ui;
    private TextMeshProUGUI boss_name_text;
    private Slider boss_hp_slider;
    private float first_stage_hp;

    //================================ player =================================
    [SerializeField] private Animator playerAnimator;

    //================================ scene1 =================================
    [SerializeField] private string scene1_boss_name;
    [SerializeField] private Animator scene1_goblin;
    [SerializeField] private Animator scene1_mushroom;
    [SerializeField] private Animator scene1_skeleton;

    //================================ scene2 =================================
    [SerializeField] private string scene2_boss_name;
    [SerializeField] private Animator scene2_sword;
    [SerializeField] private Animator scene2_spear;
    [SerializeField] private Animator scene2_bow;

    //================================ scene3 =================================
    [SerializeField] private string scene3_boss_name;
    [SerializeField] private Animator scene3_mage1;
    [SerializeField] private Animator scene3_ehero;
    [SerializeField] private Animator scene3_mage2;

    //================================ scene4 =================================
    [SerializeField] private string scene4_boss_name;
    [SerializeField] private Animator scene4_king;


    private void Awake()
    {
        instance = this;
        tokenSource = new CancellationTokenSource();
        
        // 모든 애니메이터의 속도를 0.7로 설정
        playerAnimator.speed = animation_speed;
        scene1_goblin.speed = animation_speed;
        scene1_mushroom.speed = animation_speed;
        scene1_skeleton.speed = animation_speed;
        scene2_sword.speed = animation_speed;
        scene2_spear.speed = animation_speed;
        scene2_bow.speed = animation_speed;
        scene3_mage1.speed = animation_speed;
        scene3_ehero.speed = animation_speed;
        scene3_mage2.speed = animation_speed;
        scene4_king.speed = animation_speed;
        
        moveNextStage(tokenSource.Token).Forget();
        // stopMove();
        boss_name_text = boss_hp_ui.transform.Find("bossname").GetComponent<TextMeshProUGUI>();
        boss_hp_slider = boss_hp_ui.transform.Find("hpbar").GetComponent<Slider>();
        boss_hp_ui.SetActive(false);
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

    public void stopMove(int scene_num)
    {
        current_scene = scene_num;
        BGMArea area = (BGMArea)current_scene;
        SoundManager.instance.SetBGM(area);
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
                boss_name_text.text = scene1_boss_name;
                break;
            case 2:
                playerAnimator.Play("HeroKnight_Attack1");
                scene2_sword.Play("sword_atk1");
                scene2_spear.Play("spear_atk1");
                scene2_bow.Play("bow_atk1");
                boss_name_text.text = scene2_boss_name;
                break;
            case 3:
                playerAnimator.Play("HeroKnight_Attack1");
                scene3_mage1.Play("mage_atk1");
                scene3_ehero.Play("ehero_atk1");
                scene3_mage2.Play("mage_atk1");
                boss_name_text.text = scene3_boss_name;
                break;
            case 4:
                playerAnimator.Play("HeroKnight_Attack1");
                scene4_king.Play("eking_atk1");
                boss_name_text.text = scene4_boss_name;
                break;
        }
        first_stage_hp = BattleManager.instance.stage_hp;
        boss_hp_slider.value = 1.0f;
        boss_hp_ui.SetActive(true);
    }

    public void updateBossHp(){
        boss_hp_slider.value = (float)BattleManager.instance.stage_hp / first_stage_hp;
    }

    public void winFight(int scene_num){
        switch(scene_num){
            case 1:
                scene1_goblin.Play("go_death");
                scene1_mushroom.Play("mushdeath");
                scene1_skeleton.Play("skeletondeath");
                moveNextStage(tokenSource.Token).Forget();
                break;
            case 2:
                scene2_sword.Play("sword_dead");
                scene2_spear.Play("spear_dead");
                scene2_bow.Play("bow_dead");
                moveNextStage(tokenSource.Token).Forget();
                break;
            case 3:
                scene3_mage1.Play("mage_dead");
                scene3_ehero.Play("ehero_dead");
                scene3_mage2.Play("mage_dead");
                moveNextStage(tokenSource.Token).Forget();
                break;
            case 4:
                playerAnimator.Play("HeroKnight_Idle");
                scene4_king.Play("eking_dead");
                //대충 엔딩 처리하는 내용
                break;
        }
        boss_hp_ui.SetActive(false);
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


