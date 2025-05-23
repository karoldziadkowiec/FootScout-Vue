﻿using AutoMapper;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootScout_Vue.WebAPI.Controllers
{
    // Kontroler API dla problemów aplikacji
    [Route("api/problems")]
    [Authorize(Policy = "AdminOrUserRights")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemRepository _problemRepository;
        private readonly IMapper _mapper;

        public ProblemController(IProblemRepository problemRepository, IMapper mapper)
        {
            _problemRepository = problemRepository;
            _mapper = mapper;
        }

        // GET: api/problems/:problemId  ->  zwróć problem dla konkretnego id
        [HttpGet("{problemId}")]
        public async Task<ActionResult<Problem>> GetProblem(int problemId)
        {
            var problem = await _problemRepository.GetProblem(problemId);
            if (problem == null)
                return NotFound();

            return Ok(problem);
        }

        // GET: api/problems  ->  zwróć wszystkie problemy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Problem>>> GetAllProblems()
        {
            var problems = await _problemRepository.GetAllProblems();
            return Ok(problems);
        }

        // GET: api/problems/solved  ->  zwróć rozwiązane problemy
        [HttpGet("solved")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetSolvedProblems()
        {
            var solvedProblemss = await _problemRepository.GetSolvedProblems();
            return Ok(solvedProblemss);
        }

        // GET: api/problems/solved/count  ->  zwróć liczbę rozwiązanych problemów
        [HttpGet("solved/count")]
        public async Task<IActionResult> GetSolvedProblemCount()
        {
            int count = await _problemRepository.GetSolvedProblemCount();
            return Ok(count);
        }

        // GET: api/problems/unsolved  ->  zwróć nierozwiązane problemy
        [HttpGet("unsolved")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetUnsolvedProblems()
        {
            var unsolvedProblemss = await _problemRepository.GetUnsolvedProblems();
            return Ok(unsolvedProblemss);
        }

        // GET: api/problems/unsolved/count  ->  zwróć liczbę nierozwiązanych problemów
        [HttpGet("unsolved/count")]
        public async Task<IActionResult> GetUnsolvedProblemCount()
        {
            int count = await _problemRepository.GetUnsolvedProblemCount();
            return Ok(count);
        }

        // POST: api/problems  ->  utwórz nowy problem
        [HttpPost]
        public async Task<ActionResult> CreateProblem([FromBody] ProblemCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid dto data.");

            var problem = _mapper.Map<Problem>(dto);
            await _problemRepository.CreateProblem(problem);

            return Ok(problem);
        }

        // PUT: api/problems/:problemId  ->  oznacz konkretny problem jako rozwiązany
        [HttpPut("{problemId}")]
        public async Task<ActionResult> CheckProblemSolved(int problemId, [FromBody] Problem problem)
        {
            if (problemId != problem.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _problemRepository.CheckProblemSolved(problem);
            return NoContent();
        }

        // GET: api/problems/export  ->  eksportuj problemy do pliku .csv
        [HttpGet("export")]
        public async Task<IActionResult> ExportProblemsToCsv()
        {
            var csvStream = await _problemRepository.ExportProblemsToCsv();
            return File(csvStream, "text/csv", "problems.csv");
        }
    }
}