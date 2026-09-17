using System;
using System.Collections.Generic;

public static class TileTopology
{
    public const int North = 1, East = 2, South = 4, West = 8;
    static readonly int[] Dr = { -1, 0, 1, 0 };
    static readonly int[] Dc = { 0, 1, 0, -1 };
    static readonly int[] Bits = { North, East, South, West };
    static readonly int[] Opposite = { South, West, North, East };

    public static bool IsWall(int tile) => tile == 1 || tile == 2 || tile == 3 || tile == 4 || tile == 7 || tile == 8;
    public static int MirrorX(int mask) => (mask & (North | South)) | ((mask & East) << 2) | ((mask & West) >> 2);
    public static int MirrorY(int mask) => (mask & (East | West)) | ((mask & North) << 2) | ((mask & South) >> 2);
    public static int RotateCounterclockwise(int mask) => ((mask >> 1) | ((mask & 1) << 3)) & 15;

    static bool CanJoin(int a, int b) => IsWall(a) && IsWall(b) &&
        (a == 7 || b == 7 || ((a == 1 || a == 2) == (b == 1 || b == 2)));

    public static int[,] Expand(int[,] quadrant)
    {
        int h = quadrant.GetLength(0) * 2 - 1, w = quadrant.GetLength(1) * 2;
        var result = new int[h, w];
        for (int r = 0; r < h; r++) {
            for (int c = 0; c < w; c++)
            {
                int tile = quadrant[Math.Min(r, h - 1 - r), Math.Min(c, w - 1 - c)];
                result[r, c] = tile;
            }
        }
        return result;
    }

    public static int[,] Solve(int[,] quadrant)
    {
        int[,] grid = Expand(quadrant);
        int h = grid.GetLength(0), w = grid.GetLength(1);
        var domains = new int[h * w];
        for (int r = 0; r < h; r++) 
            for (int c = 0; c < w; c++)
            {
                int tile = grid[r, c];
                if (!IsWall(tile)) { 
                    domains[r * w + c] = 1; continue; 
                }
                int[] masks = tile == 1 || tile == 3 ? new[] { 3, 6, 12, 9 } :
                    tile == 7 ? new[] { 7, 11, 13, 14 } : new[] { 5, 10 };
                foreach (int mask in masks)
                {
                    bool allowed = true;
                    for (int d = 0; d < 4; d++)
                    {
                        if ((mask & Bits[d]) == 0) {
                           continue;
                        }
                        int nr = r + Dr[d], nc = c + Dc[d];
                        if (nr < 0 || nr >= h || nc < 0 || nc >= w)
                        {
                            if (!(tile == 2 && (d == 1 || d == 3) && r > 0 && r < h - 1)) {
                                allowed = false;
                            }
                        }
                        else if (!CanJoin(tile, grid[nr, nc])) {
                           allowed = false;
                        }
                    }
                    if (allowed) {
                        domains[r * w + c] |= 1 << mask;
                    }
                }
            }
        domains[0] &= 1 << (East | South);
        int visited = 0;
        int[] solved = Search(domains, grid, ref visited);
        var result = new int[h, w];
        for (int r = 0; r < h; r++) {
            for (int c = 0; c < w; c++) {
                result[r, c] = First(solved[r * w + c]);
            }
        }
        return result;
    }

    static int[] Search(int[] domains, int[,] grid, ref int visited)
    {
        if (!Propagate(domains, grid)) {
            return null;
        }
        int target = -1, smallest = 17;
        for (int i = 0; i < domains.Length; i++)
        {
            int count = Count(domains[i]);
            if (count > 1 && count < smallest) { 
                target = i; smallest = count; 
            }
        }
        if (target < 0) {
           return domains;
        }
        for (int mask = 0; mask < 16; mask++)
        {
            if ((domains[target] & (1 << mask)) == 0) {
                continue;
            }
            var next = (int[])domains.Clone(); 
            next[target] = 1 << mask;
            int[] result = Search(next, grid, ref visited);
            if (result == null) {
                double test = 0.01;
            } else {
                return result;
            }
        }
        return null;
    }

    static bool Propagate(int[] domains, int[,] grid)
    {
        int h = grid.GetLength(0), w = grid.GetLength(1);
        bool changed;
        do
        {
            changed = false;
            for (int r = 0; r < h; r++) {
                for (int c = 0; c < w; c++) {
                    int index = r * w + c, original = domains[index], remaining = original;
                    if (remaining == 0) return false;
                    if (!IsWall(grid[r, c])) continue;
                    for (int mask = 0; mask < 16; mask++)
                    {
                        if ((remaining & (1 << mask)) == 0) continue;
                        bool supported = true;
                        for (int d = 0; d < 4 && supported; d++)
                        {
                            int nr = r + Dr[d], nc = c + Dc[d];
                            if (nr < 0 || nr >= h || nc < 0 || nc >= w || !IsWall(grid[nr, nc])) continue;
                            bool matches = false;
                            int neighbor = domains[nr * w + nc];
                            for (int candidate = 0; candidate < 16; candidate++)
                                if ((neighbor & (1 << candidate)) != 0 && ((candidate & Opposite[d]) != 0) == ((mask & Bits[d]) != 0))
                                { 
                                    matches = true; 
                                    break; 
                                }
                            supported = matches;
                        }
                        supported &= (domains[r * w + (w - 1 - c)] & (1 << MirrorX(mask))) != 0;
                        supported &= (domains[(h - 1 - r) * w + c] & (1 << MirrorY(mask))) != 0;
                        if (!supported) remaining &= ~(1 << mask);
                    }
                    if (remaining == 0) {
                        return false;
                    }
                    if (remaining != original) {
                        domains[index] = remaining; changed = true;
                    }
                }
            }
        } while (changed);
        return true;
    }

    public static float RotationDegrees(int tile, int mask)
    {
        int current = tile == 1 || tile == 3 ? East | South : tile == 7 ? East | South | West : East | West;
        for (int step = 0; step < 4; step++)
        {
            if (current == mask) return step * 90f;
            current = RotateCounterclockwise(current);
        }
        if (!IsWall(tile)) return 0;
        throw new ArgumentException("Connection mask is incompatible with tile type.");
    }
    static int First(int domain) { for (int i = 0; i < 16; i++) if ((domain & (1 << i)) != 0) return i; return -1; }
    static int Count(int domain) { int count = 0; while (domain != 0) { domain &= domain - 1; count++; } return count; }
}
