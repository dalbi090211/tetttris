using UnityEngine;
using System.Collections.Generic;

public class bomb : Iitem
{
    public void Use()
    {
        Debug.Log("bomb");
    }
}

public class broom : Iitem
{
    public void Use()
    {
        // 현재 배열의 길이를 가져와서 2개 더 큰 새 배열 생성
        tetris_type[] currentTypes = tetris.instance.current_type;
        tetris_type[] newTypes = new tetris_type[currentTypes.Length + 2];
        
        // 기존 배열 복사
        for (int i = 0; i < currentTypes.Length; i++)
        {
            newTypes[i] = currentTypes[i];
        }
        
        // ㅣ와 ㅁ을 하나씩 추가
        newTypes[currentTypes.Length] = tetris_type.ㅣ;
        newTypes[currentTypes.Length + 1] = tetris_type.ㅁ;
        
        // 새 배열을 current_type에 할당
        tetris.instance.current_type = newTypes;
        
    }
}

public class candle : Iitem
{
    public void Use()
    {
        // 현재 배열에서 ㄹ과 reverse_ㄹ이 아닌 요소들만 필터링
        tetris_type[] currentTypes = tetris.instance.current_type;
        List<tetris_type> filteredTypes = new List<tetris_type>();
        
        for (int i = 0; i < currentTypes.Length; i++)
        {
            if (currentTypes[i] != tetris_type.ㄹ && currentTypes[i] != tetris_type.reverse_ㄹ)
            {
                filteredTypes.Add(currentTypes[i]);
            }
        }
        // 필터링된 리스트를 배열로 변환하여 할당
        tetris.instance.current_type = filteredTypes.ToArray();
    }
}

public class cannon : Iitem
{
    public void Use()
    {
        BattleManager.instance.comboRatio += 0.2f;
        Debug.Log("cannon");
    }
}

public class shield : Iitem
{
    public void Use()
    {
        BattleManager.instance.player_maxHp += 20;
        Debug.Log("shield");
    }
}

public class sword : Iitem
{
    public void Use()
    {
        BattleManager.instance.comboRatio *= 1.4f;
        Debug.Log("sword");
    }
}

public class trash : Iitem
{
    public void Use()
    {
        BattleManager.instance.can_trash = true;
        Debug.Log("trash");
    }
}

