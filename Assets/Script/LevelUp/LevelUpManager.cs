using System;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    [SerializeField] LevelUpUI levelUpUI;

    int level = 1;
    int maxExperience = 2;
    int curExperience = 0;
    float experienceRatio = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void GetExperience(int exp)
    {
        curExperience += exp;
        int div = curExperience / maxExperience;

        if(div > 0)
        {
            curExperience = curExperience % maxExperience;
            level += div;
            //level 횟수만큼 증강을 표시한다.
            AugmentManager.Instance.SetAndShowAugmentUI(div);
        }

        // Update UI;
        UpdateUI();
    }
    private void UpdateUI()
    {
        experienceRatio = (float)curExperience / maxExperience;
        levelUpUI.UpdateLevelUpUI(level, experienceRatio);
    }
}
