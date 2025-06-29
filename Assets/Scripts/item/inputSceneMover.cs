using UnityEngine;
using System.Collections;

public class inputSceneMover : MonoBehaviour
{ 
    public static inputSceneMover instance;
    
    [SerializeField] private float move_distance = 4.99f;
    [SerializeField] private float move_duration = 1f;
    
    private bool is_moving = false;
    
    void Awake(){
        instance = this;
    }
    
    public void moveScene(){
        if (!is_moving)
        {
            move_distance *= -1;
            StartCoroutine(moveSceneCoroutine());
        }
    }
    
    private IEnumerator moveSceneCoroutine(){
        is_moving = true;
        float start_position = transform.position.x;
        float elapsed_time = 0f;
        
        while (elapsed_time < move_duration)
        {
            elapsed_time += Time.deltaTime;
            float progress = (elapsed_time / move_duration) * move_distance;
            transform.position = new Vector3(start_position + progress, transform.position.y, transform.position.z);
            yield return null;
        }
        
        // 정확한 목표 위치로 설정
        transform.position = new Vector3(start_position + move_distance, transform.position.y, transform.position.z);
        selection_manager.instance.endMove();
        is_moving = false;
    }
}
