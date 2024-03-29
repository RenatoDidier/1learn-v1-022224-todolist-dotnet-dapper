﻿using Microsoft.AspNetCore.Mvc;
using Todo.Shared.Repositories;
using Todo.Web.Commands;
using Todo.Shared.Commands;
using Todo.Web.Handlers.Interfaces;
using Todo.Shared.Services;
using Todo.Repository.Services;

namespace Todo.Web.Controllers
{
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IJokeService _jokeService;
        private readonly IHandler<CriarAtividadeCommand> _handlerCriarAtividade;
        private readonly IHandler<EditarAtividadeCommand> _handlerEditarAtividade;
        private readonly IHandler<ExcluirAtividadeCommand> _handlerExcluirAtividade;
        private readonly IHandler<ListarAtividadeCommand> _handlerListarAtividade;

        public TodoController(
                ITodoRepository todoRepository,
                IJokeService jokeService, 
                IHandler<CriarAtividadeCommand> handlerCriarAtividade,
                IHandler<EditarAtividadeCommand> handlerEditarAtividade,
                IHandler<ExcluirAtividadeCommand> handlerExcluirAtividade,
                IHandler<ListarAtividadeCommand> handlerListarAtividade
            )
        {
            _todoRepository = todoRepository;
            _jokeService = jokeService;
            _handlerCriarAtividade = handlerCriarAtividade;
            _handlerEditarAtividade = handlerEditarAtividade;
            _handlerExcluirAtividade = handlerExcluirAtividade;
            _handlerListarAtividade = handlerListarAtividade;
        }

        [HttpGet("/")]
        public string ChamarApi()
        {
            Console.WriteLine("Chamou aqui");
            _jokeService.ChamarJoke();
            return "Está funcionando";
        }

        [HttpGet("v1/consultar/servico/externo")]
        public async Task<CommandResult> ConsultarPiada()
        {
            try
            {
                var resultado = await _jokeService.ChamarJoke();

                if (resultado == null)
                    return new CommandResult("CP01 - Problema para carregar dados do serviço externo");

                return new CommandResult(201, resultado);

            } catch
            {
                return new CommandResult("CP02 - Problema para carregar dados do serviço externo");
            }
        }

        [HttpPost("v1/atividades/listar")]
        public async Task<CommandResult> ListarAtividade(
                [FromBody] ListarAtividadeCommand atividade
            )
        {
            var acaoListarAtividade = await _handlerListarAtividade.Handle(atividade);

            var teste = acaoListarAtividade.Status;

            return acaoListarAtividade;
        }

        [HttpGet("v1/atividades/listar/{id}")]
        public async Task<ICommandResult?> ListarAtividadePorId(
                [FromRoute] int id
            )
        {

            var retornoRepository = await _todoRepository.ListarAtividadePorIdAsync(id);

            if (retornoRepository == null)
                return new CommandResult(201);


            return new CommandResult(201, retornoRepository);
        }

        [HttpPost("v1/atividades/criar")]
        public async Task<ICommandResult> CriarAtividade(
                [FromBody] CriarAtividadeCommand atividade
            )
        {
            var acaoCriarAtividade = await _handlerCriarAtividade.Handle(atividade);

            return acaoCriarAtividade;

        }

        [HttpPut("v1/atividades/editar")]
        public async Task<ICommandResult> EditarAtividade(
                [FromBody] EditarAtividadeCommand atividade
            )
        {
            var acaoEditarAtividade = await _handlerEditarAtividade.Handle(atividade);

            return acaoEditarAtividade;
        }

        [HttpDelete("v1/atividades/excluir")]
        public async Task<ICommandResult> ExcluirAtividade(
                [FromBody] ExcluirAtividadeCommand atividade
            )
        {
            var acaoExcluirAtividade = await _handlerExcluirAtividade.Handle(atividade);

            return acaoExcluirAtividade;
        }
    }
}
