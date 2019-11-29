﻿using MediatR;

namespace Esquio.UI.Api.Features.ApiKeys.List
{
    public class ListApiKeyRequest : IRequest<ListApiKeyResponse>
    {
        public int PageIndex { get; set; } = ApiConstants.Pagination.PageIndex;

        public int PageCount { get; set; } = ApiConstants.Pagination.PageCount;
    }
}
