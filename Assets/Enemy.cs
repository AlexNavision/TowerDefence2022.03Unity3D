using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    private float health;
    private int nextPointL = 0;
    private int Id;
    private Action onDestroy;
    private Action onHeal;
    private Vector2 nextPoint; //Не хочу брать из массива, так быстрее.

    public float Speed { set { speed += value; } }
    public int ID => Id;

    private enum EnemyClass { easy, fast, friend }; //Это должно быть не здесь

    void Start()
    {
        nextPoint = MenuManager.RoadPoint[nextPointL];
        //print(nextPoint + " " + transform.position + "   " + gameObject.name);
        //StartCoroutine(Move());
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, nextPoint, speed * Time.deltaTime);
    }

    public void Create(int Id,Action<int> onDestroy)
    {
        this.Id = Id;
        System.Random random = new System.Random();
        switch (Enum.GetValues(typeof(EnemyClass)).GetValue(random.Next(3))) //Это должно быть не здесь
        {
            case EnemyClass.friend: gameObject.tag = "friend"; MenuManager.EnemyDeadCall(Id); GetComponent<Image>().color = Color.white; break;
            case EnemyClass.fast: gameObject.tag = "enemy";GetComponent<Image>().color = Color.red; break;
            case EnemyClass.easy: gameObject.tag = "enemy"; GetComponent<Image>().color = Color.red; break;
        }
        speed = random.Next(10, 20); //тут можно скорость увеличивать в зависимости от уровня (волны);
        health = 30 - speed;
        int i = Mathf.RoundToInt(health / 2); //Делегат не работает с переменными из скрипта. В данном случае это решается с помощью локальной переменной i.
        onHeal = () => onDestroy(i);
        i = Mathf.RoundToInt(health * 2);
        this.onDestroy = () => onDestroy(i);
        //healthTEST = Mathf.RoundToInt(health / 2).ToString() + " health = " + health.ToString();  //debug
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Road":
                nextPointL += 1;
                nextPoint = MenuManager.RoadPoint[nextPointL];
            break;
            case "Finish":
                if (gameObject.tag == "friend")
                { Destroy(); }//GetMoney
                else
                    MenuManager.EndGame.Invoke(false);
            break;
        }
    }
    //private string healthTEST; //debug
    public void HIT(float damage)
    {
        if (health <= 0) return;
        health -= damage;
        if (health <= 0)
        {
            onHeal();
            //print(healthTEST);//debug
            MenuManager.EnemyDeadCall(Id);
            gameObject.tag = "friend";
            GetComponent<Image>().color = Color.white;
        }
    }
    private void Destroy()
    {
        onDestroy();
        Destroy(gameObject);
        //Добавить анимацию или эффекты
    }
}