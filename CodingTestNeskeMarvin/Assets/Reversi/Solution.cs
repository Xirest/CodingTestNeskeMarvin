using System;
using System.Linq;
using UnityEngine;

public class Solution : MonoBehaviour
{
    private static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] digits = "0123456789".ToCharArray();

    /// <summary>
    /// </summary>
    /// <param name="board">reversi board as string</param>
    /// <returns><see cref="int[]"/> of length = 2. First element is the board width. Second element is the board height</returns>
    /// <exception cref="Exception">thrown when board metrics aren't allowed</exception>
    private static int[] ParseBoardSize(string board)
    {
        int width = int.Parse(board[..2]);
        int height = int.Parse(board[2..5]);

        if (width < 4 || width > 26 || height < 1 || height > 26)
        {
            throw new Exception("Board does not conform to defined dimensions");
        }
        return new int[]{ width, height};
    }

    public static string PlaceToken(string board)
    {
        int[] widthHeight = ParseBoardSize(board);
        int width = widthHeight[0];
        int height = widthHeight[1];
        int maxDiagLength = height;

        // remove the first line with width and heigth from the string (plus three because of the trailing space, the including notation, and the new line)
        board = board[(board.LastIndexOfAny(digits) + 3)..];

        // split the board string at the new line; also remove " "-string in between lines
        string[] boardSplit = board.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        char[,] boardArray = new char[width, height];
        for (int y = 0; y < height; ++y) 
        {
            // split each line into their chars
            string[] line = boardSplit[y].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            for(int x = 0; x < width; ++x)
            {
                // fill the 2D boardArray with chars
                boardArray[x, y] = char.Parse(line[x]);
            }
        }

        // result Values
        int maxConverted = 0;
        int maxX = 0;
        int maxY = 0;

        // go through all spaces
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                // I could optimize here and only continue if at least one neighbor actually contains an 'O'
                // the boundary cases aren't worth the hassle for such small boards
                // check if space is empty
                if (boardArray[x, y] == '.')
                {
                    int diag1StartX = x - y;
                    int diag2StartX = x + y;

                    // get the complete row, column and diagonals at current location
                    // the Linq usage was taken from: https://stackoverflow.com/a/51241629
                    char[] row = Enumerable.Range(0, width).Select(idx => boardArray[idx, y]).ToArray();
                    char[] column = Enumerable.Range(0, height).Select(idx => boardArray[x, idx]).ToArray();
                    char[] diag1 = Enumerable.Range(0, maxDiagLength).Select(idx =>
                    (diag1StartX + idx >= 0 && diag1StartX + idx < width) ? boardArray[diag1StartX + idx, idx] : 'B').ToArray();
                    char[] diag2 = Enumerable.Range(0, maxDiagLength).Select(idx =>
                    (diag2StartX - idx >= 0 && diag2StartX - idx < width) ? boardArray[diag2StartX - idx, idx] : 'B').ToArray();

                    // Helper Functions
                    int CountLeft(char[] array, int start)
                    {
                        if (start == 0) return 0;
                        int converted = 1;
                        while (start - converted > 0 && array[start - converted] == 'O') { converted++; }
                        return array[start - converted] == 'X' ? converted - 1 : 0;
                    }

                    int CountRight(char[] array, int start, int boundary)
                    {
                        if(start == boundary - 1) return 0;
                        int converted = 0;
                        while (start + converted + 1 < boundary - 1 && array[start + converted + 1] == 'O') { converted++; }
                        return array[start + converted + 1] == 'X' ? converted : 0;
                    }

                    // go through the arrays starting from the current pos and count left and right conversions
                    int totalConverted = 0;

                    // count conversion on row
                    totalConverted += CountLeft(row, x);
                    totalConverted += CountRight(row, x, width);
                    // count conversion on column
                    totalConverted += CountLeft(column, y);
                    totalConverted += CountRight(column, y, height);
                    // count conversion on diag1
                    totalConverted += CountLeft(diag1, y);
                    totalConverted += CountRight(diag1, y, maxDiagLength);
                    // count conversion on diag2
                    totalConverted += CountLeft(diag2, y);
                    totalConverted += CountRight(diag2, y, maxDiagLength);

                    // assign new max
                    if (totalConverted > maxConverted)
                    {
                        maxConverted = totalConverted;
                        maxX = x;
                        maxY = y;

                    }
                }
            }
        }
        // + "" converts it to string
        return alphabet[maxX].ToString() + (maxY + 1).ToString();
    }

    public static void Main(String[] args)
    {
        // 1. Correct Answer: "E1"
        string board1 = @"5 1
X O O O . ";
        string result1 = Solution.PlaceToken(board1);
        Console.WriteLine("board 1: " + result1);
        // 2. Correct Answer: "B2" 
        string board2 = @"8 7
. . . . . . . . 
. . . . . . . . 
. . O . . . . . 
. . . O X . . . 
. . . X O O . . 
. . . . . X . . 
. . . . . . X . ";
        string result2 = Solution.PlaceToken(board2);
        Console.WriteLine("board 2: " + result2);
        // 3. Correct Answer: "D3", "C4", "F5", "E6" 
        string board3 = @"8 8
. . . . . . . . 
. . . . . . . . 
. . . . . . . . 
. . . O X . . . 
. . . X O . . . 
. . . . . . . . 
. . . . . . . . 
. . . . . . . . ";
        string result3 = Solution.PlaceToken(board3);
        Console.WriteLine("board 3: " + result3);
        // 4. Correct Answer: "D6 
        string board4 = @"7 6
. . . . . . . 
. . . O . O . 
X O O X O X X 
. O X X X O X 
. X O O O . X 
. . . . . . . ";
        string result4 = Solution.PlaceToken(board4);
        Console.WriteLine("board 4: " + result4);

        //My tests!
        /*
        // last missing case was height > width
        string board5 = @"5 6
. . . . . 
. O . O . 
O X O X X 
X X X O X 
O O O . X 
. . . . . ";
        string result5 = Solution.PlaceToken(board5);
        Console.WriteLine("board 5: " + result5);
        */
        /* tests for ParseBoardSize
                string test1 = @"7 6
        . . . . . . . ";
                string result0 = Solution.PlaceToken(test1);
                Console.WriteLine(result0);
                test1 = @"26 6
        . . . . . . . ";
                result0 = Solution.PlaceToken(test1);
                Console.WriteLine(result0);
                test1 = @"26 26
        . . . . . . . ";
                result0 = Solution.PlaceToken(test1);
                Console.WriteLine(result0);
                test1 = @"7 26
        . . . . . . . ";
                result0 = Solution.PlaceToken(test1);
                Console.WriteLine(result0);
                test1 = @"7 30
        . . . . . . . ";
                result0 = Solution.PlaceToken(test1);
                Console.WriteLine(result0);
        */
    }
    private void Start()
    {
        Main(new string[0]);
    }

}
