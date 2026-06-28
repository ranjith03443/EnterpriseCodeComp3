import re
from .models import Intent, IntentResult

_PATTERNS: list[tuple[Intent, list[str]]] = [
    (Intent.SUMMARIZE_REPOSITORY, [
        r"\bsummari[sz]", r"\boverview\b", r"\bwhat is this\b",
        r"\bwhat does this (repo|project|codebase)\b",
        r"\bdescribe (this|the) (repo|project|codebase)\b",
        r"\bpurpose of\b", r"\bwhat is the (repo|project)\b",
    ]),
    (Intent.ANALYZE_ARCHITECTURE, [
        r"\barchitectur", r"\blayers?\b", r"\bpatterns?\b",
        r"\bhow is (this|the) (repo|project|codebase) (built|structured|organized)\b",
        r"\bhow is it (built|structured)\b",
    ]),
    (Intent.EXPLAIN_CODE, [
        r"\bexplain\b", r"\bhow does\b",
        r"\bwhat does.*(class|method|function|code|file)\b",
        r"\bunderstand\b", r"\bimplementation\b",
    ]),
    (Intent.ANALYZE_IMPACT, [
        r"\bimpact\b", r"\baffect", r"\bwhat happens if\b",
        r"\bdepend", r"\brisk\b", r"\bbreaking change\b",
        r"\bmodif", r"\bchanging\b", r"\bchange.*component\b",
    ]),
]


def detect_intent(query: str) -> IntentResult:
    q = query.lower().strip()
    scores: dict[Intent, int] = {i: 0 for i in Intent}

    for intent, patterns in _PATTERNS:
        for pattern in patterns:
            if re.search(pattern, q):
                scores[intent] += 1

    best = max(scores, key=lambda k: scores[k])
    total = sum(scores.values()) or 1

    if scores[best] == 0:
        best = Intent.GENERAL
        confidence = 0.60
        reasoning = "No strong keyword signals found — defaulting to general query handling."
    else:
        confidence = round(min(0.55 + scores[best] * 0.15, 0.98), 2)
        reasoning = f"Keyword analysis matched {scores[best]} pattern(s) indicating '{best.value}'."

    return IntentResult(intent=best.value, confidence=confidence, reasoning=reasoning)
