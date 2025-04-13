// using UnityEngine;
// using System.Collections;

// [CreateAssetMenu(menuName = "Player/Skill/SkillB")]
// public class TestSkillB : SkillPattern
// {
//     [SerializeField] private int damage = 10;
//     [SerializeField] private int requiredParryStack = 2;
//     [SerializeField] private float attackRadius = 2f;

//     public override IEnumerator Act(Player player)
//     {
//         if (player.ParryStack < requiredParryStack)
//         {
//             Debug.Log("�и� ���� ����");
//             yield break;
//         }

//         player.ParryStack -= requiredParryStack;


//         Collider2D[] hitObjects = Physics2D.OverlapCircleAll(player.transform.position, attackRadius);
//         foreach (Collider2D obj in hitObjects)
//         {
//             if (obj.CompareTag("Enemy"))
//             {
//                 // ���� ����� ó�� ������...
//                 Debug.Log("����(��ųB)");
//             }
//         }

//         Debug.Log($"��ųB / �����: {damage}, ���� �и� ����: {player.ParryStack}");

//         yield return null;
//     }
// }
