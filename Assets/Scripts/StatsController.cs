﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{

    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;
    private int _level = 1;
    private double Cost = 1000;
    [SerializeField] private float speed = 0.1f;

    public bool IsUnlocked { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ AbbrevationUtility.AbbreviateNumber(GetUpgradeCost()) }";
        ResourceDescription.text = $"Increase Collect/s Lv. { _level }\n{ GetOutput().ToString() }/s";
        ResourceButton.onClick.AddListener(() =>
        {
            if (_level < 10)
            {
                UpgradeLevel();
            }
            else
            {
                Debug.Log("Max Level");
            }
        });
    }

    public double GetUpgradeCost()
    {
        return Mathf.Pow(10, _level+4);
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if (GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }
        GameManager.Instance.AddGold(-upgradeCost);
        _level++;
        speed += 0.1f;
        GameManager.Instance.AutoCollectPercentage = speed;
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ AbbrevationUtility.AbbreviateNumber(GetUpgradeCost()) }";
        ResourceDescription.text = $"Increase Collect/s Lv. { _level }\n{ GetOutput().ToString() }/s";

    }

    public float GetOutput()
    {
        return speed;
    }




}
