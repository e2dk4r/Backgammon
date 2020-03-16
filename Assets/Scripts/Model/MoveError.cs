public enum MoveError
{
    // no error
    NoError,

    // you have to move counter-clockwise
    WrongHomeDirection,
    // there is not enough step value to move in
    NotEnoughSteps,
    // there is more than one enemy piece in slot
    BlockedByEnemy,
    // you must have all pieces in home to move outside
    AllPiecesNotInHome,
    
    // unknown error
    Unknown,
}
