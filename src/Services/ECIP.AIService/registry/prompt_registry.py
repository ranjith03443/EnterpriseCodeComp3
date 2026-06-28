import yaml
from pathlib import Path
from dataclasses import dataclass, field


@dataclass
class PromptTemplate:
    name: str
    version: str
    description: str
    template: str
    parameters: list[str] = field(default_factory=list)
    tags: list[str] = field(default_factory=list)

    def render(self, **kwargs) -> str:
        rendered = self.template
        for key, value in kwargs.items():
            rendered = rendered.replace("{" + key + "}", str(value))
        return rendered


class PromptRegistry:
    def __init__(self):
        self._prompts: dict[str, PromptTemplate] = {}
        self._load_all()

    def _load_all(self) -> None:
        prompts_dir = Path(__file__).parent / "prompts"
        if not prompts_dir.exists():
            return
        for yaml_file in prompts_dir.glob("*.yaml"):
            try:
                with open(yaml_file, "r", encoding="utf-8") as f:
                    data = yaml.safe_load(f)
                if data and "name" in data:
                    prompt = PromptTemplate(
                        name=data["name"],
                        version=str(data.get("version", "1.0")),
                        description=data.get("description", ""),
                        template=data.get("template", ""),
                        parameters=data.get("parameters", []),
                        tags=data.get("tags", []),
                    )
                    self._prompts[prompt.name] = prompt
            except Exception:
                pass

    def get(self, name: str) -> PromptTemplate | None:
        return self._prompts.get(name)

    def list_all(self) -> list[PromptTemplate]:
        return sorted(self._prompts.values(), key=lambda p: p.name)

    def render(self, name: str, **kwargs) -> str:
        prompt = self.get(name)
        if prompt is None:
            raise KeyError(f"Prompt '{name}' not found in registry.")
        return prompt.render(**kwargs)
