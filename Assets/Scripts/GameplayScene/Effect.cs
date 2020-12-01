using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Effect : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName = null;
    private Image[] iconEffect_Display = null;

    public void AddEffect(Sprite iconEffect)
    {
        if (iconEffect_Display == null)
            iconEffect_Display = GetComponentsInChildren<Image>();

        foreach (var item in iconEffect_Display)
        {
            if(!item.IsActive())
            {
                item.sprite = iconEffect;
                item.enabled = true;
                break;
            }
        }
    }
    public void CreateEffect(string playerName)
    {
        this.playerName.text = playerName;
    }
    public void ResetEffect()
    {
        foreach (var item in iconEffect_Display)
        {
            if(item.gameObject != gameObject)
                item.enabled = false;
        }
        gameObject.SetActive(false);
    }
}
