using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Dinah.Core.Windows.Forms
{
	public static class DataGridViewExt
	{
		public static IEnumerable<DataGridViewRow> AsEnumerable(this DataGridView dataGridView)
			=> dataGridView.Rows.Cast<DataGridViewRow>();
	}
}
