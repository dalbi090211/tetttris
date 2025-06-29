using UnityEngine;

public class ItemSelectButton : MonoBehaviour
{
    public item_name item_name;
    public void OnClick()
    {
        selection_manager.instance.UseItem(item_name);
    }
}
