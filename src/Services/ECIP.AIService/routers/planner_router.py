import logging
from fastapi import APIRouter
from planner.models import PlannerRequest, ExecutionPlan
from planner.intent_detector import detect_intent
from planner.plan_builder import build_plan
from planner.plan_executor import execute_plan
import dependencies

router = APIRouter(prefix="/planner", tags=["Planner"])
logger = logging.getLogger("ecip.planner")


@router.get("/intents")
async def list_intents():
    return {
        "intents": [
            {"name": "summarize_repository", "description": "Summarise the repository's purpose and structure"},
            {"name": "analyze_architecture", "description": "Analyse architectural patterns and layers"},
            {"name": "explain_code", "description": "Explain specific code, classes, or methods"},
            {"name": "analyze_impact", "description": "Analyse the potential impact of a change"},
            {"name": "general", "description": "General questions about the codebase"},
        ]
    }


@router.post("/ask", response_model=ExecutionPlan)
async def ask(request: PlannerRequest):
    logger.info("Planner received query: %s", request.query)

    intent_result = detect_intent(request.query)
    logger.info(
        "Detected intent: %s (confidence=%.2f, reasoning=%s)",
        intent_result.intent, intent_result.confidence, intent_result.reasoning,
    )

    plan = build_plan(intent_result, request.query, request.context or "")

    gateway = dependencies.get_gateway()
    plan = await execute_plan(plan, gateway)

    logger.info(
        "Plan %s completed in %dms — intent=%s status=%s steps=%d",
        plan.plan_id, plan.total_duration_ms, plan.intent, plan.status, len(plan.steps),
    )
    return plan
