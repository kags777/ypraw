using System;
using System.Collections.Generic;
using System.Linq;

namespace LevelEscapeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   ИГРА НА 5 УРОВНЯХ (СИМУЛЯЦИЯ)");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Демонстрация (10000 игр + пример)");
                Console.WriteLine("2. Пользовательский режим");
                Console.WriteLine("0. Выход");
                Console.WriteLine("========================================");
                Console.Write("Выберите пункт меню: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    RunDemonstration();
                }
                else if (choice == "2")
                {
                    RunCustomSimulation();
                }
                else if (choice == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Режим 1: 10000 игр для статистики + 1 пример с логом
        /// </summary>
        static void RunDemonstration()
        {
            Console.Clear();
            Console.WriteLine("=== ДЕМОНСТРАЦИЯ (10 000 ИГР) ===\n");

            // Стандартная стратегия
            List<bool> script = new List<bool>();
            script.AddRange(Enumerable.Repeat(true, 5));   // 5+
            script.AddRange(Enumerable.Repeat(false, 6));  // 6-
            script.AddRange(Enumerable.Repeat(true, 2));   // 2+
            script.AddRange(Enumerable.Repeat(false, 4));  // 4-
            script.AddRange(Enumerable.Repeat(true, 4));   // 4+
            script.AddRange(Enumerable.Repeat(false, 1));  // 1-
            script.AddRange(Enumerable.Repeat(true, 1));   // 1+
            script.AddRange(Enumerable.Repeat(false, 2));  // 2-

            int numGames = 10000;
            int numRounds = 25;

            Console.WriteLine($"Запуск {numGames} игр по {numRounds} раундов...\n");

            int player1Wins = 0;
            int player2Wins = 0;

            for (int i = 0; i < numGames; i++)
            {
                bool escaped = SimulateGame(script, numRounds, showLog: false);
                if (escaped) player2Wins++;
                else player1Wins++;
            }

            Console.WriteLine("=== РЕЗУЛЬТАТЫ СИМУЛЯЦИИ ===");
            Console.WriteLine($"Всего игр: {numGames}");
            Console.WriteLine($"Раундов в игре: {numRounds}");
            Console.WriteLine($"Игрок 1 (Удерживающий) победил: {player1Wins} раз ({((double)player1Wins / numGames) * 100:F2}%)");
            Console.WriteLine($"Игрок 2 (Убегающий) победил:   {player2Wins} раз ({((double)player2Wins / numGames) * 100:F2}%)");

            Console.WriteLine("\n=== ПРИМЕР ОДНОЙ СЛУЧАЙНОЙ ИГРЫ ===\n");
            SimulateGame(script, numRounds, showLog: true);

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        /// <summary>
        /// Режим 2: Пользовательский ввод (с проверкой длины стратегии)
        /// </summary>
        static void RunCustomSimulation()
        {
            Console.Clear();
            Console.WriteLine("=== ПОЛЬЗОВАТЕЛЬСКИЙ РЕЖИМ ===\n");

            // 1. Количество игр
            int numGames = 0;
            while (numGames <= 0)
            {
                Console.Write("Введите количество игр для симуляции (например, 1000): ");
                if (!int.TryParse(Console.ReadLine(), out numGames) || numGames <= 0)
                {
                    Console.WriteLine("Ошибка! Введите положительное целое число.");
                }
            }

            // 2. Количество раундов
            int numRounds = 0;
            while (numRounds <= 0)
            {
                Console.Write("Введите количество раундов (максимум ходов, по умолчанию 25): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    numRounds = 25;
                    break;
                }
                if (!int.TryParse(input, out numRounds) || numRounds <= 0)
                {
                    Console.WriteLine("Ошибка! Введите положительное целое число.");
                }
            }

            // 3. Стратегия (строгая проверка длины)
            List<bool> customScript = new List<bool>();
            Console.WriteLine($"\nВведите стратегию Второго игрока РОВНО на {numRounds} ходов.");
            Console.WriteLine("Используйте '+' для согласия и '-' для отказа.");
            Console.WriteLine("Нажмите Enter, чтобы использовать стандартную стратегию (только если раундов = 25).");

            while (true)
            {
                Console.Write($"Стратегия ({numRounds} символов): ");
                string input = Console.ReadLine();

                // Если пользователь нажал Enter (пустая строка)
                if (string.IsNullOrWhiteSpace(input))
                {
                    // Если раундов 25, можно подставить стандартную
                    if (numRounds == 25)
                    {
                        customScript.AddRange(Enumerable.Repeat(true, 5));
                        customScript.AddRange(Enumerable.Repeat(false, 6));
                        customScript.AddRange(Enumerable.Repeat(true, 2));
                        customScript.AddRange(Enumerable.Repeat(false, 4));
                        customScript.AddRange(Enumerable.Repeat(true, 4));
                        customScript.AddRange(Enumerable.Repeat(false, 1));
                        customScript.AddRange(Enumerable.Repeat(true, 1));
                        customScript.AddRange(Enumerable.Repeat(false, 2));
                        Console.WriteLine("Использована стандартная стратегия (25 ходов).");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка! Количество раундов = {numRounds}, стандартная стратегия не подходит.");
                        Console.WriteLine("Пожалуйста, введите стратегию вручную.");
                        continue;
                    }
                }

                // Парсим ввод
                bool valid = true;
                customScript.Clear();
                foreach (char c in input)
                {
                    if (c == '+') customScript.Add(true);
                    else if (c == '-') customScript.Add(false);
                    else if (char.IsWhiteSpace(c)) continue; // Игнорируем пробелы
                    else
                    {
                        Console.WriteLine($"Ошибка: недопустимый символ '{c}'. Используйте только '+' и '-'.");
                        valid = false;
                        break;
                    }
                }

                if (!valid) continue;

                // СТРОГАЯ ПРОВЕРКА: длина должна точно совпадать
                if (customScript.Count != numRounds)
                {
                    Console.WriteLine($"Ошибка! Вы ввели {customScript.Count} символов, а нужно ровно {numRounds}.");
                    Console.WriteLine("Пожалуйста, попробуйте снова.");
                    continue;
                }

                // Если всё ок, выходим из цикла
                break;
            }

            Console.WriteLine($"\nЗапуск {numGames} игр по {numRounds} раундов...\n");

            int player1Wins = 0;
            int player2Wins = 0;

            for (int i = 0; i < numGames; i++)
            {
                bool escaped = SimulateGame(customScript, numRounds, showLog: false);
                if (escaped) player2Wins++;
                else player1Wins++;
            }

            Console.WriteLine("=== РЕЗУЛЬТАТЫ ===");
            Console.WriteLine($"Всего игр: {numGames}");
            Console.WriteLine($"Раундов в игре: {numRounds}");
            Console.WriteLine($"Игрок 1 (Удерживающий) победил: {player1Wins} раз ({((double)player1Wins / numGames) * 100:F2}%)");
            Console.WriteLine($"Игрок 2 (Убегающий) победил:   {player2Wins} раз ({((double)player2Wins / numGames) * 100:F2}%)");

            Console.WriteLine("\n=== ПРИМЕР ОДНОЙ СЛУЧАЙНОЙ ИГРЫ ===\n");
            SimulateGame(customScript, numRounds, showLog: true);

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        /// <summary>
        /// Логика одной игры
        /// </summary>
        static bool SimulateGame(List<bool> script, int maxRounds, bool showLog)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int currentLevel = 0;

            if (showLog)
            {
                Console.WriteLine("Старт с Уровня 0.");
                Console.WriteLine($"Стратегия Игрока 2: {string.Join("", script.Select(s => s ? "+" : "-"))}\n");
            }

            for (int move = 1; move <= maxRounds; move++)
            {
                // 1. Первый предлагает
                int suggestedLevel = currentLevel;

                switch (currentLevel)
                {
                    case 0: suggestedLevel = 1; break;
                    case 1: suggestedLevel = (rnd.Next(2) == 0) ? 1 : 2; break;
                    case 2: suggestedLevel = (rnd.Next(2) == 0) ? 1 : 3; break;
                    case 3: suggestedLevel = (rnd.Next(2) == 0) ? 2 : 4; break;
                    case 4: suggestedLevel = (rnd.Next(2) == 0) ? 3 : 5; break;
                    case 5: suggestedLevel = 5; break;
                }

                // 2. Второй решает
                bool isAgreeing = (move - 1 < script.Count) ? script[move - 1] : true;
                int nextLevel = suggestedLevel;

                if (!isAgreeing)
                {
                    if (currentLevel == 1) nextLevel = (suggestedLevel == 1) ? 2 : 1;
                    else if (currentLevel == 2) nextLevel = (suggestedLevel == 1) ? 3 : 1;
                    else if (currentLevel == 3) nextLevel = (suggestedLevel == 2) ? 4 : 2;
                    else if (currentLevel == 4) nextLevel = (suggestedLevel == 3) ? 5 : 3;
                    else nextLevel = suggestedLevel;
                }

                if (showLog)
                {
                    string action = isAgreeing ? "СОГЛАСЕН" : "ОТКАЗ";
                    Console.WriteLine($"Ход {move}: Уровень {currentLevel}. Первый предложил {suggestedLevel}. Второй {action} -> идет на {nextLevel}.");
                }

                currentLevel = nextLevel;

                // 3. Проверка выхода
                if (currentLevel == 5)
                {
                    if (showLog)
                    {
                        Console.WriteLine($"\nИгрок 2 достиг Уровня 5 на {move}-м ходу!");
                        Console.WriteLine("ПОБЕДИЛ ИГРОК 2 (Убегающий)!");
                    }
                    return true;
                }
            }

            if (showLog)
            {
                Console.WriteLine($"\nИгрок 2 сделал {maxRounds} ходов, но остался на Уровне {currentLevel}.");
                Console.WriteLine("ПОБЕДИЛ ИГРОК 1 (Удерживающий)!");
            }
            return false;
        }
    }
}