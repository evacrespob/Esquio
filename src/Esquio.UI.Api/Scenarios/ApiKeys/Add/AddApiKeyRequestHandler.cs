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

namespace Esquio.UI.Api.Features.ApiKeys.Add
{
    public class AddAddApiKeyRequestHandler : IRequestHandler<AddApiKeyRequest, AddApiKeyResponse>
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly ILogger<AddAddApiKeyRequestHandler> _logger;

        private IApiKeyGenerator ApiKeyGenerator { get; set; } = new DefaultRandomApiKeyGenerator();

        public AddAddApiKeyRequestHandler(StoreDbContext storeDbContext, ILogger<AddAddApiKeyRequestHandler> logger)
        {
            _storeDbContext = storeDbContext ?? throw new ArgumentNullException(nameof(storeDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AddApiKeyResponse> Handle(AddApiKeyRequest request, CancellationToken cancellationToken)
        {
            var existing = await _storeDbContext
                .ApiKeys
                .Where(ak => ak.Name == request.Name)
                .SingleOrDefaultAsync();

            if (existing == null)
            {
                var apiKey = new ApiKeyEntity(
                    request.Name,
                    ApiKeyGenerator.CreateApiKey(),
                    request.ValidTo ?? DateTime.UtcNow.AddYears(1));

                _storeDbContext.Add(apiKey);
                await _storeDbContext.SaveChangesAsync(cancellationToken);

                return new AddApiKeyResponse()
                {
                    Name = apiKey.Name,
                    Key = apiKey.Key
                };
            }

            Log.ApiKeyAlreadyExist(_logger, request.Name);
            throw new InvalidOperationException($"A ApiKey with name {request.Name} already exist.");
        }
    }
}
