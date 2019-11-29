﻿using MediatR;

namespace Esquio.UI.Api.Features.Users.List
{
    public class ListUsersRequest
        :IRequest<ListUsersResponse>
    {
        public int PageIndex { get; set; } = ApiConstants.Pagination.PageIndex;

        public int PageCount { get; set; } = ApiConstants.Pagination.PageIndex;
    }
}
