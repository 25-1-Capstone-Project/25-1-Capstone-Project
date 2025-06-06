using UnityEngine;

public class GridManager : Singleton<GridManager>
{

    public int[,] grid; // 0 = 빈칸, 1 = 벽

    protected override void Awake()
    {
        base.Awake();

        // 테스트용 벽 (5,5) 위치
        grid[5, 5] = 1;

    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1))
            return false;
        return grid[x, y] == 0;
    }
}
