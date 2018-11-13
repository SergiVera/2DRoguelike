using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

	// Use this for initialization
	protected override void Start () {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        base.Start();
	}

    //To show how the food vary throughout the levels, and so on
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;

        //To store the position in the game, as we move
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
    // Update is called once per frame
    void Update () {
        if (!GameManager.instance.playersTurn) return;
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Everytime the player moves, he will lose 1 point of food
        food--;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnterd2D(Collider2D other){
        //In folder Prefabs we declared different GameObjects. One of this was the Exit, which we will call him with tag
        if(other.tag == "Exit"){
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food"){
            food = food + pointsPerFood;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda"){
            food = food + pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    private void CheckIfGameOver(){
        if (food <= 0)
            GameManager.instance.GameOver();
    }

    protected override void OnCantMove<T>(T component)
    {
        //Casting to a Wall (as)
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        //In Animator we declared two Trigger paramteres, called playerHit and playerChop
        animator.SetTrigger("playerChop");
    }

    private void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss){
        //In Animator we declared two Trigger paramteres, called playerHit and playerChop
        animator.SetTrigger("playerHit");
        food = food - loss;
        CheckIfGameOver();
    }
}
