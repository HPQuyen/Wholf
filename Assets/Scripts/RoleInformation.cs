using UnityEngine;

[CreateAssetMenu(fileName = "RoleInfoDefault",menuName ="Wholf/RoleInformation")]
public class RoleInformation : ScriptableObject
{
    public string nameRole = null;
    public Sprite spriteRole = null;
    public int timeRoleAction = 0;
    public Sprite[] spriteAbility = null;
    public string[] descriptionAbility = null;
}
