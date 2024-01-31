using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Solution : MonoBehaviour
{
    enum Tile { EMPTY, OPPONENT, PLAYER, OOB /*OutOfBounds*/ }

    private static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] digits = "0123456789".ToCharArray();

    /// <summary>
    /// Transforms Board coordinates like "A1", "C3" into indeces within basic board (without the width, height line)
    /// </summary>
    /// <param name="rowAsChar"></param>
    /// <param name="column"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private static int BoardCoordToIdx(char rowAsChar, int column, int width)
    {
        // on the ASCII table A-Z starts at 65
        int row = rowAsChar - 65;

        // the additional 2*row term fixes the issues introduced by the new lines
        return ((width * row + column) * 2) + 2*row;
    }

    /// <summary>
    /// </summary>
    /// <param name="board">reversi board as string</param>
    /// <returns><see cref="int[]"/> of length = 2. First element is the board width. Second element is the board height</returns>
    /// <exception cref="Exception">thrown when board metrics aren't allowed</exception>
    private static int[] ParseBoardSize(string board)
    {
        int width = int.Parse(board[..2]);
        int height = int.Parse(board[2..5]);
        if(width < 4 || width > 26 || height < 1 || height > 26)
        {
            throw new Exception("Board does not conform to defined dimensions");
        }
        Debug.Log("Board width/height: " + width + "," + height);
        return new int[]{ width, height};
    }

    public static string PlaceToken(string board)
    {
        int[] widthHeight = ParseBoardSize(board);
        int width = widthHeight[0];
        int height = widthHeight[1];
        // remove the first line with width and heigth from the string (plus three because of the trailing space, the including notation, and the new line)
        board = board[(board.LastIndexOfAny(digits)+3)..];

        for (int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {

            }
        }
        return "";
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
