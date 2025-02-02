﻿using Esquio.Abstractions.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.Seedwork
{
    class DelegatedEnvironmentNameProviderService
           : IEnvironmentNameProviderService
    {
        Func<string> _getCurrentEnvironmentFunc;
        public DelegatedEnvironmentNameProviderService(Func<string> getCurrentEnvironmentFunc)
        {
            _getCurrentEnvironmentFunc = getCurrentEnvironmentFunc ?? throw new ArgumentNullException(nameof(getCurrentEnvironmentFunc));
        }
        public Task<string> GetEnvironmentNameAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_getCurrentEnvironmentFunc());
        }
    }
}
