﻿using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.AspNetCore.Toggles
{
    /// <summary>
    /// A binary <see cref="IToggle"/> that is active depending on current User Claim value specified on ClaimType property and how this value is assigned to a specific partition using the 
    /// configured <see cref="IValuePartitioner"/>. This <see cref="IToggle"/> create 100 buckets for partitioner and assign the claim vaulue into a specific
    /// bucket. If assigned bucket is less or equal that Percentage property value this toggle is active.
    /// </summary>
    [DesignType(Description = "Toggle that is active depending on the bucket name created with the value of specified claim.", FriendlyName = "Gradual Rollout by Identity Claim value")]
    [DesignTypeParameter(ParameterName = Percentage, ParameterType = EsquioConstants.PERCENTAGE_PARAMETER_TYPE, ParameterDescription = "The percentage of users that activate this toggle. Percentage from 0 to 100.")]
    [DesignTypeParameter(ParameterName = ClaimType, ParameterType = EsquioConstants.STRING_PARAMETER_TYPE, ParameterDescription = "The claim type used to get value to rollout.")]
    public class GradualRolloutClaimValueToggle
        : IToggle
    {
        internal const string ClaimType = nameof(ClaimType);
        internal const string Percentage = nameof(Percentage);

        private readonly IValuePartitioner _partitioner;
        private readonly IRuntimeFeatureStore _featureStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="partitioner">The <see cref="IValuePartitioner"/> service to be used.</param>
        /// <param name="featureStore">The <see cref="IRuntimeFeatureStore"/> service to be used.</param>
        /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> service to  be used.</param>
        public GradualRolloutClaimValueToggle(
            IValuePartitioner partitioner,
            IRuntimeFeatureStore featureStore,
            IHttpContextAccessor httpContextAccessor)
        {
            _partitioner = partitioner ?? throw new ArgumentNullException(nameof(partitioner));
            _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        ///<inheritdoc/>
        public async Task<bool> IsActiveAsync(string featureName, string productName = null, CancellationToken cancellationToken = default)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName, cancellationToken);
            var toggle = feature.GetToggle(this.GetType().FullName);
            var data = toggle.GetData();

            if (Double.TryParse(data.Percentage.ToString(), out double percentage))
            {
                string claimType = data.ClaimType?.ToString();

                if (claimType != null && percentage > 0)
                {
                    var user = _httpContextAccessor
                        .HttpContext
                        .User;

                    if (user != null && user.Identity.IsAuthenticated)
                    {
                        var value = user.FindFirst(claimType)?.Value;

                        if (value != null)
                        {
                            // this only apply when claim exist, we apply also some entropy to current claim value.
                            // adding this entropy ensure that not all features with gradual rollout for claim value are enabled/disable at the same time for the same user.

                            var assignedPartition = _partitioner.ResolvePartition(featureName + value, partitions: 100);
                            return assignedPartition <= percentage;
                        }
                    }
                }
            }

            return false;
        }
    }
}
