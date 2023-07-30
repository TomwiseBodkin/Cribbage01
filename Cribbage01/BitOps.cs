
using System.Numerics;

public static class BitOps {
    private const ulong Bits0To15 = 0x000000000000FFFF;
    private const ulong Bits16To31 = 0x00000000FFFF0000;
    private const ulong Bits32To47 = 0x0000FFFF00000000;
    private const ulong Bits48To63 = 0xFFFF000000000000;
    public static ulong FlattenOR16(ulong value) {
        value |= (value >> 32);
        value |= (value >> 16);
        return value & 0x000000000000FFFF;
    }
    public static ulong FlattenAND16(ulong value) {

        value &= value >> 32;

        // No need to clear any junk as we've been ANDing against 0s in the higher bits meaning they'll already be 0.
        return value & (value >> 16);
    }
    public static void BitSuit(ulong value) {
        // Bit indices: Spades = 0 -> 12, Hearts = 13 -> 25, Diamonds = 26 -> 38, Clubs = 39 -> 51.
        // Bit masks: Spades = 0 -> 12, Hearts = 16 -> 28, Diamonds = 32 -> 44, Clubs = 48 -> 60.
        var spades = value & Bits0To15;
        var hearts = (value & Bits16To31) >> 3;
        var diamonds = (value & Bits32To47) >> 6;
        var clubs = (value & Bits48To63) >> 9;

        Console.WriteLine($"Popcount C => {BitOperations.PopCount(clubs)}");
        Console.WriteLine($"Popcount D => {BitOperations.PopCount(diamonds)}");
        Console.WriteLine($"Popcount H => {BitOperations.PopCount(hearts)}");
        Console.WriteLine($"Popcount S => {BitOperations.PopCount(spades)}");
    }
    public static bool IsSingleSuit(ulong handMask) =>
        // Performs better using | rather than || as it avoids branching, branch prediction, etc.
        (handMask & Bits0To15) == handMask |
        (handMask & Bits16To31) == handMask |
        (handMask & Bits32To47) == handMask |
        (handMask & Bits48To63) == handMask;



}

