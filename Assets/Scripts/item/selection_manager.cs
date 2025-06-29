using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class item_icon{
    public item_name item_name;
    public Sprite item_sprite;
}

public enum item_name
{
    bomb,
    broom,
    candle,
    cannon,
    shield,
    sword,
    trash,
}   

public class selection_manager : MonoBehaviour
{
    private Stack<item_name> can_select_item_list;
    private item_name[] cur_select_item_list;
    [SerializeField] private List<selection> selection_list;
    private int user_select = 0;
    private int max_select = 3;
    [SerializeField] private List<item_icon> item_icon_list;
    
    public static selection_manager instance;
    Dictionary<item_name, Iitem> item_list = new Dictionary<item_name, Iitem>();
    
    private void Awake()
    {
        instance = this;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item_list.Add(item_name.bomb, new bomb());
        item_list.Add(item_name.broom, new broom());
        item_list.Add(item_name.candle, new candle());
        item_list.Add(item_name.cannon, new cannon());
        item_list.Add(item_name.shield, new shield());
        item_list.Add(item_name.sword, new sword());
        item_list.Add(item_name.trash, new trash());

        can_select_item_list = new Stack<item_name>();
        can_select_item_list.Push(item_name.bomb);
        can_select_item_list.Push(item_name.broom);
        can_select_item_list.Push(item_name.candle);
        can_select_item_list.Push(item_name.cannon);
        can_select_item_list.Push(item_name.shield);
        can_select_item_list.Push(item_name.sword);
        can_select_item_list.Push(item_name.trash);
    }

    // 아이템 리스트를 랜덤으로 섞는 함수
    private void shuffleItemList()
    {
        // Stack을 List로 변환
        List<item_name> itemList = new List<item_name>();
        while (can_select_item_list.Count > 0)
        {
            itemList.Add(can_select_item_list.Pop());
        }
        
        // Fisher-Yates 셔플 알고리즘 사용
        for (int i = itemList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            item_name temp = itemList[i];
            itemList[i] = itemList[randomIndex];
            itemList[randomIndex] = temp;
        }
        
        // 섞인 리스트를 다시 Stack에 넣기
        foreach (item_name item in itemList)
        {
            can_select_item_list.Push(item);
        }
    }

    public void UseItem(item_name item_name)
    {
        if(user_select == 0){
            item_list[item_name].Use();
            for(int i = 0; i < cur_select_item_list.Length; i++){
                if(cur_select_item_list[i] != item_name){
                    can_select_item_list.Push(cur_select_item_list[i]);
                    
                }
            }
            user_select = 1;
            SceneMoveManager.instance.endSelection();
        }
    }

    private string itemExplain(item_name item_name){
        switch(item_name){  //debug
            case item_name.bomb:
                return "블록의 선택 시간이 감소하는 대신 점수의 배율 50%이 증가합니다.";
            case item_name.broom:
                return "덱에 2개의 ㅣ자와 ㅁ자 블록을 추가합니다.";
            case item_name.candle:
                return "덱에 모든 ㄹ자를 제거합니다.";
            case item_name.cannon:
                return "동일한 유형의 블록을 여러 개 클리어 했을 시의 추가 배율이 20% 증가합니다.";
            case item_name.shield:
                return "최대 체력이 20 증가합니다.";
            case item_name.sword:
                return "콤보의 배율이 40% 증가합니다.";
            case item_name.trash:
                return "자해하면서 공격 시 점수의 배율이 60% 증가합니다.";
            default:
                return "null";
        }
    }

    public void endMove(){
        user_select = 0;
    }

    public void startSelection(){
        user_select = -1;
        
        // 아이템 리스트를 랜덤으로 섞기
        shuffleItemList();
        
        if (inputSceneMover.instance != null)
        {
            inputSceneMover.instance.moveScene();
        }
        
        if (selection_list != null && selection_list.Count > 0)
        {
            cur_select_item_list = new item_name[selection_list.Count];
            for(int i = 0; i < max_select && i < selection_list.Count; i++){
                if (can_select_item_list.Count > 0)
                {
                    item_name item_name = can_select_item_list.Pop();
                    cur_select_item_list[i] = item_name;
                    string item_explain = itemExplain(item_name);
                    Sprite item_sprite = item_icon_list.Find(x => x.item_name == item_name).item_sprite;
                    if (selection_list[i] != null)
                    {
                        selection_list[i].reflesh(item_explain, item_sprite, item_name);
                    }
                }
            }
        }
    }
}
