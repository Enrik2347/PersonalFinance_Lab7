using System;
using System.Globalization;
using System.Text;

namespace PersonalFinance
{
    internal class Program
    {
        const int Capacity = 100;

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
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            Console.OutputEncoding = Encoding.UTF8;

            bool running = true;

            while (running)
            {
                ShowMenu();

                string choice = (Console.ReadLine() ?? string.Empty).Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        ShowBalanceTable();
                        Pause();
                        break;
                    case "3":
                        ShowCategoryTable();
                        Pause();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("До побачення.");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
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
            Console.ReadKey(true);
            Console.Clear();
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
            const double income = 12000;
            const int days = 30;
            const double from = 100.50;
            const double to = 550.50;
            const double step = 50.50;

            Console.WriteLine("--- Баланс за місяць ---");
            Console.WriteLine($"Дохід: {income:F2} грн, днів у місяці: {days}.");
            Console.WriteLine($"{"Витрата на день, грн",24}{"Баланс за місяць, грн",24}");

            for (double expense = from; expense <= to + 1e-9; expense += step)
            {
                double balance = income - expense * days;
                Console.WriteLine($"{expense,24:F2}{balance,24:F2}");
            }
        }

        static void ShowCategoryTable()
        {
            Console.WriteLine("--- Витрати за категоріями та місяцями ---");
            Console.WriteLine($"Доступно категорій: {Categories.Length}, місяців: {Months.Length}.");

            Console.Write($"Скільки рядків (категорій), 1–{Categories.Length}: ");
            if (!TryReadCount(Categories.Length, out int rows)) return;

            Console.Write($"Скільки стовпців (місяців), 1–{Months.Length}: ");
            if (!TryReadCount(Months.Length, out int cols)) return;

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
                    double amount = CellAmount(row, col);
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

        static double CellAmount(int row, int col)
        {
            double season = Math.Sin((col + 1) * Math.PI / 6) * 120;
            double variation = (row * 37 + col * 19) % 60;
            return 300 + row * 150 + season + variation;
        }

        static bool TryReadCount(int max, out int value)
        {
            string text = (Console.ReadLine() ?? string.Empty).Trim();

            if (!int.TryParse(text, out value) || value < 1 || value > max)
            {
                Console.WriteLine($"Помилка: потрібне ціле число від 1 до {max}. Таблицю не побудовано.");
                value = 0;
                return false;
            }

            return true;
        }
    }
}
