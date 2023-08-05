public static class ListOut {
    public static void Print(List<List<double>> list) {
        for (int i = 0; i < list.Count; i++) {
            Console.Write("new List<double> {");
            for (int j = 0; j < i; j++) {
                Console.Write($"{list[i][j]:F2},");
            }
            for (int k = i; k < list.Count; k++) {
                Console.Write($"{list[k][i]:F2},");
            }
            Console.WriteLine("},");
        }
    }
    public static void PrintSparse(List<List<double>> list) {
        for (int i = 0; i < list.Count; i++) {
            Console.Write($"new List<double> {{");
            //for (int j = 0; j <= i; j++) {
            //    Console.Write($"{list[i][j]:F2},");
            //}
            Console.Write(string.Join(",", list[i].Select(n => n.ToString("F2"))));
            Console.WriteLine("},");
        }
    }
    public static void PrintSparseInt(List<List<int>> list) {
        for (int i = 0; i < list.Count; i++) {
            Console.Write($"new List<int> {{");
            //for (int j = 0; j <= i; j++) {
            //    Console.Write($"{list[i][j]:F2},");
            //}
            Console.Write(string.Join(",", list[i]));
            Console.WriteLine("},");
        }
    }
    public static void PrintSparseInt3(List<List<List<int>>> list) {
        for (int i = 0; i < list.Count; i++) {
            Console.WriteLine($"new List<List<int>> {{");
            PrintSparseInt(list[i]);
            Console.WriteLine("},");
        }
    }
    public static void BitHandRankOut(ulong value) {
        Console.WriteLine(Convert.ToString((long)value, 2).PadLeft(64, '0').Insert(60, " ").Insert(56, " ").Insert(52, " ")
            .Insert(48, " ").Insert(44, " ").Insert(40, " ").Insert(36, " ").Insert(32, " ")
            .Insert(28, " ").Insert(24, " ").Insert(20, " ").Insert(16, " ").Insert(12, " ").Insert(8, " ").Insert(4, " "));
    }

}

