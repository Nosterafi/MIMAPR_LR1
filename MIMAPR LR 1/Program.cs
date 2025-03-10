using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MIMAPR_LR_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = SimplexMethod.Solve(Input.LinearProgrammingProblem());
        }
    }

    static class SimplexMethod
    {
        public static double[] Solve(this LinearProgrammingProblem problem)
        {
            var basis = new int[problem.RestrictionsCount];

            for(uint i = 0; i < problem.RestrictionsCount; i++)
            {
                try
                {
                    basis[i] = (int)problem.GetBasisVariable(i);
                }
                catch (InvalidOperationException)
                {
                    problem.AddArtificial(i);
                    basis[i] = problem.VariablesCount - 1;
                }
            }

            var table = problem.InitProblemTable(basis);

            throw new NotImplementedException();
        }

        //public double GetFuncValue(double[] arguments)
        //{
        //    var result = (double)0;
        //    var multiple = TaskType == TaskType.Max ? 1 : -1;

        //    for (int i = 0; i < arguments.Length; i++)
        //        result += arguments[i] * TargetFunc[i] * multiple;

        //    return result;
        //}



        //private int[] GetFirstBasis(int dimension)
        //{
        //    var result = new int[Restrictions.Count];

        //    for (int i = 0; i < Restrictions.Count; i++)
        //    {
        //        bool foundBasis = false;

        //        for (int j = 0; j < dimension; j++)
        //        {
        //            if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
        //            {
        //                bool isUnitColumn = true;

        //                for (int k = 0; k < Restrictions.Count; k++)
        //                {
        //                    if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
        //                    {
        //                        isUnitColumn = false;
        //                        break;
        //                    }
        //                }

        //                if (isUnitColumn)
        //                {
        //                    result[i] = j;
        //                    foundBasis = true;
        //                    break;
        //                }
        //            }
        //        }

        //        if (!foundBasis)
        //        {
        //            for (int j = dimension; j < Restrictions[i].Coefs.Count; j++)
        //            {
        //                if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
        //                {
        //                    bool isUnitColumn = true;

        //                    for (int k = 0; k < Restrictions.Count; k++)
        //                    {
        //                        if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
        //                        {
        //                            isUnitColumn = false;
        //                            break;
        //                        }
        //                    }

        //                    if (isUnitColumn)
        //                    {
        //                        result[i] = j;
        //                        foundBasis = true;
        //                        break;
        //                    }
        //                }
        //            }
        //        }

        //        if (!foundBasis)
        //        {
        //            throw new InvalidOperationException($"Не удалось найти базисную переменную для ограничения {i}");
        //        }
        //    }

        //    return result;
        //}

        //private int GetGuideRowIndex(double[][] taskTable, int guideColIndex)
        //{
        //    var min = double.MaxValue;
        //    var result = 0;
        //    double rowValue;

        //    for (int i = 0; i < taskTable.Length; i++)
        //        if (taskTable[i][guideColIndex] != 0)
        //        {
        //            rowValue = taskTable[i][1] / taskTable[i][guideColIndex];

        //            if (rowValue > 0 && rowValue < min)
        //            {
        //                min = rowValue;
        //                result = i;
        //            }
        //        }

        //    return result;
        //}

        //private int GetGuideColIndex(double[] simplexDivs)
        //{
        //    var min = double.MaxValue;
        //    var result = 0;

        //    for (int i = 0; i < simplexDivs.Length; i++)
        //        if (simplexDivs[i] < min)
        //        {
        //            min = simplexDivs[i];
        //            result = i + 2;
        //        }

        //    return result;
        //}

        //private double[][] GetNewTaskTable(List<double> targetCoefs, double[][] oldTable, int guideRow, int guideCol, int[] newBasis)
        //{
        //    var result = new double[oldTable.Length][];
        //    double multiple;

        //    for (int i = 0; i < oldTable.Length; i++)
        //    {
        //        result[i] = new double[oldTable[i].Length];

        //        if (i != guideRow)
        //        {
        //            multiple = -oldTable[i][guideCol] / oldTable[guideRow][guideCol];

        //            for (int j = 1; j < oldTable[i].Length; j++)
        //            {
        //                result[i][j] = oldTable[guideRow][j] * multiple + oldTable[i][j];
        //            }
        //        }
        //        else
        //        {
        //            oldTable[i].CopyTo(result[i], 0);
        //            var guideElem = oldTable[guideRow][guideCol];

        //            for (int j = 0; j < result[i].Length; j++)
        //                result[i][j] /= guideElem;
        //        }

        //        result[i][0] = TargetFunc[newBasis[i]];
        //    }

        //    return result;
        //}

        //private double[] GetSimplexDivs(List<double> targetCoefs, double[][] taskTable)
        //{
        //    var result = new double[targetCoefs.Count];

        //    for (int i = 0; i < result.Length; i++)
        //    {
        //        var scalarSum = (double)0;

        //        for (int j = 0; j < taskTable.Length; j++)
        //            scalarSum += taskTable[j][0] * taskTable[j][i + 2];

        //        result[i] = scalarSum - targetCoefs[i];
        //    }

        //    return result;
        //}

        //private void AddArtificial()
        //{
        //    int originalVarCount = TargetFunc.Count;
        //    List<int> artificialRows = new List<int>();

        //    for (int i = 0; i < Restrictions.Count; i++)
        //    {
        //        bool hasBasis = false;

        //        for (int j = 0; j < Restrictions[i].Coefs.Count; j++)
        //        {
        //            if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
        //            {
        //                bool isUnitColumn = true;

        //                for (int k = 0; k < Restrictions.Count; k++)
        //                {
        //                    if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
        //                    {
        //                        isUnitColumn = false;
        //                        break;
        //                    }
        //                }

        //                if (isUnitColumn)
        //                {
        //                    hasBasis = true;
        //                    break;
        //                }
        //            }
        //        }

        //        if (!hasBasis)
        //        {
        //            artificialRows.Add(i);
        //        }
        //    }

        //    for (int i = 0; i < artificialRows.Count; i++)
        //    {
        //        TargetFunc.Add(double.MinValue);
        //        objectiveFunc.Add(TargetFunc.Count - 1);
        //    }

        //    for (int i = 0; i < Restrictions.Count; i++)
        //    {
        //        for (int j = 0; j < artificialRows.Count; j++)
        //        {
        //            if (i == artificialRows[j])
        //                Restrictions[i].Coefs.Add(1);
        //            else
        //                Restrictions[i].Coefs.Add(0);
        //        }
        //    }
        //}

        //private void ConvertToCanon()
        //{
        //    var balanceCount = Restrictions.Where(x => x.Type != RestructionType.Equal).Count();
        //    var balancePos = 0;
        //    Restriction restr;

        //    for (int i = 0; i < balanceCount; i++)
        //        TargetFunc.Add(0);

        //    for (int i = 0; i < Restrictions.Count; i++)
        //    {
        //        restr = Restrictions[i];

        //        if (restr.Type == RestructionType.Equal)
        //        {
        //            for (int j = 0; j < balanceCount; j++)
        //                restr.Coefs.Add(0);

        //            continue;
        //        }

        //        for (int j = 0; j < balanceCount; j++)
        //        {
        //            if (j == balancePos)
        //            {
        //                if (restr.Type == RestructionType.MoreEqual)
        //                {
        //                    restr.Coefs.Add(-1);
        //                }
        //                else
        //                    restr.Coefs.Add(1);
        //            }
        //            else
        //                restr.Coefs.Add(0);
        //        }

        //        balancePos++;
        //        Restrictions[i].Type = RestructionType.Equal;
        //    }
        //}
    }

    class LinearProgrammingProblem
    {
        private readonly List<double> objectiveFunc = new List<double>();

        private readonly Restriction[] restrictions;

        public TaskType TaskType { get; set; } = TaskType.Max;

        public int VariablesCount => objectiveFunc.Count;

        public int RestrictionsCount => restrictions.Length;

        public LinearProgrammingProblem(uint varCount, uint restrCount)
        {
            restrictions = new Restriction[restrCount];

            for (int i = 0; i < varCount; i++)
                objectiveFunc.Add(0);

            for (int i = 0; i < restrCount; i++)
                restrictions[i] = new Restriction(varCount);    
        }

        public LinearProgrammingProblem SetObjectiveCoef(uint index, double value)
        {
            if (index >= objectiveFunc.Count)
                throw new IndexOutOfRangeException();

            objectiveFunc[(int)index] = value;
            return this;
        }

        public LinearProgrammingProblem SetRestrictionCoef(uint restrIndex, uint coefIndex, double value)
        {
            if (restrIndex >= restrictions.Length)
                throw new ArgumentException($"Ограничение с индексом '{restrIndex}' не существует");

            if (coefIndex >= objectiveFunc.Count)
                throw new ArgumentException($"Коэффициент с индексом '{restrIndex}' не существует");

            restrictions[restrIndex].SetCoef(coefIndex, value);
            return this;
        }

        public LinearProgrammingProblem SetRestrictionType(uint index, RestructionType value)
        {
            if (index >= restrictions.Length)
                throw new IndexOutOfRangeException();

            restrictions[index].Type = value;
            return this;
        }

        public LinearProgrammingProblem SetRestrictionRightPart(uint index, double value)
        {
            if (index >= restrictions.Length)
                throw new IndexOutOfRangeException();

            restrictions[index].RightPart = value;
            return this;
        }

        public LinearProgrammingProblem ConvertToMaxType()
        {
            if (TaskType == TaskType.Max) return this;

            for (int i = 0; i < VariablesCount; i++)
                objectiveFunc[i] *= -1;

            TaskType = TaskType.Max;
            return this;
        }

        public LinearProgrammingProblem AddBalances()
        {
            for (int i = 0; i < restrictions.Length; i++)
            {
                if (restrictions[i].Type == RestructionType.Equal)
                    continue;

                objectiveFunc.Add(0);

                for (int j = 0; j < restrictions.Length; j++)
                {
                    if (i == j)
                    {
                        if (restrictions[j].Type == RestructionType.MoreEqual)
                            restrictions[j].AddVariable(-1);
                        else restrictions[j].AddVariable(1);

                        restrictions[j].Type = RestructionType.Equal;

                        continue;
                    }

                    restrictions[j].AddVariable(0);
                }
            }

            return this;
        }

        public uint GetBasisVariable(uint restrIndex)
        {
            var basisFlag = true;

            for (uint i = 0; i < VariablesCount; i++)
            {
                for (uint j = 0; j < restrictions.Length; j++)
                {
                    if (restrIndex == j && restrictions[j].GetCoef(i) != 1)
                    {
                        basisFlag = !basisFlag;
                        break;
                    }
                        
                    if(restrIndex != j && restrictions[j].GetCoef(i) != 0)
                    {
                        basisFlag = !basisFlag;
                        break;
                    }
                }

                if (basisFlag) return i;

                basisFlag = true;
            }

            throw new InvalidOperationException("В данном ограничении ни одна переменная не может быть базисной");
        }

        public LinearProgrammingProblem AddArtificial(uint restrIndex)
        {
            if (restrIndex >= RestrictionsCount)
                throw new IndexOutOfRangeException();

            objectiveFunc.Add(double.MinValue);

            for(int i = 0; i < restrictions.Length; i++)
            {
                if(i == restrIndex) restrictions[i].AddVariable(1);
                else restrictions[i].AddVariable(0);
            }

            return this;
        }

        public double[,] InitProblemTable(int[] basis)
        {
            var result = new double[RestrictionsCount, VariablesCount + 1];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                result[i, 0] = restrictions[i].RightPart;

                for (uint j = 1; j < result.GetLength(1); j++)
                    result[i, j] = restrictions[i].GetCoef(j - 1);
            }

            return result;
        }
    }

    class Restriction
    {
        private readonly List<double> coefs;

        public RestructionType Type { get; set; } = RestructionType.Equal;

        public double RightPart { get; set; } = 0;

        public int VariablesCount => coefs.Count;

        public Restriction(uint varCount) => coefs = new double[varCount].ToList();

        public void AddVariable(double coef) => coefs.Add(coef);

        public double GetCoef(uint index)
        {
            if (index >= VariablesCount)
                throw new IndexOutOfRangeException();

            return coefs[(int)index];
        }

        public void SetCoef(uint index, double value)
        {
            if (index >= VariablesCount)
                throw new IndexOutOfRangeException();

            coefs[(int)index] = value;
        }
    }

    static class Input
    {
        public static LinearProgrammingProblem LinearProgrammingProblem()
        {
            Console.WriteLine("Введите количество переменных задачи");
            var varCount = InputNumber<uint>();

            Console.WriteLine("\nВведете количество ограничений");
            var restrCount = InputNumber<uint>();

            var result = new LinearProgrammingProblem(varCount, restrCount);

            Console.WriteLine("\nВведите коэффициенты целевой функции");

            for (uint i = 0; i < varCount; i++)
                result.SetObjectiveCoef(i, InputNumber<double>());

            Console.WriteLine("\nВведите тип задачи: 'Min' или 'Max'");
            result.TaskType = InputTaskType();

            for(uint i = 0; i < restrCount; i++)
            {
                Console.WriteLine($"\nВведите коэффициенты { i + 1 }-ого ограничения");

                for(uint j = 0; j < varCount; j++)
                    result.SetRestrictionCoef(i, j, InputNumber<double>());

                Console.WriteLine($"\nВведите знак { i + 1 }-ого ограничения: '=', '>=' или '<='");
                result.SetRestrictionType(i, InputRestructionType());

                Console.WriteLine($"\nВведите правую часть {i + 1}-ого ограничения");
                result.SetRestrictionRightPart(i, InputNumber<double>());
            }

            return result;
        }

        public static TaskType InputTaskType()
        {
            string typeStr;

            while (true)
            {
                typeStr = Console.ReadLine().ToLower();

                if (typeStr.Equals("max")) return TaskType.Max;
                if (typeStr.Equals("min")) return TaskType.Min;

                Console.WriteLine($"'{typeStr}' не является типом ограничения. Повторите ввод");
            }
        }

        private static RestructionType InputRestructionType()
        {
            string typeStr;

            while (true)
            {
                typeStr = Console.ReadLine();

                if (typeStr.Equals("=")) return RestructionType.Equal;
                if (typeStr.Equals(">=")) return RestructionType.MoreEqual;
                if (typeStr.Equals("<=")) return RestructionType.LessEqual;

                Console.WriteLine($"'{typeStr}' не является знаком ограничения. Повторите ввод");
            }
        }

        private static NumT InputNumber<NumT>()
        {
            var parse = typeof(NumT).GetMethod("Parse", new Type[] { typeof(string) }) ?? 
                throw new InvalidOperationException("NumT должен быть числовым типом");

            while (true)
            {
                try
                {
                    return (NumT)parse.Invoke(null, new object[] { Console.ReadLine() });
                }
                catch (TargetInvocationException e) when (e.InnerException is FormatException)
                {
                    Console.WriteLine("Введено не число. Повторите ввод");
                }
            }
        }
    }

    enum TaskType { Min, Max }

    enum RestructionType { Equal, MoreEqual, LessEqual }
}
