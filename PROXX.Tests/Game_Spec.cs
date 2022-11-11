using PROXX.Domain;
using Xunit;

namespace PROXX.Tests
{
    public class Game_Spec
    {
        [Fact]
        public void Cannot_create_a_game_with_board_length_more_than_max()
        {
            //Arrange
            var badLength = Game.maxAllowedLength + 1;
            var blackHolesCount = 10;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(badLength, blackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_board_length_less_than_1()
        {
            //Arrange
            var badLength = 0;
            var blackHolesCount = 10;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(badLength, blackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_number_of_black_holes_more_than_board_length_squared_minus_1()
        {
            //Arrange
            var length = 20;
            var badBlackHolesCount = length * length;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(length, badBlackHolesCount));
        }

        [Fact]
        public void Cannot_create_a_game_with_number_of_black_holes_less_than_1()
        {
            //Arrange
            var length = 20;
            var badBlackHolesCount = 0;

            //Assert
            Assert.Throws<ArgumentException>(() => new Game(length, badBlackHolesCount));
        }

        [Fact]
        public void The_move_is_not_successful_if_it_opens_black_hole()
        {
            //Arrange
            var length = 10;
            var blackHolesCount = 10;
            var game = new Game(length, blackHolesCount);
            game.Initialize();
            var moveResults = new bool[blackHolesCount];
            var counter = 0;

            //Act
            for (int i = 0; i < game.Length; i++)
                for (int j = 0; j < game.Length; j++)
                    if (game.Board[i, j].Content == Cell.CellContent.BlackHole)
                        moveResults[counter++] = game.MakeMove((x: i, y: j));

            //Assert
            for (int i = 0; i < moveResults.Length; i++)
                Assert.False(moveResults[i]);
        }

        [Fact]
        public void The_move_is_successful_if_it_opens_empty_cell()
        {
            //Arrange
            var length = 10;
            var blackHolesCount = 10;
            var game = new Game(length, blackHolesCount);
            game.Initialize();
            var emptyCellsCount = length * length - blackHolesCount;
            var moveResults = new bool[emptyCellsCount];
            var counter = 0;

            //Act
            for (int i = 0; i < game.Length; i++)
                for (int j = 0; j < game.Length; j++)
                    if (game.Board[i, j].Content == Cell.CellContent.Empty)
                        moveResults[counter++] = game.MakeMove((x: i, y: j));

            //Assert
            for (int i = 0; i < moveResults.Length; i++)
                Assert.True(moveResults[i]);
        }

        [Fact]
        public void The_game_is_won_if_all_empty_cells_are_open()
        {
            //Arrange
            var length = 10;
            var blackHolesCount = 10;
            var game = new Game(length, blackHolesCount);
            game.Initialize();

            //Act
            for (int i = 0; i < game.Length; i++)
                for (int j = 0; j < game.Length; j++)
                    if (game.Board[i, j].Content == Cell.CellContent.Empty)
                        game.MakeMove((x: i, y: j));

            var isWon = game.IsWon();

            //Assert
            Assert.True(isWon);
        }
    }
}