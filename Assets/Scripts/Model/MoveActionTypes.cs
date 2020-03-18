using System;

[FlagsAttribute]
public enum MoveActionTypes
{
    // move piece from one point to another
    Move = 0b001,
    // recover piece from bar and place it on board
    Recover = 0b0010,
    // hit opponent's piece and send it to bar
    Hit = 0b0100,
    // move outside the board
    Bear = 0b1000,
}
