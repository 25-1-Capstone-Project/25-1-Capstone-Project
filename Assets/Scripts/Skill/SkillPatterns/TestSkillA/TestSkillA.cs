using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Player/Skill/SkillA")]
public class TestSkillA : SkillPattern
{
    [SerializeField] private GameObject fireballPrefab;
 

    public override IEnumerator CommonSkill(PlayerScript player){


        GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity,SkillManager.Instance.skillObjectsParent);
        fireball.GetComponent<Rigidbody2D>().linearVelocity = player.Direction * fireballSpeed;
        
        yield return null;
    }

    public override IEnumerator UltimateSkill(PlayerScript player)
    {
        player.ParryStack = 0;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Ultimate Skill Activated!");
           GameObject fireball = Instantiate(fireballPrefab, player.transform.position, Quaternion.identity,SkillManager.Instance.skillObjectsParent);
            fireball.GetComponent<Rigidbody2D>().linearVelocity = player.Direction * fireballSpeed;
        
            yield return 0.1f;
        }
        yield return null;
    }

}
