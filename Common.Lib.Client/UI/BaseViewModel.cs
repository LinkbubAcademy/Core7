﻿using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure;

namespace Common.Lib.Client.UI
{
    public class BaseViewModel
    {
        public IContextFactory ContextFactory { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static AndOperation And
        {
            get
            {
                return new();
            }
        }
        public static OrOperation Or
        {
            get
            {
                return new();
            }
        }

        public Func<Func<Task>, Task>? InvokeAsync { get; set; }

        public Action RefreshView { get; set; }
        public Action<string>? ShowAlertAsync { get; set; }

        public BaseViewModel(IContextFactory contextFactory) 
        {
            ContextFactory = contextFactory;
        }

        public bool ProcessResult(IActionResult result)
        {
            return ProcessResult(result.IsSuccess, result.Errors);
        }

        public bool ProcessResult(bool isSuccess, IEnumerable<string> errors)
        {
            if (!isSuccess)
                Errors.AddRange(errors);

            return isSuccess;
        }
    }
}
