using System;
using System.Collections.Generic;
using System.Linq;

namespace MIMAPR_LR_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var solver = Input.SimplexMethodSolwer();
 
            Console.Write("Оптимальные значения аргументов: ");
            var solution = solver.Solve();

            foreach (var elem in solution)
                Console.Write(elem.ToString() + " ");

            Console.WriteLine($"\nОптимальное значение целевой функции: {solver.GetFuncValue(solution)}");
            Console.ReadKey();
        }
    }

    class SimplexMethodSolwer
    {
        public TaskType TaskType { get; set; } = TaskType.Max;

        public List<double> TargetFunc { get; set; } = new List<double>();

        public List<Restriction> Restrictions { get; set; } = new List<Restriction>();

        public double[] Solve()
        {
            var dimension = TargetFunc.Count;

            ConvertToMaxType();
            ConvertToCanon();
            AddArtificial();

            var basis = GetFirstBasis(dimension);
            var taskTable = InitTaskTable(basis);

            double[] simpDivs;
            int guideCol;
            int guideRow;

            while (true)
            {
                simpDivs = GetSimplexDivs(TargetFunc, taskTable);

                if (simpDivs.All(x => x >= 0)) break;

                guideCol = GetGuideColIndex(simpDivs);
                guideRow = GetGuideRowIndex(taskTable, guideCol);

                basis[guideRow] = guideCol - 2;

                taskTable = GetNewTaskTable(TargetFunc, taskTable, guideRow, guideCol, basis);
            }

            var result = new double[dimension];
            
            for(int i = 0; i < result.Length; i++)
            {
                if (!basis.Contains(i))
                {
                    result[i] = 0;
                    continue;
                }

                result[i] = taskTable[Array.IndexOf(basis, i)][1];
            }

            return result;
        }

        public double GetFuncValue(double[] arguments)
        {
            var result = (double)0;
            var multiple = TaskType == TaskType.Max ? 1 : -1;

            for(int i = 0; i < arguments.Length; i++)
                result += arguments[i] * TargetFunc[i] * multiple;

            return result;
        }

        private double[][] InitTaskTable(int[] basis)
        {
            var result = new double[Restrictions.Count][];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[TargetFunc.Count + 2];
                result[i][0] = TargetFunc[basis[i]];
                result[i][1] = Restrictions[i].Value;

                for(int j = 0; j < TargetFunc.Count; j++)
                    result[i][j + 2] = Restrictions[i].Coefs[j];
            }

            return result;    
        }

        private int[] GetFirstBasis(int dimension)
        {
            var result = new int[Restrictions.Count];

            for (int i = 0; i < Restrictions.Count; i++)
            {
                bool foundBasis = false;

                for (int j = 0; j < dimension; j++)
                {
                    if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
                    {
                        bool isUnitColumn = true;

                        for (int k = 0; k < Restrictions.Count; k++)
                        {
                            if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
                            {
                                isUnitColumn = false;
                                break;
                            }
                        }

                        if (isUnitColumn)
                        {
                            result[i] = j;
                            foundBasis = true;
                            break;
                        }
                    }
                }

                if (!foundBasis)
                {
                    for (int j = dimension; j < Restrictions[i].Coefs.Count; j++)
                    {
                        if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
                        {
                            bool isUnitColumn = true;

                            for (int k = 0; k < Restrictions.Count; k++)
                            {
                                if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
                                {
                                    isUnitColumn = false;
                                    break;
                                }
                            }

                            if (isUnitColumn)
                            {
                                result[i] = j;
                                foundBasis = true;
                                break;
                            }
                        }
                    }
                }

                if (!foundBasis)
                {
                    throw new InvalidOperationException($"Не удалось найти базисную переменную для ограничения {i}");
                }
            }

            return result;
        }

        private int GetGuideRowIndex(double[][] taskTable, int guideColIndex)
        {
            var min = double.MaxValue;
            var result = 0;
            double rowValue;

            for (int i = 0; i < taskTable.Length; i++)
                if (taskTable[i][guideColIndex] != 0)
                {
                    rowValue = taskTable[i][1] / taskTable[i][guideColIndex];

                    if (rowValue > 0 && rowValue < min)
                    {
                        min = rowValue;
                        result = i;
                    }
                }

            return result;
        }

        private int GetGuideColIndex(double[] simplexDivs)
        {
            var min = double.MaxValue;
            var result = 0;

            for(int i = 0; i < simplexDivs.Length; i++)
                if (simplexDivs[i] < min)
                {
                    min = simplexDivs[i];
                    result = i + 2;
                }

            return result;
        }

        private double[][] GetNewTaskTable(List<double> targetCoefs, double[][] oldTable, int guideRow, int guideCol, int[] newBasis)
        {
            var result = new double[oldTable.Length][];
            double multiple;

            for(int i = 0; i < oldTable.Length; i++)
            {
                result[i] = new double[oldTable[i].Length];

                if (i != guideRow)
                {
                    multiple = -oldTable[i][guideCol] / oldTable[guideRow][guideCol];

                    for (int j = 1; j < oldTable[i].Length; j++)
                    {
                        result[i][j] = oldTable[guideRow][j] * multiple + oldTable[i][j];
                    }
                }
                else
                {
                    oldTable[i].CopyTo(result[i], 0);
                    var guideElem = oldTable[guideRow][guideCol];

                    for (int j = 0; j < result[i].Length; j++)
                        result[i][j] /= guideElem;
                }
                    
                result[i][0] = TargetFunc[newBasis[i]];
            }

            return result;  
        }

        private double[] GetSimplexDivs(List<double> targetCoefs, double[][] taskTable)
        {
            var result = new double[targetCoefs.Count];

            for(int i = 0; i < result.Length; i++)
            {
                var scalarSum = (double)0;

                for(int j = 0; j < taskTable.Length; j++)
                    scalarSum += taskTable[j][0] * taskTable[j][i + 2];

                result[i] = scalarSum - targetCoefs[i];
            }

            return result;
        }

        private void AddArtificial()
        {
            int originalVarCount = TargetFunc.Count;
            List<int> artificialRows = new List<int>();

            for (int i = 0; i < Restrictions.Count; i++)
            {
                bool hasBasis = false;

                for (int j = 0; j < Restrictions[i].Coefs.Count; j++)
                {
                    if (Math.Abs(Restrictions[i].Coefs[j] - 1.0) < 0.0001)
                    {
                        bool isUnitColumn = true;

                        for (int k = 0; k < Restrictions.Count; k++)
                        {
                            if (k != i && Math.Abs(Restrictions[k].Coefs[j]) > 0.0001)
                            {
                                isUnitColumn = false;
                                break;
                            }
                        }

                        if (isUnitColumn)
                        {
                            hasBasis = true;
                            break;
                        }
                    }
                }

                if (!hasBasis)
                {
                    artificialRows.Add(i);
                }
            }

            for (int i = 0; i < artificialRows.Count; i++)
                TargetFunc.Add(double.MinValue);

            for (int i = 0; i < Restrictions.Count; i++)
            {
                for (int j = 0; j < artificialRows.Count; j++)
                {
                    if (i == artificialRows[j])
                        Restrictions[i].Coefs.Add(1);
                    else
                        Restrictions[i].Coefs.Add(0);
                }
            }
        }

        private void ConvertToCanon()
        {
            var balanceCount = Restrictions.Where(x => x.Type != RestructionType.Equal).Count();
            var balancePos = 0;
            Restriction restr;

            for(int i =  0; i < balanceCount; i++)
                TargetFunc.Add(0);

            for(int i = 0; i < Restrictions.Count; i++)
            {
                restr = Restrictions[i];

                if (restr.Type == RestructionType.Equal)
                {
                    for (int j = 0; j < balanceCount; j++)
                        restr.Coefs.Add(0);

                    continue;
                }

                for (int j = 0; j < balanceCount; j++)
                {
                    if (j == balancePos)
                    {
                        if (restr.Type == RestructionType.MoreEqual)
                        {
                            restr.Coefs.Add(-1);
                        }

                        else
                            restr.Coefs.Add(1);
                    }
                    else
                        restr.Coefs.Add(0);
                }

                balancePos++;
                Restrictions[i].Type = RestructionType.Equal;
            }     
        }

        private void ConvertToMaxType()
        {
            if (TaskType == TaskType.Max) return;

            for (int i = 0; i < TargetFunc.Count; i++)
                TargetFunc[i] *= -1;
        }
    }

    class Restriction
    {
        public RestructionType Type { get; set; }

        public List<double> Coefs { get; set; }

        public double Value { get; set; }
    }

    enum TaskType { Min, Max }

    enum RestructionType { Equal, MoreEqual, LessEqual }

    class Input
    {
        public static SimplexMethodSolwer SimplexMethodSolwer()
        {
            Console.WriteLine("Введите коэффициенты целевой функции через пробел.");
            var coefs = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();

            Console.WriteLine("Введите тип задачи: 'Min'(минимизация целевой функции), или 'Max'(Максимизация).");
            var input = Console.ReadLine();
            TaskType type = TaskType.Min;

            if(input.Equals("Max")) type = TaskType.Max;

            Console.WriteLine("Введите количество ограничений (без ограничений неотрицательности аргументов).");
            var restCount = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите ограничения по очереди.\n");
            var retrictions = new List<Restriction>();

            for (int i = 0; i < restCount; i++)
                retrictions.Add(Restriction());

            Console.WriteLine();
            return new SimplexMethodSolwer() { TargetFunc = coefs, Restrictions = retrictions, TaskType = type };
        }

        public static Restriction Restriction()
        {
            Console.WriteLine("Введите коэффициенты через пробел.");
            var coefs = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();

            Console.WriteLine("Введите знак между левой и правой частями: '<=', '>=', или '='.");
            var input = Console.ReadLine();
            var type = RestructionType.Equal;

            if(input.Equals("<=")) type = RestructionType.LessEqual;
            if(input.Equals(">=")) type = RestructionType.MoreEqual;

            Console.WriteLine("Введите значение правой части.");
            var value = double.Parse(Console.ReadLine());

            return new Restriction { Type = type, Coefs = coefs, Value = value };
        }
    }
}
