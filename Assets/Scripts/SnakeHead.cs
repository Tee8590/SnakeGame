using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : BodyPart
{
    private BodyPart tail = null;
    Vector2 movement;
    const float TIMETOADDBODYPART = 0.1f;
    float addTimer = TIMETOADDBODYPART;

    public List<BodyPart> parts = new List<BodyPart>();
    public int partsToAdd = 0;

    public AudioSource[] gulpSounds = new AudioSource[3]; 
    public AudioSource dieSound = null;
    void Start()
    {
        
        SwipeControl.OnSwipe += SwipeDetection;
    }
    override public void Update()
    {
        if (!GameController.instance.alive) return;
        base.Update();
        SetMovement(movement * Time.deltaTime);
        UpdatePosition();
        UpdateDirection();

        if(partsToAdd>0)
        {
            addTimer -= Time.deltaTime;
            if(addTimer<=0)
            {
                addTimer = TIMETOADDBODYPART;
                AddBodyPart();
                partsToAdd--;
            }
        }
    }
    void AddBodyPart()
    {
        if(tail == null)
        {
            Vector3 newPosition = transform.position;
            newPosition.z += 0.01f;
            
            BodyPart newPart = Instantiate(GameController.instance.bodyPrefab, newPosition, Quaternion.identity);
            newPart.following = this;
            tail = newPart;
            newPart.TurnIntoTail();

            parts.Add(newPart);
        }
        else
        {
            Vector3 newPosition = tail.transform.position;
            newPosition.z += 0.01f;

            BodyPart newPart = Instantiate(GameController.instance.bodyPrefab, newPosition, tail.transform.rotation);
            newPart.following = tail;
            newPart.TurnIntoTail();
            tail.TurnIntoBody();
            tail = newPart;

            parts.Add(newPart);
        }
    }
    void SwipeDetection(SwipeControl.SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeControl.SwipeDirection.Left:
                MoveLeft();
                break;
            case SwipeControl.SwipeDirection.Right:
                MoveRight();
                break;
            case SwipeControl.SwipeDirection.Up:
                MoveUp();
                break;
            case SwipeControl.SwipeDirection.Down:
                MoveDown();
                break;

        }   
    }
    void MoveUp()
    {
      movement =  Vector2.up * GameController.instance.moveSpeed;
    }
    void MoveDown()
    {
        movement = Vector2.down * GameController.instance.moveSpeed;
    }
    void MoveLeft()
    {
    movement = Vector2.left * GameController.instance.moveSpeed;
    }
    void MoveRight()
    {
    movement = Vector2.right * GameController.instance.moveSpeed;

    }

    internal void ResetSnake()
    {
        foreach(BodyPart part in parts)
        {
            Destroy(part.gameObject);
        }
        parts.Clear();

        tail = null;
        MoveUp();

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        gameObject.transform.position = new Vector3(0, 0, -0.01f);

        ResetMemory();
        partsToAdd = 5;
        addTimer = TIMETOADDBODYPART;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Egg egg = collision.GetComponent<Egg>();
        if (egg)
        {
            EatEgg(egg);
            int rand = UnityEngine.Random.Range(0, 3);
            gulpSounds[rand].Play();
        }
        else
        {
            Debug.Log("obstacle triggered ");
            GameController.instance.GameOver();
            dieSound.Play();
        }
        
    }
    public void EatEgg(Egg egg)
    {
        partsToAdd = 5;
        addTimer = 0;
        GameController.instance.EggEaten(egg);
    }
}
