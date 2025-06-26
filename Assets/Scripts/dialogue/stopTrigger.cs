using UnityEngine;

public class stopTrigger : MonoBehaviour
{
    [SerializeField] private int sceneNumber;

    public void OnTriggerEnter2D(Collider2D other){
        Debug.Log("OnTriggerEnter2D 호출됨: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");
        if(other.gameObject.tag == "Player"){
            Debug.Log("Player와 충돌! stopMove 호출");
            SceneMoveManager.instance.stopMove(sceneNumber);
        }
        Destroy(this.gameObject);
    }
}
