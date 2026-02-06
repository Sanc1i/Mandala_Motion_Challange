using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Badge
{
    public string badgeName;
    public Sprite icon;
    public bool unlocked;
}

public class GamificationManager : MonoBehaviour
{
    [Header("Points")]
    public int currentPoints = 0;
    public Text pointsText;

    [Header("Badges")]
    public List<Badge> badges = new List<Badge>();
    public Transform badgeContainer;
    public GameObject badgePrefab;

    [Header("Progress")]
    public Slider progressBar;
    public int pointsForCompletion = 1000;

    void Start()
    {
        UpdateUI();
        InitializeBadges();
    }

    void InitializeBadges()
    {
        if (badges.Count > 0) return;
        // Create default badges
        badges.Add(new Badge { badgeName = "First Steps", unlocked = false });
        badges.Add(new Badge { badgeName = "Rhythm Master", unlocked = false });
        badges.Add(new Badge { badgeName = "Mandala Master", unlocked = false });
        badges.Add(new Badge { badgeName = "Persistent", unlocked = false });
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        UpdateUI();
        CheckBadgeUnlocks();
    }

    public void AwardBadge(string badgeName)
    {
        Badge badge = badges.Find(b => b.badgeName == badgeName);
        if (badge != null && !badge.unlocked)
        {
            badge.unlocked = true;
            Debug.Log($"Badge Unlocked: {badgeName}!");
            ShowBadgeNotification(badge);
        }
    }

    void CheckBadgeUnlocks()
    {
        // Auto-unlock badges based on points
        if (currentPoints >= 100 && !badges[0].unlocked)
        {
            AwardBadge("First Steps");
        }
        
        if (currentPoints >= 500 && !badges[1].unlocked)
        {
            AwardBadge("Rhythm Master");
        }
        
        if (currentPoints >= pointsForCompletion && !badges[3].unlocked)
        {
            AwardBadge("Persistent");
        }
    }

    void ShowBadgeNotification(Badge badge)
    {
        // Create temporary notification
        Debug.Log($"🏆 Badge Earned: {badge.badgeName}");
        
        // Could instantiate a badge UI prefab here
    }

    void UpdateUI()
    {
        if (pointsText != null)
        {
            pointsText.text = $"Points: {currentPoints}";
        }

        if (progressBar != null)
        {
            progressBar.value = (float)currentPoints / pointsForCompletion;
        }
    }

    public int GetCompletedBadgeCount()
    {
        return badges.FindAll(b => b.unlocked).Count;
    }
}
