﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Dinah.Core.WindowsDesktop.Forms
{
	public static class DataGridViewExt
	{
		public static IEnumerable<DataGridViewRow> AsEnumerable(this DataGridView dataGridView)
			=> dataGridView.Rows.Cast<DataGridViewRow>();

        public static int GetRowIdOfBoundItem<T>(this DataGridView dataGridView, T item)
            where T : class
            => dataGridView.GetRowIdOfBoundItem<T>(x => item == x);

        public static int GetRowIdOfBoundItem<T>(this DataGridView dataGridView, Func<T, bool> func)
        {
            for (var r = 0; r < dataGridView.RowCount; r++)
                if (func(dataGridView.GetBoundItem<T>(r)))
                    return r;

            return -1;
        }

        public static T GetBoundItem<T>(this DataGridView dataGridView, int rowIndex)
            => (T)dataGridView.Rows[rowIndex].DataBoundItem;
    }
}
