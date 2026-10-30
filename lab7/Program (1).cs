using System;
using System.Collections.Generic;
using Lab07;

const int N = 3;
const int K = 6;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine($"N = {N}, K = {K}, a = {N % 3}, b = {N % 4}, c = {N % 5}");

var rnd = new Random(N);
var numbers = new List<int>();
for (int i = 0; i < 20; i++)
    numbers.Add(rnd.Next(-K, K + 1)); // [-K; K]

var words = new List<string>
{
    "яблуко", "дім", "сонце", "кіт-мурчик", "інтернет", "ліс",
    "програмування", "осінь", "по-перше", "вікно", "море", "алгоритм", "ока"
};

var students = new List<Student>
{
    new Student("Коваленко Андрій", "КН-3/1", 92),
    new Student("Шевченко Олена", "КН-3/1", 78),
    new Student("Бондаренко Максим", "КН-3/2", 55),
    new Student("Коваль Ірина", "КН-3/1", 60),
    new Student("Мельник Тарас", "КН-3/2", 81),
    new Student("Бойко Софія", "КН-3/2", 45),
    new Student("Ткаченко Давид", "КН-3/1", 99),
};

static bool IsVowel(char c) => "аеєиіїоуюяaeiouy".IndexOf(char.ToLower(c)) >= 0;

Console.WriteLine("\n== Завдання 1: Filter ==");
var fNum = Algorithms.Filter(numbers, x => x > K);
var fStr = Algorithms.Filter(words, s => s.Length > 5);
var fStu = Algorithms.Filter(students, s => s.Score >= 75);
Console.WriteLine($"Числа > {K} ({fNum.Count}): {string.Join(", ", fNum)}");
Console.WriteLine($"Рядки довші за 5 ({fStr.Count}): {string.Join(", ", fStr)}");
Console.WriteLine($"Студенти бал >= 75 ({fStu.Count}): {string.Join("; ", fStu)}");

Console.WriteLine("\n== Завдання 2: Map ==");
var mNum = Algorithms.Map(numbers, x => $"({x})");
var mStr = Algorithms.Map(words, s =>
    Algorithms.Aggregate(Algorithms.Map(new List<char>(s), c => IsVowel(c) ? 1 : 0), 0, (acc, v) => acc + v));
var mStu = Algorithms.Map(students, s => s.Score >= 60);
Console.WriteLine($"Числа -> рядки: {string.Join(" ", mNum)}");
Console.WriteLine($"Рядки -> голосні: {string.Join(", ", mStr)}");
Console.WriteLine($"Студенти -> склав: {string.Join(", ", mStu)}");

Console.WriteLine("\n== Завдання 3: Aggregate ==");
long sum = Algorithms.Aggregate(numbers, 0L, (acc, x) => acc + x);
long prod = Algorithms.Aggregate(numbers, 1L, (acc, x) => acc * x);
int? max = Algorithms.Aggregate<int, int?>(numbers, null, (acc, x) => acc == null || x > acc ? x : acc);
int? min = Algorithms.Aggregate<int, int?>(numbers, null, (acc, x) => acc == null || x < acc ? x : acc);
Console.WriteLine($"Сума = {sum}, добуток = {prod}, максимум = {max}, мінімум = {min}");

Console.WriteLine("\n== Завдання 4: GroupBy ==");
static string ScoreRange(int s) => s < 60 ? "0–59" : s < 75 ? "60–74" : s < 90 ? "75–89" : "90–100";

PrintGroups("За групою", Algorithms.GroupBy(students, s => s.Group));
PrintGroups("За першою літерою прізвища", Algorithms.GroupBy(students, s => s.Name[0]));
PrintGroups("За діапазоном балу", Algorithms.GroupBy(students, s => ScoreRange(s.Score)));
PrintGroups("Рядки за довжиною", Algorithms.GroupBy(words, s => s.Length));

static void PrintGroups<TKey, T>(string title, Dictionary<TKey, List<T>> groups) where TKey : notnull
{
    Console.WriteLine(title + ":");
    foreach (var pair in groups)
        Console.WriteLine($"  {pair.Key}: {pair.Value.Count}");
}
Console.WriteLine("\n== Завдання 5: захоплення ==");

int threshold = K;
Func<int, bool> above = x => x > threshold;
threshold = 1000;
Console.WriteLine($"1) above(10) = {above(10)}"); 

var viaFor = new List<Func<int>>();
for (int i = 0; i < 3; i++)
    viaFor.Add(() => i);
Console.WriteLine("2) for: " + string.Join(", ", Algorithms.Map(viaFor, f => f()))); 

var viaForeach = new List<Func<int>>();
foreach (int j in new[] { 0, 1, 2 })
    viaForeach.Add(() => j);
Console.WriteLine("3) foreach: " + string.Join(", ", Algorithms.Map(viaForeach, f => f()))); // 0, 1, 2

var fixedFor = new List<Func<int>>();
for (int i = 0; i < 3; i++)
{
    int copy = i;
    fixedFor.Add(() => copy);
}
Console.WriteLine("2*) for з копією: " + string.Join(", ", Algorithms.Map(fixedFor, f => f()))); // 0, 1, 2

Console.WriteLine("\n== Крайові випадки ==");
var empty = new List<int>();
Console.WriteLine($"Filter(порожній): {Algorithms.Filter(empty, x => x > 0).Count}");
Console.WriteLine($"Map(порожній): {Algorithms.Map(empty, x => x * x).Count}");
Console.WriteLine($"GroupBy(порожній): {Algorithms.GroupBy(empty, x => x % 2).Count}");
Console.WriteLine($"Max(порожній): {(Algorithms.Aggregate<int, int?>(empty, null, (a, x) => a == null || x > a ? x : a)?.ToString() ?? "немає значення")}");
Console.WriteLine($"Filter(жоден не підходить): {Algorithms.Filter(numbers, x => x > 1000).Count}");
Console.WriteLine($"GroupBy(усі в одній групі): {Algorithms.GroupBy(numbers, x => "all").Count}");
var one = new List<int> { 42 };
Console.WriteLine($"Min(один елемент): {Algorithms.Aggregate<int, int?>(one, null, (a, x) => a == null || x < a ? x : a)}");

public class Student
{
    public string Name { get; }
    public string Group { get; }
    public int Score { get; }

    public Student(string name, string group, int score)
    {
        Name = name;
        Group = group;
        Score = score;
    }

    public override string ToString() => $"{Name} ({Group}, {Score})";
}
