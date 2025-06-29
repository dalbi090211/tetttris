using UnityEngine;
using TMPro;

[System.Serializable]
public class selection : MonoBehaviour
{
    private item_name item_name;
    private SpriteRenderer item_sprite;
    private TextMeshProUGUI item_explain;
    
    private void Awake()
    {
        // item이라는 이름을 가진 하위 오브젝트를 찾아서 SpriteRenderer 할당
        if (item_sprite == null)
        {
            Transform itemChild = transform.Find("item");
            if (itemChild != null)
            {
                item_sprite = itemChild.GetComponent<SpriteRenderer>();
            }
        }
        if (item_explain == null)
        {
            item_explain = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    
    public void reflesh(string item_explain, Sprite item_sprite, item_name item_name){
        // null 체크 추가
        if (this.item_explain != null)
        {
            this.item_explain.text = item_explain;
        }
        if (this.item_sprite != null)
        {
            this.item_sprite.sprite = item_sprite;
        }
        this.item_name = item_name;
    }

    public void OnClick(){
        if (selection_manager.instance != null)
        {
            selection_manager.instance.UseItem(item_name);
        }
    }
}
