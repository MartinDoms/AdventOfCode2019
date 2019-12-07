using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Day5 {

    public static string Problem1() {
        // test cases
        AssertArrays(Compute(new[] {1,0,0,0,99}),          new[] {2,0,0,0,99});
        AssertArrays(Compute(new[] {2,3,0,3,99}),          new[] {2,3,0,6,99});
        AssertArrays(Compute(new[] {2,4,4,5,99,0}),        new[] {2,4,4,5,99,9801});
        AssertArrays(Compute(new[] {1,1,1,4,99,5,6,0,99}), new[] {30,1,1,4,2,5,6,0,99});

        // real case
        var input = File.ReadAllText(".\\data\\day5.txt").Split(',').Select(s => int.Parse(s)).ToArray();
        
        return Compute(input)[0].ToString();
    }

    public static string Problem2() {
        var input = File.ReadAllText(".\\data\\day2.txt").Split(',').Select(s => int.Parse(s)).ToArray();
        
        throw new ArgumentException();
    }

    private static void AssertArrays(int[] actual, int[] expected) {
        Debug.Assert(Enumerable.SequenceEqual(expected, actual),
            $"Expected {ArrayStr(expected)} but got {ArrayStr(actual)}"
        );
    }

    private static string ArrayStr(int[] arr) {
        return string.Join(',', arr);
    }

    private static int[] Compute(int[] input) {
        int i = 0;
        int opcode = -1;
        while (i < input.Length && opcode != 99) {
            Instruction instruction = new NullInstruction(input, i);
            opcode = input[i] % 100;
            var fullOpCode = input[i];
            switch (opcode) {
                case 99: break;
                case 1: 
                    instruction = new Add(input, i);
                    break;
                case 2:
                    instruction = new Multiply(input, i);
                    break;
                case 3:
                    instruction = new Input(input, i);
                    break;
                case 4:
                    instruction = new Output(input, i);
                    break;
                default: throw new ArgumentException();
            }
            (var newInput, var instructionPointer) = instruction.Compute(input);

            i = instructionPointer;
            input = newInput;
        }
        return input;
    }

    private enum ParameterType {
        Position,
        Immediate
    }

    private struct Parameter {
        public int Value { get; }
        public ParameterType ParameterType { get;}

        public Parameter(int value, ParameterType parameterType)
        {
            Value = value;
            ParameterType = parameterType;
        }
    }

    private abstract class Instruction {
        public abstract int Parameters { get; }
        private List<Parameter> arguments = new List<Parameter>();
        public List<Parameter> Arguments { get { return arguments; } }

        public int InstructionPointer { get; private set;}

        public Instruction(int[] input, int instructionPointer) {
            InstructionPointer = instructionPointer;
            var opCode = input[instructionPointer];

            opCode = (int)(opCode / 100); // throw away the first two digits
            for (int i = 0; i < Parameters; i++) {
                var paramTypeInt = opCode % 10;

                ParameterType paramType;
                if (paramTypeInt == 0) paramType = ParameterType.Position;
                else if (paramTypeInt == 1) paramType = ParameterType.Immediate;
                else throw new ArgumentException("opCode");

                Arguments.Add(new Parameter(input[instructionPointer+i+1], paramType));
                opCode = (int)(opCode / 10);
            }
        }
        public (int[] program, int instructionPointer) Compute(int[] input) {
            if (Arguments.Count != Parameters) {
                throw new ArgumentException($"Expected {Parameters} arguments but found {Arguments.Count}");
            }

            return (DoCompute(input), IncrementInstructionPointer());
        }
        protected abstract int[] DoCompute(int[] input);

        protected virtual int IncrementInstructionPointer() {
            return InstructionPointer + Parameters + 1;
        }

        protected int GetValueFromParameter(Parameter parameter, int[] input) {
            if (parameter.ParameterType == ParameterType.Position) {
                return input[parameter.Value];    
            }
            else {
                return parameter.Value;
            }
        }
    }

    private class NullInstruction : Instruction
    {
        public NullInstruction(int[] input, int offset) : base(input, offset)
        {
        }

        public override int Parameters => 0;

        protected override int[] DoCompute(int[] input)
        {
            return input;
        }
    }

    private class Add : Instruction
    {
        public Add(int[] input, int offset) : base(input, offset)
        {
        }

        public override int Parameters => 3;

        protected override int[] DoCompute(int[] input)
        {
            input[Arguments[2].Value] = GetValueFromParameter(Arguments[0], input) 
                                      + GetValueFromParameter(Arguments[1], input);
            return input;
        }
    }

    private class Multiply : Instruction
    {
        public Multiply(int[] input, int offset) : base(input, offset)
        {
        }

        public override int Parameters => 3;

        protected override int[] DoCompute(int[] input)
        {
            input[Arguments[2].Value] = GetValueFromParameter(Arguments[0], input) 
                                      * GetValueFromParameter(Arguments[1], input);
            return input;
        }
    }

    private class Input : Instruction
    {
        public Input(int[] input, int offset) : base(input, offset)
        {
        }

        public override int Parameters => 1;

        protected override int[] DoCompute(int[] input)
        {
            Console.WriteLine("Enter user input");
            //var userInput = int.Parse(Console.ReadLine());
            //Console.WriteLine($"Got input {userInput}");
            //Console.WriteLine($"Storing in {Arguments[0].Value}");
            //input[Arguments[0].Value] = userInput;
            input[Arguments[0].Value] = 1;
            return input;
        }
    }

    private class Output : Instruction
    {
        public Output(int[] input, int offset) : base(input, offset)
        {
        }

        public override int Parameters => 1;

        protected override int[] DoCompute(int[] input)
        {
            Console.WriteLine("Program output:");
            Console.WriteLine(GetValueFromParameter(Arguments[0], input));

            return input;
        }
    }

}