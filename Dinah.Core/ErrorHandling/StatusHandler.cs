﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dinah.Core.ErrorHandling
{
	// stripped-down version of GenericBizRunner.StatusGenericHandler
	// consider replacing StatusHandler with Vladimir Khorikov's CSharpFunctionalExtensions nuget
	public class StatusHandler : IEnumerable<string>
    {
        private List<string> _errors { get; } = new List<string>();

        /// <summary>Holds the list of errors. If empty, then no errors</summary>
        public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        /// <summary>True if no error messages</summary>
        public bool IsSuccess => !HasErrors;

        /// <summary>True if any error messages have been registered</summary>
        public bool HasErrors => _errors.Any();

        public void AddError(string errorMessage) => _errors.Add(errorMessage);
        public void AddErrors(IEnumerable<string> errorMessages) => _errors.AddRange(errorMessages);

        public void Add(string errorMessage) => AddError(errorMessage);
        public void AddRange(IEnumerable<string> errorMessages) => AddErrors(errorMessages);

        public IEnumerator<string> GetEnumerator() => _errors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _errors.GetEnumerator();

        public override string ToString() => IsSuccess ? "Success" : "Errors: " + _errors.Count;
    }
}
