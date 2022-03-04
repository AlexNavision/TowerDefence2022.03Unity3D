using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GifPlayer_Demo : MonoBehaviour
{
    private void Start()
    {
        frames = new Sprite[SpriteCount - 1];
        frames = Resources.LoadAll<Sprite>("enemy");
        image = GetComponent<Image>();
        StartCoroutine(UpdateIEnumerator());
    }
    private Sprite[] frames;
    [SerializeField] private int framespersecond = 40;
    [Tooltip ("Количество спрайтов в папке")]
    [SerializeField] private int SpriteCount = 24;
    private Image image;
    IEnumerator UpdateIEnumerator()
    {
        while (true)
        {
            int index = (int)(Time.time * framespersecond) % frames.Length;
            image.sprite = frames[index];
            yield return new WaitForEndOfFrame();
        }
    }
}
