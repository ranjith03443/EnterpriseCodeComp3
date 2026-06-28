from .models import ExecutionPlan, PlanStep, IntentResult

_DEFAULT_CONTEXT = "No additional context provided. Use general knowledge about enterprise .NET codebases."

_PLAN_TEMPLATES: dict[str, list[dict]] = {
    "summarize_repository": [
        {
            "step_number": 1,
            "name": "Summarise Repository",
            "description": "Generate a comprehensive summary of the repository's purpose, architecture, and key components.",
            "prompt_name": "summarize_repository",
            "param_keys": {"context": "context"},
        },
    ],
    "analyze_architecture": [
        {
            "step_number": 1,
            "name": "Analyse Architecture",
            "description": "Analyse the architectural patterns, layers, and component structure of the codebase.",
            "prompt_name": "analyze_architecture",
            "param_keys": {"context": "context"},
        },
    ],
    "explain_code": [
        {
            "step_number": 1,
            "name": "Explain Code",
            "description": "Explain the purpose, patterns, and role of the identified code or component.",
            "prompt_name": "explain_code",
            "param_keys": {"code": "query", "context": "context"},
        },
    ],
    "analyze_impact": [
        {
            "step_number": 1,
            "name": "Analyse Change Impact",
            "description": "Analyse the potential impact of a change across the system, including affected components and risk.",
            "prompt_name": "analyze_impact",
            "param_keys": {
                "component_name": "query",
                "component_type": "_literal_component",
                "dependencies": "context",
            },
        },
    ],
    "general": [
        {
            "step_number": 1,
            "name": "Gather Repository Context",
            "description": "Build context by summarising the repository before answering the question.",
            "prompt_name": "summarize_repository",
            "param_keys": {"context": "context"},
        },
        {
            "step_number": 2,
            "name": "Synthesise Answer",
            "description": "Combine the repository context with the user's question to produce a final answer.",
            "prompt_name": "synthesize_response",
            "param_keys": {"query": "query", "step_results": "_step_1_result"},
        },
    ],
}


def build_plan(intent_result: IntentResult, query: str, context: str) -> ExecutionPlan:
    ctx = context.strip() if context and context.strip() else _DEFAULT_CONTEXT
    template = _PLAN_TEMPLATES.get(intent_result.intent, _PLAN_TEMPLATES["general"])

    steps = []
    for t in template:
        params: dict[str, str] = {}
        for param_key, source in t["param_keys"].items():
            if source == "query":
                params[param_key] = query
            elif source == "context":
                params[param_key] = ctx
            elif source.startswith("_literal_"):
                params[param_key] = source[len("_literal_"):]
            elif source.startswith("_step_"):
                params[param_key] = source  # resolved at execution time
            else:
                params[param_key] = source

        steps.append(PlanStep(
            step_number=t["step_number"],
            name=t["name"],
            description=t["description"],
            prompt_name=t["prompt_name"],
            parameters=params,
        ))

    return ExecutionPlan(
        query=query,
        intent=intent_result.intent,
        confidence=intent_result.confidence,
        steps=steps,
    )
