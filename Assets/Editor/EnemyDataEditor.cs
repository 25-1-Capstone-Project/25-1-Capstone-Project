using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyData data = (EnemyData)target;

        // Draw default fields manually
        data.eEnemyType = (EEnemyType)EditorGUILayout.EnumPopup("Enemy Type", data.eEnemyType);
        // Show different attack type enum based on enemy type
        switch (data.eEnemyType)
        {
            case EEnemyType.CloseRange:
                data.closeAttackType = (ECloseAttackType)EditorGUILayout.EnumPopup("Close Attack Type", data.closeAttackType);
                break;
            case EEnemyType.LongRange:
                data.longAttackType = (ELongAttackType)EditorGUILayout.EnumPopup("Long Attack Type", data.longAttackType);
                break;
        }

        data.name = EditorGUILayout.TextField("Monster Name", data.name);
        data.maxHealth = EditorGUILayout.IntField("Max Health", data.maxHealth);
        data.moveSpeed = EditorGUILayout.FloatField("Move Speed", data.moveSpeed);
        data.attackDamage = EditorGUILayout.IntField("Attack Damage", data.attackDamage);
        data.animator = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator", data.animator, typeof(RuntimeAnimatorController), false);
        data.attackPattern = (EnemyAttackPattern)EditorGUILayout.ObjectField("Attack Pattern", data.attackPattern, typeof(EnemyAttackPattern), false);


        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }
}
