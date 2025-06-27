using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject tetris_ui;
    private Slider player_hp_slider;
    private TextMeshProUGUI combo_text;
    private TextMeshProUGUI damage_text;
    public static BattleManager instance;
    

    //================================ 실계산용 =================================
    public int player_hp;
    public int stage_hp;
    private int combo_count;

    //================================ 고정값 =================================
    public int stage1_hp = 100;
    public int stage2_hp = 150;
    public int stage3_hp = 200;

    //================================ 변동값 =================================
    public int player_maxHp = 100;
    private int comboSeq = 1;

    public void updatePlayerHP(){
        player_hp_slider.value = (float)player_hp / player_maxHp;
    }

    private void Awake()
    {
        player_hp_slider = tetris_ui.transform.Find("PlayerHPBar").GetComponent<Slider>();
        combo_text = tetris_ui.transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
        damage_text = tetris_ui.transform.Find("DamageText").GetComponent<TextMeshProUGUI>();
        combo_text.alpha = 0;
        damage_text.alpha = 0;
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

    public Boolean player_attack(int[] blocks, Boolean is_combo){   //Player에서 공격할 때 호출
        //debug용
        int damage = 50;

        if(is_combo){
            comboSeq++;
        }
        else{
            comboSeq = 1;
        }
        player_hp -= 10;
        updatePlayerHP();
        SceneMoveManager.instance.play_attack_animation();
        update_damage_text(damage, comboSeq).Forget();
        
        /**
         * 대충 한번에 부순 블록조합에 따라 퍽들에 따라 증가한 계수로 데미지 증가해서 stage_hp에 빼주는 로직
         */

        //debug용
        stage_hp -= damage;
        SceneMoveManager.instance.updateBossHp();
        if(stage_hp <= 0){  //죽기 전에 깨면 ok
            winBattle();
            return true;
        }
        if(player_hp <= 0){
            loseBattle();   
            return false;
        }
        return false;
    }

    private async UniTask update_damage_text(int damage, int comboSeq){
        combo_text.alpha = 1;
        damage_text.alpha = 1;
        combo_text.text = $"{comboSeq} Combo!!";
        damage_text.text = $"{damage} damage";
        
        // 초기 위치 설정 (Y: 1.9)
        RectTransform comboRect = combo_text.GetComponent<RectTransform>();
        RectTransform damageRect = damage_text.GetComponent<RectTransform>();
        Vector2 comboStartPos = comboRect.anchoredPosition;
        Vector2 damageStartPos = damageRect.anchoredPosition;
        
        float elapsedTime = 0f;
        float duration = 1.0f; // 1초
        float blinkSpeed = 0.2f; // 깜빡임 속도 (0.1초마다)
        float blinkTimer = 0f;
        bool isVisible = true;
        
        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            blinkTimer += Time.deltaTime;
            
            // Y 위치 애니메이션 (1.9 -> 2.0)
            float progress = elapsedTime / duration;
            float currentY = Mathf.Lerp(0f, 0.1f, progress);
            
            comboRect.anchoredPosition = new Vector2(comboStartPos.x, comboStartPos.y + currentY);
            damageRect.anchoredPosition = new Vector2(damageStartPos.x, damageStartPos.y + currentY);
            
            // 알파값 깜빡임
            if(blinkTimer >= blinkSpeed) {
                blinkTimer = 0f;
                isVisible = !isVisible;
                
                if(isVisible) {
                    combo_text.alpha = 1f;
                    damage_text.alpha = 1f;
                } else {
                    combo_text.alpha = 0f;
                    damage_text.alpha = 0f;
                }
            }
            
            await UniTask.DelayFrame(1);
        }
        
        // 애니메이션 완료 후 원래 위치로 복원
        comboRect.anchoredPosition = comboStartPos;
        damageRect.anchoredPosition = damageStartPos;
        combo_text.alpha = 0f;
        damage_text.alpha = 0f;
    }

    public void player_overlap(int damage){   //Player에서 공격할 때 호출
        player_hp -= damage;
        updatePlayerHP();
    }

    private void winBattle(){
        SceneMoveManager.instance.winFight(SceneMoveManager.instance.current_scene).Forget();
    }

    private void loseBattle(){
        /**
         * 대충 player_death 애니메이션 재생하고 슬픈 브금 들리고 죽음씬 천천히 알파값 100으로 올라가는 연출
         */
    }

}
