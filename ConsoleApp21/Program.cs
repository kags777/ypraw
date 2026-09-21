using System;

namespace LevelEscapeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            int totalRuns = 10000; // Количество симуляций для статистики
            int player1Wins = 0;
            int player2Wins = 0;

            Console.WriteLine("=== СИМУЛЯЦИЯ ИГРЫ (5 УРОВНЕЙ) ===");
            Console.WriteLine($"Запуск {totalRuns} игр...\n");

            // Запускаем множество игр для сбора статистики
            for (int i = 0; i < totalRuns; i++)
            {
                bool player2Escaped = SimulateGame(showLog: false);
                if (player2Escaped)
                    player2Wins++;
                else
                    player1Wins++;
            }

            // Вывод статистики
            Console.WriteLine("=== РЕЗУЛЬТАТЫ СИМУЛЯЦИИ ===");
            Console.WriteLine($"Игрок 1 (Удерживающий) победил: {player1Wins} раз ({((double)player1Wins / totalRuns) * 100:F2}%)");
            Console.WriteLine($"Игрок 2 (Убегающий) победил:   {player2Wins} раз ({((double)player2Wins / totalRuns) * 100:F2}%)");

            // Показываем один пример игры с логом
            Console.WriteLine("\n=== ПРИМЕР ОДНОЙ ИГРЫ ===");
            SimulateGame(showLog: true);

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Симулирует одну игру.
        /// </summary>
        /// <param name="showLog">Если true, выводит ход игры на экран.</param>
        /// <returns>True, если Игрок 2 сбежал, иначе False.</returns>
        static bool SimulateGame(bool showLog)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int currentLevel = 0;
            int maxMoves = 25;

            // Алгоритм Второго игрока: True = Согласен, False = Отказ
            // 5+, 6-, 2+, 4-, 4+, 1-, 1+, 2-
            bool[] player2Script = new bool[25];
            int idx = 0;
            for (int i = 0; i < 5; i++) player2Script[idx++] = true;   // 5 раз +
            for (int i = 0; i < 6; i++) player2Script[idx++] = false;  // 6 раз -
            for (int i = 0; i < 2; i++) player2Script[idx++] = true;   // 2 раза +
            for (int i = 0; i < 4; i++) player2Script[idx++] = false;  // 4 раза -
            for (int i = 0; i < 4; i++) player2Script[idx++] = true;   // 4 раза +
            for (int i = 0; i < 1; i++) player2Script[idx++] = false;  // 1 раз -
            for (int i = 0; i < 1; i++) player2Script[idx++] = true;   // 1 раз +
            for (int i = 0; i < 2; i++) player2Script[idx++] = false;  // 2 раза -

            if (showLog)
            {
                Console.WriteLine("Старт с Уровня 0.");
            }

            for (int move = 1; move <= maxMoves; move++)
            {
                // --- ШАГ 1: Игрок 1 предлагает ход ---
                int suggestedLevel = currentLevel;

                // Логика предложения Первого игрока (на основе правил уровней)
                switch (currentLevel)
                {
                    case 0:
                        suggestedLevel = 1; // 100% вперед
                        break;
                    case 1:
                        // Первый предлагает случайно: либо остаться на 1, либо идти на 2
                        suggestedLevel = (rnd.Next(2) == 0) ? 1 : 2;
                        break;
                    case 2:
                        // Предлагает: либо вернуться на 1, либо идти на 3
                        suggestedLevel = (rnd.Next(2) == 0) ? 1 : 3;
                        break;
                    case 3:
                        // Предлагает: либо вернуться на 2, либо идти на 4
                        suggestedLevel = (rnd.Next(2) == 0) ? 2 : 4;
                        break;
                    case 4:
                        // Предлагает: либо вернуться на 3, либо идти на 5
                        suggestedLevel = (rnd.Next(2) == 0) ? 3 : 5;
                        break;
                    case 5:
                        suggestedLevel = 5; // Уже на выходе
                        break;
                }

                // --- ШАГ 2: Игрок 2 решает, соглашаться или нет ---
                bool isAgreeing = player2Script[move - 1];
                int nextLevel = suggestedLevel;

                if (!isAgreeing)
                {
                    // Если отказ, Второй игрок выбирает ДРУГОЙ путь (противоположный предложению)
                    // Если Первый предложил вперед, Второй идет назад (если возможно)
                    // Если Первый предложил назад, Второй идет вперед (если возможно)

                    if (currentLevel == 1)
                        nextLevel = (suggestedLevel == 1) ? 2 : 1; // Если предложили остаться, идем вперед. Если вперед, остаемся.
                    else if (currentLevel == 2)
                        nextLevel = (suggestedLevel == 1) ? 3 : 1; // Если предложили назад, идем вперед. Если вперед, идем назад.
                    else if (currentLevel == 3)
                        nextLevel = (suggestedLevel == 2) ? 4 : 2;
                    else if (currentLevel == 4)
                        nextLevel = (suggestedLevel == 3) ? 5 : 3;
                    else
                        nextLevel = suggestedLevel; // На 0 и 5 уровне выбора нет
                }

                if (showLog)
                {
                    string action = isAgreeing ? "СОГЛАСЕН" : "ОТКАЗ";
                    Console.WriteLine($"Ход {move}: Уровень {currentLevel}. Первый предложил {suggestedLevel}. Второй {action} -> идет на {nextLevel}.");
                }

                currentLevel = nextLevel;

                // --- ШАГ 3: Проверка на победу Игрока 2 ---
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

            // Если цикл завершился, а мы не на 5 уровне
            if (showLog)
            {
                Console.WriteLine($"\nИгрок 2 сделал 25 ходов, но остался на Уровне {currentLevel}.");
                Console.WriteLine("ПОБЕДИЛ ИГРОК 1 (Удерживающий)!");
            }
            return false;
        }
    }
}