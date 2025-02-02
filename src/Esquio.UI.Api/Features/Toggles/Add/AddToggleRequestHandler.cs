﻿using Esquio.EntityFrameworkCore.Store;
using Esquio.EntityFrameworkCore.Store.Entities;
using Esquio.UI.Api.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.UI.Api.Features.Toggles.Add
{
    public class AddToggleRequestHandler : IRequestHandler<AddToggleRequest, int>
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly ILogger<AddToggleRequestHandler> _logger;

        public AddToggleRequestHandler(StoreDbContext storeDbContext, ILogger<AddToggleRequestHandler> logger)
        {
            _storeDbContext = storeDbContext ?? throw new ArgumentNullException(nameof(storeDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(AddToggleRequest request, CancellationToken cancellationToken)
        {
            var feature = await _storeDbContext
                .Features
                .Include(t => t.Toggles)
                .Where(t => t.Id == request.FeatureId)
                .SingleOrDefaultAsync(cancellationToken);

            if (feature != null)
            {
                var alreadyExistToggleType = feature.Toggles
                    .Any(t => t.Type == request.Type);

                if (!alreadyExistToggleType)
                {
                    var toggle = new ToggleEntity(feature.Id, request.Type);

                    foreach (var item in request.Parameters)
                    {
                        toggle.Parameters.Add(
                            new ParameterEntity(toggle.Id, item.Name, item.Value));
                    }

                    feature.Toggles.Add(toggle);
                    await _storeDbContext.SaveChangesAsync(cancellationToken);

                    return toggle.Id;
                }

                Log.ToggleTypeAlreadyExist(_logger, request.Type, feature.Name);
                throw new InvalidOperationException($"Toggle with type {request.Type} already exist on this feature.");
            }

            Log.FeatureNotExist(_logger, request.FeatureId.ToString());
            throw new InvalidOperationException($"The feature with id {request.FeatureId} does not exist in the store.");
        }
    }
}
