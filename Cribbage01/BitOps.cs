
using System.Numerics;

public static class BitOps {
    private const ulong Bits0To15 = 0x000000000000FFFF;
    private const ulong Bits16To31 = 0x00000000FFFF0000;
    private const ulong Bits32To47 = 0x0000FFFF00000000;
    private const ulong Bits48To63 = 0xFFFF000000000000;



    public static ulong FlattenOR16(ulong value) {
        value |= (value >> 32);
        value |= (value >> 16);
        return value & Bits0To15;
    }
    public static ulong FlattenXOR16(ulong value) {
        value ^= value >> 32;
        value ^= value >> 16;

        return value & Bits0To15;
    }
    public static ulong FlattenXOROR16(ulong value) {
        value ^= value >> 32;
        value |= value >> 16;

        return value & Bits0To15;
    }
    public static ulong FlattenAND16(ulong value) {

        value &= value >> 32;

        // No need to bitmask as we've been ANDing against 0s in the higher bits meaning they'll already be 0.
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
        (handMask > 1) &
        (handMask & Bits0To15) == handMask |
        (handMask & Bits16To31) == handMask |
        (handMask & Bits32To47) == handMask |
        (handMask & Bits48To63) == handMask;

    public static int CountSetBits(ulong value) {
        int count = 0;
        while (value > 0) {
            value &= (value - 1);
            count++;
        }
        return count;
    }
    public static int ConsecutiveSetBits(ulong value) {

        int count = 0;

        // Count the number of iterations
        // to reach value = 0.
        while (value != 0) {

            // Reduce length of every sequence of 1s by one.
            value = (value & (value << 1));

            count++;
        }

        return count;
    }
    public static ulong SuitToRankOrder(ulong value) {
        var spades = SpacedOut(value & Bits0To15);
        var hearts = SpacedOut((value & Bits16To31) >> 16);
        var diamonds = SpacedOut((value & Bits32To47) >> 32);
        var clubs = SpacedOut((value & Bits48To63) >> 48);

        return spades | (hearts << 1) | (diamonds << 2) | (clubs << 3);
    }

    private static ulong SpacedOut(ulong value) {
        ulong x = value &     0x000000000000FFFFL;
        x = (x | (x << 24)) & 0x000000FF000000FFL;
        x = (x | (x << 12)) & 0x000F000F000F000FL;
        x = (x | (x << 6)) &  0x0303030303030303L;
        x = (x | (x << 3)) &  0x1111111111111111L;
        return x;
    }

}

