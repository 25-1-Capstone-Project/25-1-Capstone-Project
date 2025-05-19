using System;
using Unity.Behavior;

[BlackboardEnum]
public enum EBossState
{
    IDLE,
	CHASE,
	ATTACK,
	DAMAGED,
	DEATH
}
