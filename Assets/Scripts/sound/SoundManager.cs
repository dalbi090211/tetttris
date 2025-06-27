using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.PlayerLoop;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;

public enum BGMArea{
    Shop = 0,
    Scene1 = 1,
    Scene2 = 2,
    Scene3 = 3

}

public class SoundManager : MonoBehaviour {
    public static SoundManager instance;
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance BGMInstance;
    private BGMArea currentBGM;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            eventInstances = new List<EventInstance>();
            eventEmitters = new List<StudioEventEmitter>();
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // FmodEvents가 초기화된 후에 BGM 인스턴스 생성
        if(!BGMInstance.isValid() && FmodEvents.instance != null){
            createBGMInstance(FmodEvents.instance.TotalBGM);
        }
    }


    public void PlayOneShot(EventReference sound, Vector3 worldPos){
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public async UniTask PlaySoundStopBGM(EventReference sound, Vector3 worldPos, float sound_duration){
        Debug.Log("PlaySoundStopBGM 시작");
        StopBGM();
        EventInstance soundInstance = RuntimeManager.CreateInstance(sound);
        soundInstance.start();
        await UniTask.Delay(TimeSpan.FromSeconds(sound_duration));
        soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        soundInstance.release();
        ResumeBGM();
        Debug.Log("PlaySoundStopBGM 완료");
    }

    // public void initializeAmbience(EventReference ambienceEventReference){
    //     ambienceEventInstance = CreateInstance(ambienceEventReference);
    //     ambienceEventInstance.start();
    // }

    public EventInstance CreateInstance(EventReference eventReference){ 
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void CreateInstance(EventReference eventReference, float pitch){
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.setPitch(pitch);
        eventInstance.start();
    }

    private StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject){
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void SetBGMParameter(BGMArea area){
        BGMInstance.setParameterByName("Scene", (float)area);
    }

    public void createBGMInstance(EventReference eventReference){
        if(!BGMInstance.isValid()){
            BGMInstance = CreateInstance(eventReference);
            BGMInstance.setParameterByName("Scene", (float)BGMArea.Scene1);
            currentBGM = BGMArea.Scene1;
            BGMInstance.start();
        }
    }

    public void StopBGM(){
        BGMInstance.setPaused(true);
    }

    public void ResumeBGM(){
        BGMInstance.setPaused(false);
    }

    public void SetBGM(BGMArea area)
    {
        Debug.Log("isValid : " + BGMInstance.isValid());
        Debug.Log("curMusic : " + BGMInstance.isValid());

        if(currentBGM == area){
            return;
        }

        // 기존 BGM 정리
        if (BGMInstance.isValid())
        {
            BGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            BGMInstance.release();
        }

        // 새 인스턴스 생성 및 재생
        // musicEventInstance = CreateInstance(FmodEvents.instance.TotalBGM);
        BGMInstance.setParameterByName("Scene", (float)area);
        BGMInstance.start();
    }
    
    public BGMArea GetCurBGM(){
        float areaValue;
        BGMInstance.getParameterByName("Scene", out areaValue);
        return (BGMArea)areaValue;
    }

    public void stopBGM(){
        BGMInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BGMInstance.release();
    }

    // void OnDestroy() {
    //     Debug.Log("SoundManager Destroyed: " + gameObject.GetInstanceID());
    //     if (musicEventInstance.isValid())
    //     {
    //         musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    //         musicEventInstance.release();
    //     }
    // }
    
}