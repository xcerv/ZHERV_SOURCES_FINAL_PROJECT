using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Taken from Excercise 2
public class GameManager : MonoBehaviour
{
    // Game Flags
    private static bool sGameStarted = false;
    private static bool mGameLost = false;
    private static bool TimerStarted = false;

    public GameObject PlayerEntity;

    // Level list
    [SerializeField] public List<Level> ActiveLevelList;
    [SerializeField] private List<Level> InactiveLevelList = new List<Level>();
    public Level startRoom;

    // Level Control
    [SerializeField] public List<PortalDuo> PortalQueue;
    private int PortalQueueIndex = 0;

    [SerializeField] public LinkedList<Level> LevelQueue = new LinkedList<Level>();

    // Music
    public AudioClip music_clip;
    private sound_controller_generic soundControllerGlobal; 

    // UI
    public TMP_Text mainUI_Time;
    public TMP_Text mainUI_LostTuto;
    public TMP_Text mainUI_Score;
    public TMP_Text mainUI_ScoreTxt;
    private int score = 0;

    // Timer Ents
    private float timeLeftMax = 40f;
    private float timeLeft;
    
    public float Timer
    {
        get
        {
            return timeLeft;
        }

        set
        {
            timeLeft = value;

            mainUI_Time.text = Mathf.Floor(timeLeft).ToString();
            if(timeLeft <= 0f)
            {
                LoseGame();
            }
        }

    }

    public void SelectNewRoom()
    {
        
        if(ActiveLevelList.Count <=0)
        {
            var tmp = ActiveLevelList;
            ActiveLevelList = InactiveLevelList;
            InactiveLevelList = tmp;
            foreach(var level in LevelQueue)
            {
                ActiveLevelList.Remove(level);
                InactiveLevelList.Add(level);
            }
        }
        

        var newRoom = ActiveLevelList[Random.Range(0, ActiveLevelList.Count)];
        ActiveLevelList.Remove(newRoom);
        InactiveLevelList.Add(newRoom);

        newRoom.DestroyEntities();
        newRoom.SpawnEntities();

        var freePortal = PortalQueue[PortalQueueIndex];
        freePortal.portal_crossed = false;
        PortalQueueIndex = (PortalQueueIndex+1) % PortalQueue.Count; 
    
        newRoom.SpawnPortals(freePortal.portal_ent_exit, null);
        LevelQueue.Last.Value.SpawnPortals(null, freePortal.portal_ent_entry);

        LevelQueue.AddLast(newRoom);

        if(LevelQueue.Count >= 4){
            var oldRoom = LevelQueue.First.Value; 
            
            LevelQueue.RemoveFirst();
        }
        
    }

    void Awake()
    {
        // Setup the game scene.
        soundControllerGlobal = GetComponentInParent<sound_controller_generic>();
        Timer = timeLeftMax;
        SetupGame();
    }

    void Update()
    {
        if (!sGameStarted)
        { StartGame(); }

        if (Input.GetButtonDown("Cancel"))
        { ResetGame(); }

        if (sGameStarted && TimerStarted && !mGameLost)
        {
            Timer -= Time.deltaTime; 
        } 

        if (mGameLost && Input.GetButtonDown("Jump")){
            ResetGame();
        }
        
    }

    public void SetupGame()
    {        
        mainUI_LostTuto.enabled = false;
        mainUI_Score.enabled = false;
        mainUI_ScoreTxt.enabled = false;

        if (sGameStarted)
        { // Setup already started game -> Retry.
            LevelQueue.AddFirst(startRoom);
            
            for(int i = 0; i<2; i++)
            {
                SelectNewRoom();
            }
        }
        else
        { // Setup a new game -> Wait for start.
            // Don't start spawning until we start.

            LevelQueue.AddFirst(startRoom);
            
            for(int i = 0; i<2; i++)
            {
                SelectNewRoom();
            }
        }
        
        // Set the state.
        mGameLost = false;
    }

    public void StartTimer()
    {
        if(!TimerStarted && music_clip){
            playMusic();
        }
        
        TimerStarted = true;
    }

    public void increaseTimer(int value)
    {
        Timer += value;
    }

    public void StartGame()
    {
        // Reload the scene as started.
        sGameStarted = true; 

        // Set correct values
        mainUI_LostTuto.enabled = false;
        mainUI_Score.enabled = false;
        mainUI_ScoreTxt.enabled = false;
        timeLeft = timeLeftMax;

        ResetGame();
    }

    public void ResetGame()
    {
        // Reload the active scene, triggering reset...
        TimerStarted = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseGame()
    {
        if(!mGameLost){
            if(PlayerEntity)
            {
                stopMusic();
                mainUI_LostTuto.enabled = true;
                mainUI_Score.enabled = true;
                mainUI_ScoreTxt.enabled = true;
                PlayerEntity.GetComponent<entity_player>().Death(); // Die
            }
        }

        mGameLost = true;
    }

    public void increaseScore(int amount)
    {
        score += amount; 
        mainUI_Score.text = score.ToString();
    }

    //private float fade_time = 2f;
    public void playMusic()
    {
        soundControllerGlobal.PlaySoundLoop(music_clip, true);
    }

    public void stopMusic()
    {
        soundControllerGlobal.StopLoopSound(true);
    }

}
