using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject billPrefab;
    public Transform spawnPoint;

    [SerializeField] private int money;
    [SerializeField] private int lvl;
    public static int rate = 1;
    public Text lvlText;
    public Text moneyText;
    public Text rateText;

    private float upperLimit;
    private float lowerLimit;

    void Start()
    {
        money = PlayerPrefs.GetInt("money", 0);
        rate = PlayerPrefs.GetInt("rate", 1);
        lvl = PlayerPrefs.GetInt("lvl", 1);

        Camera camera = Camera.main;
        if (camera.orthographic)
        {
            upperLimit = camera.orthographicSize;
            lowerLimit = -camera.orthographicSize;
        }
        else
        {
            Debug.LogError("Камера не является ортографической!");
        }
    }

    public void ButtonClick()
    {
        money += rate;
        PlayerPrefs.SetInt("money", money);
        LaunchBill();
    }

    public void UpdateClick()
    {
        if (money >= 50)
        {
            money -= 50;
            rate += 1;
            lvl++;
            PlayerPrefs.SetInt("money", money);
            PlayerPrefs.SetInt("rate", rate);
            PlayerPrefs.SetInt("lvl", lvl);
        }
    }

    void Update()
    {
        moneyText.text = money.ToString() + "$";
        lvlText.text = "LVL " + lvl.ToString();
        rateText.text = "+" + rate.ToString();
    }

    public void LaunchBill()
    {
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.z = 0;
        GameObject bill = Instantiate(billPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveBill(bill));
    }

    private IEnumerator MoveBill(GameObject bill)
    {
        Debug.Log("Билет создан в позиции: " + bill.transform.position);

        float speed = 5f;
        float frequency = 0.1f;
        float amplitude = Random.Range(0.4f, -0.4f);
        float startTime = Time.time;

        while (bill.transform.position.y < upperLimit / 2.75)
        {
            bill.transform.Rotate(0, 1, 0);
            float elapsedTime = Time.time - startTime;
            float xOffset = Mathf.Sin(elapsedTime * frequency) * amplitude;

            xOffset = Mathf.Clamp(xOffset, -2f, 2f);

            bill.transform.position += new Vector3(xOffset, speed * Time.deltaTime, 0);
            yield return null;
        }

        Debug.Log("Движение вверх завершено, текущая позиция: " + bill.transform.position);

        while (bill.transform.position.y > lowerLimit)
        {
            bill.transform.Rotate(0, 1, 0);
            bill.transform.Translate(Vector2.down * speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Билет уничтожен, финальная позиция: " + bill.transform.position);

        Destroy(bill);
    }
}
