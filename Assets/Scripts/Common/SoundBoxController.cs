using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoxController : MonoBehaviour
{
    [SerializeField] List<AudioClip> bgms;
    [SerializeField] List<AudioClip> sfxs;

    AudioSource player_BGM;
    AudioSource player_SFX;
    AudioSource player_SFX_2nd;
    AudioSource player_SFX_3rd;
    AudioSource player_SFX_4th; 
    AudioSource player_SFX_Loop; //loop
    protected NotOnDestroy notDestory;

    readonly Dictionary<string, string> folderPathMap = new() { { "COMMON", "Sound/InGame/Common/" } , { "MJ", "Sound/InGame/MJ/" }, { "PKBT", "Sound/InGame/Poker/" }, { "PK", "Sound/InGame/Poker/" }, { "PKD", "Sound/InGame/Poker/" }, { "PKBJ", "Sound/InGame/Poker/" } };

    public enum SoundBoxAudioType{
        BGM,
        SFX, // sound
        SFX_2nd, // sound
        SFX_3rd, // sound
        SFX_4th, // voice
        SFX_loop //loop
    }


    private void Awake()
    {
        GameObject sectObj = GameObject.Find("loginNotDestroy");
        if (sectObj != null)
        {
            notDestory = sectObj.GetComponent<NotOnDestroy>();
        }
    }

    //AudioRenderer

    // Start is called before the first frame update
    void Start()
    {
        /// set child
        player_BGM = transform.Find("Audio Source_BGM").gameObject.GetComponent<AudioSource>();
        player_SFX = transform.Find("Audio Source_SFX").gameObject.GetComponent<AudioSource>();
        player_SFX_2nd = transform.Find("Audio Source_SFX_2nd").gameObject.GetComponent<AudioSource>();
        player_SFX_3rd = transform.Find("Audio Source_SFX_3rd").gameObject.GetComponent<AudioSource>();
        player_SFX_4th = transform.Find("Audio Source_SFX_4th").gameObject.GetComponent<AudioSource>();
        player_SFX_Loop = transform.Find("Audio Source_SFX_Loop").gameObject.GetComponent<AudioSource>();


        if (bgms != null && bgms.Count > 0) {
            StartBGM(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Play the clip with the mapped appName 
    /// pass empty appName or clipName , the channel will stop
    /// </summary>
    /// <param name="type"></param>
    /// <param name="gameAppName"></param>
    /// <param name="clipName"></param>
    public void PlayClip(SoundBoxAudioType type, string gameAppName = "", string clipName = "" )
    {
        if (gameAppName.Length > 0 && clipName.Length > 0)
        {
            /// get clip
            string folderPath = folderPathMap.GetValueOrDefault(gameAppName);
            string typeFolderName = "sfx";
            if (type == SoundBoxAudioType.BGM)
            {
                typeFolderName = "bgm";
            }

            var clipToPlay = LoadClip(gameAppName, folderPath + typeFolderName + "/" + clipName);
            if (clipToPlay == null )
            {
                //do fallback
                if (gameAppName != "COMMON")
                {
                    folderPath = folderPathMap.GetValueOrDefault("COMMON");
                    clipToPlay = LoadClip(gameAppName, folderPath + typeFolderName + "/" + clipName);
                }
                else if (string.IsNullOrEmpty(notDestory.GetGameSoundAssetBundleName()) && folderPath.Contains("InGame/"))
                {
                    // further fallback (bundled app base)
                    clipToPlay = LoadClip(gameAppName, (folderPath.Replace("InGame/", "")) + typeFolderName + "/" + clipName);
                }
            }
                
            /// load & play
            if (clipToPlay != null)
            {
                AudioSource player_SFX_t = null;
                switch (type)
                {
                    case SoundBoxAudioType.BGM:
                        if (player_BGM != null)
                        {
                            player_BGM.Stop();
                            // change clip
                            player_BGM.clip = clipToPlay;
                            if (player_BGM.clip.loadState == AudioDataLoadState.Unloaded)
                            {
                                while (player_BGM.clip.preloadAudioData)
                                {
                                    Debug.Log("BGM audio clip is " + player_BGM.clip.loadState);
                                }
                            }
                            player_BGM.Play();
                        }
                        break;
                    case SoundBoxAudioType.SFX: player_SFX_t = player_SFX; break;
                    case SoundBoxAudioType.SFX_2nd: player_SFX_t = player_SFX_2nd; break;
                    case SoundBoxAudioType.SFX_3rd: player_SFX_t = player_SFX_3rd; break;
                    case SoundBoxAudioType.SFX_4th: player_SFX_t = player_SFX_4th; break;
                    case SoundBoxAudioType.SFX_loop: player_SFX_t = player_SFX_Loop; break;
                }

                if (player_SFX_t != null)
                {
                    player_SFX_t.Stop();
                    player_SFX_t.clip = clipToPlay;

                    player_SFX_t.Play();
                }
            }
        }
        else {
            /// stop all

            if (type == SoundBoxAudioType.BGM)
            {
                if (player_BGM != null) player_BGM.Stop();
            }
            else {
                if (player_SFX != null) player_SFX.Stop();
                if (player_SFX_2nd != null) player_SFX_2nd.Stop();
                if (player_SFX_3rd != null) player_SFX_3rd.Stop();
                if (player_SFX_4th != null) player_SFX_4th.Stop();
                if (player_SFX_Loop != null) player_SFX_Loop.Stop();
            }

        }
    }

    public void StopSFX(SoundBoxAudioType type)
    {
        if (player_SFX != null && sfxs != null && sfxs.Count > 0)
        {
            switch(type)
            {
                case SoundBoxAudioType.SFX:
                    if (player_SFX != null) player_SFX.Stop();
                    break;
                case SoundBoxAudioType.SFX_2nd:
                    if (player_SFX_2nd != null) player_SFX_2nd.Stop();
                    break;
                case SoundBoxAudioType.SFX_3rd:
                    if (player_SFX_3rd != null) player_SFX_3rd.Stop();
                    break;
                case SoundBoxAudioType.SFX_4th:
                    if (player_SFX_4th != null) player_SFX_4th.Stop();
                    break;
                case SoundBoxAudioType.SFX_loop:
                    if (player_SFX_Loop != null) player_SFX_Loop.Stop();
                    break;
            }
        }
    }

    public void PlaySFX(int idx = 0) {
        if (player_SFX != null && sfxs != null && sfxs.Count > 0)
        {
            player_SFX.clip = sfxs[Mathf.Min(sfxs.Count , idx)];

            //if (player_BGM.clip.loadState == AudioDataLoadState.Unloaded)
            //{
            //    while (player_BGM.clip.preloadAudioData)
            //    {
            //        Debug.Log("BGM audio clip is " + player_BGM.clip.loadState);
            //    }
            //}

            player_SFX.Play();
        }
    }

    /// <summary>
    /// Play defaulted BGM from list
    /// </summary>
    /// <param name="idx"></param>
    public void StartBGM(int idx = 0) {
        if (player_BGM != null &&  bgms != null && idx < bgms.Count) {
            player_BGM.clip = bgms[idx];
            
            if (player_BGM.clip.loadState == AudioDataLoadState.Unloaded)
            {
                while (player_BGM.clip.preloadAudioData) {
                    Debug.Log("BGM audio clip is " + player_BGM.clip.loadState);
                }
            }

            player_BGM.Play();
        }
    }

    /// <summary>
    /// Audio file load and store 
    /// </summary>

    Dictionary<string, Dictionary<string, AudioClip>> loadedClips = new();
    AudioClip LoadClip(string gameAppName, string path)
    {
        AudioClip audioClip = null;

        /// TODO : handle sound bundle
        if (notDestory != null &&  !string.IsNullOrEmpty(notDestory.GetGameSoundAssetBundleName()) && CommonConfig.IsAssetBundled())
        {
            var paths = path.Split("/");
            audioClip = BundleResourcesManager.Instance().GetResourceSound("sound", "game", paths[paths.Length-1]);

            // find clip
            if (audioClip == null)
                Debug.Log(" file not found , Please check the file path => " + paths[paths.Length - 1]);
            else
                Debug.Log("target audio clip is already loaded");
        }
        else {
            // path  => "Audio/audioClip01"
            if (path.Length > 0)
            {
                string[] pSplit = path.Split("/");
                string clipName = pSplit[pSplit.Length - 1];

                /// find the loaded clip
                Dictionary<string, AudioClip> appClips = null;
                if (loadedClips.ContainsKey(gameAppName))
                {
                    appClips = loadedClips.GetValueOrDefault(gameAppName);
                }
                else
                {
                    /// TODO : no existing key , need add new
                    appClips = new();
                }

                /// find loaded clip in storage

                if (appClips.Values.Count > 0)
                {
                    if (appClips.ContainsKey(clipName))
                    {
                        audioClip = appClips.GetValueOrDefault(clipName);
                    }
                }

                // find clip
                if (audioClip == null)
                {
                    audioClip = Resources.Load<AudioClip>(path);
                    if (audioClip != null)
                    {
                        Debug.Log(" new clip Loaded -> " + path);
                        // do save
                        appClips[clipName] = audioClip;
                        Debug.Log(" new clip [" + clipName + "] Loaded ");

                        // final update
                        loadedClips[gameAppName] = appClips;
                    }
                    else
                    {
                        Debug.Log(" file not found , Please check the file path => " + path);
                    }
                }
                else
                {
                    Debug.Log("target audio clip is already loaded");
                }
            }
            else
            {
                Debug.Log(" file path is empty , Please check");
            }
        }

        return audioClip;   
    }


    // =-=-=-=-=-=-=-=-= For Setting =-=-=-=-=-==-=-

    public void ChangeVolume(SoundBoxAudioType type, float n_level) {
        switch (type)
        {
            case SoundBoxAudioType.BGM:
                player_BGM = transform.Find("Audio Source_BGM").gameObject.GetComponent<AudioSource>();
                if (player_BGM != null) player_BGM.volume = n_level;
                break;
            case SoundBoxAudioType.SFX:
            case SoundBoxAudioType.SFX_2nd:
            case SoundBoxAudioType.SFX_3rd:
            case SoundBoxAudioType.SFX_4th:
            case SoundBoxAudioType.SFX_loop:
                player_SFX = transform.Find("Audio Source_SFX").gameObject.GetComponent<AudioSource>();
                if (player_SFX != null) player_SFX.volume = n_level;
                player_SFX_2nd = transform.Find("Audio Source_SFX_2nd").gameObject.GetComponent<AudioSource>();
                if (player_SFX_2nd != null) player_SFX_2nd.volume = n_level;
                player_SFX_3rd = transform.Find("Audio Source_SFX_3rd").gameObject.GetComponent<AudioSource>();
                if (player_SFX_3rd != null) player_SFX_3rd.volume = n_level;
                player_SFX_4th = transform.Find("Audio Source_SFX_4th").gameObject.GetComponent<AudioSource>();
                if (player_SFX_4th != null) player_SFX_4th.volume = n_level;
                player_SFX_Loop = transform.Find("Audio Source_SFX_Loop").gameObject.GetComponent<AudioSource>();
                if (player_SFX_Loop != null) player_SFX_Loop.volume = n_level;

                break;
        }
    }

    public void SetFocusSubPageBGM(bool isOnPlaying)
    {
        float volumn_BGM = 0.2f;
        if (!isOnPlaying){
            volumn_BGM = CommonConfig.GetCurBGMLv;
        }

        if (CommonConfig.GetCurMuteState == 1){
            volumn_BGM = 0;
        }

        ChangeVolume(SoundBoxController.SoundBoxAudioType.BGM, volumn_BGM);
    }

}
