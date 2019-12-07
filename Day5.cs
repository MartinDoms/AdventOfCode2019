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
        var input = File.ReadAllText(".\\data\\day5.txt").Split(',').Select(s => int.Parse(s)).ToArray();
        
        return Compute(input)[0].ToString();
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
                case 5:
                    instruction = new JumpIfTrue(input, i);
                    break;
                case 6:
                    instruction = new JumpIfFalse(input, i);
                    break;
                case 7:
                    instruction = new LessThan(input, i);
                    break;
                case 8:
                    instruction = new Equal(input, i);
                    break;
                default: throw new ArgumentException();
            }
            (var newInput, var instructionPointer) = instruction.Compute();

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
        protected abstract int ParameterCount { get; }
        private List<Parameter> arguments = new List<Parameter>();
        public List<Parameter> Arguments { get { return arguments; } }

        public int InstructionPointer { get; private set;}
        protected int[] Input { get; }

        public Instruction(int[] input, int instructionPointer) {
            InstructionPointer = instructionPointer;
            Input = input;

            var opCode = input[instructionPointer];

            opCode = (int)(opCode / 100); // throw away the first two digits
            for (int i = 0; i < ParameterCount; i++) {
                var paramTypeInt = opCode % 10;

                ParameterType paramType;
                if (paramTypeInt == 0) paramType = ParameterType.Position;
                else if (paramTypeInt == 1) paramType = ParameterType.Immediate;
                else throw new ArgumentException("opCode");

                Arguments.Add(new Parameter(input[instructionPointer+i+1], paramType));
                opCode = (int)(opCode / 10);
            }
        }
        public (int[] program, int instructionPointer) Compute() {
            if (Arguments.Count != ParameterCount) {
                throw new ArgumentException($"Expected {ParameterCount} arguments but found {Arguments.Count}");
            }

            return (DoCompute(), IncrementInstructionPointer());
        }
        protected abstract int[] DoCompute();

        protected virtual int IncrementInstructionPointer() {
            return InstructionPointer + ParameterCount + 1;
        }

        protected int GetValueFromParameter(Parameter parameter) {
            if (parameter.ParameterType == ParameterType.Position) {
                return Input[parameter.Value];    
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

        protected override int ParameterCount => 0;

        protected override int[] DoCompute()
        {
            return Input;
        }
    }

    private class Add : Instruction
    {
        public Add(int[] input, int offset) : base(input, offset)
        {
        }

        protected override int ParameterCount => 3;

        protected override int[] DoCompute()
        {
            Input[Arguments[2].Value] = GetValueFromParameter(Arguments[0]) 
                                      + GetValueFromParameter(Arguments[1]);
            return Input;
        }
    }

    private class Multiply : Instruction
    {
        public Multiply(int[] input, int offset) : base(input, offset)
        {
        }

        protected override int ParameterCount => 3;

        protected override int[] DoCompute()
        {
            Input[Arguments[2].Value] = GetValueFromParameter(Arguments[0]) 
                                      * GetValueFromParameter(Arguments[1]);
            return Input;
        }
    }

    private class Input : Instruction
    {
        public Input(int[] input, int offset) : base(input, offset)
        {
        }

        protected override int ParameterCount => 1;

        protected override int[] DoCompute()
        {
            Console.WriteLine("Enter user input");
            var userInput = int.Parse(Console.ReadLine());
            Input[Arguments[0].Value] = userInput;
            
            return Input;
        }
    }

    private class Output : Instruction
    {
        public Output(int[] input, int offset) : base(input, offset)
        {
        }

        protected override int ParameterCount => 1;

        protected override int[] DoCompute()
        {
            Console.WriteLine("Program output:");
            Console.WriteLine(GetValueFromParameter(Arguments[0]));

            return Input;
        }
    }

    private class JumpIfTrue : Instruction {
        public JumpIfTrue(int[] input, int instructionPointer) : base(input, instructionPointer)
        {
        }

        protected override int ParameterCount => 2;

        protected override int[] DoCompute()
        {
            return Input;
        }

        protected override int IncrementInstructionPointer() {
            if (GetValueFromParameter(Arguments[0]) == 0) {
                return base.IncrementInstructionPointer();
            }
            else {
                return GetValueFromParameter(Arguments[1]);
            }
        }
    }

    private class JumpIfFalse : Instruction {
        public JumpIfFalse(int[] input, int instructionPointer) : base(input, instructionPointer)
        {
        }

        protected override int ParameterCount => 2;

        protected override int[] DoCompute()
        {
            return Input;
        }

        protected override int IncrementInstructionPointer() {
            if (GetValueFromParameter(Arguments[0]) != 0) {
                return base.IncrementInstructionPointer();
            }
            else {
                return GetValueFromParameter(Arguments[1]);
            }
        }
    }

    private class LessThan : Instruction {
        public LessThan(int[] input, int instructionPointer) : base(input, instructionPointer)
        {
        }

        protected override int ParameterCount => 3;

        protected override int[] DoCompute()
        {
            if (GetValueFromParameter(Arguments[0]) < GetValueFromParameter(Arguments[1])) {
                Input[Arguments[2].Value] = 1;
            }
            else {
                Input[Arguments[2].Value] = 0;
            }

            return Input;
        }
    }

    private class Equal : Instruction {
        public Equal(int[] input, int instructionPointer) : base(input, instructionPointer)
        {
        }

        protected override int ParameterCount => 3;

        protected override int[] DoCompute()
        {
            if (GetValueFromParameter(Arguments[0]) == GetValueFromParameter(Arguments[1])) {
                Input[Arguments[2].Value] = 1;
            }
            else {
                Input[Arguments[2].Value] = 0;
            }

            return Input;
        }
    }
}