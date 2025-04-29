using System;
using System.Collections.Generic;
using System.Linq;

namespace MIMAPR_LR_1
{
    interface IReadOnlyRestriction
    {
        IReadOnlyList<double> Coefs { get; }
        RestrictionType Type { get; }
        double RightPart { get; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var a = Input.LinearProgrammingProblem().Solve();
        }
    }

    static class SimplexMethod
    {
        public static ProblemSolution[] Solve(this LinearProgrammingProblem problem)
        {
            if(problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (problem.ObjectiveFunc.Count == 0)
                throw new ArgumentException("Задача не содержит переменных");

            problem = problem.ConvertToMaxType().ConvertToCanon();

            return null;
        }
    }

    class LinearProgrammingProblem
    {
        private readonly List<double> _objectiveFunc = new List<double>();

        private readonly List<Restriction> _restrictions = new List<Restriction>();

        public IReadOnlyList<double> ObjectiveFunc => _objectiveFunc;

        public IReadOnlyList<IReadOnlyRestriction> Restrictions => _restrictions;

        public TaskType TaskType { get; set; } = TaskType.Max;

        public LinearProgrammingProblem ConvertToMaxType()
        {
            var factor = TaskType == TaskType.Min ? -1 : 1;
            TaskType = TaskType.Max;

            for(int i = 0; i < _objectiveFunc.Count; i++)
                _objectiveFunc[i] *= factor;

            return this;
        }

        public LinearProgrammingProblem ConvertToCanon()
        {
            RestrictionType currentType;

            for (int i = 0; i < _restrictions.Count; i++)
            {
                currentType = _restrictions[i].Type;

                if (currentType == RestrictionType.Equal)
                    continue;

                AddVariable(0);
                _restrictions[i].SetCoef((uint)_objectiveFunc.Count - 1, currentType == RestrictionType.LessEqual ? 1 : -1);
            }

            return this;
        }

        public LinearProgrammingProblem AddVariable(double coef)
        {
            if (double.IsNaN(coef))
                throw new ArgumentException("Коэффициент не может быть NaN");

            if (double.IsInfinity(coef))
                throw new ArgumentException("Коэффициент не может быть бесконечностью");

            _objectiveFunc.Add(coef);

            foreach (var restr in _restrictions)
                restr.AddVariable(0);

            return this;
        }

        public LinearProgrammingProblem AddRestriction(IReadOnlyRestriction restr)
        {
            if (!(restr is Restriction))
                throw new ArgumentException("restr должен быть объектом класса Restriction");

            if (restr.Coefs.Count != _objectiveFunc.Count)
                throw new ArgumentException("Ограничение должно иметь столько же переменных, сколько и целевая функция");

            _restrictions.Add((Restriction)restr);
            return this;
        }
    }

    class ProblemSolution
    {
        public readonly IReadOnlyList<double> ArgumentsValues;

        public readonly double FuncValue;

        public ProblemSolution(double[] argsValues, double funcValue)
        {
            ArgumentsValues = argsValues;
            FuncValue = funcValue;
        }
    }

    class Restriction : IReadOnlyRestriction
    {
        private readonly List<double> _coefs = new List<double>();

        public IReadOnlyList<double> Coefs => _coefs;

        public RestrictionType Type { get; set; } = RestrictionType.Equal;

        public double RightPart { get; set; }

        public Restriction AddVariable(double coef)
        {
            if (double.IsNaN(coef))
                throw new ArgumentException("Коэффициент не может равнятся NaN", nameof(coef));

            _coefs.Add(coef);
            return this;
        }

        public Restriction SetCoef(uint index, double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Коэффициент не может быть NaN");

            if (double.IsInfinity(value))
                throw new ArgumentException("Коэффициент не может быть бесконечностью");

            if (index >= _coefs.Count)
                throw new ArgumentException($"Коэффициент с индексом {index} не существует");

            _coefs[(int)index] = value;
            return this;
        }
    }

    static class Input
    {
        private readonly static Type[] numberTypes = new Type[]
        {
            typeof(int), typeof(double), typeof(decimal),
            typeof(float), typeof(long), typeof(short),
            typeof(byte), typeof(sbyte), typeof(ulong),
            typeof(ushort), typeof(uint)
        };

        public static LinearProgrammingProblem LinearProgrammingProblem()
        {
            Console.Write("Количество переменных задачи: ");
            var varCount = InputNumber<uint>();

            Console.Write("Введете количество ограничений: ");
            var restrCount = InputNumber<uint>();

            var result = new LinearProgrammingProblem();

            Console.WriteLine("Введите коэффициенты целевой функции");

            for (uint i = 0; i < varCount; i++)
                result.AddVariable(InputNumber<double>());

            Console.Write("Тип задачи ('Min' или 'Max'):");
            result.TaskType = InputTaskType();

            for (uint i = 0; i < restrCount; i++)
            {
                Console.WriteLine($"\nВвод {i + 1}-ого ограничения");
                result.AddRestriction(InputRestriction(varCount));
            }

            return result;
        }

        public static Restriction InputRestriction(uint varCount)
        {
            var restriction = new Restriction();

            Console.WriteLine("\nВведите коэффициенты:");
            for (int i = 0; i < varCount; i++)
            {
                Console.Write($"Коэффициент x{i + 1}: ");
                restriction.AddVariable(InputNumber<double>());
            }

            Console.WriteLine("\nВведите тип ограничения: '=', '>=' или '<='");
            restriction.Type = InputRestrictionType();

            Console.WriteLine("\nВведите правую часть ограничения:");
            restriction.RightPart = InputNumber<double>();

            return restriction;
        }

        public static TaskType InputTaskType()
        {
            while (true)
            {
                Console.Write("Введите тип задачи ('max' или 'min'): ");
                var input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "max": return TaskType.Max;
                    case "min": return TaskType.Min;
                    default:
                        Console.WriteLine($"'{input}' не является типом задачи. Повторите ввод");
                        break;
                }
            }
        }

        private static RestrictionType InputRestrictionType()
        {
            while (true)
            {
                Console.Write("Тип ограничения (=, >=, <=): ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "=": return RestrictionType.Equal;
                    case ">=": return RestrictionType.MoreEqual;
                    case "<=": return RestrictionType.LessEqual;
                    default:
                        Console.WriteLine($"'{input}' не является допустимым ограничением");
                        break;
                }
            }
        }

        private static T InputNumber<T>() where T : struct, IConvertible
        {
            if (!numberTypes.Contains(typeof(T)))
                throw new NotSupportedException($"Тип {typeof(T).Name} не поддерживается");

            while (true)
            {
                try
                {
                    return (T)Convert.ChangeType(Console.ReadLine(), typeof(T));
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Введено не число типа {typeof(T)}. Повторите ввод:"); ;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Число слишком большое/маленькое. Повторите ввод:");
                }
            }
        }
    }

    enum TaskType { Max, Min }

    enum RestrictionType {  Equal, LessEqual, MoreEqual }
}
