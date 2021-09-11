using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    // Fungsi [Range (min, max)] ialah menjaga value agar tetap berada di antara min dan max-nya

    [Range(0f, 1f)]
    public float AutoCollectPercentage = 0.1f;

    public ResourceConfig[] ResourcesConfigs;
    public Sprite[] ResourcesSprites;
    public Transform ResourcesParent;
    public Transform CoinIcon;
    public ResourceController ResourcePrefab;
    public TapText TapTextPrefab;
    public Text GoldInfo;
    public Text AutoCollectInfo;




    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;


    [SerializeField]
    public double TotalGold { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        AddAllResources();

    }

    // Update is called once per frame
    void Update()
    {
        // Fungsi untuk selalu mengeksekusi CollectPerSecond setiap detik
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;

        }
        CheckResourceCost();
        CheckAchiveGold();
        CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);

    }

    private void CheckAchiveGold()
    {
        if (TotalGold >= 1000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "1000");
        }
        if (TotalGold >= 10000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "10000");
        }
        if (TotalGold >= 100000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "100000");
        }
        if (TotalGold >= 1000000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "1000000");
        }
        if (TotalGold >= 100000000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "100000000");
        }
        if (TotalGold >= 1000000000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "1000000000");
        }
        if (TotalGold >= 1000000000000)
        {
            AchievementController.Instance.UnlockAchievement(AchievementType.Gold, "1000000000000");
        }
    }
    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isBuyable = TotalGold >= resource.GetUpgradeCost();
            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
        }

    }

    private void AddAllResources()
    {
        bool showResources = true;
        foreach (ResourceConfig config in ResourcesConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefab.gameObject, ResourcesParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();
            resource.SetConfig(config);
            obj.gameObject.SetActive(showResources);
            if (showResources && !resource.IsUnlocked)
            {
                showResources = false;
            }
            _activeResources.Add(resource);
        }
    }

    public void ShowNextResource()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)

    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }
        }
        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;
        tapText.Text.text = $"+{ AbbrevationUtility.AbbreviateNumber(output) }";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if (tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }
        return tapText;
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        output *= AutoCollectPercentage;

        // Fungsi ToString("F1") ialah membulatkan angka menjadi desimal yang memiliki 1 angka di belakang koma

        AutoCollectInfo.text = $"Auto Collect: { AbbrevationUtility.AbbreviateNumber(output) } / second";
        AddGold(output);

    }

    public void AddGold(double value)

    {

        TotalGold += value;

        GoldInfo.text = $"Gold: { AbbrevationUtility.AbbreviateNumber(TotalGold) }";

    }
}


// Fungsi System.Serializable adalah agar object bisa di-serialize dan
// value dapat di-set dari inspector
[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}
