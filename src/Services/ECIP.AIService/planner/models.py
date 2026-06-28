import uuid
from datetime import datetime, timezone
from enum import Enum
from typing import Optional
from pydantic import BaseModel, Field


class PlanStepStatus(str, Enum):
    PENDING = "pending"
    RUNNING = "running"
    COMPLETED = "completed"
    FAILED = "failed"


class Intent(str, Enum):
    SUMMARIZE_REPOSITORY = "summarize_repository"
    ANALYZE_ARCHITECTURE = "analyze_architecture"
    EXPLAIN_CODE = "explain_code"
    ANALYZE_IMPACT = "analyze_impact"
    GENERAL = "general"


class PlanStep(BaseModel):
    step_id: str = Field(default_factory=lambda: str(uuid.uuid4())[:8])
    step_number: int
    name: str
    description: str
    prompt_name: str
    parameters: dict[str, str] = {}
    status: PlanStepStatus = PlanStepStatus.PENDING
    result: Optional[str] = None
    error: Optional[str] = None
    duration_ms: Optional[int] = None


class ExecutionPlan(BaseModel):
    plan_id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    query: str
    intent: str
    confidence: float
    steps: list[PlanStep] = []
    status: PlanStepStatus = PlanStepStatus.PENDING
    summary: Optional[str] = None
    total_duration_ms: int = 0
    created_at: str = Field(default_factory=lambda: datetime.now(timezone.utc).isoformat())
    completed_at: Optional[str] = None


class PlannerRequest(BaseModel):
    query: str
    context: Optional[str] = None


class IntentResult(BaseModel):
    intent: str
    confidence: float
    reasoning: str
