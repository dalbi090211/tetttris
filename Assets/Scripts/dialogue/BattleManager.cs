using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    //================================ 실계산용 =================================
    public int player_hp;
    public int stage_hp;

    //================================ 고정값 =================================
    public int stage1_hp = 100;
    public int stage2_hp = 150;
    public int stage3_hp = 200;

    //================================ 변동값 =================================
    public int player_maxHp = 100;



    private void Awake()
    {
        instance = this;
    }

    public void setBattle(int scene_num){   //SceneMoveManager에서 다음 씬에 도착했고 전투를 시작할 때 호출
        player_hp = player_maxHp;
        switch(scene_num){
            case 1:
                stage_hp = stage1_hp;
                break;
            case 2:
                stage_hp = stage2_hp;
                break;
            case 3:
                stage_hp = stage3_hp;
                break;
        }
    }

    public void player_attack(int[] blocks){   //Player에서 공격할 때 호출
        player_hp -= 10;

        /**
         * 대충 한번에 부순 블록조합에 따라 퍽들에 따라 증가한 계수로 데미지 증가해서 stage_hp에 빼주는 로직
         */

        //debug용
        stage_hp -= 2000;
        SceneMoveManager.instance.updateBossHp();
        if(stage_hp <= 0){  //죽기 전에 깨면 ok
            winBattle();
        }
        if(player_hp <= 0){
            loseBattle();   
        }
    }

    public void player_overlap(int damage){   //Player에서 공격할 때 호출
        player_hp -= damage;
    }

    private void winBattle(){
        SceneMoveManager.instance.winFight(SceneMoveManager.instance.current_scene);
    }

    private void loseBattle(){
        /**
         * 대충 player_death 애니메이션 재생하고 슬픈 브금 들리고 죽음씬 천천히 알파값 100으로 올라가는 연출
         */
    }

}
