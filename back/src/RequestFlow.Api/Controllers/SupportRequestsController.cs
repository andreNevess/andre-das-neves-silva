using RequestFlow.Api.Contracts;
using RequestFlow.Application.Common.Models;
using RequestFlow.Application.SupportRequests.Commands.CreateSupportRequest;
using RequestFlow.Application.SupportRequests.Commands.DeleteSupportRequest;
using RequestFlow.Application.SupportRequests.Commands.UpdateSupportRequest;
using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Application.SupportRequests.Queries.GetSupportRequestById;
using RequestFlow.Application.SupportRequests.Queries.ListSupportRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RequestFlow.Api.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class SupportRequestsController : ControllerBase
{
    private readonly IMediator mediator;

    public SupportRequestsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Lista solicitacoes internas com filtros, pesquisa e paginacao.
    /// </summary>
    /// <param name="request">Filtros opcionais e parametros de paginacao.</param>
    /// <param name="cancellationToken">Token para cancelamento da operacao.</param>
    /// <returns>Pagina de solicitacoes ordenadas pelas mais recentes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SupportRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<SupportRequestDto>>> ListAsync(
        [FromQuery] ListSupportRequestsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ListSupportRequestsQuery(
                request.Status,
                request.Priority,
                request.Search,
                request.PageNumber,
                request.PageSize),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Consulta uma solicitacao interna pelo identificador.
    /// </summary>
    /// <param name="id">Identificador unico da solicitacao.</param>
    /// <param name="cancellationToken">Token para cancelamento da operacao.</param>
    /// <returns>Dados detalhados da solicitacao.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupportRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupportRequestDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSupportRequestByIdQuery(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cadastra uma nova solicitacao interna.
    /// </summary>
    /// <param name="request">Dados obrigatorios para abertura da solicitacao.</param>
    /// <param name="cancellationToken">Token para cancelamento da operacao.</param>
    /// <returns>Solicitacao criada com status aberta.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SupportRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SupportRequestDto>> CreateAsync(
        CreateSupportRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateSupportRequestCommand(
                request.Title,
                request.Description,
                request.Requester,
                request.Priority),
            cancellationToken);

        return Created($"/api/requests/{result.Id}", result);
    }

    /// <summary>
    /// Atualiza prioridade e status de uma solicitacao existente.
    /// </summary>
    /// <param name="id">Identificador unico da solicitacao.</param>
    /// <param name="request">Novos valores de prioridade e status.</param>
    /// <param name="cancellationToken">Token para cancelamento da operacao.</param>
    /// <returns>Solicitacao atualizada.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(SupportRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SupportRequestDto>> UpdateAsync(
        Guid id,
        UpdateSupportRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateSupportRequestCommand(id, request.Priority, request.Status),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Exclui uma solicitacao ainda aberta.
    /// </summary>
    /// <param name="id">Identificador unico da solicitacao.</param>
    /// <param name="cancellationToken">Token para cancelamento da operacao.</param>
    /// <returns>Resposta sem conteudo quando a exclusao e concluida.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSupportRequestCommand(id), cancellationToken);

        return NoContent();
    }
}
