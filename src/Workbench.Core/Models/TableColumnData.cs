﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Workbench.Core.Models
{
    public class TableColumnData
    {
        private readonly TableColumnModel _column;
        private readonly IList<TableCellModel> _cells;

        public TableColumnData(TableColumnModel theColumn, IEnumerable<TableCellModel> theColumnCells)
        {
            _column = theColumn;
            _cells = new List<TableCellModel>(theColumnCells);
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        public TableColumnModel Column => _column;

        /// <summary>
        /// Gets the cells in the column.
        /// </summary>
        public IReadOnlyCollection<TableCellModel> Cells => new ReadOnlyCollection<TableCellModel>(_cells);

        /// <summary>
        /// Get the cells in the column.
        /// </summary>
        /// <returns>Cells in the column.</returns>
        public IReadOnlyCollection<TableCellModel> GetCells()
        {
            return new ReadOnlyCollection<TableCellModel>(_cells.ToList());
        }

        /// <summary>
        /// Get the cell at the one based index.
        /// </summary>
        /// <param name="cellIndex">One based index.</param>
        /// <returns>Cell at the index location.</returns>
        public TableCellModel GetCellAt(int cellIndex)
        {
            if (cellIndex <= 0 || cellIndex > _cells.Count)
                throw new ArgumentOutOfRangeException(nameof(cellIndex));

            return _cells[cellIndex - 1];
        }
    }
}
