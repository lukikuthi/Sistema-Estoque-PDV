namespace SistemaEstoquePDV.Helpers
{
    public static class MenuHelper
    {
        public static void ExibirMensagem(string mensagem, ConsoleColor cor = ConsoleColor.White)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"\n{mensagem}");
            Console.ForegroundColor = ConsoleColor.White;
            PausarTela();
        }

        public static void PausarTela()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static bool ConfirmarAcao(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{mensagem} (S/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            
            var resposta = Console.ReadLine()?.ToUpper();
            return resposta == "S";
        }

        public static void ExibirCabecalho(string titulo)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"═══ {titulo.ToUpper()} ═══\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LimparLinha()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}