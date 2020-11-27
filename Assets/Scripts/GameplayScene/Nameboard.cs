using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Nameboard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameDisplay_Text = null;
    [SerializeField]
    private TextMeshProUGUI voteDisplay_Text = null;

    public void AddName(string name, bool isLocalPlayer)
    {
        if (isLocalPlayer)
            nameDisplay_Text.color = Color.red;
        nameDisplay_Text.text = name;
    }
    public void Vote(int amount)
    {
        if (!voteDisplay_Text.gameObject.activeSelf)
            voteDisplay_Text.gameObject.SetActive(true);
        voteDisplay_Text.text = amount + " Vote";
    }
    public void ResetVote()
    {
        voteDisplay_Text.gameObject.SetActive(false);
    }
}
