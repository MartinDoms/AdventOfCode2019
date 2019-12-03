using System.Diagnostics;
using System.IO;
using System;
using System.Linq;

public static class Day2 {

    public static string Problem1() {
        // test cases
        Debug.Assert(Enumerable.SequenceEqual(Compute(new[] {1,0,0,0,99}),          new[] {2,0,0,0,99}));
        Debug.Assert(Enumerable.SequenceEqual(Compute(new[] {2,3,0,3,99}),          new[] {2,3,0,6,99}));
        Debug.Assert(Enumerable.SequenceEqual(Compute(new[] {2,4,4,5,99,0}),        new[] {2,4,4,5,99,9801}));
        Debug.Assert(Enumerable.SequenceEqual(Compute(new[] {1,1,1,4,99,5,6,0,99}), new[] {30,1,1,4,2,5,6,0,99}));

        // real case
        var input = File.ReadAllText(".\\data\\day2.txt").Split(',').Select(s => int.Parse(s)).ToArray();
        
        return Compute(input, 12, 2)[0].ToString();
    }

    public static string Problem2() {
        var input = File.ReadAllText(".\\data\\day2.txt").Split(',').Select(s => int.Parse(s)).ToArray();
        for (int i = 0; i < 100; i++) {
            for (int j = 0; j < 100; j++) {
                if (Compute(input, i, j)[0] == 19690720) return $"{100 * i + j}";
            }
        }
        throw new ArgumentException();
    }

    private static int[] Compute(int[] input, int noun, int verb) {
        var output = new int[input.Length];
        input.CopyTo(output, 0);
        
        output[1] = noun;
        output[2] = verb;
        return Compute(output);
    }

    private static int[] Compute(int[] input) {
        int i = 0;
        int opcode = -1;
        while (i < input.Length && opcode != 99) {
            opcode = input[i];
            switch (opcode) {
                case 99: break;
                case 1: 
                    input[input[i+3]] = input[input[i+1]] + input[input[i+2]];
                    break;
                case 2:
                    input[input[i+3]] = input[input[i+1]] * input[input[i+2]];
                    break;
                default: throw new ArgumentException();
            }

            i += 4;
        }
        return input;
    }

}