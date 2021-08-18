using System;
using System.Collections;

namespace Dinah.Core.DataBinding
{
	public interface IMemberComparable
	{
		IComparer GetMemberComparer(Type memberType);
		object GetMemberValue(string memberName);
	}
}
