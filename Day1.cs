using System;
using System.Linq;
using System.IO;

public static class Day1 {

    public static string Problem1() {
        var file = ".\\data\\day1.txt";

        var lines = File.ReadAllLines(file);

        var result = lines.Select(ToInt).Sum(FuelNeeded);

        return result.ToString();
    }

    public static string Problem2() {
        var file = ".\\data\\day1.txt";

        var lines = File.ReadAllLines(file);

        var result = lines.Select(ToInt).Sum(FuelNeeded2);

        return result.ToString();
    }

    private static int FuelNeeded(int mass) {
        return (int)Math.Floor((double)mass / 3.0) - 2;
    }

    private static int FuelNeeded2(int mass) {
        var newMass = mass;
        var result = 0;
        while (newMass > 0) {
            var fuel = (int)Math.Floor((double)newMass / 3.0) - 2;
            if (fuel > 0) {
                result += fuel;
                newMass = fuel;
            }
            else {
                newMass = 0;
            }
        }

        return result;
    }

    private static int ToInt(string input) {
        return int.Parse(input);
    }
}