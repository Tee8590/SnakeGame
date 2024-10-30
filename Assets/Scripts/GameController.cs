using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public float moveSpeed =1;
    public BodyPart bodyPrefab = null;
    public Sprite tailSprite = null;
    public Sprite bodySprite = null;

    const float width = 3.7f;
    const float hight = 7f;
        
    public GameObject rockPrefab = null;
    public SnakeHead snakeHead = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;
    
    public GameObject spikePrefab = null;

    public bool alive = true;  
    public bool waithingToPlay = true;

    public List<Egg> eggs = new List<Egg>();
    public List<Spike> spikes = new List<Spike>();

    int level = 0;
    int noOfEggsForNextLevel = 0;
    int noOfSpikesForNextLevel = 1;

    public int score = 0;
    public int hiScore = 0;

    public TextMeshProUGUI scoreText = null;
    public TextMeshProUGUI HiScoreText= null;
    public TextMeshProUGUI GameOverText = null;
    public TextMeshProUGUI TaptoplayText = null;
    void Start()
    {
        instance = this;
        Debug.Log("Starting Game");
        CreateWalls();
        
        CreateEgg();
        alive = false;
    }
    void Update()
    {
       if(waithingToPlay)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    StartGamePlay();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                StartGamePlay();
            }
        }
    }

    void CreateWalls()
    {
        float z = -1;
        Vector3 start = new Vector3(-width, -hight, z);
        Vector3 end = new Vector3(-width, +hight, z);
        CreateWall(start, end);

        start = new Vector3(+width, -hight, z);
        end = new Vector3(+width, +hight, z);
        CreateWall(start, end);
        
        start = new Vector3(-width, -hight, z);
        end = new Vector3(+width, -hight, z);
        CreateWall(start, end);

        start = new Vector3(-width, +hight,z);
        end = new Vector3(+width, +hight,z);
        CreateWall(start, end);
    }
    void CreateWall(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        int noOfRocks = (int)(distance * 3f);//33
        Vector3 delta = (end - start) / noOfRocks;//11.5

        Vector3 position = start;
        for (int rock = 0; rock <= noOfRocks; rock++)
        {
            float rotation = UnityEngine.Random.Range(0, 360);
            float scale = UnityEngine.Random.Range(1.5f, 2f);
            CreateRock(position, scale, rotation);
            position += delta; 

        }
    }
    void CreateRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }
    void CreateEgg( bool goldenEgg = false)
    {
        Vector3 position;
        position.x = -width + UnityEngine.Random.Range(1f,(width * 2) - 2f);
        position.y = -hight + UnityEngine.Random.Range(1f, (hight * 2) - 2f);
        position.z = -1;

        Egg egg = null;

        if (goldenEgg)
            egg = Instantiate(goldEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        else
            egg = Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();

        eggs.Add(egg);
    }
    void CreateSpike()
    {
        Vector3 position;

        position.x = -width + UnityEngine.Random.Range(1f, (width * 2) - 2f);
        position.y = -hight + UnityEngine.Random.Range(1f, (hight * 2) - 2f);
        position.z = -1;

        Spike spike = null;

        spike = Instantiate(spikePrefab, position, Quaternion.identity).GetComponent<Spike>();
        spikes.Add(spike);
    }
    public void GameOver()
    {
        alive = false;
        waithingToPlay = true;
        GameOverText.gameObject.SetActive(true);
        TaptoplayText.gameObject.SetActive(true);
        KIllOldSpike();


    }
    void StartGamePlay()
    {
        GameOverText.gameObject.SetActive(false);
        TaptoplayText.gameObject.SetActive(false);
        score = 0;
        level = 0;
        scoreText.text = "Score :" + score;
        HiScoreText.text = "HiScore :" + hiScore;

        waithingToPlay = false;
        alive = true;
        KillOldEggs();
       
        LevelUp();
    }
    void LevelUp()
    {
        level++;
        noOfEggsForNextLevel = 4 + (level * 2);
        noOfSpikesForNextLevel++;
        moveSpeed = 1f + (level/4f);
        if(moveSpeed>6) moveSpeed = 6;
        snakeHead.ResetSnake();
        CreateEgg();
        CreateSpike();

    }
    void KillOldEggs()
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }
    void KIllOldSpike()
    {

       foreach(Spike spike in spikes)
        {
            Destroy(spike.gameObject);
        }
        spikes.Clear();

    }

    public void EggEaten(Egg egg)
    {
        score++;
        noOfEggsForNextLevel--;
        if (noOfEggsForNextLevel == 0)
        {
            LevelUp();
            score += 10;    
        }       
        else if (noOfEggsForNextLevel == 1)
            CreateEgg(true);
        else
            CreateEgg(false);

        if (score > hiScore)
        {
            hiScore = score;
            HiScoreText.text = "HiScore :" + hiScore;
        }
        scoreText.text = "Score :" + score;
        
        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }

}
