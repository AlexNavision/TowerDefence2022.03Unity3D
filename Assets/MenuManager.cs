using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance { get; private set; }

    public static Vector2[] RoadPoint;
    public static Sprite[] Towers;
    [SerializeField] private Text Money;
    [SerializeField] private GameObject Enemy;
    private int money = 300;
    public static Action<bool> EndGame;
    public delegate void EnemyDead(int i);
    public static event EnemyDead OnDead;
    public static bool MoneyCheck(int money)
    {
        if (instance.money < money)
            return false;
        else
        {
            instance.UpdateMoney(-1 * money);
            return true;
        }
    }
    [SerializeField] private Image MoneyFill;
    private void Awake()
    {
        instance = this;
        EndGame = End;
        UpdateMoney(0);

        Towers = new Sprite[Resources.LoadAll<Sprite>("tower").Length];
        Towers = Resources.LoadAll<Sprite>("tower");

        List<GameObject> RoadPointTmp = new List<GameObject>(GameObject.FindGameObjectsWithTag("Road"));
        RoadPointTmp.OrderBy(gameObj => gameObj.name).ToList();
        RoadPointTmp.Add(GameObject.FindGameObjectWithTag("Finish"));
        RoadPoint = new Vector2[RoadPointTmp.Count];
        int count = 0;
        foreach (GameObject RP in RoadPointTmp)
        {
            RoadPoint[count] = RP.transform.position;
            count++;
        }
    }
    private void Start()
    {
        NextLvl();
    }
    private void UpdateMoney(int moneyIncome)
    {
        //if (Mathf.Abs(moneyIncome) > 1) //debug
        //print(moneyIncome);
        instance.money += moneyIncome;
        if (instance.money >= 1000)
            End(true);
        instance.Money.text = instance.money.ToString();
        instance.MoneyFill.fillAmount = instance.money / 1000f;
    }


    public static void EnemyDeadCall(int id)
    {
        OnDead?.Invoke(id);
        instance.EnemyInField--;
        instance.EnemyCountText.text = instance.EnemyInField.ToString();
        if (instance.Enemycount <= 0 && instance.EnemyInField <= 0)
        {
            instance.NextLvl();
        }
    }

    //End Game
    private void End(bool win)  //Не обращайте внимание, нужно было быстро сделать окно перезапуска :) В этой функции все сплошной костыль
    {
        Level = 3; //Единственный костыль в проекте
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("enemy"))
            Destroy(t);
        StopAllCoroutines();
        GameObject restart = Instantiate(Resources.Load<GameObject>("RestartWindow"),FindObjectsOfType<Canvas>()[1].transform);
        restart.GetComponentInChildren<Button>().onClick.AddListener(StartAgain);
        if (win)
            restart.GetComponentInChildren<Text>().text = "Победа!";
        else
            restart.GetComponentInChildren<Text>().text = "Проиграли!";
    }

    private int Level = 0;
    [SerializeField] private int EnemyLVLUP = 25;
    private int Enemycount = 0,EnemyInField = 0;
    [SerializeField] Text EnemyCountAtLeastText, EnemyCountText, LvlText;
    private void ShowEnemy()
    {
        EnemyInField++;
        Enemycount--;
        EnemyCountAtLeastText.text = Enemycount.ToString();
        EnemyCountText.text = (int.Parse(EnemyCountText.text) + 1).ToString();
    }
    private void NextLvl()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("enemy")) //Тут можно заменить
            t.GetComponent<Enemy>().Speed = 100;
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("friend")) //Тут можно заменить
            t.GetComponent<Enemy>().Speed = 100;
        Level++;
        if (Level == 3)
        {
            LvlText.text = "Победа";
            End(true);
            return;
        }
        Enemycount = EnemyLVLUP * Level;
        EnemyCountAtLeastText.text = Enemycount.ToString();
        EnemyCountText.text = "0";
        LvlText.text = "Lvl" + Level.ToString();
        StartCoroutine(Spawn());
    }
    private IEnumerator Spawn()
    {
        //print(Enemycount);
        while (Enemycount > 0)
        {
            if (Input.touchCount > 0 || Input.anyKey)
            {
                ShowEnemy();
                Instantiate(Enemy, new Vector2(-125f, -125f), Quaternion.identity, transform).GetComponent<Enemy>().Create(Enemycount, UpdateMoney);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }



    public void StartAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}