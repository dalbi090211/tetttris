using UnityEngine;
using FMOD.Studio;

public class TitleScene : MonoBehaviour
{
    private EventInstance titleBGMInstance;
    
    private void Awake(){
        // 다른 스크립트들이 완전히 초기화될 때까지 잠시 대기
        Invoke("InitializeBGM", 0.1f);
    }
    
    private void InitializeBGM(){
        titleBGMInstance = SoundManager.instance.CreateInstance(FmodEvents.instance.TitleBGM);
        titleBGMInstance.setVolume(0.5f);
        titleBGMInstance.start();
    }
    
    private void OnDestroy(){
        titleBGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        titleBGMInstance.release();
    }
}
