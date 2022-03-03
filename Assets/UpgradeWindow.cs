using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
    [SerializeField] private Image imageNew, imageOld;
    [SerializeField] private Text Money;
    private static UpgradeWindow window;
    private void Awake()
    {
        window = this;
        window.gameObject.SetActive(false);
    }
    private static Tower SelectTower = null;
    public static void UpdateWindow(Tower tower = null)
    {
        if (SelectTower == null)
        {
            if (tower == null)
            {
                Debug.LogError("Ошибка 404 (Tower)");
                return;
            }
            SelectTower = tower;
        }
        else if (SelectTower == tower)
        {
            SelectTower.DrawCircle(false);
            SelectTower = null;
            window.gameObject.SetActive(false);
            return;
        }
        else
        {
            SelectTower.DrawCircle(false);
            SelectTower = tower;
        }
        window.gameObject.SetActive(true);
        window.UpdateWindowUI();
    }
    private void UpdateWindowUI()
    {
        if (!SelectTower.readyUpgrade())
        {
            imageOld.gameObject.SetActive(false);
            imageNew.sprite = MenuManager.Towers[SelectTower.LVL - 1];
            Money.text = "Max Level";
            return;
        }
        else if (SelectTower.LVL > 0)
        {
            imageOld.gameObject.SetActive(true);
            imageOld.sprite = MenuManager.Towers[SelectTower.LVL - 1];
        }
        else imageOld.gameObject.SetActive(false);
        imageNew.sprite = MenuManager.Towers[SelectTower.LVL];
        Money.text = SelectTower.LvlUpCost().ToString() + "\n---->";
    }
    public void UpgradeTower()
    {
        if (SelectTower == null) //ошибка
            return;
        if (!SelectTower.readyUpgrade())
            return;
        if (!MenuManager.MoneyCheck(SelectTower.LvlUpCost()))
            return;
        SelectTower.Upgrade();
        UpdateWindowUI();
    }






    public void CloseMenu()
    {
        SelectTower = null;
        gameObject.SetActive(false);
    }

}
