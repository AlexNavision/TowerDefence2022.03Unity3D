using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    private int Lvl = 0;
    public int LVL => Lvl;
    [SerializeField,Tooltip("Уровень 0 стоимость")] private int MoneyLvl0 = 100;
    [SerializeField] private float MoneylvlUp = 1.2f; //Множитель улучшения
    [SerializeField] private float Damage = 2;
    [SerializeField] private float DamageLvlUp = 1.5f;
    [SerializeField] private float AttackCooldown = 0.5f;
    //[SerializeField] private TowerType Towertype = TowerType.Assasin;
    [SerializeField, Range(100f, 600f)] private float FireRadius = 350f;
    [SerializeField, Range(50f, 100f)] private float FireRadiusLvlUp = 75f;
    [SerializeField] private GameObject FireObj; //Поместить в менеджер, но можно и держать в башне, так у башни можно менять тип снарядов
    [SerializeField] private RectTransform CircleColliderView;
    private CircleCollider2D circleCollider; //Не хочу использовать GetComponent
    private float AttackCooldownTime = 0.5f;
    private float AttackCooldownTimeLvlUp = 0.1f;
    public float Fireradius => FireRadius;
    public SortedDictionary<int, Enemy> enemies = new SortedDictionary<int, Enemy>();

    //public enum TowerType { Assasin,Gets };
    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        CircleColliderView = GetComponentsInChildren<Image>()[1].rectTransform;
        CircleColliderView.gameObject.SetActive(false);
    }
    public void Create()
    {
        GetComponent<Image>().sprite = MenuManager.Towers[Lvl];
        MenuManager.OnDead += OnDead;
        Lvl++;
        GetComponentInChildren<Text>().text = "LVL" + Lvl;
        circleCollider.radius = FireRadius;
        DrawCircle(true);
        //Можно перенести создание новых башней отдельно
        StartCoroutine(Fire());
        //GetComponent<Trigger>
        //if(MenuManager.money)
    }
    public int LvlUpCost()
    {
        return Mathf.RoundToInt(MoneyLvl0 * MoneylvlUp * Lvl) == 0?MoneyLvl0 : Mathf.RoundToInt(MoneyLvl0 * MoneylvlUp * Lvl);
    }
    public bool readyUpgrade()
    {
        if (Lvl == MenuManager.Towers.Length)
            return false;
        else return true;
    }
    public void Upgrade()
    {
        if (Lvl == 0)
        {
            Create();
            return;
        }
        GetComponent<Image>().sprite = MenuManager.Towers[Lvl++];
        GetComponentInChildren<Text>().text = "LVL" + Lvl;
        Damage += DamageLvlUp * Lvl;
        AttackCooldownTime -= AttackCooldownTimeLvlUp;
        FireRadius += FireRadiusLvlUp;
        circleCollider.radius = FireRadius;
        DrawCircle(true);
        //Можно перенести создание новых башней отдельно
    }
    private IEnumerator Fire()
    {
        while (true)
        {
            if (Target == null)
            {
                //Враг в поле зрения
                Target = GetNearestEnemy();
            }
            else if (Vector2.Distance(Target.transform.localPosition, transform.localPosition) > FireRadius)
                    Target = null;


            if (AttackCooldownTime <= 0 && Target != null)  //находим ближайшего врага
            {
                if (MenuManager.MoneyCheck(1))
                {
                    Instantiate(FireObj, transform.parent).GetComponent<FireObject>().Create(Target, Damage, this);
                }
                //Target. стрельба


                AttackCooldownTime = AttackCooldown; //Если сделать +=, то будем накапливать снаряды
                yield return new WaitForEndOfFrame();
            }
            
            AttackCooldownTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    public Enemy GetNearestEnemy() //Придумал способ нахождения самого ближнего к концу карты, могу рассказать
    {
        //Enemy enemy = null;
        if (enemies.Count > 0)
        {
            if (enemies.Values.Last() != null)
                return enemies.Values.Last();
            else
            {
                while (enemies.Count > 0 && enemies.Values.Last() == null)
                {
                    enemies.Remove(enemies.Keys.Last());
                }
                if (enemies.Count > 0)
                    return enemies.Values.Last();
            }
        }
        return null;
        /*
        Enemy enemy = null;
        float distanceMin = float.PositiveInfinity;
        foreach(Enemy enemy1 in enemies)
        {
            float distanceTemp = Vector2.Distance(transform.position, enemy1.transform.position);
            if (distanceTemp < distanceMin)
            {
                distanceMin = distanceTemp;
                enemy = enemy1;
            }
        }
        return enemy;
        */ 
    }

    public void CustomClick()
    {
        DrawCircle(true);
        UpgradeWindow.UpdateWindow(this);
    }
    public void DrawCircle(bool view) //круг при нажатии
    {
        if (view)
        {
            CircleColliderView.sizeDelta = new Vector2(FireRadius * 2, FireRadius * 2);
            CircleColliderView.gameObject.SetActive(true);
        }
        else
        {
            CircleColliderView.gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //print("Вошел в триггер");
        if (collision.tag == "enemy")
        {
            Enemy enemyTemp = collision.GetComponent<Enemy>();
            enemies.Add(enemyTemp.ID, enemyTemp);
        }
    }
    Enemy Target = null;
    public void OnDead(int id)
    {
        enemies.Remove(id);
        if (Target != null && Target.ID == id)
            Target = null;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //print("Вышел из триггера или был убит");
        if (collision.tag == "enemy" && enemies.Count > 0)
        {
            //Если врага убили, то Enemy пропадет из списка, но элемент списка будет жить. Тут мы можем подчистить всех убитых врагов.
            int id = collision.GetComponent<Enemy>().ID;
            enemies.Remove(id);
            if (Target != null && Target.ID == id)
                Target = null;
        }
    }

}
