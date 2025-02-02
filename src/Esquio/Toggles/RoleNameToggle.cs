﻿using Esquio.Abstractions;
using Esquio.Abstractions.Providers;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.Toggles
{
    /// <summary>
    /// A binary <see cref="IToggle"/> that is active depending on the current Role name and if this is contained in configured Roles property.
    /// </summary>
    [DesignType(Description = "Toggle that is active depending on current user role name.", FriendlyName = "Identity Role")]
    [DesignTypeParameter(ParameterName = Roles, ParameterType = EsquioConstants.SEMICOLON_LIST_PARAMETER_TYPE, ParameterDescription = "The collection of rol(es) to activate this toggle separated by ';' character")]
    public class RoleNameToggle
       : IToggle
    {
        internal const string Roles = nameof(Roles);

        private readonly IRoleNameProviderService _roleNameProviderService;
        private readonly IRuntimeFeatureStore _featureStore;

        /// <summary>
        /// Create a new instance of <see cref="RoleNameToggle"/>.
        /// </summary>
        /// <param name="roleNameProviderService">The <see cref="IRoleNameProviderService"/> service to be used.</param>
        /// <param name="featureStore">The <see cref="IRuntimeFeatureStore"/> service to be used.</param>
        public RoleNameToggle(IRoleNameProviderService roleNameProviderService, IRuntimeFeatureStore featureStore)
        {
            _roleNameProviderService = roleNameProviderService ?? throw new ArgumentNullException(nameof(roleNameProviderService));
            _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
        }

        /// <inheritdoc/>
        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var currentRole = await _roleNameProviderService
                .GetCurrentRoleNameAsync();

            if (currentRole != null)
            {
                var feature = await _featureStore.FindFeatureAsync(featureName, productName, cancellationToken);
                var toggle = feature.GetToggle(this.GetType().FullName);
                var data = toggle.GetData();

                string activeRoles = data.Roles?.ToString();

                if (activeRoles != null)
                {
                    var tokenizer = new StringTokenizer(activeRoles, EsquioConstants.DEFAULT_SPLIT_SEPARATOR);

                    return tokenizer.Contains(
                        currentRole, StringSegmentComparer.OrdinalIgnoreCase);
                }
            }
            return false;
        }
    }
}
