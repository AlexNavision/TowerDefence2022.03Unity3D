using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObject : MonoBehaviour
{
    private Tower tower; //Для анимации возвращения в башню
    private Enemy enemy;
    private float damage;
    [SerializeField,Range(10f,100f)] private float speed = 100f;

    public Tower TowerParent => tower;

    public void Create(Enemy target, float hitdamage, Tower parentTower)
    {
        enemy = target;
        damage = hitdamage;
        tower = parentTower;
        transform.position = tower.transform.position;
        StartCoroutine(Move());
    }
    private IEnumerator Move()
    {
        while (enemy != null && GetTargetDistance() > 20f)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemy.transform.position, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        if (enemy != null)
            enemy.HIT(damage);
        Destroy(gameObject);
    }
    private float GetTargetDistance()
    {
        if (enemy == null)
        {
            /*  //Движение к след. врагу
            enemy = tower.GetNearestEnemy();
            if (enemy == null)
            {
                DestroyCustom();
                return 0f;
            }
            */
            return 0;
        }
        return Mathf.Abs(Vector2.Distance(transform.localPosition, enemy.transform.localPosition));
    }
}
