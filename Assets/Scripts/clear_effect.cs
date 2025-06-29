using UnityEngine;
using Cysharp.Threading.Tasks;

public class clear_effect : MonoBehaviour
{
    private float abs_unit;
    private Vector3 offset;
    private float scale_duration = 0.05f;
    private float hold_duration = 0.05f;
    
    public void Initialize(float abs_unit, Vector3 offset)
    {
        this.abs_unit = abs_unit;
        this.offset = offset;
    }
    
    public async UniTask play_effect(int x, int y, bool is_row)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x * abs_unit + offset.x, y * abs_unit + offset.y);
        
        // 초기 스케일 설정 (0.8)
        Vector3 initialScale = new Vector3(0.8f, 0.8f, 1f);
        Vector3 targetScale = new Vector3(1f, 1f, 1f);
        
        if (is_row)
        {
            // Row 애니메이션: X값이 0.8에서 1로 커짐
            initialScale.x = 0.8f;
            targetScale.x = 1f;
            initialScale.y = 1f;
            targetScale.y = 1f;
        }
        else
        {
            // Col 애니메이션: Y값이 0.8에서 1로 커짐
            initialScale.x = 1f;
            targetScale.x = 1f;
            initialScale.y = 0.8f;
            targetScale.y = 1f;
        }
        
        // 초기 스케일 설정
        rect.localScale = initialScale;
        
        // 0.05초 동안 스케일 애니메이션
        float elapsedTime = 0f;
        while (elapsedTime < scale_duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scale_duration;
            rect.localScale = Vector3.Lerp(initialScale, targetScale, progress);
            await UniTask.DelayFrame(1);
        }
        
        // 최종 스케일로 설정
        rect.localScale = targetScale;
        
        // 0.05초 동안 유지
        await UniTask.Delay((int)(hold_duration * 1000));
        
        // 오브젝트 제거
        Destroy(gameObject);
    }
}
