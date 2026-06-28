import time
import logging
from datetime import datetime, timezone
from .models import ExecutionPlan, PlanStepStatus
from gateway.ai_gateway import AiGateway

logger = logging.getLogger("ecip.planner.executor")


async def execute_plan(plan: ExecutionPlan, gateway: AiGateway) -> ExecutionPlan:
    plan.status = PlanStepStatus.RUNNING
    total_start = time.monotonic()
    step_results: dict[str, str] = {}

    for step in plan.steps:
        step.status = PlanStepStatus.RUNNING
        step_start = time.monotonic()

        try:
            params: dict[str, str] = {}
            for k, v in step.parameters.items():
                resolved = v
                for ref_key, result_text in step_results.items():
                    resolved = resolved.replace(f"_{ref_key}_result", result_text)
                params[k] = resolved

            response = await gateway.complete_with_prompt(step.prompt_name, params)
            step.result = response.content
            step.status = PlanStepStatus.COMPLETED
            step_results[f"step_{step.step_number}"] = response.content
            logger.info(
                "Step %d (%s) completed in %dms via %s",
                step.step_number, step.name, response.duration_ms, response.provider,
            )
        except Exception as exc:
            step.status = PlanStepStatus.FAILED
            step.error = str(exc)
            logger.error("Step %d (%s) failed: %s", step.step_number, step.name, exc)

        step.duration_ms = int((time.monotonic() - step_start) * 1000)

    plan.total_duration_ms = int((time.monotonic() - total_start) * 1000)
    plan.completed_at = datetime.now(timezone.utc).isoformat()

    completed = [s for s in plan.steps if s.status == PlanStepStatus.COMPLETED]
    failed = [s for s in plan.steps if s.status == PlanStepStatus.FAILED]

    if failed and not completed:
        plan.status = PlanStepStatus.FAILED
    else:
        plan.status = PlanStepStatus.COMPLETED
        if completed:
            plan.summary = completed[-1].result

    return plan
