using PROXX.Domain;
using PROXX.Domain.Exceptions;
using Xunit;

namespace PROXX.Tests
{
    public class Cell_Spec
    {
        [Fact]
        public void Adjacent_black_holes_count_cannot_be_more_than_max_adjacent_cells()
        {
            //Arrange
            var content = Cell.CellContent.Empty;
            var visibility = Cell.CellVisibility.Open;
            var cell = new Cell(content, visibility)
            {
                AdjacentBlackHolesCount = Cell.maxAdjacentCellsCount
            };

            //Assert
            Assert.Throws<MaxAdjacentCellsExceededException>(() => cell.AdjacentBlackHolesCount++);
        }
    }
}
