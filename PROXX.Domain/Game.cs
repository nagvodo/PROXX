using System.Data;

namespace PROXX.Domain
{
    public class Game
    {
        public const int maxAllowedLength = 40;
        public int Length { get; private set; }

        private int blackHolesCount;
        private int cellsToOpen;

        public Cell[,] Board { get; private set; }

        private List<(int x, int y)> blackHolesCoordinates;

        public Game(int length, int blackHolesCount)
        {
            if (length <= 0 || length > maxAllowedLength)
            {
                var errorMessage = string.Format(
                    "Length value must be greater than zero and less or equal to {0}", maxAllowedLength);
                throw new ArgumentException(errorMessage);
            }

            var maxBlackHolesCount = length * length - 1;
            if (blackHolesCount <= 0 || blackHolesCount > maxBlackHolesCount)
            {
                var errorMessage = string.Format(
                    "Black holes count must be greater than zero and less or equal to {0}", maxBlackHolesCount);
                throw new ArgumentException(errorMessage);
            }

            this.Length = length;
            this.blackHolesCount = blackHolesCount;

            Board = new Cell[Length, Length];
            blackHolesCoordinates = new List<(int x, int y)>();
        }

        public void Initialize()
        {
            var random = new Random();

            for (int i = 0; i < blackHolesCount; i++)
            {
                var x = -1;
                var y = -1;
                while (true)
                {
                    x = random.Next(Length);
                    y = random.Next(Length);

                    if (Board[x, y] != null && Board[x, y].Content == Cell.CellContent.BlackHole)
                        continue;

                    break;
                }

                Board[x, y] = new Cell(Cell.CellContent.BlackHole, Cell.CellVisibility.Hidden);
                blackHolesCoordinates.Add((x, y));
            }

            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                {
                    if (Board[i, j] == null)
                        Board[i, j] = new Cell(Cell.CellContent.Empty, Cell.CellVisibility.Hidden);
                }


            this.cellsToOpen = Length * Length - blackHolesCount;
        }

        public bool IsWon() => cellsToOpen == 0;

        private bool IsBlackHole((int x, int y) coordinates) =>
            Board[coordinates.x, coordinates.y].Content == Cell.CellContent.BlackHole;

        private bool IsInBounds((int x, int y) coordinates)
            => coordinates.x >= 0 && coordinates.x < Length
                && coordinates.y >= 0 && coordinates.y < Length;

        private bool IsOpen((int x, int y) coordinates)
            => Board[coordinates.x, coordinates.y].Visibility == Cell.CellVisibility.Open;

        public void Flag((int x, int y) coordinates)
        {
            var currentCell = Board[coordinates.x, coordinates.y];

            if (currentCell.Visibility == Cell.CellVisibility.Hidden)
                currentCell.Visibility = Cell.CellVisibility.Flagged;
        }
        public void UnFlag((int x, int y) coordinates)
        {
            var currentCell = Board[coordinates.x, coordinates.y];

            if (currentCell.Visibility == Cell.CellVisibility.Flagged)
                currentCell.Visibility = Cell.CellVisibility.Hidden;
        }

        public bool MakeMove((int x, int y) coordinates)
        {
            var successfulMove = true;
            var isBlackHole = IsBlackHole(coordinates);
            if (isBlackHole)
            {
                foreach (var c in blackHolesCoordinates)
                    Board[c.x, c.y].Visibility = Cell.CellVisibility.Open;

                return !successfulMove;
            }

            var firstCoordinates = new List<(int x, int y)>(1) { coordinates };
            CheckAdjacentCells(firstCoordinates);

            return successfulMove;
        }

        private void CheckAdjacentCells(List<(int x, int y)> coordinatesToCheck)
        {
            foreach (var coordinate in coordinatesToCheck)
            {
                if (IsOpen(coordinate))
                    continue;

                var x = coordinate.x;
                var y = coordinate.y;

                var adjacentBlackHolesCount = default(byte);
                var adjacentCoordinates = new List<(int x, int y)>();

                var northCoordinates = (x, y: y + 1);
                if (IsInBounds(northCoordinates) && !IsOpen(northCoordinates))
                    adjacentCoordinates.Add(northCoordinates);

                var southCoordinates = (x, y: y - 1);
                if (IsInBounds(southCoordinates) && !IsOpen(southCoordinates))
                    adjacentCoordinates.Add(southCoordinates);

                var westCoordinates = (x: x - 1, y);
                if (IsInBounds(westCoordinates) && !IsOpen(westCoordinates))
                    adjacentCoordinates.Add(westCoordinates);

                var eastCoordinates = (x: x + 1, y);
                if (IsInBounds(eastCoordinates) && !IsOpen(eastCoordinates))
                    adjacentCoordinates.Add(eastCoordinates);

                var northWestCoordinates = (x: x - 1, y: y + 1);
                if (IsInBounds(northWestCoordinates) && !IsOpen(northWestCoordinates))
                    adjacentCoordinates.Add(northWestCoordinates);

                var northEastCoordinates = (x: x + 1, y: y + 1);
                if (IsInBounds(northEastCoordinates) && !IsOpen(northEastCoordinates))
                    adjacentCoordinates.Add(northEastCoordinates);

                var southWestCoordinates = (x: x - 1, y: y - 1);
                if (IsInBounds(southWestCoordinates) && !IsOpen(southWestCoordinates))
                    adjacentCoordinates.Add(southWestCoordinates);

                var southEastCoordinates = (x: x + 1, y: y - 1);
                if (IsInBounds(southEastCoordinates) && !IsOpen(southEastCoordinates))
                    adjacentCoordinates.Add(southEastCoordinates);

                foreach (var coordinates in adjacentCoordinates)
                    if (IsBlackHole(coordinates))
                        adjacentBlackHolesCount++;

                Board[x, y].AdjacentBlackHolesCount = adjacentBlackHolesCount;
                Board[x, y].Visibility = Cell.CellVisibility.Open;
                cellsToOpen--;

                if (adjacentBlackHolesCount == 0)
                {
                    var moreCoordinatesToCheck =
                        adjacentCoordinates
                        .Where(x => !coordinatesToCheck.Contains(x))
                        .ToList();

                    CheckAdjacentCells(moreCoordinatesToCheck);
                }
            }
        }
    }
}