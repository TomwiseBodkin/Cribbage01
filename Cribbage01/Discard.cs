﻿using System;

public static class Discard {
    public static List<Card> ToCrib(Player player, int numHand) {
        List<Card> cribCards = new List<Card>();
        Random rand = new Random();
        IDictionary<ScoreType, int> toCribScore = new Dictionary<ScoreType, int>();
        IDictionary<List<Card>, double> potentialCribs = new Dictionary<List<Card>, double>();

        // lookup tables for statistical data on discards, calculated from averages for all possible card combinations
        // Crib decimal averages for 5-card cribbage when hand discards A-K:
        
        // List<double> discard5o = new List<double> { 4.43, 4.67, 4.68, 4.71, 6.75, 4.68, 4.58, 4.45, 4.42, 4.42, 4.75, 4.27, 4.09 };
        List<double> discard5 = new List<double> { 4.39, 4.61, 4.77, 4.78, 6.59, 4.73, 4.62, 4.58, 4.47, 4.40, 4.42, 4.22, 4.03 };
        // Crib decimal averages when pone discards AA-KK:
        List<List<double>> listFull6 = new List<List<double>> {
            new List<double> { 5.49 },
            new List<double> { 4.41, 5.79 },
            new List<double> { 4.53, 6.81, 6.12 },
            new List<double> { 5.44, 4.81, 5.47, 6.10 },
            new List<double> { 5.70, 5.73, 6.39, 6.96, 8.96 },
            new List<double> { 4.23, 4.33, 4.24, 4.93, 7.06, 6.25 },
            new List<double> { 4.05, 4.25, 4.32, 4.15, 6.38, 5.51, 6.07 },
            new List<double> { 4.09, 4.20, 4.26, 4.27, 5.72, 4.87, 6.73, 5.60 },
            new List<double> { 4.01, 4.11, 4.08, 4.17, 5.70, 5.54, 4.32, 4.90, 5.49 },
            new List<double> { 3.93, 4.04, 4.11, 4.11, 6.99, 3.81, 3.69, 4.27, 4.81, 5.43 },
            new List<double> { 3.93, 4.04, 4.12, 4.12, 7.00, 3.81, 3.76, 3.68, 4.23, 4.79, 5.43 },
            new List<double> { 3.82, 3.94, 4.01, 4.01, 6.89, 3.70, 3.65, 3.64, 3.52, 4.08, 4.75, 5.21 },
            new List<double> { 3.71, 3.83, 3.90, 3.90, 6.78, 3.60, 3.54, 3.53, 3.47, 3.38, 4.05, 3.94, 4.99 }
        };

        // Tables for AAA-KKK discards...!
        List<List<List<double>>> listFull9 = new List<List<List<double>>> {
            new List<List<double>> {// A♠
            new List<double> {19.90,18.16,20.29,20.37,18.28,16.02,15.65,15.29,14.94,14.61,14.63,14.43,14.24},
            new List<double> {18.16,20.10,22.64,17.76,17.04,14.87,14.50,14.14,13.83,13.49,13.53,13.31,13.12},
            new List<double> {20.29,22.64,18.45,18.58,17.87,14.81,14.53,14.18,13.87,13.54,13.58,13.36,13.17},
            new List<double> {20.37,17.76,18.58,20.28,19.51,16.46,15.29,15.03,14.71,15.85,15.87,15.68,15.48},
            new List<double> {18.28,17.04,17.87,19.51,19.86,17.17,16.01,14.87,16.08,15.30,15.32,15.12,14.93},
            new List<double> {16.02,14.87,14.81,16.46,17.17,15.46,14.63,14.95,13.36,11.25,11.29,11.07,10.88},
            new List<double> {15.65,14.50,14.53,15.29,16.01,14.63,16.22,14.95,11.96,10.98,11.08,10.88,10.69},
            new List<double> {15.29,14.14,14.18,15.03,14.87,14.95,14.95,13.27,12.59,11.61,10.82,10.70,10.51},
            new List<double> {14.94,13.83,13.87,14.71,16.08,13.36,11.96,12.59,12.91,12.25,11.45,10.45,10.33},
            new List<double> {14.61,13.49,13.54,15.85,15.30,11.25,10.98,11.61,12.25,12.58,12.10,11.09,10.08},
            new List<double> {14.63,13.53,13.58,15.87,15.32,11.29,11.08,10.82,11.45,12.10,12.58,12.02,11.01},
            new List<double> {14.43,13.31,13.36,15.68,15.12,11.07,10.88,10.70,10.45,11.09,12.02,12.19,10.82},
            new List<double> {14.24,13.12,13.17,15.48,14.93,10.88,10.69,10.51,10.33,10.08,11.01,10.82,11.81}
            },
            new List<List<double>> {// 2♠
            new List<double> {18.16,20.10,22.61,17.76,17.04,14.87,14.50,14.14,13.81,13.49,13.50,13.31,13.12},
            new List<double> {20.10,20.43,24.17,18.31,18.33,16.08,15.69,15.36,15.03,14.72,14.76,14.55,14.35},
            new List<double> {22.61,24.17,24.05,22.90,20.34,17.61,17.31,16.97,16.67,17.79,17.83,17.61,17.42},
            new List<double> {17.76,18.31,22.90,18.12,18.11,15.38,14.20,13.94,15.08,12.84,12.86,12.67,12.48},
            new List<double> {17.04,18.33,20.34,18.11,19.81,17.06,15.88,16.19,14.03,15.25,15.26,15.07,14.88},
            new List<double> {14.87,16.08,17.61,15.38,17.06,15.43,16.04,12.97,13.39,11.29,11.32,11.11,10.91},
            new List<double> {14.50,15.69,17.31,14.20,15.88,16.04,14.27,14.98,12.00,11.02,11.11,10.92,10.72},
            new List<double> {14.14,15.36,16.97,13.94,16.19,12.97,14.98,13.30,12.63,11.65,10.86,10.74,10.54},
            new List<double> {13.81,15.03,16.67,15.08,14.03,13.39,12.00,12.63,12.95,12.29,11.50,10.49,10.37},
            new List<double> {13.49,14.72,17.79,12.84,15.25,11.29,11.02,11.65,12.29,12.62,12.15,11.14,10.13},
            new List<double> {13.50,14.76,17.83,12.86,15.26,11.32,11.11,10.86,11.50,12.15,12.63,12.06,11.06},
            new List<double> {13.31,14.55,17.61,12.67,15.07,11.11,10.92,10.74,10.49,11.14,12.06,12.24,10.86},
            new List<double> {13.12,14.35,17.42,12.48,14.88,10.91,10.72,10.54,10.37,10.13,11.06,10.86,11.85}
            },
            new List<List<double>> {// 3♥
            new List<double> {20.29,22.61,18.43,18.58,17.87,14.81,14.53,14.18,13.85,13.54,13.55,13.36,13.17},
            new List<double> {22.61,24.14,24.03,22.90,20.34,17.61,17.31,16.97,16.64,17.79,17.81,17.61,17.42},
            new List<double> {18.43,24.03,20.61,19.94,19.98,16.11,15.78,15.43,16.54,14.33,14.35,14.16,13.96},
            new List<double> {18.58,22.90,19.94,19.64,22.26,15.84,14.98,16.16,13.91,13.68,13.70,13.51,13.31},
            new List<double> {17.87,20.34,19.98,22.26,21.37,17.52,18.12,15.03,14.87,16.09,16.11,15.91,15.72},
            new List<double> {14.81,17.61,16.11,15.84,17.52,16.82,14.01,12.92,13.32,11.24,11.26,11.06,10.87},
            new List<double> {14.53,17.31,15.78,14.98,18.12,14.01,14.32,15.01,12.03,11.06,11.15,10.96,10.76},
            new List<double> {14.18,16.97,15.43,16.16,15.03,12.92,15.01,13.35,12.67,11.71,10.90,10.80,10.61},
            new List<double> {13.85,16.64,16.54,13.91,14.87,13.32,12.03,12.67,12.99,12.34,11.54,10.53,10.42},
            new List<double> {13.54,17.79,14.33,13.68,16.09,11.24,11.06,11.71,12.34,12.69,12.19,11.21,10.20},
            new List<double> {13.55,17.81,14.35,13.70,16.11,11.26,11.15,10.90,11.54,12.19,12.68,12.13,11.12},
            new List<double> {13.36,17.61,14.16,13.51,15.91,11.06,10.96,10.80,10.53,11.21,12.13,12.31,10.93},
            new List<double> {13.17,17.42,13.96,13.31,15.72,10.87,10.76,10.61,10.42,10.20,11.12,10.93,11.90}
            },
            new List<List<double>> {// 4♣
            new List<double> {20.37,17.76,18.58,20.28,19.51,16.46,15.29,15.03,14.71,15.85,15.87,15.68,15.48},
            new List<double> {17.76,18.31,22.90,18.12,18.11,15.38,14.20,13.94,15.08,12.84,12.86,12.67,12.48},
            new List<double> {18.58,22.90,19.94,19.64,22.26,15.84,14.98,16.16,13.91,13.68,13.70,13.51,13.31},
            new List<double> {20.28,18.12,19.64,19.87,21.08,17.26,16.72,14.48,14.22,13.99,14.01,13.82,13.62},
            new List<double> {19.51,18.11,22.26,21.08,22.73,22.94,16.46,15.68,15.50,16.72,16.74,16.55,16.35},
            new List<double> {16.46,15.38,15.84,17.26,22.94,16.29,14.35,13.58,13.96,11.88,11.89,11.70,11.51},
            new List<double> {15.29,14.20,14.98,16.72,16.46,14.35,14.15,14.78,11.78,10.81,10.90,10.71,10.51},
            new List<double> {15.03,13.94,16.16,14.48,15.68,13.58,14.78,13.17,12.50,11.53,10.73,10.61,10.42},
            new List<double> {14.71,15.08,13.91,14.22,15.50,13.96,11.78,12.50,12.83,12.17,11.38,10.37,10.25},
            new List<double> {15.85,12.84,13.68,13.99,16.72,11.88,10.81,11.53,12.17,12.51,12.03,11.03,10.02},
            new List<double> {15.87,12.86,13.70,14.01,16.74,11.89,10.90,10.73,11.38,12.03,12.52,11.95,10.94},
            new List<double> {15.68,12.67,13.51,13.82,16.55,11.70,10.71,10.61,10.37,11.03,11.95,12.13,10.75},
            new List<double> {15.48,12.48,13.31,13.62,16.35,11.51,10.51,10.42,10.25,10.02,10.94,10.75,11.74}
            },
            new List<List<double>> {// 5♣
            new List<double> {18.28,17.04,17.87,19.51,19.86,17.17,16.01,14.87,16.08,15.30,15.32,15.12,14.93},
            new List<double> {17.04,18.33,20.34,18.11,19.81,17.06,15.88,16.19,14.03,15.25,15.26,15.07,14.88},
            new List<double> {17.87,20.34,19.98,22.26,21.37,17.52,18.12,15.03,14.87,16.09,16.11,15.91,15.72},
            new List<double> {19.51,18.11,22.26,21.08,22.73,22.94,16.46,15.68,15.50,16.72,16.74,16.55,16.35},
            new List<double> {19.86,19.81,21.37,22.73,25.91,21.76,19.88,18.01,17.75,20.41,20.42,20.23,20.04},
            new List<double> {17.17,17.06,17.52,22.94,21.76,19.69,20.34,15.88,16.57,15.92,15.94,15.75,15.55},
            new List<double> {16.01,15.88,18.12,16.46,19.88,20.34,17.56,17.08,14.40,14.86,14.95,14.76,14.56},
            new List<double> {14.87,16.19,15.03,15.68,18.01,15.88,17.08,14.98,14.23,14.69,13.89,13.77,13.58},
            new List<double> {16.08,14.03,14.87,15.50,17.75,16.57,14.40,14.23,14.63,15.42,14.62,13.61,13.50},
            new List<double> {15.30,15.25,16.09,16.72,20.41,15.92,14.86,14.69,15.42,17.20,16.73,15.72,14.71},
            new List<double> {15.32,15.26,16.11,16.74,20.42,15.94,14.95,13.89,14.62,16.73,17.21,16.65,15.64},
            new List<double> {15.12,15.07,15.91,16.55,20.23,15.75,14.76,13.77,13.61,15.72,16.65,16.82,15.45},
            new List<double> {14.93,14.88,15.72,16.35,20.04,15.55,14.56,13.58,13.50,14.71,15.64,15.45,16.43}
            },
            new List<List<double>> {// 6♦
            new List<double> {16.02,14.87,14.81,16.46,17.17,15.46,14.63,14.95,13.34,11.25,11.27,11.07,10.88},
            new List<double> {14.87,16.08,17.61,15.38,17.06,15.43,16.04,12.97,13.37,11.29,11.30,11.11,10.91},
            new List<double> {14.81,17.61,16.11,15.84,17.52,16.82,14.01,12.92,13.32,11.24,11.26,11.06,10.87},
            new List<double> {16.46,15.38,15.84,17.26,22.94,16.29,14.35,13.58,13.96,11.88,11.89,11.70,11.51},
            new List<double> {17.17,17.06,17.52,22.94,21.76,19.69,20.34,15.88,16.57,15.92,15.94,15.75,15.55},
            new List<double> {15.46,15.43,16.82,16.29,19.69,18.07,17.33,15.47,16.51,12.41,12.41,12.21,12.02},
            new List<double> {14.63,16.04,14.01,14.35,20.34,17.33,17.09,19.22,14.29,11.67,11.74,11.55,11.35},
            new List<double> {14.95,12.97,12.92,13.58,15.88,15.47,19.22,14.51,14.12,11.51,10.69,10.57,10.37},
            new List<double> {13.34,13.37,13.32,13.96,16.57,16.51,14.29,14.12,15.46,12.80,11.98,10.97,10.86},
            new List<double> {11.25,11.29,11.24,11.88,15.92,12.41,11.67,11.51,12.80,11.28,10.79,9.78,8.77},
            new List<double> {11.27,11.30,11.26,11.89,15.94,12.41,11.74,10.69,11.98,10.79,11.27,10.70,9.69},
            new List<double> {11.07,11.11,11.06,11.70,15.75,12.21,11.55,10.57,10.97,9.78,10.70,10.88,9.50},
            new List<double> {10.88,10.91,10.87,11.51,15.55,12.02,11.35,10.37,10.86,8.77,9.69,9.50,10.49}
            },
            new List<List<double>> {// 7♠
            new List<double> {15.65,14.50,14.53,15.29,16.01,14.63,16.22,14.95,11.96,10.98,11.08,10.88,10.69},
            new List<double> {14.50,15.69,17.33,14.20,15.88,16.04,14.27,14.98,12.02,11.02,11.14,10.92,10.72},
            new List<double> {14.53,17.33,15.80,14.98,18.12,14.01,14.32,15.01,12.05,11.06,11.18,10.96,10.76},
            new List<double> {15.29,14.20,14.98,16.72,16.46,14.35,14.15,14.78,11.78,10.81,10.90,10.71,10.51},
            new List<double> {16.01,15.88,18.12,16.46,19.88,20.34,17.56,17.08,14.40,14.86,14.95,14.76,14.56},
            new List<double> {14.63,16.04,14.01,14.35,20.34,17.33,17.09,19.22,14.31,11.67,11.77,11.55,11.35},
            new List<double> {16.22,14.27,14.32,14.15,17.56,17.09,17.33,19.51,13.80,12.09,12.08,11.87,11.68},
            new List<double> {14.95,14.98,15.01,14.78,17.08,19.22,19.51,18.80,17.63,13.32,12.81,12.67,12.47},
            new List<double> {11.96,12.02,12.05,11.78,14.40,14.31,13.80,17.63,13.00,11.23,10.72,9.69,9.57},
            new List<double> {10.98,11.02,11.06,10.81,14.86,11.67,12.09,13.32,11.23,11.20,10.64,9.60,8.59},
            new List<double> {11.08,11.14,11.18,10.90,14.95,11.77,12.08,12.81,10.72,10.64,11.18,10.61,9.60},
            new List<double> {10.88,10.92,10.96,10.71,14.76,11.55,11.87,12.67,9.69,9.60,10.61,10.78,9.40},
            new List<double> {10.69,10.72,10.76,10.51,14.56,11.35,11.68,12.47,9.57,8.59,9.60,9.40,10.39}
            },
            new List<List<double>> {// 8♣
            new List<double> {15.29,14.14,14.18,15.03,14.87,14.95,14.95,13.27,12.59,11.61,10.82,10.70,10.51},
            new List<double> {14.14,15.36,16.97,13.94,16.19,12.97,14.98,13.30,12.63,11.65,10.86,10.74,10.54},
            new List<double> {14.18,16.97,15.43,16.16,15.03,12.92,15.01,13.33,12.67,11.69,10.90,10.78,10.59},
            new List<double> {15.03,13.94,16.16,14.48,15.68,13.58,14.78,13.17,12.50,11.53,10.73,10.61,10.42},
            new List<double> {14.87,16.19,15.03,15.68,18.01,15.88,17.08,14.98,14.23,14.69,13.89,13.77,13.58},
            new List<double> {14.95,12.97,12.92,13.58,15.88,15.47,19.22,14.51,14.12,11.51,10.69,10.57,10.37},
            new List<double> {14.95,14.98,15.01,14.78,17.08,19.22,19.51,18.80,17.63,13.32,12.81,12.67,12.47},
            new List<double> {13.27,13.30,13.33,13.17,14.98,14.51,18.80,15.24,14.63,12.91,11.30,11.09,10.88},
            new List<double> {12.59,12.63,12.67,12.50,14.23,14.12,17.63,14.63,14.54,15.36,11.16,10.44,10.29},
            new List<double> {11.61,11.65,11.69,11.53,14.69,11.51,13.32,12.91,15.36,12.74,11.07,10.35,9.32},
            new List<double> {10.82,10.86,10.90,10.73,13.89,10.69,12.81,11.30,11.16,11.07,11.11,10.46,9.43},
            new List<double> {10.70,10.74,10.78,10.61,13.77,10.57,12.67,11.09,10.44,10.35,10.46,10.70,9.31},
            new List<double> {10.51,10.54,10.59,10.42,13.58,10.37,12.47,10.88,10.29,9.32,9.43,9.31,10.30}
            },
            new List<List<double>> {// 9♦
            new List<double> {14.94,13.81,13.85,14.71,16.08,13.34,11.96,12.59,12.91,12.25,11.45,10.45,10.33},
            new List<double> {13.81,15.00,16.64,15.08,14.03,13.37,12.00,12.63,12.95,12.29,11.50,10.49,10.37},
            new List<double> {13.85,16.64,16.54,13.91,14.87,13.32,12.03,12.67,12.99,12.34,11.54,10.53,10.42},
            new List<double> {14.71,15.08,13.91,14.22,15.50,13.96,11.78,12.50,12.83,12.17,11.38,10.37,10.25},
            new List<double> {16.08,14.03,14.87,15.50,17.75,16.57,14.40,14.23,14.63,15.42,14.62,13.61,13.50},
            new List<double> {13.34,13.37,13.32,13.96,16.57,16.51,14.29,14.12,15.46,12.80,11.98,10.97,10.86},
            new List<double> {11.96,12.00,12.03,11.78,14.40,14.29,13.80,17.63,13.00,11.23,10.72,9.69,9.57},
            new List<double> {12.59,12.63,12.67,12.50,14.23,14.12,17.63,14.63,14.54,15.36,11.16,10.44,10.29},
            new List<double> {12.91,12.95,12.99,12.83,14.63,15.46,13.00,14.54,14.96,14.36,12.75,10.93,10.71},
            new List<double> {12.25,12.29,12.34,12.17,15.42,12.80,11.23,15.36,14.36,14.28,15.20,10.78,10.06},
            new List<double> {11.45,11.50,11.54,11.38,14.62,11.98,10.72,11.16,12.75,15.20,12.65,10.90,10.18},
            new List<double> {10.45,10.49,10.53,10.37,13.61,10.97,9.69,10.44,10.93,10.78,10.90,10.64,9.17},
            new List<double> {10.33,10.37,10.42,10.25,13.50,10.86,9.57,10.29,10.71,10.06,10.18,9.17,10.22}
            },
            new List<List<double>> {// T♦
            new List<double> {14.61,13.49,13.54,15.85,15.30,11.25,10.98,11.61,12.25,12.58,12.10,11.09,10.08},
            new List<double> {13.49,14.72,17.79,12.84,15.25,11.29,11.02,11.65,12.29,12.62,12.15,11.14,10.13},
            new List<double> {13.54,17.79,14.33,13.68,16.09,11.24,11.06,11.69,12.34,12.67,12.19,11.19,10.18},
            new List<double> {15.85,12.84,13.68,13.99,16.72,11.88,10.81,11.53,12.17,12.51,12.03,11.03,10.02},
            new List<double> {15.30,15.25,16.09,16.72,20.41,15.92,14.86,14.69,15.42,17.20,16.73,15.72,14.71},
            new List<double> {11.25,11.29,11.24,11.88,15.92,12.41,11.67,11.51,12.80,11.28,10.79,9.78,8.77},
            new List<double> {10.98,11.02,11.06,10.81,14.86,11.67,12.09,13.32,11.23,11.20,10.64,9.60,8.59},
            new List<double> {11.61,11.65,11.69,11.53,14.69,11.51,13.32,12.91,15.36,12.74,11.07,10.35,9.32},
            new List<double> {12.25,12.29,12.34,12.17,15.42,12.80,11.23,15.36,14.36,14.28,15.20,10.78,10.06},
            new List<double> {12.58,12.62,12.67,12.51,17.20,11.28,11.20,12.74,14.28,14.70,14.19,12.37,10.55},
            new List<double> {12.10,12.15,12.19,12.03,16.73,10.79,10.64,11.07,15.20,14.19,14.19,15.01,10.60},
            new List<double> {11.09,11.14,11.19,11.03,15.72,9.78,9.60,10.35,10.78,12.37,15.01,12.18,9.59},
            new List<double> {10.08,10.13,10.18,10.02,14.71,8.77,8.59,9.32,10.06,10.55,10.60,9.59,10.16}
            },
            new List<List<double>> {// J♦
            new List<double> {14.63,13.50,13.55,15.87,15.32,11.27,11.08,10.82,11.45,12.10,12.58,12.02,11.01},
            new List<double> {13.50,14.74,17.81,12.86,15.26,11.30,11.11,10.86,11.50,12.15,12.63,12.06,11.06},
            new List<double> {13.55,17.81,14.35,13.70,16.11,11.26,11.15,10.90,11.54,12.19,12.68,12.11,11.11},
            new List<double> {15.87,12.86,13.70,14.01,16.74,11.89,10.90,10.73,11.38,12.03,12.52,11.95,10.94},
            new List<double> {15.32,15.26,16.11,16.74,20.42,15.94,14.95,13.89,14.62,16.73,17.21,16.65,15.64},
            new List<double> {11.27,11.30,11.26,11.89,15.94,12.41,11.74,10.69,11.98,10.79,11.27,10.70,9.69},
            new List<double> {11.08,11.11,11.15,10.90,14.95,11.74,12.08,12.81,10.72,10.64,11.18,10.61,9.60},
            new List<double> {10.82,10.86,10.90,10.73,13.89,10.69,12.81,11.30,11.16,11.07,11.11,10.46,9.43},
            new List<double> {11.45,11.50,11.54,11.38,14.62,11.98,10.72,11.16,12.75,15.20,12.65,10.90,10.18},
            new List<double> {12.10,12.15,12.19,12.03,16.73,10.79,10.64,11.07,15.20,14.19,14.19,15.01,10.60},
            new List<double> {12.58,12.63,12.68,12.52,17.21,11.27,11.18,11.11,12.65,14.19,14.69,14.01,12.19},
            new List<double> {12.02,12.06,12.11,11.95,16.65,10.70,10.61,10.46,10.90,15.01,14.01,14.01,14.32},
            new List<double> {11.01,11.06,11.11,10.94,15.64,9.69,9.60,9.43,10.18,10.60,12.19,14.32,11.99}
            },
            new List<List<double>> {// Q♥
            new List<double> {14.43,13.31,13.36,15.68,15.12,11.07,10.88,10.70,10.45,11.09,12.02,12.19,10.82},
            new List<double> {13.31,14.55,17.61,12.67,15.07,11.11,10.92,10.74,10.49,11.14,12.06,12.24,10.86},
            new List<double> {13.36,17.61,14.16,13.51,15.91,11.06,10.96,10.78,10.53,11.19,12.11,12.29,10.91},
            new List<double> {15.68,12.67,13.51,13.82,16.55,11.70,10.71,10.61,10.37,11.03,11.95,12.13,10.75},
            new List<double> {15.12,15.07,15.91,16.55,20.23,15.75,14.76,13.77,13.61,15.72,16.65,16.82,15.45},
            new List<double> {11.07,11.11,11.06,11.70,15.75,12.21,11.55,10.57,10.97,9.78,10.70,10.88,9.50},
            new List<double> {10.88,10.92,10.96,10.71,14.76,11.55,11.87,12.67,9.69,9.60,10.61,10.78,9.40},
            new List<double> {10.70,10.74,10.78,10.61,13.77,10.57,12.67,11.11,10.44,10.37,10.46,10.72,9.33},
            new List<double> {10.45,10.49,10.53,10.37,13.61,10.97,9.69,10.44,10.93,10.78,10.90,10.64,9.17},
            new List<double> {11.09,11.14,11.19,11.03,15.72,9.78,9.60,10.37,10.78,12.39,15.01,12.20,9.61},
            new List<double> {12.02,12.06,12.11,11.95,16.65,10.70,10.61,10.46,10.90,15.01,14.01,14.03,14.34},
            new List<double> {12.19,12.24,12.29,12.13,16.82,10.88,10.78,10.72,10.64,12.20,14.03,14.12,11.82},
            new List<double> {10.82,10.86,10.91,10.75,15.45,9.50,9.40,9.33,9.17,9.61,14.34,11.82,11.80}
            },
            new List<List<double>> {// K♣
            new List<double> {14.24,13.12,13.17,15.48,14.93,10.88,10.69,10.51,10.33,10.08,11.01,10.82,11.81},
            new List<double> {13.12,14.35,17.42,12.48,14.88,10.91,10.72,10.54,10.37,10.13,11.06,10.86,11.85},
            new List<double> {13.17,17.42,13.96,13.31,15.72,10.87,10.76,10.59,10.42,10.18,11.11,10.91,11.90},
            new List<double> {15.48,12.48,13.31,13.62,16.35,11.51,10.51,10.42,10.25,10.02,10.94,10.75,11.74},
            new List<double> {14.93,14.88,15.72,16.35,20.04,15.55,14.56,13.58,13.50,14.71,15.64,15.45,16.43},
            new List<double> {10.88,10.91,10.87,11.51,15.55,12.02,11.35,10.37,10.86,8.77,9.69,9.50,10.49},
            new List<double> {10.69,10.72,10.76,10.51,14.56,11.35,11.68,12.47,9.57,8.59,9.60,9.40,10.39},
            new List<double> {10.51,10.54,10.59,10.42,13.58,10.37,12.47,10.88,10.29,9.32,9.43,9.31,10.30},
            new List<double> {10.33,10.37,10.42,10.25,13.50,10.86,9.57,10.29,10.71,10.06,10.18,9.17,10.22},
            new List<double> {10.08,10.13,10.18,10.02,14.71,8.77,8.59,9.32,10.06,10.55,10.60,9.59,10.16},
            new List<double> {11.01,11.06,11.11,10.94,15.64,9.69,9.60,9.43,10.18,10.60,12.19,14.32,11.99},
            new List<double> {10.82,10.86,10.91,10.75,15.45,9.50,9.40,9.31,9.17,9.59,14.32,11.80,11.80},
            new List<double> {11.81,11.85,11.90,11.74,16.43,10.49,10.39,10.30,10.22,10.16,11.99,11.80,13.50}
            }

        };

        List<List<Card>> cardCombos = Score.GetAllCombos(player.hand.Cards);

        Array values = Enum.GetValues(typeof(SuitValue));
        List<Card> testCutCards = new List<Card>();
        foreach (Ordinal face in Enum.GetValues(typeof(Ordinal))) {
            testCutCards.Add(new Card((SuitValue)(rand.Next(values.Length)), face));
        }

        double discardAdjust = 0.0;
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count == numHand) { // only look at hands with X number of cards, set by game type (generally 4)
                // Loop through all possible cut cards and average scores to maximize discard potential
                double avgScore = 0.0;
                foreach (Card testcard in testCutCards) {
                    testcard.isCut = true;
                    toCribScore = Score.ScoreHandFast(cardCombos[i], testcard);
                    avgScore += toCribScore[ScoreType.Total];
                }
                avgScore /= testCutCards.Count;
                List<Card> tmpCribCards = player.hand.Cards.Except(cardCombos[i]).ToList();
                tmpCribCards.Sort((x, y) => x.OrdinalVal.CompareTo(y.OrdinalVal));
                // compare and rank potential hands of the same score range
                // add something to look at the crib cards: don't want to 
                // put pairs of cards, addups to 15s, or 5s into the crib 
                // if it's not ours.
                if (tmpCribCards.Count == 1) {
                    if (player.isDealer) {
                        discardAdjust = discard5[(int)tmpCribCards[0].OrdinalVal];
                        potentialCribs.Add(tmpCribCards, avgScore + discardAdjust);
                    } else {
                        discardAdjust = discard5[(int)tmpCribCards[0].OrdinalVal];
                        potentialCribs.Add(tmpCribCards, avgScore - discardAdjust);
                    }
                } else if (tmpCribCards.Count == 2) {
                    if (player.isDealer) {
                        discardAdjust = listFull6[(int)tmpCribCards[1].OrdinalVal][(int)tmpCribCards[0].OrdinalVal];
                        potentialCribs.Add(tmpCribCards, avgScore + discardAdjust);
                    } else {
                        discardAdjust = listFull6[(int)tmpCribCards[1].OrdinalVal][(int)tmpCribCards[0].OrdinalVal];
                        potentialCribs.Add(tmpCribCards, avgScore - discardAdjust);
                    }
                } else if (tmpCribCards.Count == 3) { // SUPER CRIBBAGE!!!
                    // Console.WriteLine(string.Join(",", tmpCribCards));
                    discardAdjust = listFull9[(int)tmpCribCards[0].OrdinalVal][(int)tmpCribCards[1].OrdinalVal][(int)tmpCribCards[2].OrdinalVal];
                    if (player.isDealer) {
                        potentialCribs.Add(tmpCribCards, avgScore + discardAdjust);
                    } else {
                        potentialCribs.Add(tmpCribCards, avgScore - discardAdjust);
                    }
                }
                // Console.WriteLine($"Test crib score: {testcard.ToString()} {toCribScore[ScoreType.Total] - discardAdjust:F2} for {string.Join(",", cardCombos[i])} => {string.Join(",", tmpCribCards)}");

            }
        }


        // Console.WriteLine();
        List<Card> finalCrib = potentialCribs.OrderByDescending(o => o.Value).First().Key;
        double finalCribScore = potentialCribs.OrderByDescending(o => o.Value).First().Value;

        player.hand.Cards.Sort((x, y) => x.OrdinalVal.CompareTo(y.OrdinalVal));
        // Console.WriteLine($"Best score: {finalCribScore:F2} for {string.Join(",", DealtHand.cards)} " + $"=> {string.Join(",", finalCrib)}");

        foreach (Card cCard in finalCrib) {
            player.hand.Cards.Remove(cCard);
            cribCards.Add(cCard);
        }

        return cribCards;
    }
}
