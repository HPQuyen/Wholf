using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
public class Affection : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName = null;
    private Image[] iconAffection_Display = null;

    public void AddAffection(Sprite iconAffect)
    {
        if (iconAffection_Display == null)
            iconAffection_Display = GetComponentsInChildren<Image>();

        foreach (var item in iconAffection_Display)
        {
            if(!item.IsActive())
            {
                item.sprite = iconAffect;
                item.enabled = true;
                break;
            }
        }
    }
    public void CreateAffection(string playerName)
    {
        this.playerName.text = playerName;
    }
    public void ResetAffection()
    {
        foreach (var item in iconAffection_Display)
        {
            if(item.gameObject != gameObject)
                item.enabled = false;
        }
        gameObject.SetActive(false);
    }
}
