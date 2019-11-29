﻿using Esquio.UI.Api.Features.Toggles.Add;
using Esquio.UI.Api.Features.Toggles.AddParameter;
using Esquio.UI.Api.Features.Toggles.Delete;
using Esquio.UI.Api.Features.Toggles.Details;
using Esquio.UI.Api.Features.Toggles.KnownTypes;
using Esquio.UI.Api.Features.Toggles.Reveal;
using Esquio.UI.Api.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.UI.Api.Features.Toggles
{
    [Authorize]
    [ApiVersion("2.0")]
    [ApiController]
    public class TogglesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TogglesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [Authorize(Policies.Read)]
        [Route("api/products/{productName:slug}/features/{featureName:slug}/toggles/{toggleType}")]
        [ProducesResponseType(typeof(DetailsToggleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DetailsToggleResponse>> Details([FromRoute]DetailsToggleRequest detailsToggleRequest, CancellationToken cancellationToken = default)
        {
            var toggle = await _mediator.Send(detailsToggleRequest, cancellationToken);

            if (toggle != null)
            {
                return Ok(toggle);
            }

            return NotFound();
        }

        [HttpDelete]
        [Authorize(Policies.Write)]
        [Route("api/products/{productName:slug}/features/{featureName:slug}/toggles/{toggleType}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute]DeleteToggleRequest deleteToggleRequest, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(deleteToggleRequest, cancellationToken);

            return NoContent();
        }

        [HttpGet]
        [Authorize(Policies.Read)]
        [Route("api/toggles/parameters/{toggleType}")]
        [ProducesResponseType(typeof(RevealToggleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RevealToggleResponse>> Reveal([FromRoute]RevealToggleRequest revealToggleRequest, CancellationToken cancellationToken = default)
        {
            var reveal = await _mediator.Send(revealToggleRequest, cancellationToken);

            return Ok(reveal);
        }

        [HttpGet]
        [Authorize(Policies.Read)]
        [Route("api/toggles/types")]
        [ProducesResponseType(typeof(KnownTypesToggleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KnownTypesToggleResponse>> KnownTypes(CancellationToken cancellationToken = default)
        {
            var toggleList = await _mediator.Send(new KnownTypesToggleRequest(), cancellationToken);

            return Ok(toggleList);
        }

        [HttpPost]
        [Authorize(Policies.Write)]
        [Route("api/toggles/parameters")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddParameter(AddParameterToggleRequest parameterToggleRequest, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(parameterToggleRequest, cancellationToken);

            return Created($"api/v1/products/{parameterToggleRequest.ProductName}/features/{parameterToggleRequest.FeatureName}/toggles/{parameterToggleRequest.ToggleType}", null);
        }

        [HttpPost]
        [Authorize(Policies.Write)]
        [Route("api/toggles")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(AddToggleRequest postToggleRequest, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(postToggleRequest, cancellationToken);

            return Created($"api/v1/products/{postToggleRequest.ProductName}/features/{postToggleRequest.FeatureName}/toggles/{postToggleRequest.ToggleType}", null);
        }
    }
}
