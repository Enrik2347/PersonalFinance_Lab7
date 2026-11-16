using System;
using System.Globalization;
using System.Text;

namespace PersonalFinance
{
    internal class Program
    {
        const int Capacity = 100;

        const double MonthlyIncome = 12000;
        const int DaysInMonth = 30;

        const double MaxExpense = 100000;
        const double MaxStep = 1000;
        const int MaxTableRows = 50;

        static readonly string[] Categories =
        {
            "Продукти", "Транспорт", "Житло", "Розваги",
            "Здоров'я", "Комуналка", "Одяг", "Інше"
        };

        static readonly string[] Months =
        {
            "Січ", "Лют", "Бер", "Кві", "Тра", "Чер",
            "Лип", "Сер", "Вер", "Жов", "Лис", "Гру"
        };

        static void Main()
        {
            ConfigureConsole();

            bool running = true;

            while (running)
            {
                ShowMenu();

                string choice = (Console.ReadLine() ?? string.Empty).Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": ShowBalanceTable(); Pause(); break;
                    case "3": ShowCategoryTable(); Pause(); break;
                    case "0": running = false; break;
                    default: RejectChoice(); break;
                }
            }

            Console.WriteLine("До побачення.");
        }

        static void ConfigureConsole()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Особисті фінанси ===");
            Console.WriteLine("1 — довідка про тему");
            Console.WriteLine("2 — баланс за місяць (табулювання)");
            Console.WriteLine("3 — витрати за категоріями та місяцями");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");

            if (Console.IsInputRedirected)
                Console.ReadLine();
            else
                Console.ReadKey(true);

            if (!Console.IsOutputRedirected)
                Console.Clear();
        }

        static void RejectChoice()
        {
            Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
            Pause();
        }

        static void ShowAbout()
        {
            Console.WriteLine("Тема проєкту: Особисті фінанси (варіант 22).");
            Console.WriteLine("Головна сутність: Операція.");
            Console.WriteLine();
            Console.WriteLine("Поля сутності «Операція»:");
            Console.WriteLine("  int      код операції (призначає програма)");
            Console.WriteLine("  DateTime дата операції");
            Console.WriteLine("  int      код категорії (посилання на довідник «Категорія»)");
            Console.WriteLine("  string   опис операції");
            Console.WriteLine("  double   сума, грн");
            Console.WriteLine("  string   тип: дохід або витрата");
            Console.WriteLine("  string   рахунок");
            Console.WriteLine();
            Console.WriteLine("Довідник «Категорія»: int код, string назва, bool чи активна.");
            Console.WriteLine("Підсумкова величина: баланс за місяць і витрати за категоріями.");
            Console.WriteLine($"Місткість сховища: {Capacity} записів.");
        }

        static void ShowBalanceTable()
        {
            Console.WriteLine("--- Баланс за місяць ---");
            Console.WriteLine($"Дохід: {MonthlyIncome:F2} грн, днів у місяці: {DaysInMonth}.");

            double from = ReadDouble($"Початок проміжку, грн (0–{MaxExpense:F0}): ", 0, MaxExpense);
            double to = ReadDouble($"Кінець проміжку, грн ({from:F2}–{MaxExpense:F0}): ", from, MaxExpense);
            double step = ReadDouble("Крок табулювання, грн (0,01–1000): ", 0.01, MaxStep);

            int rowCount = (int)Math.Floor((to - from) / step + 1e-9) + 1;
            if (rowCount > MaxTableRows)
            {
                Console.WriteLine($"Помилка: вийде {rowCount} рядків, а максимум — {MaxTableRows}.");
                Console.WriteLine("Зменште проміжок або збільште крок.");
                return;
            }

            PrintBalanceTable(from, to, step);
        }

        static void PrintBalanceTable(double from, double to, double step)
        {
            Console.WriteLine($"{"Витрата на день, грн",24}{"Баланс за місяць, грн",24}");

            for (double expense = from; expense <= to + 1e-9; expense += step)
            {
                double balance = CalculateMonthlyBalance(MonthlyIncome, DaysInMonth, expense);
                Console.WriteLine($"{expense,24:F2}{balance,24:F2}");
            }
        }

        static double CalculateMonthlyBalance(double income, int days, double dailyExpense)
        {
            return income - dailyExpense * days;
        }

        static void ShowCategoryTable()
        {
            Console.WriteLine("--- Витрати за категоріями та місяцями ---");
            Console.WriteLine($"Доступно категорій: {Categories.Length}, місяців: {Months.Length}.");

            int rows = ReadInt($"Скільки рядків (категорій), 1–{Categories.Length}: ", 1, Categories.Length);
            int cols = ReadInt($"Скільки стовпців (місяців), 1–{Months.Length}: ", 1, Months.Length);

            double[] columnTotals = new double[cols];
            double grandTotal = 0;

            Console.WriteLine();

            Console.Write($"{"Категорія",-14}");
            for (int col = 0; col < cols; col++)
                Console.Write($"{Months[col],10}");
            Console.WriteLine($"{"Разом",12}");

            for (int row = 0; row < rows; row++)
            {
                Console.Write($"{Categories[row],-14}");
                double rowTotal = 0;

                for (int col = 0; col < cols; col++)
                {
                    double amount = CalculateCellAmount(row, col);
                    rowTotal += amount;
                    columnTotals[col] += amount;
                    Console.Write($"{amount,10:F2}");
                }

                grandTotal += rowTotal;
                Console.WriteLine($"{rowTotal,12:F2}");
            }

            Console.Write($"{"РАЗОМ",-14}");
            for (int col = 0; col < cols; col++)
                Console.Write($"{columnTotals[col],10:F2}");
            Console.WriteLine($"{grandTotal,12:F2}");

            Console.WriteLine();
            Console.WriteLine($"Загальна сума витрат: {grandTotal:F2} грн.");
        }

        static double CalculateCellAmount(int row, int col)
        {
            double season = Math.Sin((col + 1) * Math.PI / 6) * 120;
            double variation = (row * 37 + col * 19) % 60;
            return 300 + row * 150 + season + variation;
        }

        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? string.Empty).Trim();

                double value;
                bool parsed = double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);

                if (!parsed)
                {
                    string normalized = text.Replace(',', '.');
                    parsed = double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                }

                if (!parsed)
                {
                    Console.WriteLine("Помилка: це не число. Спробуйте ще раз.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути від {min:F2} до {max:F2}.");
                    continue;
                }

                return value;
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? string.Empty).Trim();

                if (!int.TryParse(text, out int value))
                {
                    Console.WriteLine("Помилка: потрібне ціле число. Спробуйте ще раз.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути від {min} до {max}.");
                    continue;
                }

                return value;
            }
        }
    }
}
