﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns
{
    public interface IResiliencePolicy
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}
