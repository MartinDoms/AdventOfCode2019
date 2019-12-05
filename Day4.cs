using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public static class Day4 {

    static int lower = 136760;
    static int upper = 595730;

    public static string Problem1() {
        Debug.Assert(Match(111111) == true);
        Debug.Assert(Match(223450) == false);
        Debug.Assert(Match(123789) == false);

        var result = GetRange(lower, upper).Count(Match);

        return result.ToString();
    }

    public static string Problem2() {
        Debug.Assert(Match2(112233) == true);
        Debug.Assert(Match2(123444) == false);
        Debug.Assert(Match2(111122) == true);

        var result = GetRange(lower, upper).Count(Match2);

        return result.ToString();
    }

    private static bool Match(int input) {
        var digits = input.ToString().ToList();
        var dub = false;
        var previous = -1;
        foreach (var dig in digits) {
            var num = int.Parse(dig.ToString());
            if (previous == num) dub = true;
            
            if (previous > num) return false;
            
            previous = num;
        }

        return dub;
    }

    private static bool Match2(int input) {
        var digits = input.ToString().ToList();
        var groups = new List<int>();
        var currentGroup = 1;
        var previous = -1;

        foreach (var dig in digits) {
            
            var num = int.Parse(dig.ToString());
            
            if (previous == num) {
                currentGroup++;
            }
            else {
                groups.Add(currentGroup);
                currentGroup = 1;
            }
            
            if (previous > num) return false;
            
            previous = num;
        }
        groups.Add(currentGroup);

        return groups.Contains(2);
    }

    private static IEnumerable<int> GetRange(int lower, int upper) {
        for (int i = lower; i <= upper; i++) {
            yield return i;
        }
    }

}